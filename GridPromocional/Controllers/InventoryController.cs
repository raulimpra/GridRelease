using GridPromocional.Data;
using GridPromocional.Exceptions;
using GridPromocional.Extensions;
using GridPromocional.Helpers;
using GridPromocional.Models;
using GridPromocional.Models.Enums;
using GridPromocional.Services;
using Microsoft.AspNetCore.Mvc;

namespace GridPromocional.Controllers
{
    /// <summary>
    /// Class for Inventory actions
    /// Index: Upload form
    /// UploadFile:
    ///     Use _upload.CsvService.Parameters for fixed values not from the file
    ///     Use _upload.CsvService.Lookups to map foreign keys
    ///         Use _context to read the catalog and create a dictionary desc -> id
    ///     Custom validations and logic can be added between GetRegisters from file and
    ///     UploadRegisters to database
    ///     Define TRUNCATE = true to replace existing values, false for incremental load
    /// </summary>
    [AuthorizeAction]
    public class InventoryController : Controller
    {
        private const bool TRUNCATE = true;

        private readonly ILogger<InventoryController> _logger;
        private readonly IUploadService<PgFactInventories, PgStgFactInventories> _upload;
        private readonly GridContext _context;

        public InventoryController(ILogger<InventoryController> logger
            , IUploadService<PgFactInventories, PgStgFactInventories> upload
            , GridContext context)
        {
            _logger = logger;
            _upload = upload;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Messages"] = TempData.GetList<MessageViewModel>("Messages");
            ViewData["Title"] = $"Cargar Archivo de {_upload.GetDisplayName()}";

            var previousErrors = _upload.GetPreviousErrors();

            return View(previousErrors);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            int countR = 0;
            int countW = 0;
            int countE = 0;

            // Fixed values: Class property - value
            // Default value if missing Optional column
            _upload.CsvService.Parameters.Add("IgnoreExpiration", "0");

            // Foreign keys: Class property - lookup table: description - id
            // Fields already have the key, just validate the value
            _upload.CsvService.Lookups.Add("Code", _context.PgCatProducts.ToDictionary(c => c.Code, c => (object)c.Code));
            _upload.CsvService.Lookups.Add("IdFam", _context.PgCatFamily.ToDictionary(c => c.IdFam, c => (object)c.IdFam));
            _upload.CsvService.Lookups.Add("IdType", _context.PgCatMaterialType.ToDictionary(c => c.IdType, c => (object)c.IdType));
            _upload.CsvService.Lookups.Add("IgnoreExpiration", new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "false", false },
                { "true", true },
                { "0", false },
                { "1", true },
                { "no", false },
                { "si", true },
                { "", false },
                { "sí", true }
            });

            // Remove previous error messages for this user and model
            _upload.RemovePreviousErrors();

            foreach (var file in files)
            {
                try
                {
                    // Read from file stream
                    await _upload.GetRegisters(file);

                    // Do custom logic if necesary
                    ConsolidateValidProducts();

                    // Write to table
                    await _upload.UploadTarget(TRUNCATE ? BulkOperation.Replace : BulkOperation.Upsert);
                }
                catch (Exception ex)
                {
                    GridException gridEx = _upload.LogException(ex, file.FileName);
                    TempData.PutListItem("Messages", new MessageViewModel(gridEx.ToStringGrid(), true, gridEx.ToString()));
                }
                finally
                {
                    await _upload.UploadSource();
                    await _upload.LogUploadActivity();

                    countR += _upload.UploadStats.UploadedRegisters;
                    countW += _upload.UploadStats.WrongRegisters;
                    countE += _upload.UploadStats.TotalErrors;
                }
            }

            if (countR > 0)
                TempData.PutListItem("Messages", new MessageViewModel($"{countR} registros cargados."));
            if (countE > 0)
                TempData.PutListItem("Messages", new MessageViewModel($"{countW} registros con {countE} errores totales.", true));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Add register quatities grouped by code if the register is valid.
        /// Validate expiration date.
        /// </summary>
        private void ConsolidateValidProducts()
        {
            var today = DateTime.Now;   // TODO Is UTC???
            var groups = from reg in _upload.CsvService.GetCorrectRegisters()
                         join f in _context.PgCatMaterialType on reg.Target.IdType equals f.IdType
                         group new { reg, f.ExpirationTimeMonth } by reg.Target.Code;
            foreach (var group in groups)
            {
                PgFactInventories? first = null;
                foreach (var join in group)
                {
                    var minMonths = join.ExpirationTimeMonth;
                    var expiration = join.reg.Target.ExpirationDate;
                    var months = ((expiration.Year * 12) + expiration.Month) - ((today.Year * 12) + today.Month);

                    if (expiration.Day < today.Day)
                        months--;

                    if (!join.reg.Target.IgnoreExpiration && months < minMonths)
                    {
                        GridException ex = new($"La fecha de caducidad {expiration.ToShortDateString()} debe rebasar {minMonths} meses.");
                        join.reg.AddError("FECHA_CADUCIDAD", ex);
                    }
                    else if (first == null)
                    {
                        first = join.reg.Target;
                    }
                    else
                    {
                        first.Quantity += join.reg.Target.Quantity;
                        _upload.CsvService.Registers.Remove(join.reg);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult DownloadErrors()
        {
            var downloadFile = _upload.DownloadErrors();
            if (downloadFile != null)
            {
                return downloadFile;
            }
            else
            {
                TempData.PutListItem("Messages", new MessageViewModel("No hay errores por descargar.", true));

                return RedirectToAction("Index");
            }
        }

        // GET: History
        public async Task<IActionResult> History(DateTime? start, DateTime? end)
        {
            try
            {
                ViewData["Title"] = $"Historial carga de {_upload.GetDisplayName()}";

                // Default to today
                var startDay = start ?? DateTime.Today;
                var endDay = end ?? DateTime.Today;

                ViewBag.Start = startDay.ToString("yyyy-MM-dd");
                ViewBag.End = endDay.ToString("yyyy-MM-dd");

                // end has 00:00 time, include whole day by adding 1 day
                var nextDay = endDay.AddDays(1);
                List<PgLogUploadHistory> history = await _upload.GetUploadHistory(startDay, nextDay);

                return View(history);
            }
            catch (Exception ex)
            {
                var entity = _upload.GetSourceModelName();
                _logger.LogError(ex, "Error en consulta de historial de {start} a {end} para entity '{entity}'", start, end, entity);
                ViewData.PutListItem("Messages", new MessageViewModel("Error listando historial.", true, ex.ToString()));
                return View();
            }
        }
    }
}
