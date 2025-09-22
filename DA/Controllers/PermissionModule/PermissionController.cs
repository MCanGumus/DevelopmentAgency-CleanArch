using AutoMapper;
using DA.Components.System;
using DA.Application.Abstractions;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Persistence.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using DA.Domain.Enums;
using DA.Models;
using System.Windows.Forms;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DA.Controllers
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class PermissionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SavePermissionDto> _saveValidator;
        private readonly IValidator<UpdatePermissionDto> _updateValidator;
        private readonly IPermissionService _permissionService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPublicHolidayService _publicHolidayService;
        private readonly IAddressService _addressService;

        public PermissionController(IMapper mapper,
            IValidator<SavePermissionDto> saveValidator,
            IValidator<UpdatePermissionDto> updateValidator,
            IPermissionService permissionService,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IWebHostEnvironment webHostEnvironment,
            IPublicHolidayService publicHolidayService,
            IAddressService addressService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _permissionService = permissionService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _webHostEnvironment = webHostEnvironment;
            _publicHolidayService = publicHolidayService;
            _addressService = addressService;
        }

        public IActionResult Permission()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListPermission();
            return View();
        }

        public IActionResult ListPermission()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            PermissionsWithEmployees permissionsWithEmployees = new PermissionsWithEmployees();

            List<PermissionDto> lstPermission = _permissionService.GetAllPermissionsByEmployee(loginnedEmployee.UserGid).OrderByDescending(x => x.RecordDate).ToList();
            List<EmployeeDto> lstEmployees = _employeeService.GetAll().ToList();
            List<PublicHolidayDto> lstHolidays = _publicHolidayService.GetAll().ToList();
            List<AddressDto> lstAddresses = _addressService.GetAllMyAddresses(loginnedEmployee.UserGid).ToList();

            permissionsWithEmployees.Employees = lstEmployees;
            permissionsWithEmployees.Permissions = lstPermission;
            permissionsWithEmployees.Holidays = lstHolidays;
            permissionsWithEmployees.Addresses = lstAddresses;
            return View(permissionsWithEmployees);
        }

        [HttpPost]
        [Route("Permissions/Save")]
        public IActionResult Save(SavePermissionDto permission, string ExcusedLeaveSelect)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            permission.IdEmployeeFK = loginnedEmployee.UserGid;
            permission.IdDepartmentFK = loginnedEmployee.DepartmentGid;
            permission.State = EnumState.Pending;

            #region Check another permission same dates

            List<PermissionDto> permissionsOfUser =
                _permissionService.GetAllPermissionsByEmployee(loginnedEmployee.UserGid).Where(x => x.State != EnumState.Declined).ToList();

            foreach (PermissionDto item in permissionsOfUser)
            {
                for (DateTime date = item.StartDate.Date; date < item.EndDate.Date; date = date.AddDays(1))
                {
                    if (permission.StartDate.Date == date || permission.EndDate.Date == date)
                    {
                        return Ok($@"ShowErrorMessage(""Seçtiğiniz gün aralığıyla daha önce kaydettiğiniz başka bir izniniz çakışıyor. Lütfen izinlerinizi kontrol ediniz. Önceki izni ya silin, ya da güncelleyin."");");
                    }
                }
            }

            #endregion

            Random random = new Random();

            string strDocumentCode;

            #region Create barcode and check it
            PermissionDto isThere = null;

            do
            {
                string dateTime = DateTime.Now.ToString("ddMMyyyy");
                int rndNumberSecondSix = random.Next(1, 99999);

                strDocumentCode = dateTime + rndNumberSecondSix.ToString().PadLeft(5, '0');
                isThere = _permissionService.GetPermissionByDocumentId(strDocumentCode);
            } while (isThere != null);

            #endregion

            permission.DocumentId = strDocumentCode;
            permission.PermissionType = EnumPermissionType.Yearly;

            int day = 0;

            var holidays = _publicHolidayService.GetAll().ToList();
            
            foreach (var item in holidays)
            {
                if (item.IsNationalHoliday)
                {
                    item.Date = new DateTime(DateTime.Now.Year, item.Date.Month, item.Date.Day);
                }
            }

            for (DateTime date = permission.StartDate; date < permission.EndDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (!holidays.Any(x => x.Date == date))
                    {
                        day++;
                    }
                }
            }

            switch (permission.PermissionType)
            {
                case EnumPermissionType.Normal:
                    permission.ExcusedLeave = "";
                    break;
                case EnumPermissionType.Yearly:
                    permission.ExcusedLeave = "";
                        break;
                case EnumPermissionType.Excused:

                    if (day > Convert.ToInt16(ExcusedLeaveSelect))
                        return Ok($@"ShowErrorMessage(""Seçtiğiniz gün aralığı mazaret izni için ayrılan gün sayısından {day} gün daha fazladır. Lütfen tekrar deneyin."");");

                    permission.PermissionReason = permission.ExcusedLeave;
                    break;
                default:
                    break;
            }

            ValidationResult valResult = _saveValidator.Validate(permission);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                             .Replace("IdEmployeeFK", "Çalışan")
                                             .Replace("IdDepartmentFK", "Departman bilginiz girilmemiş. Yöneticinize başvurun.")
                                             .Replace("IdDelegateFK", "Yetkili")
                                             .Replace("PermissionReason", "İzin Sebebi")
                                             .Replace("ExcusedLeave", "Mazaretli İzin")
                                             .Replace("PermissionAddress", "İzin Adresi")
                                             .Replace("StartDate", "Başlangıç Tarihi")
                                             .Replace("EndDate", "Bitiş Tarihi")
                                             .Replace("Description", "Açıklama");

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
                    permission.State = EnumState.Accepted;
                }
            }

            var result = _permissionService.Insert(permission);

            if (result == null)
            {
                return BadRequest();
            }

            try
            {
                EmployeeDto employee = _employeeService.GetById(loginnedEmployee.UserGid);
                Department department = _departmentService.GetDepartmentWithEmployee(loginnedEmployee.DepartmentGid);

                string employeeMail = department.Employee.Email;
                string subject = "İzin Onay Talebi hk.";
                string body = $"Merhaba,\n Personel Bilgi Sisteminde onay ekranında işlem yapmanızı bekleyen {employee.Name + " " + employee.Surname} adlı personele ait izin bulunmaktadır.\n Bilginize.\n Buraya tıklayarak <a href="+ Constants.ProjectURL + "/Izinler'>Birim İzinleri</a> sayfasına ilerleyebilirsiniz.";

                MailSenderService.SendEmail(employeeMail, subject, body);

            }
            catch (Exception ex)
            {
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.DocumentId);
            datas.Add(result.Result.PermissionReason);
            datas.Add(result.Result.StartDate.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(result.Result.EndDate.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(day.ToString() + " gün");
            datas.Add(StateTR(result.Result.State));
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);
            resultJs += "$('#ModalPermission').modal('hide');";
            resultJs += "ShowSuccessMessage('İzin bilgisi başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Permissions/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            Permission permission = _permissionService.GetEntityById(guid);

            //switch (permission.PermissionType)
            //{
            //    case EnumPermissionType.Normal:

            //        resultJs += $"$('#uPermissionReason').prop('disabled', false);";
            //        resultJs += $"$('#uPermissionReason').val('');";
            //        resultJs += $"$('#uDiv_ExcusedLeave').css('display', 'none');";
            //        resultJs += $"$('#uExcusedLeave').val('');";
            //        resultJs += $"$('#uExcusedLeaveSelect').val('0').trigger('change');";
            //        resultJs += $"$('#uNormalPermission').prop('checked', true);";
            //        break;

            //    case EnumPermissionType.Yearly:

            //        resultJs += $"$('#uPermissionReason').prop('disabled', true);";
            //        resultJs += $"$('#uPermissionReason').val('');";
            //        resultJs += $"$('#uDiv_ExcusedLeave').css('display', 'none');";
            //        resultJs += $"$('#uExcusedLeave').val('');";
            //        resultJs += $"$('#uExcusedLeaveSelect').val('0').trigger('change');";
            //        resultJs += $"$('#uYearlyPermission').prop('checked', true);";

            //        break;

            //    case EnumPermissionType.Excused:

            //        resultJs += $"$('#uPermissionReason').prop('disabled', true);";
            //        resultJs += $"$('#uPermissionReason').val('');";
            //        resultJs += $"$('#uDiv_ExcusedLeave').css('display', '');";
            //        resultJs += $"$('#uExcusedLeave').val('');";
            //        resultJs += $"$('#uExcusedLeaveSelect').val('0').trigger('change');";
            //        resultJs += $"$('#uExcusedPermission').prop('checked', true);";

            //        break;

            //    default:
            //        break;
            //}


            resultJs += $"$('#uPermissionReason').val('{permission.PermissionReason}');";
            resultJs += $"$('#uPermissionAddress').val('{permission.PermissionAddress}');";
            resultJs += $"$('#uIdProxyFK').val('{permission.IdProxyFK}').trigger('change');";
            resultJs += $"$('#uExcusedLeave').val('{permission.ExcusedLeave}');";
            resultJs += $"$('#uStartDate').val('{permission.StartDate.ToString("yyyy-MM-dd") + "T" + permission.StartDate.ToString("HH:mm")}');";
            resultJs += $"$('#uEndDate').val('{permission.EndDate.ToString("yyyy-MM-dd") + "T" + permission.EndDate.ToString("HH:mm")}');";
            resultJs += $"$('#uDescription').val('{permission.Description}');";
            resultJs += $"$('#uId').val('{permission.Id}');";
            resultJs += $"$('#Title').text('{permission.PermissionReason}');";

            resultJs += $"$('#ModalUpdatePermission').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Permissions/OpenDetail")]
        public IActionResult OpenDetails(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            Permission permission = _permissionService.GetPermission(guid);

            if (permission == null)
                return BadRequest();

            resultJs += $"$('#sPermissionReason').text('{permission.PermissionReason}');";
            resultJs += $"$('#sPermissionAddress').text('{permission.PermissionAddress}');";
            resultJs += $"$('#sExcusedLeave').text('{permission.ExcusedLeave}');";
            resultJs += $"$('#sStartDate').text('{permission.StartDate.ToString("dd.MM.yyyy HH:mm")}');";
            resultJs += $"$('#sEndDate').text('{permission.EndDate.ToString("dd.MM.yyyy HH:mm")}');";
            resultJs += $"$('#sState').text('{StateTR(permission.State)}');";
            resultJs += $"$('#sDescription').text('{permission.Description}');";
            resultJs += $"$('#sRejectReason').text('{permission.RejectReason}');";

            if (permission.Employee != null)
            {
                resultJs += $"$('#sEmployee').text('{permission.Employee.Name} {permission.Employee.Surname}');";
            }
            if (permission.Department != null)
            {
                resultJs += $"$('#sDepartment').text('{permission.Department.Name}');";
            }
            if (permission.Delegate != null)
            {
                resultJs += $"$('#sDelegate').text('{permission.Delegate.Name} {permission.Delegate.Surname}');";
            }
            if (permission.Proxy != null)
            {
                resultJs += $"$('#sProxy').text('{permission.Proxy.Name} {permission.Proxy.Surname}');";
            }

            resultJs += $"$('#Title').text('{permission.PermissionReason}');";
            resultJs += $"$('#ModalDetailPermission').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Permissions/Update")]
        public IActionResult Update(UpdatePermissionDto permission, string ExcusedLeaveSelect)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            permission.IdEmployeeFK = loginnedEmployee.UserGid;
            permission.IdDepartmentFK = loginnedEmployee.DepartmentGid;

            #region How many days?
            int day = 0;

            var holidays = _publicHolidayService.GetAll().ToList();

            foreach (var item in holidays)
            {
                if (item.IsNationalHoliday)
                {
                    item.Date = new DateTime(DateTime.Now.Year, item.Date.Month, item.Date.Day);
                }
            }

            for (DateTime date = permission.StartDate; date < permission.EndDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (!holidays.Any(x => x.Date == date))
                    {
                        day++;
                    }
                }
            }
            #endregion

            switch (permission.PermissionType)
            {
                case EnumPermissionType.Normal:
                    permission.ExcusedLeave = "";
                    break;
                case EnumPermissionType.Yearly:
                    permission.PermissionReason = "Yıllık İzin";
                    break;
                case EnumPermissionType.Excused:

                    if (day > Convert.ToInt16(ExcusedLeaveSelect))
                        return Ok($@"ShowErrorMessage(""Seçtiğiniz gün aralığı mazaret izni için ayrılan gün sayısından {day} gün daha fazladır. Lütfen tekrar deneyin."");");

                    permission.PermissionReason = permission.ExcusedLeave;
                    break;
                default:
                    break;
            }

            ValidationResult valResult = _updateValidator.Validate(permission);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdEmployeeFK", "Çalışan")
                                            .Replace("IdDepartmentFK", "Departman bilginiz girilmemiş. Yöneticinize başvurun.")
                                            .Replace("IdDelegateFK", "Yetkili")
                                            .Replace("PermissionReason", "İzin Sebebi")
                                            .Replace("ExcusedLeave", "Mazaretli İzin")
                                            .Replace("PermissionAddress", "İzin Adresi")
                                            .Replace("StartDate", "Başlangıç Tarihi")
                                            .Replace("EndDate", "Bitiş Tarihi")
                                            .Replace("Description", "Açıklama");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            Permission permissionDto = _permissionService.GetEntityById(permission.Id);

            if (permissionDto == null)
                return BadRequest();

            permissionDto.PermissionReason = permission.PermissionReason;
            permissionDto.IdProxyFK = permission.IdProxyFK;
            permissionDto.PermissionAddress = permission.PermissionAddress;
            permissionDto.StartDate = permission.StartDate;
            permissionDto.EndDate = permission.EndDate;
            permissionDto.State = EnumState.Pending;
            permissionDto.Description = permission.Description;
            permissionDto.PermissionType = permission.PermissionType;

            _permissionService.UpdateEntity(permissionDto);

            List<string> datas = new List<string>();

            datas.Add(permissionDto.DocumentId);
            datas.Add(permissionDto.PermissionReason);
            datas.Add(permissionDto.StartDate.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(permissionDto.EndDate.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(day.ToString() + " gün");
            datas.Add(StateTR(permissionDto.State));
            datas.Add(string.Format(htmlCode, permission.Id));

            resultJs += TableTransactions.UpdateTable(datas, permission.Id);

            resultJs += "$('#ModalUpdatePermission').modal('hide');";
            resultJs += "ShowSuccessMessage('İzin bilgisi başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Permissions/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            Permission permission = _permissionService.GetEntityById(Id);

            if (permission != null)
            {
                permission.DataType = Domain.Enums.EnumDataType.Deleted;
                _permissionService.UpdateEntity(permission);

                resultJs += TableTransactions.DeleteTable(permission.Id);

                resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Permissions/ExportDocument")]
        public IActionResult ExportDocument(Guid Id)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Permission permission = _permissionService.GetPermission(Id);

            string nameSurname = permission.Employee.Name + " " + permission.Employee.Surname;

            string filePath = nameSurname.Replace(" ", "") + "_" + permission.DocumentId + ".pdf";
            string existFolderPath = _webHostEnvironment.WebRootPath + "/Files/Reports/" + filePath;

            if (System.IO.File.Exists(existFolderPath))
            {
                resultJs += string.Format("window.open('Files/Reports/{0}', '_blank');", filePath);

                resultJs += "ShowSuccessMessage('Başarılı.');";

                return Ok(resultJs);
            }


            if (permission != null)
            {
                List<Parameters> lstParams = new List<Parameters>();

                
                string apellation = permission.Employee.Apellation == null ? "" : permission.Employee.Apellation; //Değişecek
                string department = permission.Department.Name;
                string presidentOfDepartment = permission.Department.Employee.Name + " " + permission.Department.Employee.Surname;
                string presidentOfDepartmentApellation = permission.Department.Employee.Apellation;

                TimeSpan seniorityTotal = DateTime.Now - permission.Employee.DateOfStart;
                int seniorityDay = (int)seniorityTotal.TotalDays;

                int day = 0;


                var holidays = _publicHolidayService.GetAll().ToList();

                foreach (var item in holidays)
                {
                    if (item.IsNationalHoliday)
                    {
                        item.Date = new DateTime(DateTime.Now.Year, item.Date.Month, item.Date.Day);
                    }
                }

                for (DateTime date = permission.StartDate; date < permission.EndDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (!holidays.Any(x => x.Date == date))
                        {
                            day++;
                        }
                    }
                }

                string howManyDays = day.ToString();
                string seniority = seniorityDay.ToString();
                string totalPermission = (permission.Employee.TotalYearlyLeave + day).ToString();
                string startDate = permission.StartDate.ToString("dd MMMM yyyy HH:mm");
                string endDate = permission.EndDate.ToString("dd MMMM yyyy HH:mm");
                string proxy = permission.Proxy.Name + " " + permission.Proxy.Surname;

                string permissionType = "";

                switch (permission.PermissionType)
                {
                    case EnumPermissionType.Normal:
                        permissionType = "Ücretsiz İzin";
                        break;
                    case EnumPermissionType.Yearly:
                        permissionType = "Yıllık İzin";
                        break;

                    case EnumPermissionType.Excused:
                        permissionType = "Mazaret İzni";
                        break;

                    default:
                        permissionType = "Yıllık İzin";
                        break;
                }

                string remainingDay = permission.Employee.TotalYearlyLeave.ToString();
                string address = permission.PermissionAddress;
                string humanResources = "Talip NAMAL"; //Değişecek

                lstParams.Add(new Parameters() { key = "NameSurname", value = nameSurname });
                lstParams.Add(new Parameters() { key = "Apellation", value = apellation });
                lstParams.Add(new Parameters() { key = "Department", value = department });
                lstParams.Add(new Parameters() { key = "Seniority", value = seniority });
                lstParams.Add(new Parameters() { key = "TotalPermission", value = totalPermission });
                lstParams.Add(new Parameters() { key = "PermissionType", value = permissionType });
                lstParams.Add(new Parameters() { key = "StartDate", value = startDate });
                lstParams.Add(new Parameters() { key = "EndDate", value = endDate });
                lstParams.Add(new Parameters() { key = "HowManyDays", value = howManyDays });
                lstParams.Add(new Parameters() { key = "RemainingDays", value = remainingDay });
                lstParams.Add(new Parameters() { key = "Address", value = address });
                lstParams.Add(new Parameters() { key = "Delegate", value = proxy });
                lstParams.Add(new Parameters() { key = "HumanResources", value = humanResources });
                lstParams.Add(new Parameters() { key = "NameSurname2", value = nameSurname });
                lstParams.Add(new Parameters() { key = "Chief", value = presidentOfDepartment });
                lstParams.Add(new Parameters() { key = "ApellationChief", value = presidentOfDepartmentApellation });
                lstParams.Add(new Parameters() { key = "DocumentId", value = permission.DocumentId });

                FastReportClass fstReport = new FastReportClass(_webHostEnvironment);

                string resultReport = fstReport.GetReport("PermissionDocument.frx", nameSurname.Replace(" ", "") + "_" + permission.DocumentId, "PermissionInfos", "", "Document", "PermissionDocument", lstParams, Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/logo.png"));

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


        public static string TypeTR(EnumPermissionType PermissionType)
        {
            switch (PermissionType)
            {
                case EnumPermissionType.Normal:
                    return "Normal İzin";
                case EnumPermissionType.Yearly:
                    return "Yıllık İzin";
                case EnumPermissionType.Excused:
                    return "Mazaretli İzin";

                default:
                    return "Yıllık İzin";
            }

        }


        // Other methods as needed
        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Permissions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a><a onclick=\"AjaxMethod(&apos;Permissions/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Permissions/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
