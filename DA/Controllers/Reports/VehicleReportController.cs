using ClosedXML.Excel;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Enums;
using DA.Models;
using DA.Persistence.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.Reports
{
    public class VehicleReportController : Controller
    {
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmployeeService _employeeService;

        public VehicleReportController(IVehicleRequestService vehicleRequest, IWebHostEnvironment webHostEnvironment, IEmployeeService employeeService)
        {
            _vehicleRequestService = vehicleRequest;
            _webHostEnvironment = webHostEnvironment;
            _employeeService = employeeService;
        }

        public IActionResult VehicleRequestReport()
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

            ListVehicleRequestReport(DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(3));

            return View();
        }

        [HttpPost]
        [Route("VehicleReport/VehicleRequestReportWithFilter")]
        public IActionResult ListVehicleRequestReport(DateTime startDate, DateTime endDate)
        {
            List<VehicleRequestDto> allRequests = _vehicleRequestService.GetFullVehicleRequests(startDate, endDate);

            VehicleRequestReportModel model = new VehicleRequestReportModel();

            model.VehicleRequests = allRequests;
            model.StartDate = startDate;
            model.EndDate = endDate;

            return PartialView("_ListPartialView", model);
        }

        [HttpPost]
        [Route("VehicleReport/ExcelExportReport")]
        public IActionResult ExcelExportReport(DateTime startDate, DateTime endDate)
        {
            string resultJs = "";

            List<VehicleRequestDto> allRequests = _vehicleRequestService.GetFullVehicleRequests(startDate, endDate);

            System.Data.DataTable requests = new System.Data.DataTable();

            requests.Columns.Add("Plaka");
            requests.Columns.Add("Görev");
            requests.Columns.Add("Çalışan");
            requests.Columns.Add("Açıklama");
            requests.Columns.Add("Başlangıç Tarihi");
            requests.Columns.Add("Bitiş Tarihi");
            requests.Columns.Add("Gidiş/Geliş");

            int index = 0;
            foreach (VehicleRequestDto request in allRequests)
            {
                index = 0;
                System.Data.DataRow rowExcel = requests.NewRow();

                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.Vehicle.Plate);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.Mission != null ? request.Mission.Subject : "Valiliğe atandı.");
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.Mission != null ? request.Mission.Employee.Name + " " + request.Mission.Employee.Surname : "Valiliğe atandı.");
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.Description);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.DateOfStart.ToString("dd.MM.yyyy HH:mm"));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.DateOfEnd.ToString("dd.MM.yyyy HH:mm"));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(request.IsGoing ? "Gidiş" : "Geliş");

                requests.Rows.Add(rowExcel);
            }

            string path = _webHostEnvironment.WebRootPath + "/Files/GeneralReports/VehicleRequests";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region Excel Dosyasını Oluştur ve İndir

            string fileName = string.Format("{0}_{1}.xlsm", startDate.ToString("dd-MM-yyyy") + "_" + endDate.ToString("dd-MM-yyyy") + "_AracTalepleri_", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            string filePath = path + "/" + fileName;
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(requests, "Report");
            int ColumnRange = allRequests.Count + 2;
            ws.Range("D2", "D" + ColumnRange).Style.Alignment.WrapText = true;
            ws.Columns(1, 20).AdjustToContents();
            ws.Column(4).Width = 150;
            wb.SaveAs(filePath);
            #endregion

            resultJs += string.Format("downloadURI('/Files/GeneralReports/VehicleRequests/{0}');", fileName);

            return Ok(resultJs);
        }
    }
}
