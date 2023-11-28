﻿using GridPromocional.Data;
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
    /// Class for Purchase Order actions
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
    public class PurchaseOrderController : Controller
    {
        private const bool TRUNCATE = false;

        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly IUploadService<PgFactPurchaseOrder, PgStgFactPurchaseOrder> _upload;
        private readonly GridContext _context;

        public PurchaseOrderController(ILogger<PurchaseOrderController> logger
            , IUploadService<PgFactPurchaseOrder, PgStgFactPurchaseOrder> upload
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
            _upload.CsvService.Lookups.Add("Code", _context.PgCatProducts.Select(x => x.Code) .ToDictionary(c => c, c => (object)c));

            // Define field transformations
            _upload.CsvService.Transformations.Add("Code", (x, _) => x.Target.PoItemDescription?.Split(' ')[0]);

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
                    _upload.ValidateDuplicates(x => new { x.PurchasingDocument, x.Code });

                    // Validate existing
                    _upload.ValidateExisting(x => new { x.PurchasingDocument, x.Code });

                    // Write to table
                    await _upload.UploadTarget(TRUNCATE ? BulkOperation.Replace : BulkOperation.Insert);
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
    }
}
