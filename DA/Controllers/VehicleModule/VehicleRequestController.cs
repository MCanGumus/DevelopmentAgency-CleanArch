using AutoMapper;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Enums;
using DA.Models;
using DA.Persistence.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.VehicleModule
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class VehicleRequestController : Controller
    {
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IMissionService _missionService;
        private readonly IVehicleService _vehicleService;
        private readonly IMapper _mapper;
        private readonly IValidator<SaveVehicleRequestDto> _saveValidator;
        private readonly IValidator<UpdateVehicleRequestDto> _updateValidator;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IVehiclePassengerService _vehiclePassengerService;
        private readonly IEmployeeService _employeeService;
        public VehicleRequestController(
            IVehicleRequestService vehicleRequestService,
            IVehicleService vehicleService,
            IValidator<SaveVehicleRequestDto> saveValidator,
            IValidator<UpdateVehicleRequestDto> updateValidator,
            IMissionService missionService,
            IWebHostEnvironment webHostEnvironment,
            IVehiclePassengerService vehiclePassengerService,
            IMapper mapper,
            IEmployeeService employeeService)
        {
            _vehicleRequestService = vehicleRequestService;
            _saveValidator = saveValidator;
            _missionService = missionService;
            _webHostEnvironment = webHostEnvironment;
            _vehicleService = vehicleService;
            _updateValidator = updateValidator;
            _vehiclePassengerService = vehiclePassengerService;
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public IActionResult VehicleRequest()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            try
            {
                Employee employee = _employeeService.GetEntityById(loginnedEmployee.UserGid);

                if (employee.AuthorizationStatus == EnumAuthorizationStatus.Employee)
                {
                    return Redirect("/YetkiYok");
                }
            }
            catch (Exception)
            {
                return Redirect("/");
            }

            ListVehicleRequests();
            return View();
        }

        public IActionResult ListVehicleRequests()
        {
            VehicleRequestsWithEmployees VE = new VehicleRequestsWithEmployees();

            VE.Requests = _vehicleRequestService.GetVehicleRequests();
            VE.Employees = _employeeService.GetAll().ToList();

            return View(VE);
        }

        public IActionResult MyVehicleRequests()
        {
            MyListVehicleRequests();
            return View();
        }

        public IActionResult MyListVehicleRequests()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            VehicleRequestsWithEmployees VE = new VehicleRequestsWithEmployees();

            VE.Requests = _vehicleRequestService.GetMyVehicleRequests(model.UserGid);
            VE.Employees = _employeeService.GetAll().ToList();

            return View(VE);
        }

        [HttpPost]
        [Route("VehicleRequest/Save")]
        public IActionResult Save(SaveVehicleRequestDto sDto)
        {
            string resultJs = "";

            ValidationResult valResult = _saveValidator.Validate(sDto);
            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Description", "Açıklama");
                foreach (var item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            sDto.IdMissionFK = null;
            sDto.RequestType = EnumRequestType.Valilik;

            var result = _vehicleRequestService.Insert(sDto);
            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>
            {
                _vehicleService.GetById(result.Result.IdVehicleFK).Plate,
                "Çalışan Yok",
                result.Result.RequestType.ToString(),
                result.Result.DateOfStart.ToString("dd/MM/yyyy"),
                result.Result.DateOfEnd.ToString("dd/MM/yyyy"),
                result.Result.IsGoing ? "Gidiş" : "Dönüş",
                result.Result.Description,
                string.Format(htmlCode, result.Result.Id)
            };

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);
            resultJs += "$('#ModalVehicleRequest').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("VehicleRequest/SaveForMe")]
        public IActionResult SaveForMe(SaveVehicleRequestDto sDto)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            ValidationResult valResult = _saveValidator.Validate(sDto);
            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Description", "Açıklama");
                foreach (var item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            #region Check another permission same dates

            List<VehicleRequestDto> vhcRequests =
                _vehicleRequestService.GetMyVehicleRequests(loginnedEmployee.UserGid).ToList();

            foreach (VehicleRequestDto item in vhcRequests)
            {
                for (DateTime date = item.DateOfStart.Date; date < item.DateOfEnd.Date; date = date.AddDays(1))
                {
                    if (sDto.DateOfStart.Date == date || sDto.DateOfEnd.Date == date)
                    {
                        return Ok($@"ShowErrorMessage(""Seçtiğiniz gün aralığıyla daha önce kaydettiğiniz başka bir araç talebiniz çakışıyor. Lütfen taleplerinizi kontrol ediniz."");");
                    }
                }
            }

            #endregion

            sDto.IdEmployeeFK = loginnedEmployee.UserGid;
            sDto.IdMissionFK = null;
            sDto.RequestType = EnumRequestType.Kendine;

            var result = _vehicleRequestService.Insert(sDto);
            if (result == null)
            {
                return BadRequest();
            }

            var employee = _employeeService.GetById(result.Result.IdEmployeeFK.Value);

            List<string> datas = new List<string>
            {
                _vehicleService.GetById(result.Result.IdVehicleFK).Plate,
                employee.Name + " " + employee.Surname,
                result.Result.RequestType.ToString(),
                result.Result.DateOfStart.ToString("dd/MM/yyyy"),
                result.Result.DateOfEnd.ToString("dd/MM/yyyy"),
                result.Result.IsGoing ? "Gidiş" : "Dönüş",
                result.Result.Description,
                string.Format(htmlCode, result.Result.Id)
            };

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);
            resultJs += "$('#ModalVehicleRequest').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("VehicleRequest/ChangeSelectItems")]
        public IActionResult ChangeSelectItems(DateTime startDate, DateTime endDate)
        {
            string resultJs = "";

            List<Guid> lstFullCars = _vehicleRequestService.GetFullVehicles(startDate, endDate).Select(x => x.Vehicle.Id).ToList();

            List<VehicleDto> lstCars = _vehicleService.GetAll().ToList();

            List<VehicleDto> lstEmptyCars = new List<VehicleDto>();

            foreach (var item in lstCars)
            {
                if (!lstFullCars.Contains(item.Id))
                {
                    lstEmptyCars.Add(item);
                }
            }

            resultJs += $@"$('#IdDepartureVehicleFK').html('');";

            string options = "";
            options = $@"<option value=""0"">Seçiniz...</option>";

            foreach (var item in lstEmptyCars)
                options += $@"<option value=""{item.Id}"">{item.Plate}</option>";

            resultJs += $@"$('#IdDepartureVehicleFK').html('{options}');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("VehicleRequest/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            var requestEntity = _vehicleRequestService.GetEntityById(Id);
            requestEntity.DataType = Domain.Enums.EnumDataType.Deleted;

            _vehicleRequestService.UpdateEntity(requestEntity);

            resultJs += TableTransactions.DeleteTable(requestEntity.Id);
            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("VehicleRequest/ExportDocument")]
        public IActionResult ExportDocument(Guid Id)
        {
            string resultJs = "";

            VehicleRequestDto vhcReq = _vehicleRequestService.GetVehicleRequest(Id);

            Random random = new Random();

            if (vhcReq != null)
            {
                List<Parameters> lstParams = new List<Parameters>();
                List<VehicleRequestDocumentModel> lst = new List<VehicleRequestDocumentModel>();

                List<VehiclePassengerDto> passengers = _vehiclePassengerService.GetVehiclePassengers(Id);

                VehicleRequestDocumentModel model = new VehicleRequestDocumentModel();
                foreach (VehiclePassengerDto passenger in passengers)
                {

                    model.Name += passenger.Employee.Name + " " + passenger.Employee.Surname + "@";
                    model.Apellation += passenger.Employee.Apellation + "@";
                }

                if (vhcReq.Mission != null)
                {
                    model.Name = model.Name.Replace("@", System.Environment.NewLine + System.Environment.NewLine);
                    model.Apellation = model.Apellation.Replace("@", System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine);
                }

                lst.Add(model);

                string docNumber = random.Next(100000, 999999).ToString();
                string dateNow = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                string plate = vhcReq.Vehicle.Plate;
                string driver = vhcReq.Mission == null ? "" : vhcReq.Mission.Employee.Name + " " + vhcReq.Mission.Employee.Surname;
                string missionType = vhcReq.Mission == null ? vhcReq.RequestType.ToString() :  TypeTR(vhcReq.Mission.MissionType) + " / " + vhcReq.Mission.Area;

                lstParams.Add(new Parameters() { key = "DocumentNumber", value = docNumber });
                lstParams.Add(new Parameters() { key = "DateNow", value = dateNow });
                lstParams.Add(new Parameters() { key = "Plate", value = plate });
                lstParams.Add(new Parameters() { key = "DriverName", value = driver });
                lstParams.Add(new Parameters() { key = "MissionType", value = missionType });

                FastReportClass fstReport = new FastReportClass(_webHostEnvironment);

                string resultReport = fstReport.GetReportWithTable("CarDocument.frx", plate.Replace(" ", "") + "_" + DateTime.Now.ToString("dd.MM.yyyy.HH.mm"), "EmployeesRef", "", "Document", "CarRequestDocument", lstParams, lst, Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/logo.png"));

                if (resultReport.Substring(0, 4).ToString() == "HATA")
                    resultJs += "ShowErrorMessage('Bir hata oluştu. Sistem yöneticisiyle görüşün.');";
                else
                    resultJs += string.Format("window.open('/Files/Reports/{0}', '_blank');", resultReport);

                resultJs += "ShowSuccessMessage('Başarılı.');";

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        public static string TypeTR(EnumMissionType type)
        {
            switch (type)
            {
                case EnumMissionType.YurtDisi:
                    return "Yurt Dışı";
                case EnumMissionType.YurtIci:
                    return "Yurt İçi";
                case EnumMissionType.BolgeIci:
                    return "Bölge İçi";
                default:
                    return "";
            }
        }

        private const string htmlCode = "<a onclick=\"AjaxMethod(&apos;VehicleRequest/ExportDocument&apos;, &apos;{0}&apos;, &apos;Print&apos;)\" href=\"\"><i class=\"mdi mdi-printer text-warning md20\"></i></a><a onclick=\"AjaxMethod(&apos;VehicleRequest/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
