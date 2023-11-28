using GridPromocional.Data;
using GridPromocional.Extensions;
using GridPromocional.Helpers;
using GridPromocional.Models;
using GridPromocional.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;

namespace GridPromocional
{
    [AuthorizeAction]
    public class UploadHistoryController : Controller
    {
        private const string MODELS_NAMESPACE = "GridPromocional.Models.";

        private readonly ILogger<UploadHistoryController> _logger;
        private readonly IUploadService<object, UploadError> _upload;
        private readonly GridContext _context;

        public UploadHistoryController(ILogger<UploadHistoryController> logger
            , IUploadService<object, UploadError> upload
            , GridContext context)
        {
            _logger = logger;
            _upload = upload;
            _context = context;
        }

        // GET: UploadHistory
        public async Task<IActionResult> Index(DateTime? start, DateTime? end, string? entity)
        {
            try
            {
                // Default to today
                var startDay = start ?? DateTime.Today;
                var endDay = end ?? DateTime.Today;

                ViewBag.Start = startDay.ToString("yyyy-MM-dd");
                ViewBag.End = endDay.ToString("yyyy-MM-dd");

                // List of entities
                var entities = _context.PgLogUploadHistory.AsNoTracking()
                    .Select(x => x.ModelName).Distinct()
                    .Select(x => new SelectListItem() { Value = x, Text = GetText(x) })
                    .ToList();
                ViewBag.Entities = entities;

                var select = entities.Find(x => x.Value == entity) ?? entities.FirstOrDefault();
                if (select != null)
                {
                    select!.Selected = true;
                    entity ??= select?.Text;
                }
                else
                {
                    ViewData.PutListItem("Messages", new MessageViewModel("No existen registros en el historial.", true));
                }

                // end has 00:00 time, include whole day by adding 1 day
                var nextDay = endDay.AddDays(1);
                List<PgLogUploadHistory> history = await _upload.GetUploadHistory(startDay, nextDay, entity);

                return View(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en consulta de historial de {start} a {end} para entity '{entity}'", start, end, entity);
                ViewData.PutListItem("Messages", new MessageViewModel("Error listando historial.", true, ex.ToString()));
                return View();
            }
        }

        private static string GetText(string entity)
        {
            return Type.GetType(MODELS_NAMESPACE + entity)?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? entity;
        }
    }
}
