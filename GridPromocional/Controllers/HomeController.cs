using DocumentFormat.OpenXml.Presentation;
using GridPromocional.Data;
using GridPromocional.Extensions;
using GridPromocional.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GridPromocional.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GridContext _context;

        public HomeController(ILogger<HomeController> logger, GridContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var x = _context.Users.Where(x => x.NormalizedUserName == User.Identity.Name.ToUpper()).ToList();
            }
            catch (Exception ex)
            {
                ViewData.PutListItem("Messages", new MessageViewModel("Error de conexion", true, ex.ToString()));
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}