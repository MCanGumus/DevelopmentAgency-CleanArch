using AutoMapper;
using ClosedXML.Excel;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Enums;
using DA.Models;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Transactions;
using System.Windows.Forms;

namespace DA.Controllers.PermissionModule
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class AllPermissionsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SavePermissionDto> _saveValidator;
        private readonly IValidator<UpdatePermissionDto> _updateValidator;
        private readonly IPermissionService _permissionService;
        private readonly IEmployeeService _employeeService;
        private readonly IPublicHolidayService _publicHolidayService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AllPermissionsController(IMapper mapper,
            IValidator<SavePermissionDto> saveValidator,
            IValidator<UpdatePermissionDto> updateValidator,
            IPermissionService permissionService,
            IEmployeeService employeeService,
            IPublicHolidayService publicHolidayService,
            IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _permissionService = permissionService;
            _employeeService = employeeService;
            _publicHolidayService = publicHolidayService;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult AllPermissions()
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
                if (loginnedEmployee.DepartmentHeadGid == null)
                {
                    if (employee.AuthorizationStatus == EnumAuthorizationStatus.SuperAdmin)
                    {
                        return Redirect("/IzinRaporlari");
                    }

                    return Redirect("/Anasayfa");
                }

            }
            catch (Exception)
            {
                return Redirect("/");
            }

            ListAllPermission(DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(3));

            return View();
        }


        [HttpPost]
        [Route("AllPermissions/ListAllPermission")]
        public IActionResult ListAllPermission(DateTime startDate, DateTime endDate)
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            //Başkanı olduğu departmandan çağırman gerek.

            List<PermissionDto> lstPermission = _permissionService.GetAllPermissionsByDepartment(loginnedEmployee.DepartmentHeadGid.Value, startDate, endDate).OrderByDescending(x => x.RecordDate).ToList();

            if (loginnedEmployee.DepartmentHelperGid != null)
            {
                lstPermission.AddRange(_permissionService.GetAllPermissionsByDepartment(loginnedEmployee.DepartmentHelperGid.Value, startDate, endDate).OrderByDescending(x => x.RecordDate).ToList());
            }

            if (loginnedEmployee.DepartmentAppointments.Count != 0)
            {
                foreach (var item in loginnedEmployee.DepartmentAppointments)
                {
                    lstPermission.AddRange(_permissionService.GetAllPermissionsByDepartment(item, startDate, endDate).OrderByDescending(x => x.RecordDate).ToList());
                }
            }
            
            List<PublicHolidayDto> lstPublicHolidays = _publicHolidayService.GetAll().ToList();

            PermissionsWithHolidays permissionsWithHolidays = new PermissionsWithHolidays();

            permissionsWithHolidays.Permissions = lstPermission.OrderBy(x => x.State).ToList();
            permissionsWithHolidays.Holidays = lstPublicHolidays;
            permissionsWithHolidays.StartDate = startDate;
            permissionsWithHolidays.EndDate = endDate;

            return PartialView("_ListPartialView", permissionsWithHolidays);
        }

        [HttpPost]
        [Route("AllPermissions/Accept")]
        public IActionResult Accept(Guid Id, EnumPermissionType PermissionType)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Permission permission = _permissionService.GetPermission(Id);

            if (permission != null)
            {
                if (loginnedEmployee.UserGid == permission.IdEmployeeFK)
                {
                    return Ok("ShowErrorMessage('Kendi izninizi onaylayamazsınız.');");
                }

                permission.State = EnumState.Accepted;
                permission.IdDelegateFK = loginnedEmployee.UserGid;
                permission.PermissionType = PermissionType;

                Employee employee = _employeeService.GetEntityById(permission.IdEmployeeFK);

                if (employee == null)
                    return BadRequest();

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


                using (TransactionScope scope = new TransactionScope())
                {

                    switch (PermissionType)
                    {
                        case EnumPermissionType.Normal:

                            if (employee.TotalUnpaidLeave < day)
                                return Ok("ShowErrorMessage('Kişinin yeteri kadar ücretsiz izin günü kalmamış.');");

                            employee.TotalUnpaidLeave -= day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));

                            break;

                        case EnumPermissionType.Yearly:

                            if (employee.TotalYearlyLeave < day)
                                return Ok("ShowErrorMessage('Kişinin yeteri kadar yıllık izin günü kalmamış.');");

                            employee.TotalYearlyLeave -= day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));

                            break;

                        case EnumPermissionType.Excused:

                            if (employee.TotalExcusedLeave < day)
                                return Ok("ShowErrorMessage('Kişinin yeteri kadar mazaret izin günü kalmamış.');");

                            employee.TotalExcusedLeave -= day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));
                            break;

                        case EnumPermissionType.Equalization:

                            if (employee.TotalEqualizationLeave < day)
                                return Ok("ShowErrorMessage('Kişinin yeteri kadar denkleştirme izin günü kalmamış.');");

                            employee.TotalEqualizationLeave -= day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));
                            break;

                        default:
                            break;
                    }


                    _permissionService.Update(_mapper.Map<UpdatePermissionDto>(permission));

                    scope.Complete();
                }


                List<string> datas = new List<string>();

                datas.Add(permission.Employee.Name + " " + permission.Employee.Surname);
                datas.Add(permission.Department.Name);
                datas.Add(permission.StartDate.ToString("dd.MM.yyyy HH:mm"));
                datas.Add(permission.EndDate.ToString("dd.MM.yyyy HH:mm"));
                datas.Add(day.ToString() + " gün");
                datas.Add(permission.PermissionReason);
                datas.Add(permission.PermissionAddress);
                datas.Add(StateTR(permission.State));
                datas.Add(string.Format(htmlCodeAccept, permission.Id));

                resultJs += TableTransactions.UpdateTable(datas, permission.Id);

                try
                {
                    string employeeMail = permission.Employee.Email;
                    string subject = Constants.ProjectShortName + " İzin Belgeniz hk.";
                    string body = $"Merhaba {permission.Employee.Name + " " + permission.Employee.Surname},\n\n{permission.DocumentId} belge numaralı {permission.StartDate} - {permission.EndDate} tarihli izin belgeniz," +
                        $" {loginnedEmployee.UserName} tarafından onaylanmıştır.\n Bilginize.\n Buraya tıklayarak <a href="+ Constants.ProjectURL + "/Izinlerim'>İzinlerim</a> sayfasına ilerleyebilirsiniz.";

                    MailSenderService.SendEmail(employeeMail, subject, body);

                    string allMail = Constants.SystemMailAddress;
                    string allMailSubject = $"{permission.Employee.Name + " " + permission.Employee.Surname} Yıllık İzin Kullanımı hk.";
                    string allMailBody = $"Merhaba, \n Ajans çalışanlarımızdan {permission.Employee.Name + " " + permission.Employee.Surname}, {permission.StartDate} - {permission.EndDate} tarihleri arasında yıllık izin kullanıyor olacaktır.\n Bilginize.";

                    MailSenderService.SendEmail(allMail, allMailSubject, allMailBody);

                    resultJs += "ShowSuccessMessage('Başarıyla kabul edildi. Kişinin mail adresine bildirim yapıldı.');";
                }
                catch (Exception ex)
                {
                    resultJs += "ShowWarningMessage('Başarıyla kabul edildi ancak kişiye bildirim maili atarken bir hata oluştu. Lütfen sistem yöneticisine haber verin.');";
                }

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("AllPermissions/Reject")]
        public IActionResult Reject(Guid Id, string rejectReason)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Permission permission = _permissionService.GetPermission(Id);

            if (permission != null)
            {
                if (loginnedEmployee.UserGid == permission.IdEmployeeFK)
                {
                    return Ok("ShowErrorMessage('Kendi izninizi reddedemezsiniz.');");
                }

                permission.State = EnumState.Declined;
                permission.IdDelegateFK = loginnedEmployee.UserGid;
                permission.RejectReason = rejectReason;

                Employee employee = _employeeService.GetEntityById(permission.IdEmployeeFK);

                if (employee == null)
                    return BadRequest();

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

                using (TransactionScope scope = new TransactionScope())
                {

                    switch (permission.PermissionType)
                    {
                        case EnumPermissionType.Normal:

                            employee.TotalUnpaidLeave += day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));

                            break;

                        case EnumPermissionType.Yearly:

                            employee.TotalYearlyLeave += day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));

                            break;

                        case EnumPermissionType.Excused:

                            employee.TotalExcusedLeave += day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));
                            break;

                        case EnumPermissionType.Equalization:

                            employee.TotalEqualizationLeave += day;

                            _employeeService.Update(_mapper.Map<UpdateEmployeeDto>(employee));
                            break;

                        default:
                            break;
                    }


                    _permissionService.Update(_mapper.Map<UpdatePermissionDto>(permission));

                    scope.Complete();
                }


                List<string> datas = new List<string>();

                datas.Add(permission.Employee.Name + " " + permission.Employee.Surname);
                datas.Add(permission.Department.Name);
                datas.Add(permission.StartDate.ToString("dd.MM.yyyy HH:mm"));
                datas.Add(permission.EndDate.ToString("dd.MM.yyyy HH:mm"));
                datas.Add(day.ToString() + " gün");
                datas.Add(permission.PermissionReason);
                datas.Add(permission.PermissionAddress);
                datas.Add(StateTR(permission.State));
                datas.Add(string.Format(htmlCodeReject, permission.Id));

                resultJs += TableTransactions.UpdateTable(datas, permission.Id);

                try
                {
                    string employeeMail = permission.Employee.Email;
                    string subject = "Yıllık İzin Onay Durumu hk.";
                    string body = $"Merhaba {permission.Employee.Name + " " + permission.Employee.Surname},\n\n{permission.DocumentId} belge numaralı {permission.StartDate} - {permission.EndDate} tarihli izin belgeniz," +
                         $" {loginnedEmployee.UserName} tarafından reddedilmiştir.\n Reddedilme notu: {permission.RejectReason}.\n Bilginize.\n Buraya tıklayarak <a href="+ Constants.ProjectURL + "/Izinlerim'>İzinlerim</a> sayfasına ilerleyebilirsiniz.";

                    MailSenderService.SendEmail(employeeMail, subject, body);

                    resultJs += "ShowSuccessMessage('Başarıyla reddedildi. Kişinin mail adresine bildirim yapıldı.');";
                }
                catch (Exception ex)
                {
                    resultJs += "ShowWarningMessage('Başarıyla reddedildi ancak kişiye bildirim maili atarken bir hata oluştu. Lütfen sistem yöneticisine haber verin.');";
                }

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("AllPermissions/OpenDetail")]
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
                resultJs += $"$('#Title').text('{permission.Employee.Name} {permission.Employee.Surname}');";
            }
            if (permission.Department != null)
            {
                resultJs += $"$('#sDepartment').text('{permission.Department.Name}');";
            }
            if (permission.Delegate != null)
            {
                resultJs += $"$('#sDelegate').text('{permission.Delegate.Name} {permission.Delegate.Surname}');";
            }


            resultJs += $"$('#ModalDetailPermission').modal('show');";

            return Ok(resultJs);
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

        [HttpPost]
        [Route("AllPermissions/ExcelExportReportDepartment")]
        public IActionResult ExcelExportReport(DateTime startDate, DateTime endDate)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<PermissionDto> allPermissions = _permissionService.GetAllPermissionsByDepartment(loginnedEmployee.DepartmentGid, startDate, endDate);

            System.Data.DataTable permissions = new System.Data.DataTable();

            permissions.Columns.Add("Belge Numarası");
            permissions.Columns.Add("İzin Tipi");
            permissions.Columns.Add("Çalışan");
            permissions.Columns.Add("İzin Nedeni");
            permissions.Columns.Add("İzin Adresi");
            permissions.Columns.Add("Açıklama");
            permissions.Columns.Add("İzin Başlangıç Tarihi");
            permissions.Columns.Add("İşe Başlama Tarihi");
            permissions.Columns.Add("Gün Sayısı");
            permissions.Columns.Add("Durum");
            permissions.Columns.Add("Red Sebebi");

            int index = 0;
            int day = 0;

            var holidays = _publicHolidayService.GetAll().ToList();

            foreach (var item in holidays)
            {
                if (item.IsNationalHoliday)
                {
                    item.Date = new DateTime(DateTime.Now.Year, item.Date.Month, item.Date.Day);
                }
            }

            foreach (PermissionDto permission in allPermissions)
            {
                #region How many days?
                day = 0;

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

                index = 0;
                System.Data.DataRow rowExcel = permissions.NewRow();

                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.DocumentId);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(PermissionController.TypeTR(permission.PermissionType));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.Employee.Name + " " + permission.Employee.Surname);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.PermissionReason);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.PermissionAddress);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.Description);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.StartDate.ToString("dd.MM.yyyy HH:mm"));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.EndDate.ToString("dd.MM.yyyy HH:mm"));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(day.ToString());
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(PermissionController.StateTR(permission.State));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(permission.RejectReason);
                permissions.Rows.Add(rowExcel);
            }

            string path = _webHostEnvironment.WebRootPath + "/Files/GeneralReports/Permissions";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region Excel Dosyasını Oluştur ve İndir

            string fileName = string.Format("{0}_{1}.xlsm", startDate.ToString("dd-MM-yyyy") + "_" + endDate.ToString("dd-MM-yyyy") + "_Izinler_", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            string filePath = path + "/" + fileName;
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(permissions, "Report");
            int ColumnRange = allPermissions.Count + 2;
            ws.Range("D2", "D" + ColumnRange).Style.Alignment.WrapText = true;
            ws.Columns(1, 20).AdjustToContents();
            ws.Column(4).Width = 150;
            wb.SaveAs(filePath);
            #endregion

            resultJs += string.Format("downloadURI('/Files/GeneralReports/Permissions/{0}');", fileName);

            return Ok(resultJs);
        }


        public const string htmlCodeAccept = "<a onclick=\"AjaxMethod(&apos;Permissions/ExportDocument&apos;, &apos;{0}&apos;, &apos;Print&apos;)\" href=\"\"><i class=\"mdi mdi-printer text-warning md20\"></i></a><a onclick=\"AjaxMethod(&apos;AllPermissions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a>";
        public const string htmlCodeReject = "<a onclick=\"AjaxMethod(&apos;AllPermissions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a>";
    }
}
