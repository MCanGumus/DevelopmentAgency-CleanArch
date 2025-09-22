using ClosedXML.Excel;
using FastReport;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Enums;
using DA.Models;
using DA.Persistence.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace DA.Controllers.Reports
{
    public class PermissionReportController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmployeeService _employeeService;
        private readonly IPublicHolidayService _publicHolidayService;
        public PermissionReportController(IPermissionService permissionService, IWebHostEnvironment webHostEnvironment, IEmployeeService employeeService, IPublicHolidayService publicHolidayService)
        {
            _permissionService = permissionService;
            _webHostEnvironment = webHostEnvironment;
            _employeeService = employeeService;
            _publicHolidayService = publicHolidayService;
        }

        public IActionResult PermissionReport()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            try
            {
                Employee employee = _employeeService.GetEntityById(loginnedEmployee.UserGid);

                if (employee.AuthorizationStatus != EnumAuthorizationStatus.SuperAdmin)
                {
                    return Redirect("/YetkiYok");
                }
            }
            catch (Exception)
            {
                return Redirect("/");
            }
            ListPermissionReport(DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(3));

            return View();
        }

        [HttpPost]
        [Route("PermissionReport/PermissionReportWithFilter")]
        public IActionResult ListPermissionReport(DateTime startDate, DateTime endDate)
        {
            List<PermissionDto> allPermissions = _permissionService.GetAllPermissionsByFilter(startDate, endDate);

            List<PublicHolidayDto> lstPublicHolidays = _publicHolidayService.GetAll().ToList();

            PermissionsWithHolidays permissionsWithHolidays = new PermissionsWithHolidays();

            permissionsWithHolidays.Permissions = allPermissions.OrderBy(x => x.State).ToList(); ;
            permissionsWithHolidays.Holidays = lstPublicHolidays;
            permissionsWithHolidays.StartDate = startDate;
            permissionsWithHolidays.EndDate = endDate;

            return PartialView("_ListPartialView", permissionsWithHolidays);
        }


        [HttpPost]
        [Route("PermissionReport/ExcelExportReport")]
        public IActionResult ExcelExportReport(DateTime startDate, DateTime endDate)
        {
            string resultJs = "";

            List<PermissionDto> allPermissions = _permissionService.GetAllPermissionsByFilter(startDate, endDate);

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

            string fileName = string.Format("{0}_{1}.xlsm",startDate.ToString("dd-MM-yyyy") + "_" + endDate.ToString("dd-MM-yyyy") + "_Izinler_", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        [HttpPost]
        [Route("PermissionReport/ExcelExportPermissionStates")]
        public IActionResult ExcelExportPermissionStates()
        {
            string resultJs = "";

            List<EmployeeDto> allEmployees = _employeeService.GetAll().ToList();

            System.Data.DataTable employees = new System.Data.DataTable();

            employees.Columns.Add("Çalışan Adı");
            employees.Columns.Add("Yıllık İzin Toplamı");
            employees.Columns.Add("Ücretsiz İzin Toplamı");
            employees.Columns.Add("Mazeret İzni Toplamı");
            employees.Columns.Add("Denkleştirme İzni Toplamı");

            int index = 0;
            foreach (EmployeeDto employee in allEmployees)
            {
                index = 0;
                System.Data.DataRow rowExcel = employees.NewRow();

                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(employee.Name + " " + employee.Surname);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalYearlyLeave.ToString());
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalUnpaidLeave.ToString());
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalExcusedLeave.ToString());
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalEqualizationLeave.ToString());


                employees.Rows.Add(rowExcel);
            }

            string path = _webHostEnvironment.WebRootPath + "/Files/GeneralReports/Permissions";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region Excel Dosyasını Oluştur ve İndir

            string fileName = string.Format("{0}_{1}.xlsm", "Calisanlarin_Izin_Sayilari_", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            string filePath = path + "/" + fileName;
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(employees, "Report");
            int ColumnRange = allEmployees.Count + 2;
            ws.Range("D2", "D" + ColumnRange).Style.Alignment.WrapText = true;
            ws.Columns(1, 20).AdjustToContents();
            wb.SaveAs(filePath);
            #endregion

            resultJs += string.Format("downloadURI('/Files/GeneralReports/Permissions/{0}');", fileName);

            return Ok(resultJs);
        }
    }
}
