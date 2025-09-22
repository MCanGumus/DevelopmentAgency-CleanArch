using AutoMapper;
using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport.DevComponents.DotNetBar;
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
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System;
using System.Transactions;

namespace DA.Controllers.MissionModule
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class MissionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveMissionDto> _saveValidator;
        private readonly IValidator<UpdateMissionDto> _updateValidator;
        private readonly IMissionService _missionService;
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IVehicleService _vehicleService;
        private readonly IEmployeeService _employeeService;
        private readonly IVehiclePassengerService _vehiclePassengerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDepartmentService _departmentService;
        public MissionController(IMapper mapper,
            IValidator<SaveMissionDto> saveValidator,
            IValidator<UpdateMissionDto> updateValidator,
            IMissionService missionService,
            IVehicleRequestService vehicleRequestService,
            IVehicleService vehicleService,
            IEmployeeService employeeService,
            IVehiclePassengerService vehiclePassengerService,
            IWebHostEnvironment webHostEnvironment,
            IDepartmentService departmentService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _missionService = missionService;
            _vehicleRequestService = vehicleRequestService;
            _vehicleService = vehicleService;
            _employeeService = employeeService;
            _vehiclePassengerService = vehiclePassengerService;
            _webHostEnvironment = webHostEnvironment;
            _departmentService = departmentService;
        }

        public IActionResult Mission()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListMission();
            return View();
        }

        public IActionResult ListMission()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<MissionDto> lstMission = _missionService.GetAllMissionsOfMe(loginnedEmployee.UserGid).OrderByDescending(x => x.DateOfStart).ToList();
            List<EmployeeDto> employees = _employeeService.GetAll().Where(x => x.Id != loginnedEmployee.UserGid).ToList();

            MissionsAndEmployees ME = new MissionsAndEmployees();

            ME.Missions = lstMission;
            ME.Employees = employees;

            return View(ME);
        }

        [HttpPost]
        [Route("Missions/Save")]
        public IActionResult Save(SaveMissionDto mission)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            mission.IdEmployeeFK = loginnedEmployee.UserGid;
            mission.State = EnumState.Pending;

            Random random = new Random();

            string strDocumentCode;

            #region Create barcode and check it
            MissionDto isThere = null;
            do
            {
                string dateTime = DateTime.Now.ToString("ddMMyyyy");

                int rndNumberFirstSeven = random.Next(1, 9999999);
                int rndNumberSecondSix = random.Next(1, 99999);

                strDocumentCode = dateTime + rndNumberSecondSix.ToString().PadLeft(5, '0');
                isThere = _missionService.GetMissionWithDocId(strDocumentCode);
            } while (isThere != null);

            #endregion

            mission.DocumentId = strDocumentCode;

            #region Check another mission same dates

            List<MissionDto> missionsOfUser =
                _missionService.GetAllMissionsOfMe(loginnedEmployee.UserGid).Where(x => x.State != EnumState.Declined).ToList();

            foreach (MissionDto item in missionsOfUser)
            {
                for (DateTime date = item.DateOfStart.Date; date < item.DateOfEnd.Date; date = date.AddDays(1))
                {
                    if (mission.DateOfStart.Date == date || mission.DateOfEnd.Date == date)
                    {
                        return Ok($@"ShowErrorMessage(""Seçtiğiniz gün aralığıyla daha önce kaydettiğiniz başka bir göreviniz çakışıyor. Lütfen görevlerinizi kontrol ediniz. Önceki görevi ya silin, ya da güncelleyin."");");
                    }
                }
            }

            #endregion

            ValidationResult valResult = _saveValidator.Validate(mission);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdEmployeeFK", "Çalışan")
                                            .Replace("IdProxyFK", "Vekil")
                                            .Replace("Area", "Alan")
                                            .Replace("Subject", "Konu")
                                            .Replace("IsAdvanceRequested", "Avans Talebi")
                                            .Replace("AdvanceAmount", "Avans Miktarı")
                                            .Replace("Notes", "Notlar")
                                            .Replace("DateOfStart", "Başlangıç Tarihi")
                                            .Replace("DateOfEnd", "Bitiş Tarihi");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            if (loginnedEmployee.DepartmentHeadGid != null)
            {
                if (loginnedEmployee.DepartmentHeadGid == Guid.Parse("0cc08cdf-23ba-4811-8dd6-08dc7b428a1b"))
                {
                    mission.State = EnumState.Accepted;
                }
            }

            var result = _missionService.Insert(mission);

            try
            {
                EmployeeDto employee = _employeeService.GetById(loginnedEmployee.UserGid);
                Department department = _departmentService.GetDepartmentWithEmployee(loginnedEmployee.DepartmentGid);

                string employeeMail = department.Employee.Email;
                string subject = "Görevlendirme Onay Talebi hk.";
                string body = $"Merhaba,\n Personel Bilgi Sisteminde onay ekranında işlem yapmanızı bekleyen {employee.Name + " " + employee.Surname} adlı personele ait görevlendirme bulunmaktadır.\n Bilginize.\n Buraya tıklayarak <a href="+ Constants.ProjectURL + "/TumGorevler'>Birim Görevleri</a> sayfasına ilerleyebilirsiniz.";

                MailSenderService.SendEmail(employeeMail, subject, body);

            }
            catch (Exception ex)
            {
            }

            List<string> datas = new List<string>();

            datas.Add(TypeTR(result.Result.MissionType));
            datas.Add(result.Result.SubjectType.ToString());
            datas.Add(result.Result.Subject);
            datas.Add(result.Result.Area);
            datas.Add(result.Result.DateOfStart.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(result.Result.DateOfEnd.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(CarDepartureTypeTR(result.Result.DepartureVehicle));
            datas.Add(CarReturnTypeTR(result.Result.ReturnVehicle));
            datas.Add(StateTR(result.Result.State));

            if (result.Result.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicle || result.Result.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicle ||
                            result.Result.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicleTraveller || result.Result.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicleTraveller)
            {
                datas.Add(string.Format(htmlCodeWithCar, result.Result.Id));
            }
            else
            {
                datas.Add(string.Format(htmlCode, result.Result.Id));
            }

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);
            resultJs += "$('#ModalMission').modal('hide');";

            if (!(mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller &&
                mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicle && mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicleTraveller))
                resultJs += OpenModalSelectCarString(result.Result);

            resultJs += "ShowSuccessMessage('Görev bilgisi başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            Mission mission = _missionService.GetEntityById(guid);

            switch (mission.DepartureVehicle)
            {
                case EnumDepartureVehicle.InstitutionalVehicle:
                    break;

                case EnumDepartureVehicle.InstitutionalVehicleTraveller:
                    break;

                default:
                    break;
            }


            resultJs += $"$('#uArea').val('{mission.Area}');";
            resultJs += $"$('#uSubject').val('{mission.Subject}');";

            if (mission.IsAdvanceRequested)
            {
                resultJs += $"$('#uIsAdvanceRequested').prop('checked',true);";
                resultJs += $" $('#uAdvanceAmount').prop('disabled', false);";
                resultJs += $"$('#uAdvanceAmount').val('{mission.AdvanceAmount}');";
            }
            else
            {
                resultJs += $"$('#uIsAdvanceRequested').prop('checked',false);";
                resultJs += $" $('#uAdvanceAmount').prop('disabled', true);";
                resultJs += $"$('#uAdvanceAmount').val('');";
            }

            List<VehicleRequestDto> vhcReqs = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            if (vhcReqs.Count == 0)
            {
                resultJs += "$('#SelectDates').show();";
            }
            else
            {
                resultJs += "$('#SelectDates').hide();";
            }

            resultJs += $"$('#uDateOfStart').val('{mission.DateOfStart.ToString("yyyy-MM-dd") + "T" + mission.DateOfStart.ToString("HH:mm")}');";
            resultJs += $"$('#uDateOfEnd').val('{mission.DateOfEnd.ToString("yyyy-MM-dd") + "T" + mission.DateOfEnd.ToString("HH:mm")}');";
            resultJs += $"$('#uNotes').val('{mission.Notes}');";
            resultJs += $"$('#uMissionType').val('{(int)mission.MissionType}').trigger('change');";
            resultJs += $"$('#uDepartureVehicle').val('{(int)mission.DepartureVehicle}').trigger('change');";
            resultJs += $"$('#uReturnVehicle').val('{(int)mission.ReturnVehicle}').trigger('change');";
            resultJs += $"$('#uIdProxyFK').val('{mission.IdProxyFK}').trigger('change');";
            resultJs += $"$('#uId').val('{mission.Id}');";
            resultJs += $"$('#Title').text('{mission.Area}');";

            resultJs += $"$('#ModalUpdateMission').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/OpenDetail")]
        public IActionResult OpenDetails(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }


            MissionDto mission = _missionService.GetMission(guid);

            switch (mission.DepartureVehicle)
            {
                case EnumDepartureVehicle.InstitutionalVehicle:
                    break;

                case EnumDepartureVehicle.InstitutionalVehicleTraveller:
                    break;

                default:
                    break;
            }


            resultJs += $"$('#sArea').text('{mission.Area}');";
            resultJs += $"$('#sSubject').text('{mission.Subject}');";

            if (mission.IsAdvanceRequested)
            {
                resultJs += $"$('#sIsAdvanceRequested').text('Evet');";
                resultJs += $"$('#sAdvanceAmount').text('{mission.AdvanceAmount}');";
            }
            else
            {
                resultJs += $"$('#sIsAdvanceRequested').text('Hayır');";
                resultJs += $"$('#sAdvanceAmount').text('0');";
            }

            resultJs += $"$('#sDateOfStart').text('{mission.DateOfStart.ToString("yyyy-MM-dd HH:mm")}');";
            resultJs += $"$('#sDateOfEnd').text('{mission.DateOfEnd.ToString("yyyy-MM-dd HH:mm")}');";
            resultJs += $"$('#sNotes').text('{mission.Notes}');";
            resultJs += $"$('#sMissionType').text('{TypeTR(mission.MissionType)}');";

            List<VehicleRequestDto> requests = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            if (requests.Count != 0)
            {
                VehicleRequestDto requestGoes = requests.Where(x => x.IsGoing == true).FirstOrDefault();
                VehicleRequestDto requestReturns = requests.Where(x => x.IsGoing == false).FirstOrDefault();

                if (requestGoes != null)
                {
                    resultJs += $"$('#sDepartureVehicle').text('{requestGoes.Vehicle.Plate + " (" + requestGoes.DateOfStart + "-" + requestGoes.DateOfEnd + ")"}');";
                }
                else
                {
                    resultJs += $"$('#sDepartureVehicle').text('');";
                }
                if (requestReturns != null)
                {
                    resultJs += $"$('#sReturnVehicle').text('{requestReturns.Vehicle.Plate + " (" + requestReturns.DateOfStart + "-" + requestReturns.DateOfEnd + ")"}');";
                }
                else
                {
                    resultJs += $"$('#sReturnVehicle').text('');";
                }
            }

            resultJs += $"$('#sEmployee').text('{mission.Employee.Name + " " + mission.Employee.Surname}');";
            resultJs += $"$('#sProxy').text('{mission.Proxy.Name + " " + mission.Proxy.Surname}');";



            resultJs += $"$('#Title').text('{mission.Area}');";

            resultJs += $"$('#ModalDetailMission').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/Update")]
        public IActionResult Update(UpdateMissionDto mission)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            mission.IdEmployeeFK = loginnedEmployee.UserGid;

            switch (mission.DepartureVehicle)
            {
                case EnumDepartureVehicle.InstitutionalVehicle:
                    break;

                case EnumDepartureVehicle.InstitutionalVehicleTraveller:
                    break;

                default:

                    break;
            }

            Mission missionDto = _missionService.GetEntityById(mission.Id);

            if (missionDto == null)
                return BadRequest();

            UpdateMissionDto uDto = _mapper.Map<UpdateMissionDto>(missionDto);

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdEmployeeFK", "Çalışan")
                                            .Replace("IdProxyFK", "Vekil")
                                            .Replace("IdDepartureVehicleFK", "Gidiş Aracı")
                                            .Replace("IdReturnVehicleFK", "Dönüş Aracı")
                                            .Replace("Area", "Alan")
                                            .Replace("Subject", "Konu")
                                            .Replace("IsAdvanceRequested", "Avans Talebi")
                                            .Replace("AdvanceAmount", "Avans Miktarı")
                                            .Replace("Notes", "Notlar")
                                            .Replace("DateOfStart", "Başlangıç Tarihi")
                                            .Replace("DateOfEnd", "Bitiş Tarihi");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            uDto.Area = mission.Area;
            uDto.Subject = mission.Subject;
            uDto.IsAdvanceRequested = mission.IsAdvanceRequested;
            uDto.DepartureVehicle = mission.DepartureVehicle;
            uDto.ReturnVehicle = mission.ReturnVehicle;
            uDto.AdvanceAmount = mission.AdvanceAmount;
            uDto.Notes = mission.Notes;

            List<VehicleRequestDto> vhcReqs = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            if (vhcReqs.Count == 0)
            {
                uDto.DateOfStart = mission.DateOfStart;
                uDto.DateOfEnd = mission.DateOfEnd;
            }

            _missionService.Update(uDto);

            List<string> datas = new List<string>();

            datas.Add(TypeTR(uDto.MissionType));
            datas.Add(uDto.SubjectType.ToString());
            datas.Add(uDto.Subject);
            datas.Add(uDto.Area);
            datas.Add(uDto.DateOfStart.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(uDto.DateOfEnd.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(CarDepartureTypeTR(uDto.DepartureVehicle));
            datas.Add(CarReturnTypeTR(uDto.ReturnVehicle));
            datas.Add(StateTR(uDto.State));

            if (uDto.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicle || uDto.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicle ||
                            uDto.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicleTraveller || uDto.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicleTraveller)
            {
                datas.Add(string.Format(htmlCodeWithCar, uDto.Id));
            }
            else
            {
                datas.Add(string.Format(htmlCode, uDto.Id));
            }

            resultJs += TableTransactions.UpdateTable(datas, uDto.Id);

            resultJs += "$('#ModalUpdateMission').modal('hide');";
            resultJs += "ShowSuccessMessage('Görev bilgisi başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            Mission mission = _missionService.GetEntityById(Id);

            if (mission != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    mission.DataType = Domain.Enums.EnumDataType.Deleted;
                    _missionService.UpdateEntity(mission);

                    List<VehicleRequest> vehicleRequests = _vehicleRequestService.GetVehicleRequestByMissionEntities(mission.Id);

                    foreach (var item in vehicleRequests)
                    {
                        item.DataType = EnumDataType.Deleted;

                        _vehicleRequestService.Update(_mapper.Map<UpdateVehicleRequestDto>(item));
                    }

                    scope.Complete();
                };

                resultJs += TableTransactions.DeleteTable(mission.Id);
                resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        public string OpenModalSelectCarString(Mission mission)
        {
            string resultJs = "";

            if (mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller)
            {
                resultJs += "$('#DepartureStartDate').prop('disabled', true);";
                resultJs += "$('#DepartureEndDate').prop('disabled', true);";
                resultJs += "$('#GetDepartureCars').prop('disabled', true);";
                resultJs += "$('#IdDepartureVehicleFK').prop('disabled', true);";
                resultJs += "$('#submitGoes').prop('disabled', true);";
            }
            else
            {
                resultJs += "$('#DepartureStartDate').prop('disabled', false);";
                resultJs += "$('#DepartureEndDate').prop('disabled', false);";
                resultJs += "$('#GetDepartureCars').prop('disabled', false);";
                resultJs += "$('#IdDepartureVehicleFK').prop('disabled', false);";
                resultJs += "$('#submitGoes').prop('disabled', false);";
            }

            if (mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicle && mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicleTraveller)
            {
                resultJs += "$('#ReturnStartDate').prop('disabled', true);";
                resultJs += "$('#ReturnEndDate').prop('disabled', true);";
                resultJs += "$('#GetReturnCars').prop('disabled', true);";
                resultJs += "$('#IdReturnVehicleFK').prop('disabled', true);";
                resultJs += "$('#submitReturns').prop('disabled', true);";
            }
            else
            {
                resultJs += "$('#ReturnStartDate').prop('disabled', false);";
                resultJs += "$('#ReturnEndDate').prop('disabled', false);";
                resultJs += "$('#GetReturnCars').prop('disabled', false);";
                resultJs += "$('#IdReturnVehicleFK').prop('disabled', false);";
                resultJs += "$('#submitReturns').prop('disabled', false);";
            }

            resultJs += $"$('#TitleSelectCar').text('{mission.Subject} ({CarDepartureTypeTR(mission.DepartureVehicle) + " - " + CarReturnTypeTR(mission.ReturnVehicle) + " / " + mission.DateOfStart.ToString("dd.MM.yyyy HH:mm") + " - " + mission.DateOfEnd.ToString("dd.MM.yyyy HH:mm")})');";

            resultJs += $"$('#departureMissionId').val('{mission.Id}');";
            resultJs += $"$('#returnMissionId').val('{mission.Id}');";
            resultJs += $"$('#departureReturnMissionId').val('{mission.Id}');";

            resultJs += $@"$('#IdDepartureVehicleFK').html('');";
            resultJs += $@"$('#IdReturnVehicleFK').html('');";

            resultJs += $"SetDateRangePickers('{mission.DateOfStart.ToString("yyyy-MM-ddTHH:mm")}', '{mission.DateOfEnd.ToString("yyyy-MM-ddTHH:mm")}');";

            resultJs += $"$('#DepartureStartDate').val('');";
            resultJs += $"$('#DepartureEndDate').val('');";

            resultJs += $"$('#currentDepartureCar').text('(Seçilmemiş)');";

            resultJs += $"$('#ReturnStartDate').val('');";
            resultJs += $"$('#ReturnEndDate').val('');";

            resultJs += $"$('#currentReturnCar').text('(Seçilmemiş)');";

            #region Single Car Select

            if (mission.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicle && mission.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicle)
            {
                List<Guid> lstFullCars = _vehicleRequestService.GetFullVehicles(mission.DateOfStart, mission.DateOfEnd).Select(x => x.Vehicle.Id).ToList();

                List<VehicleDto> lstCars = _vehicleService.GetAll().ToList();

                List<VehicleDto> lstEmptyCars = new List<VehicleDto>();

                foreach (var item in lstCars)
                {
                    if (!lstFullCars.Contains(item.Id))
                    {
                        lstEmptyCars.Add(item);
                    }
                }

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('');";

                string options = "";
                options = $@"<option value=""0"">Seçiniz...</option>";

                foreach (var item in lstEmptyCars)
                    options += $@"<option value=""{item.Id}"">{item.Plate}</option>";

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('{options}');";


                resultJs += "$('#SingleSelectController').show();";

            }
            else if (mission.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicleTraveller && mission.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicleTraveller)
            {
                List<VehicleRequestDto> lstFullCars = _vehicleRequestService.GetFullVehicles(mission.DateOfStart, mission.DateOfEnd).ToList();

                List<VehiclePassengerDto> passengers = null;

                List<VehiclePassengerCounts> goes = new List<VehiclePassengerCounts>();
                List<VehiclePassengerCounts> returns = new List<VehiclePassengerCounts>();

                foreach (var request in lstFullCars)
                {
                    passengers = _vehiclePassengerService.GetVehiclePassengers(request.Id);

                    int countOfReturns = passengers.Count();

                    foreach (var passenger in passengers)
                    {
                        VehiclePassengerCounts vhc = new VehiclePassengerCounts()
                        {
                            IdRequestFK = request.Id,
                            Plate = request.Vehicle.Plate,
                            CountPercentage = countOfReturns.ToString() + "/" + request.Vehicle.Capacity,
                            IsGoing = false
                        };

                        returns.Add(vhc);
                    }
                }
                
                returns = returns.DistinctBy(x => x.Plate).ToList();

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('');";

                string optionsReturner = "";
                optionsReturner = $@"<option value=""0"">Seçiniz...</option>";

                foreach (var item in returns)
                    optionsReturner += $@"<option value=""{item.IdRequestFK}"">{item.Plate} ({item.CountPercentage})</option>";

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('{optionsReturner}');";

                resultJs += "$('#SingleSelectController').show();";

            }
            else
            {
                resultJs += "$('#SingleSelect').hide();";
                resultJs += "$('#SeperatedSelects').show();";

                resultJs += "$('#SingleSelectController').hide();";
            }


            #endregion

            resultJs += $"$('#IsSingleVehicle').prop('checked', true).trigger('change');";

            resultJs += $"$('#ModalMissionSelectCar').modal('show');";

            return resultJs;
        }

        [HttpPost]
        [Route("Missions/OpenModalSelectCar")]
        public IActionResult OpenModalSelectCar(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            Mission mission = _missionService.GetEntityById(guid);

            if (mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller &&
                mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicle && mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicleTraveller)
                return Ok("ShowErrorMessage('Görevin aracı kurum aracı ile ilişkilendirilmemiş.')");

            List<VehicleRequestDto> requests = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            if (requests.Count > 2)
            {
                return Ok("ShowErrorMessage('Teknik hata var. Lütfen yöneticinizle görüşün.')");
            }

            if (mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller)
            {
                resultJs += "$('#DepartureStartDate').prop('disabled', true);";
                resultJs += "$('#DepartureEndDate').prop('disabled', true);";
                resultJs += "$('#GetDepartureCars').prop('disabled', true);";
                resultJs += "$('#IdDepartureVehicleFK').prop('disabled', true);";
                resultJs += "$('#submitGoes').prop('disabled', true);";
            }
            else
            {
                resultJs += "$('#DepartureStartDate').prop('disabled', false);";
                resultJs += "$('#DepartureEndDate').prop('disabled', false);";
                resultJs += "$('#GetDepartureCars').prop('disabled', false);";
                resultJs += "$('#IdDepartureVehicleFK').prop('disabled', false);";
                resultJs += "$('#submitGoes').prop('disabled', false);";
            }

            if (mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicle && mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicleTraveller)
            {
                resultJs += "$('#ReturnStartDate').prop('disabled', true);";
                resultJs += "$('#ReturnEndDate').prop('disabled', true);";
                resultJs += "$('#GetReturnCars').prop('disabled', true);";
                resultJs += "$('#IdReturnVehicleFK').prop('disabled', true);";
                resultJs += "$('#submitReturns').prop('disabled', true);";
            }
            else
            {
                resultJs += "$('#ReturnStartDate').prop('disabled', false);";
                resultJs += "$('#ReturnEndDate').prop('disabled', false);";
                resultJs += "$('#GetReturnCars').prop('disabled', false);";
                resultJs += "$('#IdReturnVehicleFK').prop('disabled', false);";
                resultJs += "$('#submitReturns').prop('disabled', false);";
            }

            resultJs += $"$('#TitleSelectCar').text('{mission.Subject} ({CarDepartureTypeTR(mission.DepartureVehicle) + " - " + CarReturnTypeTR(mission.ReturnVehicle) + " / " + mission.DateOfStart.ToString("dd.MM.yyyy HH:mm") + " - " + mission.DateOfEnd.ToString("dd.MM.yyyy HH:mm")})');";

            resultJs += $"$('#departureMissionId').val('{mission.Id}');";
            resultJs += $"$('#returnMissionId').val('{mission.Id}');";
            resultJs += $"$('#departureReturnMissionId').val('{mission.Id}');";

            resultJs += $@"$('#IdDepartureVehicleFK').html('');";
            resultJs += $@"$('#IdReturnVehicleFK').html('');";

            resultJs += $"SetDateRangePickers('{mission.DateOfStart.ToString("yyyy-MM-ddTHH:mm")}', '{mission.DateOfEnd.ToString("yyyy-MM-ddTHH:mm")}');";

            if (requests.Count != 0)
            {
                VehicleRequestDto requestGoes = requests.Where(x => x.IsGoing == true).FirstOrDefault();
                VehicleRequestDto requestReturns = requests.Where(x => x.IsGoing == false).FirstOrDefault();

                if (requestGoes != null)
                {
                    resultJs += $"$('#DepartureStartDate').val('{requestGoes.DateOfStart.ToString("yyyy-MM-ddTHH:mm")}');";
                    resultJs += $"$('#DepartureEndDate').val('{requestGoes.DateOfEnd.ToString("yyyy-MM-ddTHH:mm")}');";

                    resultJs += $"$('#currentDepartureCar').text('({requestGoes.Vehicle.Plate})');";
                }
                else
                {
                    resultJs += $"$('#DepartureStartDate').val('');";
                    resultJs += $"$('#DepartureEndDate').val('');";

                    resultJs += $"$('#currentDepartureCar').text('(Seçilmemiş)');";
                }
                if (requestReturns != null)
                {
                    resultJs += $"$('#ReturnStartDate').val('{requestReturns.DateOfStart.ToString("yyyy-MM-ddTHH:mm")}');";
                    resultJs += $"$('#ReturnEndDate').val('{requestReturns.DateOfEnd.ToString("yyyy-MM-ddTHH:mm")}');";

                    resultJs += $"$('#currentReturnCar').text('({requestReturns.Vehicle.Plate})');";
                }
                else
                {
                    resultJs += $"$('#ReturnStartDate').val('');";
                    resultJs += $"$('#ReturnEndDate').val('');";

                    resultJs += $"$('#currentReturnCar').text('(Seçilmemiş)');";
                }
            }

            #region Single Car Select

            if (mission.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicle && mission.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicle)
            {
                List<Guid> lstFullCars = _vehicleRequestService.GetFullVehicles(mission.DateOfStart, mission.DateOfEnd).Select(x => x.Vehicle.Id).ToList();

                List<VehicleDto> lstCars = _vehicleService.GetAll().ToList();

                List<VehicleDto> lstEmptyCars = new List<VehicleDto>();

                foreach (var item in lstCars)
                {
                    if (!lstFullCars.Contains(item.Id))
                    {
                        lstEmptyCars.Add(item);
                    }
                }

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('');";

                string options = "";
                options = $@"<option value=""0"">Seçiniz...</option>";

                foreach (var item in lstEmptyCars)
                    options += $@"<option value=""{item.Id}"">{item.Plate}</option>";

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('{options}');";


                resultJs += "$('#SingleSelectController').show();";

            }
            else if (mission.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicleTraveller && mission.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicleTraveller)
            {
                List<VehicleRequestDto> lstFullCars = _vehicleRequestService.GetFullVehicles(mission.DateOfStart, mission.DateOfEnd).ToList();

                List<VehiclePassengerDto> passengers = null;

                List<VehiclePassengerCounts> goes = new List<VehiclePassengerCounts>();
                List<VehiclePassengerCounts> returns = new List<VehiclePassengerCounts>();

                foreach (var request in lstFullCars)
                {
                    passengers = _vehiclePassengerService.GetVehiclePassengers(request.Id);

                    int countOfReturns = passengers.Count();

                    foreach (var passenger in passengers)
                    {
                        VehiclePassengerCounts vhc = new VehiclePassengerCounts()
                        {
                            IdRequestFK = request.Id,
                            Plate = request.Vehicle.Plate,
                            CountPercentage = countOfReturns.ToString() + "/" + request.Vehicle.Capacity,
                            IsGoing = false
                        };

                        returns.Add(vhc);
                    }
                }

                returns = returns.DistinctBy(x => x.Plate).ToList();

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('');";

                string optionsReturner = "";
                optionsReturner = $@"<option value=""0"">Seçiniz...</option>";

                foreach (var item in returns)
                    optionsReturner += $@"<option value=""{item.IdRequestFK}"">{item.Plate} ({item.CountPercentage})</option>";

                resultJs += $@"$('#IdDepartureReturnVehicleFK').html('{optionsReturner}');";

                resultJs += "$('#SingleSelectController').show();";

            }
            else
            {
                resultJs += "$('#SingleSelect').hide();";
                resultJs += "$('#SeperatedSelects').show();";

                resultJs += "$('#SingleSelectController').hide();";
            }


            #endregion

            resultJs += $"$('#ModalMissionSelectCar').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/ChangeSelectItems")]
        public IActionResult ChangeSelectItems(Guid guid, DateTime startDate, DateTime endDate, string type)
        {
            string resultJs = "";

            Mission mission = _missionService.GetEntityById(guid);

            if (type == "Departure")
            {
                if ((int)mission.DepartureVehicle == 1)
                {
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
                }
                else if ((int)mission.DepartureVehicle == 5)
                {
                    List<VehicleRequestDto> lstFullCars = _vehicleRequestService.GetFullVehicles(startDate, endDate).ToList();

                    List<VehiclePassengerDto> passengers = null;

                    List<VehiclePassengerCounts> goes = new List<VehiclePassengerCounts>();

                    foreach (var request in lstFullCars)
                    {
                        passengers = _vehiclePassengerService.GetVehiclePassengers(request.Id);

                        int countOfGoes = passengers.Count();

                        foreach (var passenger in passengers)
                        {
                            VehiclePassengerCounts vhc = new VehiclePassengerCounts()
                            {
                                IdRequestFK = request.Id,
                                Plate = request.Vehicle.Plate,
                                CountPercentage = countOfGoes.ToString() + "/" + request.Vehicle.Capacity,
                                IsGoing = true
                            };

                            goes.Add(vhc);

                        }
                    }

                    goes = goes.DistinctBy(x => x.Plate).ToList();

                    resultJs += $@"$('#IdDepartureVehicleFK').html('');";

                    string optionsDeparture = "";
                    optionsDeparture = $@"<option value=""0"">Seçiniz...</option>";

                    foreach (var item in goes)
                        optionsDeparture += $@"<option value=""{item.IdRequestFK}"">{item.Plate} ({item.CountPercentage})</option>";

                    resultJs += $@"$('#IdDepartureVehicleFK').html('{optionsDeparture}');";
                }
            }
            else if (type == "Return")
            {
                if ((int)mission.ReturnVehicle == 1)
                {
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

                    resultJs += $@"$('#IdReturnVehicleFK').html('');";

                    string options = "";
                    options = $@"<option value=""0"">Seçiniz...</option>";

                    foreach (var item in lstEmptyCars)
                        options += $@"<option value=""{item.Id}"">{item.Plate}</option>";

                    resultJs += $@"$('#IdReturnVehicleFK').html('{options}');";
                }
                else if ((int)mission.ReturnVehicle == 5)
                {
                    List<VehicleRequestDto> lstFullCars = _vehicleRequestService.GetFullVehicles(startDate, endDate).ToList();

                    List<VehiclePassengerDto> passengers = null;

                    List<VehiclePassengerCounts> goes = new List<VehiclePassengerCounts>();
                    List<VehiclePassengerCounts> returns = new List<VehiclePassengerCounts>();

                    foreach (var request in lstFullCars)
                    {
                        passengers = _vehiclePassengerService.GetVehiclePassengers(request.Id);

                        int countOfReturns = passengers.Count();

                        foreach (var passenger in passengers)
                        {
                            VehiclePassengerCounts vhc = new VehiclePassengerCounts()
                            {
                                IdRequestFK = request.Id,
                                Plate = request.Vehicle.Plate,
                                CountPercentage = countOfReturns.ToString() + "/" + request.Vehicle.Capacity,
                                IsGoing = false
                            };

                            returns.Add(vhc);
                        }
                    }

                    returns = returns.DistinctBy(x => x.Plate).ToList();

                    resultJs += $@"$('#IdReturnVehicleFK').html('');";

                    string optionsReturner = "";
                    optionsReturner = $@"<option value=""0"">Seçiniz...</option>";

                    foreach (var item in returns)
                        optionsReturner += $@"<option value=""{item.IdRequestFK}"">{item.Plate} ({item.CountPercentage})</option>";

                    resultJs += $@"$('#IdReturnVehicleFK').html('{optionsReturner}');";
                }
            }

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/SaveDepartureVehicleRequest")]
        public IActionResult SaveDepartureVehicleRequest(SaveVehicleRequestDto saveVehicleRequestDto)
        {
            string resultJs = "";
            Guid idVehicle = Guid.Empty;

            Mission mission = _missionService.GetEntityById(saveVehicleRequestDto.IdMissionFK.Value);

            if (mission == null)
                return BadRequest();

            if (mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller)
                return Ok("ShowErrorMessage('Görevin aracı kurum aracı ile ilişkilendirilmemiş.')");

            bool isDriver = mission.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicle ? true : false;

            saveVehicleRequestDto.IdEmployeeFK = SessionHelper.GetEmployeeLoggingIn(HttpContext).UserGid;
            saveVehicleRequestDto.Description = mission.Subject;
            saveVehicleRequestDto.RequestType = EnumRequestType.Mission;
            saveVehicleRequestDto.IsGoing = true;
            saveVehicleRequestDto.IdVehicleFK = isDriver ? saveVehicleRequestDto.IdVehicleFK : _vehicleRequestService.GetVehicleRequest(saveVehicleRequestDto.IdVehicleFK).Vehicle.Id;

            List<VehicleRequestDto> requests = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            using (TransactionScope scope = new TransactionScope())
            {
                if (requests.Where(x => x.IsGoing == true).FirstOrDefault() != null)
                {
                    VehicleRequest request = _vehicleRequestService.GetEntityById(requests.Where(x => x.IsGoing == true).FirstOrDefault().Id);

                    request.DataType = EnumDataType.Deleted;

                    _vehicleRequestService.UpdateEntity(request);
                }

                var result = _vehicleRequestService.Insert(saveVehicleRequestDto);

                if (result == null)
                {
                    return BadRequest();
                }

                idVehicle = result.Result.IdVehicleFK;

                SaveVehiclePassengerDto saveVehiclePassengerDto = new SaveVehiclePassengerDto()
                {
                    IdEmployeeFK = mission.IdEmployeeFK,
                    IdVehicleRequestFK = isDriver ? result.Result.Id : saveVehicleRequestDto.IdVehicleFK,
                    IsDriver = isDriver,
                };

                _vehiclePassengerService.Insert(saveVehiclePassengerDto);

                scope.Complete();

            }

            resultJs += $"$('#currentDepartureCar').text('({_vehicleService.GetById(idVehicle).Plate})');";
            resultJs += "ShowSuccessMessage('Gidiş Aracı başarıyla eklendi.')";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/SaveReturnVehicleRequest")]
        public IActionResult SaveReturnVehicleRequest(SaveVehicleRequestDto saveVehicleRequestDto)
        {
            string resultJs = "";
            Guid vehicleId = Guid.Empty;

            Mission mission = _missionService.GetEntityById(saveVehicleRequestDto.IdMissionFK.Value);

            if (mission == null)
                return BadRequest();

            if (mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicle && mission.ReturnVehicle != EnumReturnVehicle.InstitutionalVehicleTraveller)
                return Ok("ShowErrorMessage('Görevin aracı kurum aracı ile ilişkilendirilmemiş.')");

            bool isDriver = mission.ReturnVehicle == EnumReturnVehicle.InstitutionalVehicle ? true : false;

            saveVehicleRequestDto.IdEmployeeFK = SessionHelper.GetEmployeeLoggingIn(HttpContext).UserGid;
            saveVehicleRequestDto.Description = mission.Subject;
            saveVehicleRequestDto.RequestType = EnumRequestType.Mission;
            saveVehicleRequestDto.IsGoing = false;
            saveVehicleRequestDto.IdVehicleFK = isDriver ? saveVehicleRequestDto.IdVehicleFK : _vehicleRequestService.GetVehicleRequest(saveVehicleRequestDto.IdVehicleFK).Vehicle.Id;

            List<VehicleRequestDto> requests = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            using (TransactionScope scope = new TransactionScope())
            {
                if (requests.Where(x => x.IsGoing == false).FirstOrDefault() != null)
                {
                    VehicleRequest request = _vehicleRequestService.GetEntityById(requests.Where(x => x.IsGoing == false).FirstOrDefault().Id);

                    request.DataType = EnumDataType.Deleted;

                    _vehicleRequestService.UpdateEntity(request);
                }

                var result = _vehicleRequestService.Insert(saveVehicleRequestDto);

                if (result == null)
                {
                    return BadRequest();
                }

                vehicleId = result.Result.IdVehicleFK;

                SaveVehiclePassengerDto saveVehiclePassengerDto = new SaveVehiclePassengerDto()
                {
                    IdEmployeeFK = mission.IdEmployeeFK,
                    IdVehicleRequestFK = isDriver ? result.Result.Id : saveVehicleRequestDto.IdVehicleFK,
                    IsDriver = isDriver,
                };

                _vehiclePassengerService.Insert(saveVehiclePassengerDto);

                scope.Complete();
            }

            resultJs += $"$('#currentReturnCar').text('({_vehicleService.GetById(vehicleId).Plate})');";
            resultJs += "ShowSuccessMessage('Dönüş Aracı başarıyla eklendi.')";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Missions/SaveDepartureReturnVehicleRequest")]
        public IActionResult SaveDepartureReturnVehicleRequest(SaveVehicleRequestDto saveVehicleRequestDto)
        {
            string resultJs = "";
            Guid idVehicle = Guid.Empty;

            Mission mission = _missionService.GetEntityById(saveVehicleRequestDto.IdMissionFK.Value);

            if (mission == null)
                return BadRequest();

            if ((mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicle) &&
                (mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller && mission.DepartureVehicle != EnumDepartureVehicle.InstitutionalVehicleTraveller))
                return Ok("ShowErrorMessage('Görevin aracı kurum aracı ile ilişkilendirilmemiş. Bu yüzden ikisini aynı araçla aynı tarihlerde seçemezsiniz.')");

            bool isDriver = mission.DepartureVehicle == EnumDepartureVehicle.InstitutionalVehicle ? true : false;

            saveVehicleRequestDto.IdEmployeeFK = SessionHelper.GetEmployeeLoggingIn(HttpContext).UserGid;
            saveVehicleRequestDto.Description = mission.Subject;
            saveVehicleRequestDto.RequestType = EnumRequestType.Mission;
            saveVehicleRequestDto.IsGoing = true;
            saveVehicleRequestDto.DateOfEnd = mission.DateOfEnd;
            saveVehicleRequestDto.DateOfStart = mission.DateOfStart;
            saveVehicleRequestDto.IdVehicleFK = isDriver ? saveVehicleRequestDto.IdVehicleFK : _vehicleRequestService.GetVehicleRequest(saveVehicleRequestDto.IdVehicleFK).Vehicle.Id;

            List<VehicleRequestDto> requests = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);

            using (TransactionScope scope = new TransactionScope())
            {
                if (requests.Where(x => x.IsGoing == true).FirstOrDefault() != null)
                {
                    VehicleRequest request = _vehicleRequestService.GetEntityById(requests.Where(x => x.IsGoing == true).FirstOrDefault().Id);

                    request.DataType = EnumDataType.Deleted;

                    _vehicleRequestService.UpdateEntity(request);
                }

                if (requests.Where(x => x.IsGoing == false).FirstOrDefault() != null)
                {
                    VehicleRequest request = _vehicleRequestService.GetEntityById(requests.Where(x => x.IsGoing == false).FirstOrDefault().Id);

                    request.DataType = EnumDataType.Deleted;

                    _vehicleRequestService.UpdateEntity(request);
                }

                var result = _vehicleRequestService.Insert(saveVehicleRequestDto);

                if (result == null)
                {
                    return BadRequest();
                }


                idVehicle = result.Result.IdVehicleFK;

                SaveVehiclePassengerDto saveVehiclePassengerDto = new SaveVehiclePassengerDto()
                {
                    IdEmployeeFK = mission.IdEmployeeFK,
                    IdVehicleRequestFK = isDriver ? result.Result.Id : saveVehicleRequestDto.IdVehicleFK,
                    IsDriver = isDriver,
                };

                _vehiclePassengerService.Insert(saveVehiclePassengerDto);

                saveVehicleRequestDto.IsGoing = false;

                var resultSecondData = _vehicleRequestService.Insert(saveVehicleRequestDto);

                if (result == null)
                {
                    return BadRequest();
                }

                idVehicle = result.Result.IdVehicleFK;

                SaveVehiclePassengerDto saveVehiclePassengerDtoData = new SaveVehiclePassengerDto()
                {
                    IdEmployeeFK = mission.IdEmployeeFK,
                    IdVehicleRequestFK = isDriver ? resultSecondData.Result.Id : saveVehicleRequestDto.IdVehicleFK,
                    IsDriver = isDriver,
                };

                _vehiclePassengerService.Insert(saveVehiclePassengerDtoData);

                scope.Complete();
            }

            resultJs += $"$('#IdDepartureReturnVehicleFK').val('({idVehicle})');";
            resultJs += "ShowSuccessMessage('Gidiş-Dönüş Aracı başarıyla eklendi. Ekranı kapatabilirsiniz.')";

            return Ok(resultJs);
        }


        [HttpPost]
        [Route("Missions/ExportDocument")]
        public IActionResult ExportDocument(Guid Id)
        {
            string resultJs = "";

            MissionDto mission = _missionService.GetMission(Id);

            if (mission != null)
            {
                List<Parameters> lstParams = new List<Parameters>();

                string nameSurname = mission.Employee.Name + " " + mission.Employee.Surname;
                string apellation = mission.Employee.Apellation == null ? "" : mission.Employee.Apellation; //Değişecek
                string delegator = mission.Proxy.Name + " " + mission.Proxy.Surname;
                string presidentOfDepartment = mission.Employee.Department.Employee.Name + " " + mission.Employee.Department.Employee.Surname;
                string presidentOfDepartmentApellation = mission.Employee.Department.Employee.Apellation;

                TimeSpan minus = mission.DateOfEnd - mission.DateOfStart;
                int day = (int)minus.TotalDays;

                string howManyDays = day.ToString();
                string startDate = mission.DateOfStart.ToString("dd MMMM yyyy HH:mm");
                string endDate = mission.DateOfEnd.ToString("dd MMMM yyyy HH:mm");
                string missionType = TypeTR(mission.MissionType);

                string subject = mission.Subject;
                string note = mission.Notes;
                string area = mission.Area;
                string departureVehicle = CarDepartureTypeTR(mission.DepartureVehicle);
                string returnVehicle = CarReturnTypeTR(mission.ReturnVehicle);

                lstParams.Add(new Parameters() { key = "NameSurname", value = nameSurname });
                lstParams.Add(new Parameters() { key = "Apellation", value = apellation });
                lstParams.Add(new Parameters() { key = "Type", value = missionType });
                lstParams.Add(new Parameters() { key = "Area", value = area });
                lstParams.Add(new Parameters() { key = "StartDate", value = startDate });
                lstParams.Add(new Parameters() { key = "EndDate", value = endDate });
                lstParams.Add(new Parameters() { key = "HowManyDays", value = howManyDays + " gün" });
                lstParams.Add(new Parameters() { key = "WhichVehicles", value = departureVehicle + "(Gidiş) - " + returnVehicle + "(Dönüş)" });
                lstParams.Add(new Parameters() { key = "Subject", value = subject });
                lstParams.Add(new Parameters() { key = "Notes", value = note == null ? "" : note });
                lstParams.Add(new Parameters() { key = "Delegate", value = delegator });
                lstParams.Add(new Parameters() { key = "Chief", value = presidentOfDepartment });
                lstParams.Add(new Parameters() { key = "ApellationChief", value = presidentOfDepartmentApellation });
                lstParams.Add(new Parameters() { key = "DocumentId", value = mission.DocumentId });

                FastReportClass fstReport = new FastReportClass(_webHostEnvironment);

                string resultReport = fstReport.GetReport("MissionDocument.frx", nameSurname.Replace(" ", "") + "_" + DateTime.Now.ToString("dd.MM.yyyy.HH.mm"), "MissionInfos", "", "Document", "MissionDocument", lstParams, Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/logo.png"));

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

        public static string CarDepartureTypeTR(EnumDepartureVehicle type)
        {
            switch (type)
            {
                case EnumDepartureVehicle.Other:
                    return "Diğer";
                case EnumDepartureVehicle.InstitutionalVehicle:
                    return "Kurum Aracı";
                case EnumDepartureVehicle.InstitutionalVehicleTraveller:
                    return "Kurum Aracı Yolcusu";
                case EnumDepartureVehicle.OtherRoadVehicles:
                    return "Diğer Kara Aracı";
                case EnumDepartureVehicle.Plane:
                    return "Uçak";
                default:
                    return "";
            }
        }

        public static string CarReturnTypeTR(EnumReturnVehicle type)
        {
            switch (type)
            {
                case EnumReturnVehicle.Other:
                    return "Diğer";
                case EnumReturnVehicle.OtherRoadVehicles:
                    return "Diğer Kara Aracı";
                case EnumReturnVehicle.InstitutionalVehicle:
                    return "Kurum Aracı";
                case EnumReturnVehicle.InstitutionalVehicleTraveller:
                    return "Kurum Aracı Yolcusu";
                case EnumReturnVehicle.Plane:
                    return "Uçak";
                default:
                    return "";
            }
        }

        public static string StateTR(EnumState state)
        {
            switch (state)
            {
                case EnumState.Accepted:
                    return "Kabul Edildi";
                case EnumState.Pending:
                    return "Beklemede";
                case EnumState.Declined:
                    return "Reddedildi";
                default:
                    return "";
            }
        }

        // Other methods as needed
        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Missions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a><a onclick=\"AjaxMethod(&apos;Missions/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Missions/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
        public const string htmlCodeWithCar = "<a onclick=\"AjaxMethod(&apos;Missions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a><a onclick = \"AjaxMethod(&apos;Missions/OpenModalSelectCar&apos;, &apos;{0}&apos;, &apos;OpenModalSelectCar&apos;)\" href=\"\"><i class=\"mdi mdi-car text-primary md20\"></i></a><a onclick=\"AjaxMethod(&apos;Missions/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Missions/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}

