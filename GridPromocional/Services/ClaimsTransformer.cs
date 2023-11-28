using GridPromocional.Areas.Identity.Data;
using GridPromocional.Data;
using GridPromocional.Exceptions;
using GridPromocional.Helpers;
using GridPromocional.Models;
using GridPromocional.Models.Views;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace GridPromocional.Services
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly Settings _settings;
        private readonly ILogger<ClaimsTransformer> _logger;
        private readonly GridContext _context;
        private readonly UserManager<GridUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClaimsTransformer(IOptions<Settings> settings, ILogger<ClaimsTransformer> logger, GridContext context
            , UserManager<GridUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _context = context;
            _settings = settings.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Assign roles from MicrosoftIdentity to the user Identity from ADD
        /// Called by Identity
        /// </summary>
        /// <param name="principal"></param>
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;
            string? name = identity?.Name;

            try
            {
                if (name != null && identity != null)
                {
                    UserClaims userClaims = Helper.getUserClaims(identity.Claims.ToList());
                    _contextAccessor.HttpContext!.Items["User"] = userClaims;

                    // Register new user if necesary
                    var user = await RegisterIfNew(name);

                    // Assign the role as a claim (there's just one)
                    //var roles = await _userManager.GetRolesAsync(user);
                    //var roleName = roles[0];
                    //var claim = new Claim(ClaimTypes.Role, roleName);
                    var claims = await _userManager.GetClaimsAsync(user);
                    var claim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                    var roleName = claim?.Value;

                    if (claim != null)
                        identity!.AddClaim(claim);

                    userClaims.actions =_context.PgRoleActions
                        .Include(x => x.IdMaNavigation)
                        .Where(x => x.RoleName == roleName)
                        .Select(x => x.IdMaNavigation)
                        .OrderBy(x => x.Sort)
                        .AsNoTracking()
                        .ToList();

                    _logger.LogInformation("Ingreso de usuario '{name}' con rol '{roleName}'", name, roleName);
                }
                else
                {
                    _logger.LogError("Ingreso de usuario no identificado, rol no asignado");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error asignando rol del usuario '{name}'", name);
            }

            return principal;
        }

        /// <summary>
        /// Create the default roles and super user, if necesary
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task CreateDefaults(IServiceProvider serviceProvider)
        {
            var transformer = serviceProvider.GetRequiredService<IClaimsTransformation>() as ClaimsTransformer;
            await transformer!.CreateDefaultRoles();
            await transformer!.CreateDefaultUser();
        }

        /// <summary>
        /// Create default super user, if necesary
        /// </summary>
        private async Task CreateDefaultUser()
        {
            string powerUser = _settings.SuperUser;
            try
            {
                if (powerUser != null && !_userManager.Users.Any())
                {
                    var roleName = _settings.Roles[0];
                    await RegisterNewUser(powerUser, roleName);
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "Error creando usuario default '{poweruser}'", powerUser);
            }
        }

        /// <summary>
        /// Create default roles, if necesary
        /// </summary>
        private async Task CreateDefaultRoles()
        {
            try
            {
                if (!_roleManager.Roles.Any())
                {
                    foreach (var roleName in _settings.Roles)
                    {
                        // create the roles and seed them to the database
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                        if (roleResult.Succeeded)
                        {
                            _logger!.LogInformation("Role creado '{roleName}'", roleName);
                        }
                        else
                        {
                            var errors = roleResult.Errors.Select(x => x.Description).Aggregate((x, y) => x + "; " + y);
                            _logger!.LogError("Error creando role '{roleName}': {errors}", roleName, errors);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "Error creando roles default");
            }
        }

        /// <summary>
        /// Register a new user in MicrosoftIdentity
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roleName"></param>
        private async Task<GridUser> RegisterNewUser(string username, string roleName)
        {
            var user = new GridUser
            {
                UserName = username,
                Email = username
            };

            var createPowerUser = await _userManager.CreateAsync(user);
            if (createPowerUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleName);
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, roleName));
                _logger!.LogInformation("Usuario creado '{user}' con role '{roleName}'", user, roleName);
            }
            else
            {
                var errors = createPowerUser.Errors.Select(x => x.Description).Aggregate((x, y) => x + "; " + y);
                _logger!.LogError("Error, creando usuario'{user}' con role '{roleName}': {errors}", user, roleName, errors);
            }

            return user;
        }

        /// <summary>
        /// Check if user already exists and register it if necesary.
        /// Real authenticated users are saved with the name as is.
        /// Names from a file migth need a suffix of the form @my.domain added to it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="GridException"></exception>
        public async Task<GridUser> RegisterIfNew(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            var roleNames = _settings.Roles;

            // Registrar new user
            user ??= await RegisterNewUser(name, roleNames.Last());

            return user;
        }
    }
}