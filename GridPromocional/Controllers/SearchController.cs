using GridPromocional.Models;
using Microsoft.AspNetCore.Mvc;
using GridPromocional.Data;
using GridPromocional.Models.Views;
using GridPromocional.Services.Interfaces;
using GridPromocional.Helpers;

namespace GridPromocional.Controllers
{
    [AuthorizeAction]
    public class SearchController : Controller
    {
        private readonly ILogger<UploadHistoryController> _logger;
        private readonly GridContext _context;
        private readonly IProductsServices _products;
        private readonly ICatalogServices _catalogs;
        private readonly UserClaims _user;

        public SearchController(ILogger<UploadHistoryController> logger, IProductsServices products, ICatalogServices catalogs,
            IHttpContextAccessor httpContext, GridContext context)
        {
            _logger = logger;
            _context = context;
            _products = products;
            _catalogs = catalogs;
            _user = (UserClaims)httpContext.HttpContext.Items["User"];
        }
        public IActionResult Index()
        {
            List<MessageViewModel> listError = new List<MessageViewModel>();
            try
            {
                fillCatalogs();
                ViewData["InventoriesSearch"] = _products.getInventories(new ViewInventories(), _user.codemp);
            }
            catch (Exception ex)
            {
                listError.Add(new MessageViewModel("Ocurrio un error al cargarlos catalogos, intentelo mas tarde", true, ex.Message));
            }
            ViewData["Messages"] = listError;
            return View(new ViewInventories());
        }
        public IActionResult SearchInventories(ViewInventories element)
        {
            List<MessageViewModel> listError = new List<MessageViewModel>();
            try
            {
                ViewData["InventoriesSearch"] = _products.getInventories(element, _user.codemp);
                fillCatalogs();

            }
            catch (Exception ex)
            {
                listError.Add(new MessageViewModel("Ocurrio un error al cargarlos catalogos, intentelo mas tarde", true, ex.Message));
            }
            ViewData["Messages"] = listError;
            ViewData["View"] = "Index";
            return View("Index", element);
        }

        private void fillCatalogs()
        {
            ViewData["prodCatalogs"] = _catalogs.getCatalogs(_user.codemp, "M");
            ViewBag.Rol = _user.rol;
        }
    }
}
