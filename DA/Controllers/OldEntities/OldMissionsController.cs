using DA.Application.Repositories.OldEntities;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities.OldDatas;
using DA.Models;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.OldEntities
{
    public class OldMissionsController : Controller
    {
        private readonly IOldMissionsReadRepository _readRepository;
        private readonly IOldEmployeesReadRepository _employeesReadRepository;
        public OldMissionsController(IOldMissionsReadRepository readRepository, IOldEmployeesReadRepository employeesReadRepository)
        {
            _readRepository = readRepository;
            _employeesReadRepository = employeesReadRepository;
        }
        public IActionResult OldMissions()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            OldMissionsReport(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));

            return View();
        }

        [HttpPost]
        [Route("OldMissions/OldMissionsWithFilter")]
        public IActionResult OldMissionsReport(DateTime startDate, DateTime endDate)
        {
            List<OldMissions> allMissions = _readRepository.GetWhere(x => (x.B_Tarih >= startDate && x.B_Tarih<= endDate) || (x.G_Tarih >= startDate && x.G_Tarih <= endDate)).ToList();

            OldMissionsModel model = new OldMissionsModel();

            model.Missions = allMissions;
            model.StartDate = startDate;
            model.EndDate = endDate;

            return PartialView("_ListPartialView", model);
        }
    }
}
