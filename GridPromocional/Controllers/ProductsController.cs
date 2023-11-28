using GridPromocional.Data;
using GridPromocional.Exceptions;
using GridPromocional.Extensions;
using GridPromocional.Helpers;
using GridPromocional.Models;
using GridPromocional.Models.Enums;
using GridPromocional.Models.Errors;
using GridPromocional.Models.Views;
using GridPromocional.Services;
using GridPromocional.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace GridPromocional.Controllers
{
    [AuthorizeAction]
    public class ProductsController : Controller
    {
        private const string CODE_PREFIX = "MX016883";

        private readonly ILogger<ProductsController> _logger;
        private readonly IUploadService<PgCatProducts, PgStgCatProducts> _upload;
        private readonly GridContext _context;
        private readonly IProductsServices _products;
        private readonly ICatalogServices _catalog;
        private readonly UserClaims _user;

        public ProductsController(ILogger<ProductsController> logger, IUploadService<PgCatProducts, PgStgCatProducts> upload,
            IProductsServices products, ICatalogServices catalogs, IHttpContextAccessor httpContext, GridContext context)
        {
            _logger = logger;
            _upload = upload;
            _context = context;
            _products = products;
            _catalog = catalogs;
            _user = (UserClaims)httpContext.HttpContext.Items["User"];
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
            _upload.CsvService.Parameters.Add("Code", string.Empty);
            _upload.CsvService.Parameters.Add("IdSt", "V");
            _upload.CsvService.Parameters.Add("Status", true);

            // Foreign keys: Class property - lookup table: description - id
            _upload.CsvService.Lookups.Add("IdType", _context.PgCatMaterialType.ToDictionary(c => c.MaterialType, c => (object)c.IdType, StringComparer.InvariantCultureIgnoreCase));
            _upload.CsvService.Lookups.Add("IdFam", _context.PgCatFamily.ToDictionary(c => c.IdFam, c => (object)c.IdFam));

            // Define field transformations
            var code = _context.PgCatProducts.Where(x => x.Code.Contains(CODE_PREFIX)).Max(x => x.Code);
            int consecutive = code != null ? Int32.Parse(code[CODE_PREFIX.Length..]) : 0;
            // _upload.CsvService.Transformations.Add("Code", (_, v) => $"{CODE_PREFIX}{++consecutive}");

            // Remove previous error messages for this user and model
            _upload.RemovePreviousErrors();

            foreach (var file in files)
            {
                try
                {
                    // Read from file stream
                    await _upload.GetRegisters(file);

                    // Do custom logic if necesary
                    _upload.ValidateExisting(x => x.Project);

                    // Validate duplicate keys
                    _upload.ValidateDuplicates(x => x.Project);

                    // Assign code to correct registers only
                    foreach(var reg in _upload.CsvService.GetCorrectRegisters())
                        reg.Target.Code = $"{CODE_PREFIX}{++consecutive}";

                    // Write to table
                    await _upload.UploadTarget(BulkOperation.Insert);
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
                List<PgLogUploadHistory> history = await _upload.GetUploadHistoryFull(startDay, nextDay);

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

        public IActionResult InsertManual()
        {
            List<MessageViewModel> listError = new List<MessageViewModel>();
            try
            {
                fillCatalogs("M","R");
            }
            catch (Exception ex)
            {
                listError.Add(new MessageViewModel("Ocurrio un error al llenar los catalogs, intentalo mas tarde", true));
            }
            ViewData["Messages"] = listError;
            return View(new PgCatProductsErrors());
        }

        public IActionResult SaveProduct(PgCatProductsErrors elemnt)
        {
            bool validationFlag = true;
            List<MessageViewModel> listError = new List<MessageViewModel>();
            try
            {
                if (elemnt.IdType == null)
                {
                    elemnt.MaterialTypeError = "Favor de seleccionar un material";
                    validationFlag = false;
                }
                if (String.IsNullOrEmpty(elemnt.Project))
                {
                    elemnt.ProjectError = "Favor de colocar un proyecto";
                    validationFlag = false;
                }
                if (_context.PgCatProducts.Any(x => x.Project == elemnt.Project))
                {
                    elemnt.ProjectError = "Ya existe un Proyecto con ese nombre";
                    validationFlag = false;
                }
                if (String.IsNullOrEmpty(elemnt.Description))
                {
                    elemnt.DescriptionError = "Favor de colocar una descripcion";
                    validationFlag = false;
                }
                if (elemnt.IdFam == null)
                {
                    elemnt.FamilyError = "Favor de seleccionar una familia";
                    validationFlag = false;
                }
                if (validationFlag)
                {
                    _products.AddProducts(new PgCatProducts()
                    {
                        Code = elemnt.Code,
                        IdType = elemnt.IdType,
                        IdFam = elemnt.IdFam,
                        Description = elemnt.Description,
                        Project = elemnt.Project,
                        IdSt = elemnt.IdSt,
                        Status = true
                    }, _user.email);

                    listError.Add(new MessageViewModel("Producto ingresado correctamente", false));

                    fillCatalogs("M");
                    ViewData["InventoriesSearch"] = _products.getInventories(new ViewInventories() { Code = "", Project = elemnt.Project }, _user.codemp);
                    ViewData["View"] = "ViewSKUs";
                    return View("ViewSKUs", new ViewInventories());

                }
                else
                {
                    listError.Add(new MessageViewModel("Favor de revisar tu informacion", true));
                }
            }
            catch (Exception ex)
            {
                listError.Add(new MessageViewModel("Ocurrio un error al ingresarla informacion, intentelo mas tarde", true, ex.Message));
            }

            fillCatalogs("M", "R");
            ViewData["Messages"] = listError;
            ViewData["View"] = "InsertManual";
            return View("InsertManual", elemnt);
        }

        public IActionResult ViewSKUs()
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
            ViewData["View"] = "ViewSKUs";
            return View("ViewSKUs", element);
        }
        private void fillCatalogs(string material = "M", string status = "")
        {
            ViewData["prodCatalogs"] = _catalog.getCatalogs(_user.codemp, material, status);
            ViewBag.Rol = _user.rol;
        }

    }
}
