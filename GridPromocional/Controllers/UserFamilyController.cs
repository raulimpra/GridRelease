using GridPromocional.Helpers;
using GridPromocional.Models;
using GridPromocional.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GridPromocional.Controllers
{
    [AuthorizeAction]
    public class UserFamilyController : Controller
    {
        private readonly IUserFamilyService _userFamilyService;
        public UserFamilyController(IUserFamilyService userFamilyService)
        {
            _userFamilyService = userFamilyService;
        }

        public IActionResult Index()
        {
            ViewBag.LowRole = "Miembro";
            List<PgCatFamily> pf = _userFamilyService.GetFamilies();
            ViewBag.Families = new SelectList(pf, "IdFam", "Family");
            var rls = _userFamilyService.GetRoles();
            ViewBag.Roles = new SelectList(rls, "Id", "FamilyName");
            return View();
        }

        public IActionResult Report()
        {
            List<PgCatFamily> pf = _userFamilyService.GetFamilies();
            ViewBag.Families = new SelectList(pf, "IdFam", "Family"); ;
            var rls = _userFamilyService.GetRoles();
            ViewBag.Roles = new SelectList(rls, "Id", "FamilyName");
            return View();
        }

        public JsonResult GetUsersList(string role, string family)
        {
            List<UserRecord> rf = _userFamilyService.GetUsers(role, family);
            return Json(new { data = rf });
        }

        public JsonResult GetUserFamiliesReport(string role, string family)
        {
            List<UserFamiliesReportRecord> rf = _userFamilyService.GetUserFamiliesReport(role, family);
            return Json(new { data = rf });
        }


        public JsonResult GetFamiliesList()
        {
            List<PgCatFamily> ff = _userFamilyService.GetFamilies();
            return Json(new { data = ff });
        }

        [HttpGet]
        public JsonResult GetUserFamiliesList(string user)
        {
            List<PgCatFamily> ff = _userFamilyService.GetUserFamilies(user);
            return Json(new { data = ff });
        }

        [HttpPost]
        public IActionResult SaveUserFamilies(string user, string[] families)
        {
            if (string.IsNullOrEmpty(user)) return BadRequest("El usuario no debe estar vacio");

            string val = string.Join(",", families.Distinct().ToArray()).Trim();
            _userFamilyService.SaveUserFamilies(user, val);
            return Ok();

        }

        [HttpPost]
        public IActionResult SetRoles(string role, string[] users)
        {
            if (string.IsNullOrEmpty(role)) return BadRequest("El rol no debe estar vacio");
            if (users == null) return BadRequest("El usuario no debe estar vacio");
            if (users.Length <= 0) return BadRequest("El usuario no debe estar vacio");

            string val = string.Join(",", users.Distinct().ToArray());
            _userFamilyService.UpdateUserRole(role, val);
            return Ok();

        }

        [HttpGet]
        public IActionResult GetUserFamiliesView(string user)
        {
            ViewBag.UserName = user;
            return PartialView("_UserFamilesList");
        }

        [HttpGet]
        public IActionResult EdittUserFamilies(string user)
        {
            ViewBag.UserName = user;
            return PartialView("_GetUserFamilies");
        }
    }
}
