using ClosedXML.Excel;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Controllers.MissionModule;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Enums;
using DA.Models;
using DA.Persistence.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DA.Controllers.Reports
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class MissionReportController : Controller
    {
        private readonly IMissionService _missionService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmployeeService _employeeService;

        public MissionReportController(IMissionService missionService, IWebHostEnvironment webHostEnvironment, IEmployeeService employeeService)
        {
            _missionService = missionService;
            _webHostEnvironment = webHostEnvironment;
            _employeeService = employeeService;
        }

        public IActionResult MissionReport()
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

            ListMissionReport(DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(3));

            return View();
        }

        [HttpPost]
        [Route("MissionReport/MissionReportWithFilter")]
        public IActionResult ListMissionReport(DateTime startDate, DateTime endDate)
        {
            List<MissionDto> allMissions = _missionService.GetAllMissions(startDate, endDate);

            MissionReportModel model = new MissionReportModel();

            model.Missions = allMissions.OrderBy(x => x.State).ToList(); ;
            model.StartDate = startDate;
            model.EndDate = endDate;

            return PartialView("_ListPartialView", model);
        }

        [HttpPost]
        [Route("MissionReport/ExcelExportReport")]
        public IActionResult ExcelExportReport(DateTime startDate, DateTime endDate)
        {
            string resultJs = "";

            List<MissionDto> allMissions = _missionService.GetAllMissions(startDate, endDate);

            System.Data.DataTable missions = new System.Data.DataTable();

            missions.Columns.Add("Belge Numarası");
            missions.Columns.Add("Görev Tipi");
            missions.Columns.Add("Çalışan");
            missions.Columns.Add("Yer");
            missions.Columns.Add("Konu Tipi");
            missions.Columns.Add("Konu");
            missions.Columns.Add("Avans");
            missions.Columns.Add("Başlangıç Tarihi");
            missions.Columns.Add("Bitiş Tarihi");
            missions.Columns.Add("Gidiş Aracı");
            missions.Columns.Add("Dönüş Aracı");
            

            int index = 0;
            foreach (MissionDto mission in allMissions)
            {
                index = 0;
                System.Data.DataRow rowExcel = missions.NewRow();

                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.DocumentId);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(MissionController.TypeTR(mission.MissionType));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.Employee.Name + " " + mission.Employee.Surname);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.Area);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.SubjectType.ToString());
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.Subject);
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.IsAdvanceRequested ? mission.AdvanceAmount.ToString() : "0");
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.DateOfStart.ToString("dd.MM.yyyy HH:mm"));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(mission.DateOfEnd.ToString("dd.MM.yyyy HH:mm"));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(MissionController.CarDepartureTypeTR(mission.DepartureVehicle));
                rowExcel[index++] = Components.System.ExcelMethods.ChangeXMLChars(MissionController.CarReturnTypeTR(mission.ReturnVehicle));

                missions.Rows.Add(rowExcel);
            }

            string path = _webHostEnvironment.WebRootPath + "/Files/GeneralReports/Missions";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region Excel Dosyasını Oluştur ve İndir

            string fileName = string.Format("{0}_{1}.xlsm", startDate.ToString("dd-MM-yyyy") + "_" + endDate.ToString("dd-MM-yyyy") + "_Gorevler_", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            string filePath = path + "/" + fileName;
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(missions, "Report");
            int ColumnRange = allMissions.Count + 2;
            ws.Range("D2", "D" + ColumnRange).Style.Alignment.WrapText = true;
            ws.Columns(1, 20).AdjustToContents();
            ws.Column(6).Width = 150;
            wb.SaveAs(filePath);
            #endregion

            resultJs += string.Format("downloadURI('/Files/GeneralReports/Missions/{0}');", fileName);

            return Ok(resultJs);
        }
    }
}
