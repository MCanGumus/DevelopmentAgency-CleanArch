using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Models;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IEmployeeService _employeeService;
        public HomeController(IVehicleService vehicleService, IVehicleRequestService vehicleRequestService, IEmployeeService employeeService)
        {
            _vehicleRequestService = vehicleRequestService;
            _vehicleService = vehicleService;
            _employeeService = employeeService;

        }

        [ServiceFilter(typeof(LoggingFilterAttribute))]
        public IActionResult Index()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }


            DashboardModel dashboardModel = new DashboardModel();

            LoginSessionModel loginSessionModel = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            EmployeeDto employeeDto = _employeeService.GetById(loginSessionModel.UserGid);

            dashboardModel.RemainingFreePermission = employeeDto.TotalUnpaidLeave;
            dashboardModel.RemainingExcusedPermission = employeeDto.TotalExcusedLeave;
            dashboardModel.RemainingPaidPermission = employeeDto.TotalYearlyLeave;
            dashboardModel.RemainingEqualizationPermission = employeeDto.TotalEqualizationLeave;

            List<VehicleDto> listAllCars = _vehicleService.GetAll().ToList();

            dashboardModel.Cars = new List<DashboardCarsModel>();
            DashboardCarsModel carDto = null;

            DateTime today = DateTime.Today;
            DateTime tomarrow = DateTime.Today.AddDays(1);

            foreach (VehicleDto car in listAllCars)
            {
                carDto = new DashboardCarsModel();

                carDto.Plate = car.Plate;

                if (car.IsActive)
                {
                    List<VehicleRequestDto> isBusy = _vehicleRequestService.GetFullVehicles(today, tomarrow).Where(x => x.Vehicle.Id == car.Id).ToList();

                    if (isBusy.Count == 0)
                        carDto.State = "Available";
                    else
                        carDto.State = "Busy";
                }
                else
                {
                    carDto.State = "Not Active";
                }

                dashboardModel.Cars.Add(carDto);
            }

            return View(dashboardModel);
        }
    }
}
