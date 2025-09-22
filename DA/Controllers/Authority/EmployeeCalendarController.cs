using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DA.Controllers.Authority
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class EmployeeCalendarController : Controller
    {

        IMissionService _missionService;
        IPermissionService _permissionService;
        public EmployeeCalendarController(IMissionService missionService, IPermissionService permissionService)
        {
            _missionService = missionService;
            _permissionService = permissionService;
        }
        public IActionResult EmployeeCalendar()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model == null)
            {
                return Redirect("/");
            }


            EmployeeCalendarList(DateTime.Today);
            return View();
        }

        [HttpPost]
        [Route("EmployeeCalendar/Listing")]
        public IActionResult EmployeeCalendarList(DateTime filter)
        {

            List<MissionDto> missions = _missionService.GetAllMissionsMonthly(filter).Where(x => x.State == Domain.Enums.EnumState.Accepted).ToList();
            List<PermissionDto> permissions = _permissionService.GetPermissionsMonthly(filter).Where(x => x.State == Domain.Enums.EnumState.Accepted).ToList();
            
            var daysInMonth = DateTime.DaysInMonth(filter.Year, filter.Month);

            EmployeeCalendarModelPart emp = null;
            List<EmployeeCalendarModelPart> employeeCalendarModelParts = new List<EmployeeCalendarModelPart>();
            Dictionary<DateTime, List<EmployeeCalendarModelPart>> eventsByDay = new Dictionary<DateTime, List<EmployeeCalendarModelPart>>();

            foreach (MissionDto item in missions)
            {
                emp = new EmployeeCalendarModelPart()
                {
                    Description = item.Subject,
                    IsMission = true,
                    Name = item.Employee.Name + " " + item.Employee.Surname,
                    DateOfStart = item.DateOfStart,
                    DateOfEnd = item.DateOfEnd,
                };

                employeeCalendarModelParts.Add(emp);
            }

            foreach (PermissionDto item in permissions)
            {
                emp = new EmployeeCalendarModelPart()
                {
                    Description = item.PermissionReason,
                    IsMission = false,
                    Name = item.Employee.Name + " " + item.Employee.Surname,
                    DateOfStart = item.StartDate,
                    DateOfEnd = item.EndDate.AddDays(-1),
                };

                employeeCalendarModelParts.Add(emp);
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                eventsByDay[new DateTime(filter.Year, filter.Month, day)] = employeeCalendarModelParts
                    .Where(v => v.DateOfStart.Date <= new DateTime(filter.Year, filter.Month, day) && v.DateOfEnd.Date >= new DateTime(filter.Year, filter.Month, day))
                    .ToList();
            }


            EmployeeCalendarModel model = new EmployeeCalendarModel();

            model.Filter = filter;
            model.Employees = eventsByDay;

            return PartialView("_ListPartialView", model);
        }
        
    }
}
