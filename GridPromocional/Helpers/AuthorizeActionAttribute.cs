using GridPromocional.Services;
using Microsoft.AspNetCore.Mvc;

namespace GridPromocional.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeActionAttribute : TypeFilterAttribute
    {
        public AuthorizeActionAttribute() : base(typeof(AuthorizeActionFilter))
        {
        }
    }
}
