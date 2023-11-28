using CsvHelper.Configuration.Attributes;
using GridPromocional.Areas.Identity.Data;
using GridPromocional.Data;
using GridPromocional.Exceptions;
using GridPromocional.Models;
using GridPromocional.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace GridPromocional.Services
{
    public class UploadService<T, S> : IUploadService<T, S> where T : class where S : UploadError
    {
        private const bool JUST_ERRORS = false;

        private readonly ClaimsIdentity? _identity;
        private readonly ILogger<UploadService<T, S>> _logger;
        private readonly IBulkService _bulk;
        private readonly GridContext _context;
        private readonly Settings _settings;

        public ICsvService<T, S> CsvService { get; }

        public PgLogUploadHistory UploadStats { get; set; } = new();

        public UploadService(IHttpContextAccessor httpContextAccessor
            , ILogger<UploadService<T, S>> logger
            , ICsvService<T, S> csv
            , IBulkService bulk
            , GridContext context
            , IOptions<Settings> settings)
        {
            _identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            _logger = logger;
            CsvService = csv;
            _bulk = bulk;
            _context = context;
            _settings = settings.Value;
        }

        public IQueryable<S> GetPreviousErrors()
        {
            return _context.Set<S>().Where(e => e.ModelName == GetSourceModelName() && e.Error != null);
        }

        public void RemovePreviousErrors()
        {
            TruncateTable(typeof(S));
        }

        public async Task<PgLogUploadHistory> GetRegisters(IFormFile file, Encoding? enconding = null)
        {
            UploadStats = new()
            {
                Username = _identity?.Name,
                UploadDate = CsvService.ProcessDate,
                ModelName = GetSourceModelName(),
                Filename = file.FileName
            };

            CsvService.Parameters["Filename"] = file.FileName;
            var registers = await CsvService.GetRegisters(file.OpenReadStream(), enconding);
            UploadStats.TotalRegisters = registers.Count(x => x.Source.Row != null);

            var errors = registers.Where(x => x.HasErrors());
            UploadStats.WrongRegisters = errors.Count(x => x.Source.Row != null);
            UploadStats.TotalErrors = errors.Sum(x => x.Errors.Count);

            return UploadStats;
        }

        /// <summary>
        /// Get Upload History based on Source model
        /// </summary>
        /// <param name="start"></param>
        /// <param name="next"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<PgLogUploadHistory>> GetUploadHistory(DateTime start, DateTime next, string? entity = null)
        {
            entity ??= GetSourceModelName();
            return await _context.Set<PgLogUploadHistory>().AsNoTracking()
                .Where(x => x.UploadDate >= start && x.UploadDate < next && x.ModelName == entity)
                .OrderByDescending(x => x.UploadDate)
                .ToListAsync();
        }
        /// <summary>
        /// Get Upload History based on both, Source and Target, model
        /// </summary>
        /// <param name="start"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<List<PgLogUploadHistory>> GetUploadHistoryFull(DateTime start, DateTime next)
        {
            return await _context.Set<PgLogUploadHistory>().AsNoTracking()
                .Where(x => x.UploadDate >= start && x.UploadDate < next && (x.ModelName == GetSourceModelName() || x.ModelName == GetTargetModelName()))
                .OrderByDescending(x => x.UploadDate)
                .ToListAsync();
        }

        /// <summary>
        /// Upload target records that had no errors.
        /// In case of Exception, store error and continue with next record.
        /// </summary>
        /// <param name="operation">Insert, InsertOrUpdate or InsertOrUpdateOrDelete</param>
        /// <returns></returns>
        public async Task UploadTarget(BulkOperation operation)
        {
            var registers = CsvService.GetCorrectRegisters();

            if (!registers.Any())
            {
                var ex = new GridException("Carga abortada, no hay registros sin errores!");
                CsvService.AddErrorRegister(string.Empty, ex);
                return;
            }

            // Try full bulk first
            try
            {
                var target = registers.Select(x => x.Target);
                UploadStats.UploadedRegisters = await _bulk.BulkMerge(target, operation);
            }
            catch (Exception exBulk)
            {
                _logger.LogError(exBulk, "Error en BulkMerge tipo {operation}", operation);
                if (BulkOperation.Replace == operation)
                {
                    TruncateTable(typeof(T));
                    operation = BulkOperation.Insert;
                }

                // Something went wrong, upload one by one to identify the wrong records
                foreach (var reg in registers)
                {
                    try
                    {
                        UploadStats.UploadedRegisters += await _bulk.BulkMerge(new T[] { reg.Target }, operation);
                    }
                    catch (Exception ex)
                    {
                        reg.AddError(string.Empty, new GridException("Error cargando registro a base de datos", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Upload source records with error information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GridException"></exception>
        public async Task UploadSource()
        {
            try
            {
                // TBD use true for only errors, or null for all
                var errors = CsvService.GetSourceRecords(JUST_ERRORS ? true : null);
                UploadStats.TotalErrors = await _bulk.BulkMerge(errors, BulkOperation.Replace);
                if (!JUST_ERRORS)
                    UploadStats.TotalErrors = errors.Count(x => x.Error != null);
            }
            catch (Exception ex)
            {
                string filename = UploadStats.Filename;
                _logger.LogError(ex, "Error inexperado en carga de errors de '{filename}'", filename);
                // throw new GridException($"Guardando errores de '{filename}'", ex);
            }
            finally
            {
                var errors = CsvService.Registers.Where(x => x.HasErrors());
                UploadStats.WrongRegisters = errors.Count(x => x.Source.Row != null);
                UploadStats.TotalErrors = errors.Sum(x => x.Errors.Count);

                await SaveHistory();
            }
        }

        /// <summary>
        /// Save upload stats.
        /// </summary>
        private async Task SaveHistory()
        {
            _context.Add(UploadStats);
            await _context.SaveChangesAsync();
        }

        private void TruncateTable(Type type)
        {
            var tableAtt = type.GetCustomAttribute<TableAttribute>();
            var name = tableAtt?.Name;

            if (name != null)
                _context.Database.ExecuteSqlRaw($"TRUNCATE TABLE {name}");
        }

        /// <summary>
        /// Generate stream to download list of errors.
        /// </summary>
        /// <returns></returns>
        public FileStreamResult? DownloadErrors(Encoding? encoding = null)
        {
            FileStreamResult? file = null;

            var previousErrors = GetPreviousErrors();
            if (previousErrors.Any())
            {
                var result = CsvService.WriteData(previousErrors, encoding ?? Encoding.UTF8);
                var memoryStream = new MemoryStream(result);
                string fileDownloadName = $"Errores Carga {GetDisplayName()}.csv";
                file = new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = fileDownloadName };
            }

            return file;
        }

        /// <summary>
        /// Return the Display Name of the target model
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName() => typeof(T).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? GetTargetModelName();

        /// <summary>
        /// Return the Model Name of the target model
        /// </summary>
        /// <returns></returns>
        public string GetSourceModelName() => typeof(S).Name;

        /// <summary>
        /// Return the Model Name of the target model
        /// </summary>
        /// <returns></returns>
        public string GetTargetModelName() => typeof(T).Name;

        /// <summary>
        /// Validate if there are registers with duplicate keyCombinations
        /// </summary>
        /// <param name="keyCombination">linq expresion that matches the key combination</param>
        public bool ValidateDuplicates(Expression<Func<T, object>> keyCombination)
        {
            var keyCompiled = keyCombination.Compile();

            // Validate duplicate keys, but only on correct records
            var groups = CsvService.GetCorrectRegisters()
                .GroupBy(x => keyCompiled(x.Target))
                .Where(x => x.Count() > 1);

            if (!groups.Any())
                return false;

            // Get key column names from the expression
            var names = GetColumnNames(keyCombination);
            var keyNames = names.Aggregate((x, y) => $"{x}', '{y}");

            // Add error message to each record
            foreach (var group in groups)
            {
                // Row list
                string rows = group
                    .Select(x => x.Source.Row?.ToString() ?? string.Empty)
                    .Aggregate((x, y) => $"{x}, {y}");

                foreach (var register in group)
                {
                    register.AddError($"'{keyNames}'", new GridException($"Llave repetida en renglones {rows}."));
                }
            }

            return true;
        }

        /// <summary>
        /// Validate if there are registers with same keyCombinations in database
        /// </summary>
        /// <param name="keyCombination">linq expresion that matches the key combination</param>
        public bool ValidateExisting(Expression<Func<T, object>> keyCombination)
        {
            var keyCompiled = keyCombination.Compile();

            // Valida if it exists already
            var existing = from po in _context.Set<T>().Select(keyCombination).ToList()
                           join n in CsvService.GetCorrectRegisters()
                                on po equals keyCompiled(n.Target)
                           select n;

            if (!existing.Any())
                return false;

            // Get key column names from the expression
            var names = GetColumnNames(keyCombination);
            var keyNames = names.Aggregate((x, y) => $"{x}', '{y}");

            // Add error message to each record
            foreach (var reg in existing)
            {
                reg.AddError($"'{keyNames}'", new GridException("Ya exite un registro con esa llave en la base de datos."));
            }

            return true;
        }

        /// <summary>
        /// Get the columns Name from the keyCombination expression
        /// </summary>
        /// <param name="keyCombination"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetColumnNames<K>(Expression<K> keyCombination)
        {
            if (keyCombination.Body is NewExpression newExpression)
            {
                foreach (var argument in newExpression.Arguments)
                {
                    if (argument is MemberExpression memberExpression)
                    {
                        var x = memberExpression.Member;
                        yield return x.GetCustomAttribute<NameAttribute>()?.Names[0] ?? x.Name;
                    }
                }
            }
            else if (keyCombination.Body is MemberExpression memberExpression)
            {
                var x = memberExpression.Member;
                yield return x.GetCustomAttribute<NameAttribute>()?.Names[0] ?? x.Name;
            }
        }

        /// <summary>
        /// Update user catalog from registers
        /// </summary>
        /// <param name="truncate">deletes the subset of users from this source T not in Registers</param>
        /// <param name="selectorName">linq username selector</param>
        /// <param name="selectorA">linq active user selector</param>
        /// <param name="selectorB">linq removed user selector</param>
        /// <returns></returns>
        public async Task<List<int>> UpdateUserList(bool truncate, Func<T, string> selectorName
            , Func<Register<T, S>, bool> selectorA, Func<Register<T, S>, bool> selectorB)
        {
            var results = new List<int>();
            var currentUser = _identity?.Name?.ToUpper();
            var currentNames = _context.Users.AsNoTracking().Select(x => x.NormalizedUserName).ToList();
            var registers = CsvService.GetCorrectRegisters()
                .ToDictionary(x => selectorName(x.Target) + _settings.UsernameSuffix);

            int countA = await AddUsersA(selectorA, currentNames, registers);
            results.Add(countA);

            // Remove existing users (status B)
            int countB = await RemoveUsersB(selectorB, currentUser, currentNames, registers);
            results.Add(countB);

            // Remove Users that also exist in T but not in registers list
            int countX = await RemoveUsersMissing(truncate, selectorName, currentUser, currentNames, registers);
            results.Add(countX);

            return results;
        }

        /// <summary>
        /// Add active users, selected using selectorA
        /// </summary>
        /// <param name="selectorA"></param>
        /// <param name="currentNames"></param>
        /// <param name="registers"></param>
        /// <returns></returns>
        private async Task<int> AddUsersA(Func<Register<T, S>, bool> selectorA, List<string> currentNames, Dictionary<string, Register<T, S>> registers)
        {
            // Add new users (status A)
            var aUsers = registers.Where(x => selectorA(x.Value) && !currentNames.Contains(x.Key.ToUpper()))
                .Select(x => new GridUser
                {
                    Id = Guid.NewGuid().ToString("D"),
                    UserName = x.Key,
                    NormalizedUserName = x.Key.ToUpper(),
                    Email = x.Key,
                    NormalizedEmail = x.Key.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                });

            var count = await _bulk.BulkMerge(aUsers, BulkOperation.Insert);

            // Add role to all users (aUsers and any other without a role)
            string  roleName = _settings.Roles.Last();
            _context.Database.ExecuteSqlRaw("AddMissingRole @Perfil", new SqlParameter("@Perfil", roleName));

            return count;
        }

        /// <summary>
        /// Remove users "Baja", selected using selectorB
        /// </summary>
        /// <param name="selectorB"></param>
        /// <param name="currentUser"></param>
        /// <param name="currentNames"></param>
        /// <param name="registers"></param>
        /// <returns></returns>
        private async Task<int> RemoveUsersB(Func<Register<T, S>, bool> selectorB, string? currentUser, List<string> currentNames, Dictionary<string, Register<T, S>> registers)
        {
            var bUsers = registers.Where(x => selectorB(x.Value)).Select(x => x.Key.ToUpper());
            var remove = currentNames.Intersect(bUsers);

            if (remove.Contains(currentUser))
            {
                var current = registers.SingleOrDefault(x => x.Key.ToUpper() == currentUser);
                remove = remove.Where(x => x != currentUser);
                GridException exSelfRemove = new("Usuario no se puede eliminar a sí mismo.");
                current.Value.AddError(string.Empty, exSelfRemove);
            }

            var xUsers = _context.Users.Where(x => remove.Contains(x.UserName));
            int count = await _bulk.BulkMerge(xUsers, BulkOperation.Delete);

            var ids = xUsers.Select(x => x.Id).ToList();
            var xClaims = _context.UserClaims.Where(x => ids.Contains(x.UserId));
            await _bulk.BulkMerge(xClaims, BulkOperation.Delete);

            return count;
        }

        /// <summary>
        /// Remove users missing in registers list
        /// </summary>
        /// <param name="truncate"></param>
        /// <param name="selectorName"></param>
        /// <param name="currentUser"></param>
        /// <param name="currentNames"></param>
        /// <param name="registers"></param>
        /// <returns></returns>
        private async Task<int> RemoveUsersMissing(bool truncate, Func<T, string> selectorName, string? currentUser, List<string> currentNames, Dictionary<string, Register<T, S>> registers)
        {
            if (truncate && registers.Any())
            {
                // Remove users that are in the current target but not in the uploaded registers
                var currentTarget = _context.Set<T>().Select(x => (selectorName(x) + _settings.UsernameSuffix).ToUpper());
                var remove = currentNames.Intersect(currentTarget).Except(registers.Keys.Select(x => x.ToUpper()));

                if (remove.Contains(currentUser))
                {
                    remove = remove.Where(x => x != currentUser);
                    GridException exSelfRemove = new("Usuario no se puede eliminar a sí mismo por omisión.");
                    CsvService.AddErrorRegister(string.Empty, exSelfRemove);
                }

                var xUsers = _context.Users.Where(x => remove.Contains(x.UserName));
                int count = await _bulk.BulkMerge(xUsers, BulkOperation.Delete);

                var ids = xUsers.Select(x => x.Id).ToList();
                var xClaims = _context.UserClaims.Where(x => ids.Contains(x.UserId));
                await _bulk.BulkMerge(xClaims, BulkOperation.Delete);

                return count;
            }
            return 0;
        }

        /// <summary>
        /// Log fatal Exception in logger and CsvService
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public GridException LogException(Exception ex, string archivo)
        {
            var modelName = UploadStats.ModelName;
            var gridEx = new GridException($"Error en carga de archivo '{archivo}'", ex);

            _logger.LogError(gridEx, "En carga de modelo '{modelName}':", modelName);
            CsvService.AddErrorRegister(string.Empty, gridEx);

            return gridEx;
        }

        /// <summary>
        /// Log the activity
        /// </summary>
        public async Task LogUploadActivity()
        {
            try
            {
                string conErrores = CsvService.Registers.Any(x => x.HasErrors()) ? ", con errores" : string.Empty;
                var activity = new PgLogActivity()
                {
                    Date = UploadStats.UploadDate,
                    Username = UploadStats.Username,
                    Activity = $"Carga {GetDisplayName()}{conErrores}.",
                    Filename = UploadStats.Filename
                };
                _context.Add(activity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando actividad");
            }
        }
    }
}
