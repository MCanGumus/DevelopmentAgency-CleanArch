using DA.Application.Repositories.OldEntities;
using DA.Components.System;
using DA.Domain.Entities.OldDatas;
using DA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace DA.Controllers.OldEntities
{
    public class OldPermissionsController : Controller
    {
        private readonly IOldPermissionsReadRepository _readRepository;
        private readonly IOldEmployeesReadRepository _employeesReadRepository;
        public OldPermissionsController(IOldPermissionsReadRepository readRepository, IOldEmployeesReadRepository employeesReadRepository)
        {
            _readRepository = readRepository;
            _employeesReadRepository = employeesReadRepository;
        }

        public IActionResult OldPermissions()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            OldPermissionsReport(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));

            return View();
        }

        [HttpPost]
        [Route("OldPermissions/OldPermissionsWithFilter")]
        public IActionResult OldPermissionsReport(DateTime startDate, DateTime endDate)
        {
            List<OldPermissions> allPermissions = _readRepository.GetWhere(x => (x.IzinBaslangic >= startDate && x.IzinBaslangic <= endDate) || (x.IzinBitis >= startDate && x.IzinBitis <= endDate)).ToList();

            List<OldEmployees> allEmployees = _employeesReadRepository.GetAll().ToList();

            OldPermissionsModel model = new OldPermissionsModel();

            model.Permissions = allPermissions;
            model.Employees = allEmployees;
            model.StartDate = startDate;
            model.EndDate = endDate;

            return PartialView("_ListPartialView", model);
        }

    }
}
