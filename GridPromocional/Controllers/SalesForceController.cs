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
    /// Class for Sales Force actions
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
    public class SalesForceController : Controller
    {
        private const bool TRUNCATE = true;

        private readonly ILogger<SalesForceController> _logger;
        private readonly IUploadService<PgCatSalesForce, PgStgCatSalesForce> _upload;
        private readonly GridContext _context;

        public SalesForceController(ILogger<SalesForceController> logger
            , IUploadService<PgCatSalesForce, PgStgCatSalesForce> upload
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
            //_upload.CsvService.Parameters.Add("myProperty", value);

            // Foreign keys: Class property - lookup table: description - id
            //_upload.CsvService.Lookups.Add("myProperty", _context.MyCatalog.ToDictionary(c => c.Desc, c => (object)c.Id));

            // Remove previous error messages for this user and model
            _upload.RemovePreviousErrors();

            foreach (var file in files)
            {
                try
                {
                    // Read from file stream
                    await _upload.GetRegisters(file);

                    // Do custom logic if necesary
                    // Validate duplicate keys
                    //_upload.ValidateDuplicates(x => new { x.Target.Customer, x.Target.SearchTerm1, x.Target.SearchTerm2 });
                    _upload.ValidateDuplicates(x => x.SearchTerm2);

                    // Update Identity users
                    await UpdateUserList(TRUNCATE);

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

        /// <summary>
        /// Update user catalog from registers
        /// </summary>
        /// <param name="truncate"></param>
        /// <returns></returns>
        private async Task UpdateUserList(bool truncate)
        {
            var results = await _upload.UpdateUserList(truncate, x => x.SearchTerm2, _ => true, _ => false);

            if (results[0] > 0)
                TempData.PutListItem("Messages", new MessageViewModel($"Altas en el sistema: {results[0]}."));
            if (results[1] > 0)
                TempData.PutListItem("Messages", new MessageViewModel($"Bajas en el sistema por estado B: {results[1]}."));
            if (results[2] > 0)
                TempData.PutListItem("Messages", new MessageViewModel($"Bajas en el sistema por omisión: {results[2]}."));
        }
    }
}
