using AutoMapper;
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
using System.Transactions;

namespace DA.Controllers.MissionModule
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class AllMissionsController : Controller
    {
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IMissionService _missionService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AllMissionsController(IMissionService missionService, IMapper mapper, IVehicleRequestService vehicleRequestService, IEmployeeService employeeService, IWebHostEnvironment webHostEnvironment, IDepartmentService departmentService)
        {
            _missionService = missionService;
            _mapper = mapper;
            _vehicleRequestService = vehicleRequestService;
            _employeeService = employeeService;
            _webHostEnvironment = webHostEnvironment;
            _departmentService = departmentService;
        }

        public IActionResult AllMissions()
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
                        return Redirect("/GorevRaporlari");
                    }

                    return Redirect("/Anasayfa");
                }
            }
            catch (Exception)
            {
                return Redirect("/");
            }

            AllMissionsList(DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(3));
            return View();
        }

        [HttpPost]
        [Route("AllMissions/AllMissionsList")]
        public IActionResult AllMissionsList(DateTime startDate, DateTime endDate)
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<MissionDto> lstMission = _missionService.GetAllMissionsDepartment(loginnedEmployee.DepartmentHeadGid.Value, startDate, endDate).OrderByDescending(x => x.DateOfStart).ToList();

            if (loginnedEmployee.DepartmentHelperGid != null)
            {
                lstMission.AddRange(_missionService.GetAllMissionsDepartment(loginnedEmployee.DepartmentHelperGid.Value, startDate, endDate).OrderByDescending(x => x.RecordDate).ToList());
            }

            if (loginnedEmployee.DepartmentAppointments.Count != 0)
            {
                foreach (var item in loginnedEmployee.DepartmentAppointments)
                {
                    lstMission.AddRange(_missionService.GetAllMissionsDepartment(item, startDate, endDate).OrderByDescending(x => x.RecordDate).ToList());
                }
            }

            MissionsAndEmployees ME = new MissionsAndEmployees();

            ME.Missions = lstMission.OrderBy(x => x.State).ToList(); ;
            ME.StartDate = startDate;
            ME.EndDate = endDate;

            return PartialView("_ListPartialView", ME);
        }

        [HttpPost]
        [Route("AllMissions/Accept")]
        public IActionResult Accept(Guid Id)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Mission mission = _missionService.GetEntityById(Id);

            if (mission == null) { return Ok("ShowErrorMessage('Bir hata oluştu.')"); }

            if (loginnedEmployee.UserGid == mission.IdEmployeeFK)
            {
                return Ok("ShowErrorMessage('Kendi görevinizi onaylayamazsınız.');");
            }

            mission.State = EnumState.Accepted;
            mission.IdWhoAcceptedFK = loginnedEmployee.UserGid;

            _missionService.Update(_mapper.Map<UpdateMissionDto>(mission));

            Employee employee = _employeeService.GetEntityById(mission.IdEmployeeFK);

            DepartmentDto department = _departmentService.GetById(employee.IdDepartmentFK.Value);

            List<string> datas = new List<string>();

            datas.Add(TypeTR(mission.MissionType));
            datas.Add(employee.Name + " " + employee.Surname);
            datas.Add(department.Name);
            datas.Add(mission.SubjectType.ToString());
            datas.Add(mission.Subject);
            datas.Add(mission.Area);
            datas.Add(mission.DateOfStart.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(mission.DateOfEnd.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(StateTR(mission.State));
            datas.Add(string.Format(htmlCodeAccept, mission.Id));

            resultJs += TableTransactions.UpdateTable(datas, mission.Id);

            try
            {
                string employeeMail = employee.Email;
                string subject = "Görevlendirme Onay Durumu hk.";
                string body = $"Merhaba {employee.Name + " " + employee.Surname},\n\n{mission.DocumentId} belge numaralı {mission.DateOfStart} - {mission.DateOfEnd} tarihli görevlendirme belgeniz," +
                    $" {loginnedEmployee.UserName} tarafından onaylanmıştır.\n Bilginize. \n Buraya tıklayarak <a href="+ Constants.ProjectURL + "/Gorevlerim'>Görevlerim</a> sayfasına ilerleyebilirsiniz.";

                MailSenderService.SendEmail(employeeMail, subject, body);

                if (mission.MissionType != EnumMissionType.BolgeIci)
                {
                    string allMail = Constants.SystemMailAddress;
                    string allMailSubject = $"{employee.Name + " " + employee.Surname} Görevlendirmesi hk.";
                    string allMailBody = $"Merhaba, \n Ajans çalışanlarımızdan {employee.Name + " " + employee.Surname}, {mission.DateOfStart} - {mission.DateOfEnd} tarihleri arasında görevde olacaktır.\n Bilginize.";

                    MailSenderService.SendEmail(allMail, allMailSubject, allMailBody);
                }

                resultJs += "ShowSuccessMessage('Başarıyla kabul edildi. Kişinin mail adresine bildirim yapıldı.');";
            }
            catch (Exception ex)
            {
                resultJs += "ShowWarningMessage('Başarıyla kabul edildi ancak kişiye bildirim maili atarken bir hata oluştu. Lütfen sistem yöneticisine haber verin.');";
            }


            return Ok(resultJs);
        }

        [HttpPost]
        [Route("AllMissions/Reject")]
        public IActionResult Reject(Guid Id)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Mission mission = _missionService.GetEntityById(Id);

            if (mission == null) { return Ok("ShowErrorMessage('Bir hata oluştu.')"); }

            if (loginnedEmployee.UserGid == mission.IdEmployeeFK)
            {
                return Ok("ShowErrorMessage('Kendi görevinizi reddedemezsiniz.');");
            }

            mission.State = EnumState.Declined;
            // mission.IdWhoAcceptedFK = loginnedEmployee.UserGid;

            using (TransactionScope scope = new TransactionScope())
            {
                _missionService.Update(_mapper.Map<UpdateMissionDto>(mission));

                List<VehicleRequestDto> vhcReq = _vehicleRequestService.GetVehicleRequestByMission(mission.Id);
                UpdateVehicleRequestDto updateVehicleDto = new UpdateVehicleRequestDto();

                foreach (var item in vhcReq)
                {
                    updateVehicleDto.Id = item.Id;
                    updateVehicleDto.RecordDate = DateTime.Now;
                    updateVehicleDto.DataType = EnumDataType.Deleted;
                    updateVehicleDto.Description = item.Description;
                    updateVehicleDto.DateOfEnd = item.DateOfEnd;
                    updateVehicleDto.DateOfStart = item.DateOfStart;
                    updateVehicleDto.IsGoing = item.IsGoing;
                    updateVehicleDto.IdMissionFK = item.Mission.Id;
                    updateVehicleDto.IdVehicleFK = item.Vehicle.Id;

                    _vehicleRequestService.Update(updateVehicleDto);
                }

                scope.Complete();
            }

            Employee employee = _employeeService.GetEntityById(mission.IdEmployeeFK);

            DepartmentDto department = _departmentService.GetById(employee.IdDepartmentFK.Value);

            List<string> datas = new List<string>();

            datas.Add(TypeTR(mission.MissionType));
            datas.Add(employee.Name + " " + employee.Surname);
            datas.Add(department.Name);
            datas.Add(mission.SubjectType.ToString());
            datas.Add(mission.Subject);
            datas.Add(mission.Area);
            datas.Add(mission.DateOfStart.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(mission.DateOfEnd.ToString("dd.MM.yyyy HH:mm"));
            datas.Add(StateTR(mission.State));
            datas.Add(string.Format(htmlCodeReject, mission.Id));

            resultJs += TableTransactions.UpdateTable(datas, mission.Id);

            try
            {
                string employeeMail = employee.Email;
                string subject = "Görevlendirme Onay Durumu hk.";
                string body = $"Merhaba {employee.Name + " " + employee.Surname},\n\n{mission.DocumentId} belge numaralı {mission.DateOfStart} - {mission.DateOfEnd} tarihli görevlendirme belgeniz," +
                    $" {loginnedEmployee.UserName} tarafından reddedilmiştir.\nBilginize.\n Buraya tıklayarak <a href="+ Constants.ProjectURL + "/Gorevlerim'>Görevlerim</a> sayfasına ilerleyebilirsiniz.";

                MailSenderService.SendEmail(employeeMail, subject, body);

                resultJs += "ShowSuccessMessage('Başarıyla reddedildi. Kişinin mail adresine bildirim yapıldı.');";
            }
            catch (Exception ex)
            {
                resultJs += "ShowWarningMessage('Başarıyla reddedildi ancak kişiye bildirim maili atarken bir hata oluştu. Lütfen sistem yöneticisine haber verin.');";
            }

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("AllMissions/ExcelExport")]
        public IActionResult ExcelExportReport(DateTime startDate, DateTime endDate)
        {
            string resultJs = "";

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<MissionDto> allMissions = _missionService.GetAllMissionsDepartment(loginnedEmployee.DepartmentGid, startDate, endDate).OrderByDescending(x => x.DateOfStart).ToList();

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
        public const string htmlCodeAccept = "<a onclick=\"AjaxMethod(&apos;Missions/ExportDocument&apos;, &apos;{0}&apos;, &apos;Print&apos;)\" href=\"\"><i class=\"mdi mdi-printer text-warning md20\"></i></a><a onclick=\"AjaxMethod(&apos;Missions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a>";
        public const string htmlCodeReject = "<a onclick=\"AjaxMethod(&apos;Missions/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a>";
    }
}
