using DA.Application.Abstractions;
using DA.Application.Repositories;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Models;
using DA.Persistence.Services;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.VehicleModule
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class VehicleCalendarController : Controller
    {
        IVehicleRequestService _vehicleRequestService;
        IVehiclePassengerService _vehiclePassengerService;
        public VehicleCalendarController(IVehicleRequestService vehicleRequestService, IVehiclePassengerService vehiclePassengerService)
        {
             _vehicleRequestService = vehicleRequestService;
            _vehiclePassengerService = vehiclePassengerService;
        }
        public IActionResult VehicleCalendar()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            VehicleCalendarList(DateTime.Today);
            return View();
        }

        [HttpPost]
        [Route("VehicleCalendar/Listing")]
        public IActionResult VehicleCalendarList(DateTime filter) 
        {
            List<VehicleRequestDto> vhcRequests = _vehicleRequestService.GetFullVehiclesMonthly(filter);

            var daysInMonth = DateTime.DaysInMonth(filter.Year, filter.Month);

            Dictionary<DateTime, List<VehicleRequestDto>> requestsByDay = new Dictionary<DateTime, List<VehicleRequestDto>>();
            Dictionary<Guid, List<VehiclePassengerDto>> passengersByRequests = new Dictionary<Guid, List<VehiclePassengerDto>>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                requestsByDay[new DateTime(filter.Year, filter.Month, day)] = vhcRequests
                    .Where(v => v.DateOfStart.Date <= new DateTime(filter.Year, filter.Month, day) && v.DateOfEnd.Date >= new DateTime(filter.Year, filter.Month, day))
                    .ToList();
            }

            foreach (var item in vhcRequests)
                passengersByRequests[item.Id] = _vehiclePassengerService.GetVehiclePassengers(item.Id);

            VehicleCalendarModel model = new VehicleCalendarModel();

            model.Filter = filter;
            model.Requests = requestsByDay;
            model.Passengers = passengersByRequests;

            return PartialView("_ListPartialView", model);
        }
    }
}
