using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using GridPromocional.Models.Views;

namespace GridPromocional.Services
{
    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly UserClaims? _user;

        public AuthorizeActionFilter(IHttpContextAccessor httpContext)
        {
            _user = (UserClaims?)httpContext.HttpContext?.Items["User"];
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controller = context.ActionDescriptor.RouteValues["controller"];
            var action = context.ActionDescriptor.RouteValues["action"];

            // Realiza la lógica para verificar en la base de datos
            if (!IsAuthorized(controller, action))
            {
                context.Result = new ForbidResult();
            }
        }

        public bool IsAuthorized(string? controller, string? action)
        {
            return _user?.actions.Any(x => x.Controller == controller && x.Action == action) == true;
        }
    }
}
