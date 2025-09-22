using DA.Application.Abstractions;
using DA.Components.System;
using DA.Models;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.Authority
{
    public class DetailedEmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAcademyInfoService _academyInfoService;
        private readonly IEMailService _emailService;
        private readonly IGSMNumberService _numberService;
        private readonly IAddressService _addressService;
        public DetailedEmployeeController(IEmployeeService employeeService, IAcademyInfoService academyInfoService, IEMailService emailService, IGSMNumberService numberService, IAddressService addressService)
        {
            _employeeService = employeeService;
            _academyInfoService = academyInfoService;
            _emailService = emailService;
            _numberService = numberService;
            _addressService = addressService;
        }


        public IActionResult DetailedEmployee()
        {
            DetailedEmployeeModel detailedEmployeeModel = new DetailedEmployeeModel();

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model == null)
            {
                return Redirect("/");
            }

            detailedEmployeeModel.Employee = _employeeService.GetById(model.UserGid);
            detailedEmployeeModel.AcademyInfo = _academyInfoService.GetAllUserAcademyInfos(model.UserGid);
            detailedEmployeeModel.Address = _addressService.GetAllMyAddresses(model.UserGid);
            detailedEmployeeModel.GSMNumber = _numberService.GetAllMyNumbers(model.UserGid);
            detailedEmployeeModel.Emails = _emailService.GetAllMyMails(model.UserGid);

            return View(detailedEmployeeModel);
        }
    }
}
