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
using System.Collections.Generic;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;

namespace DA.Controllers.Authority
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class EmployeeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveEmployeeDto> _saveValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IAcademyInfoService _academyInfoService;
        private readonly IAddressService _addressService;
        private readonly IGSMNumberService _numberService;
        private readonly IEMailService _mailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(IMapper mapper,
            IValidator<SaveEmployeeDto> saveValidator,
            IValidator<UpdateEmployeeDto> updateValidator,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IAcademyInfoService academyInfoService,
            IAddressService addressService,
            IGSMNumberService numberService,
            IEMailService mailService,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _academyInfoService = academyInfoService;
            _addressService = addressService;
            _numberService = numberService;
            _mailService = mailService;
        }

        public IActionResult Employee()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model == null)
            {
                return Redirect("/");
            }


            ListEmployee();
            return View();
        }

        public IActionResult ListEmployee()
        {
            DepartmentAndEmployees DE = new DepartmentAndEmployees();

            DE.Employees = _employeeService.GetAllEmployeesWithDepartment().OrderBy(x => x.Name).ToList();
            DE.Departments = _departmentService.GetAllDepartmentWithEmployee();

            return View(DE);
        }

        public IActionResult EmployeesExited()
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

            ListEmployeeExited();
            return View();
        }

        public IActionResult ListEmployeeExited()
        {
            List<EmployeeDto> employees =  _employeeService.GetAllEmployeesExited().OrderBy(x => x.Name).ToList();

            return View(employees);
        }

        [HttpPost]
        [Route("Employees/Save")]
        public IActionResult Save(SaveEmployeeDto employee)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model.Role != "SuperAdmin") 
            {
                return Ok($@"ShowErrorMessage('Bu işleme yetkiniz yok.');");
            }

            if (employee.Password == null)
            {
                return Ok($@"ShowErrorMessage('Şifre boş olamaz.');");
            }

            PasswordHash passwordHash = new PasswordHash();

            employee.Password = passwordHash.HashPasword(employee.Password, out var salt);
            employee.PasswordSalt = Convert.ToBase64String(salt);

            Random random = new Random();

            bool checkIt = true;
            int number = 0;
            do
            {
                number = random.Next(1000, 9999);
                checkIt = _employeeService.CheckRegistrationNumber(number);
            } while (checkIt);

            employee.RegistrationNumber = number.ToString();

            ValidationResult valResult = _saveValidator.Validate(employee);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdentificationNumber", "Kimlik Numarası")
                                            .Replace("Name", "İsim")
                                            .Replace("Surname", "Soyisim")
                                            .Replace("MotherName", "Anne Adı")
                                            .Replace("FatherName", "Baba Adı")
                                            .Replace("PlaceOfBirth", "Doğum Yeri")
                                            .Replace("DateOfBirth", "Doğum Tarihi")
                                            .Replace("DateOfStart", "Başlangıç Tarihi")
                                            .Replace("Email", "E-posta")
                                            .Replace("Password", "Şifre")
                                            .Replace("PasswordSalt", "Şifre Tuzu")
                                            .Replace("TotalYearlyLeave", "Yıllık İzin Toplamı")
                                            .Replace("TotalUnpaidLeave", "Ücretsiz İzin Toplamı")
                                            .Replace("TotalExcusedLeave", "Mazeretli İzin Toplamı")
                                            .Replace("TotalEqualizationLeave", "Denkleştirme İzin Toplamı")
                                            .Replace("RegistrationNumber", "Kayıt Numarası");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _employeeService.Insert(employee);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.Name + " " + result.Result.Surname);
            datas.Add(result.Result.IdentificationNumber);
            datas.Add(result.Result.PhoneNumber);
            datas.Add(result.Result.PlaceOfBirth);
            datas.Add(result.Result.DateOfBirth.Value.ToString("yyyy.MM.dd"));
            datas.Add(StrBloodGroup.StrEnumBloodGroup((EnumBloodGroup)result.Result.BloodGroup));
            datas.Add(result.Result.Gender.ToString());

            if (result.Result.IdDepartmentFK != null)
            {
                datas.Add(_departmentService.GetById(result.Result.IdDepartmentFK.Value).Name);
            }
            else
                datas.Add("Belirtilmemiş");

            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);
            resultJs += "$('#ModalEmployee').modal('hide');";
            resultJs += "ShowSuccessMessage('Çalışan bilgisi başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Employees/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model.Role != "SuperAdmin")
            {
                return Ok($@"ShowErrorMessage('Bu işleme yetkiniz yok.');");
            }

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            Employee employee = _employeeService.GetEntityById(guid);

            resultJs += $"$('#uIdentificationNumber').val('{employee.IdentificationNumber}');";
            resultJs += $"$('#uPhoneNumber').val('{employee.PhoneNumber}');";
            resultJs += string.Format("$('#uApellation').val('{0}');", employee.Apellation == null ? "" : employee.Apellation);
            resultJs += $"$('#uName').val('{employee.Name}');";
            resultJs += $"$('#uSurname').val('{employee.Surname}');";
            resultJs += $"$('#uMotherName').val('{employee.MotherName}');";
            resultJs += $"$('#uFatherName').val('{employee.FatherName}');";
            resultJs += $"$('#uPlaceOfBirth').val('{employee.PlaceOfBirth}');";
            resultJs += $"$('#uDateOfBirth').val('{employee.DateOfBirth.Value.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#uBloodGroup').val('{(int)employee.BloodGroup}').trigger('change');";
            resultJs += $"$('#uGender').val('{(int)employee.Gender}').trigger('change');";
            resultJs += $"$('#uEmail').val('{employee.Email}');";
            resultJs += $"$('#uAuthorizationStatus').val('{(int)employee.AuthorizationStatus}').trigger('change');";
            resultJs += $"$('#uRefresherYearlyLeave').val('{employee.RefresherYearlyLeave}');";
            resultJs += $"$('#uTotalYearlyLeave').val('{employee.TotalYearlyLeave}');";
            resultJs += $"$('#uTotalUnpaidLeave').val('{employee.TotalUnpaidLeave}');";
            resultJs += $"$('#uTotalExcusedLeave').val('{employee.TotalExcusedLeave}');";
            resultJs += $"$('#uTotalEqualizationLeave').val('{employee.TotalEqualizationLeave}');";
            resultJs += $"$('#uDateOfStart').val('{employee.DateOfStart.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#uChief').val('{employee.Chief}').trigger('change');";

            if (employee.IdDepartmentFK != null)
            {
                resultJs += $"$('#uIdDepartmentFK').val('{employee.IdDepartmentFK}').trigger('change');";
            }
            else
                resultJs += $"$('#uIdDepartmentFK').val('0').trigger('change');";

            resultJs += $"$('#uId').val('{employee.Id}');";
            resultJs += $"$('#UpdateModalTitle').text('{employee.Name} {employee.Surname}');";

            resultJs += $"$('#ModalUpdateEmployee').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Employees/OpenDetail")]
        public IActionResult OpenDetails(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            Employee employee = _employeeService.GetEntityById(guid);

            if (employee == null)
                return BadRequest();

            resultJs += $"$('#dId').val('{employee.Id}');";
            resultJs += $"$('#sIdentificationNumber').text('{employee.IdentificationNumber}');";
            resultJs += $"$('#sPhoneNumber').text('{employee.PhoneNumber}');";
            resultJs += $"$('#sName').text('{employee.Name}');";
            resultJs += string.Format("$('#sApellation').text('{0}');", employee.Apellation == null ? "" : employee.Apellation);
            resultJs += $"$('#sSurname').text('{employee.Surname}');";
            resultJs += $"$('#sMotherName').text('{employee.MotherName}');";
            resultJs += $"$('#sFatherName').text('{employee.FatherName}');";
            resultJs += $"$('#sPlaceOfBirth').text('{employee.PlaceOfBirth}');";

            if (employee.ExitedDate != null)
                resultJs += $"$('#sExitedDate').text('{employee.ExitedDate.Value.ToString("yyyy-MM-dd")}');";
            else
                resultJs += $"$('#sExitedDate').text('');";


            if (employee.DateOfBirth != null)
                resultJs += $"$('#sDateOfBirth').text('{employee.DateOfBirth.Value.ToString("yyyy-MM-dd")}');";
            else
                resultJs += $"$('#sDateOfBirth').text('');";

            resultJs += $"$('#sBloodGroup').text('{StrBloodGroup.StrEnumBloodGroup((EnumBloodGroup)employee.BloodGroup)}');";
            resultJs += $"$('#sGender').text('{employee.Gender}');";
            resultJs += $"$('#sEmail').text('{employee.Email}');";
            resultJs += $"$('#sAuthorizationStatus').text('{employee.AuthorizationStatus}');";
            resultJs += $"$('#sRefresherYearlyLeave').text('{employee.RefresherYearlyLeave}');";
            resultJs += $"$('#sTotalYearlyLeave').text('{employee.TotalYearlyLeave}');";
            resultJs += $"$('#sTotalUnpaidLeave').text('{employee.TotalUnpaidLeave}');";
            resultJs += $"$('#sTotalExcusedLeave').text('{employee.TotalExcusedLeave}');";
            resultJs += $"$('#sTotalEqualizationLeave').text('{employee.TotalEqualizationLeave}');";
            resultJs += $"$('#sDateOfStart').text('{employee.DateOfStart.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#Title').text('{employee.Name} {employee.Surname}');";

            if (employee.Chief != null)
            {
                Employee chief = _employeeService.GetEntityById(employee.Chief.Value);

                resultJs += $"$('#sChief').text('{chief.Name} {chief.Surname}');";
            }

            if (employee.Department != null)
                resultJs += $"$('#sIdDepartmentFK').text('{employee.Department.Name}');";
            else
                resultJs += $"$('#sIdDepartmentFK').text('Belirtilmemiş');";

            #region Academy Info
            var listAcademy = _academyInfoService.GetAllUserAcademyInfos(employee.Id);

            if (listAcademy.Count() == 0)
                resultJs += "$( document ).ready(function() { $('#AcademiaHtml').html('Hiç akademik bilgi eklenmemiş.'); });";
            else
            {
                string headers = "Üniversite-Fakülte-Bölüm-Tez-Türü-Başlama/Bitiş Tarihi";
                string buttonsHeader = "";
                string buttonsOperation = "";

                List<string> data = new List<string>();
                foreach (var item in listAcademy)
                {
                    data.Add(item.Id.ToString());
                    data.Add(item.University);
                    data.Add(item.Faculty);
                    data.Add(item.Department);
                    data.Add((string.IsNullOrEmpty(item.ThesisTopic) ? " " : item.ThesisTopic));
                    data.Add(StrAcademyType((EnumAcademyType)item.AcademyType));
                    data.Add(item.StartDate.ToString("dd.MM.yyyy") + " / " + (item.EndDate == null ? "-" : item.StartDate.ToString("dd.MM.yyyy")));
                }

                string html = TableTransactions.TableCreate_ByGuid("academyTable", "AcademiaHtml", headers, buttonsHeader, buttonsOperation, data);
                resultJs += string.Format("$( document ).ready(function() {{ $('#AcademiaHtml').html('{0}'); TableReload('academyTable', $('#academyTable').DataTable(), 50); }});", html);
            }
            #endregion

            #region Addresses
            var listAddresses = _addressService.GetAllMyAddresses(employee.Id);

            if (listAddresses.Count() == 0)
                resultJs += "$( document ).ready(function() { $('#AddressesHtml').html('Hiç adres eklenmemiş.'); });";
            else
            {
                string headers = "Başlık-Adres";
                string buttonsHeader = "";
                string buttonsOperation = "";

                List<string> data = new List<string>();
                foreach (var item in listAddresses)
                {
                    data.Add(item.Id.ToString());
                    data.Add(item.AddressTitle);
                    data.Add(item.FullAddress);
                }

                string html = TableTransactions.TableCreate_ByGuid("addressTable", "AddressesHtml", headers, buttonsHeader, buttonsOperation, data);
                resultJs += string.Format("$( document ).ready(function() {{ $('#AddressesHtml').html('{0}'); TableReload('addressTable', $('#addressTable').DataTable(), 50); }});", html);
            }
            #endregion

            #region GsmNumber
            var listNumber= _numberService.GetAllMyNumbers(employee.Id);

            if (listNumber.Count() == 0)
                resultJs += "$( document ).ready(function() { $('#GSMSHtml').html('Hiç numara eklenmemiş.'); });";
            else
            {
                string headers = "Numara";
                string buttonsHeader = "";
                string buttonsOperation = "";

                List<string> data = new List<string>();
                foreach (var item in listNumber)
                {
                    data.Add(item.Id.ToString());
                    data.Add(item.GSM);
                }

                string html = TableTransactions.TableCreate_ByGuid("gsmTable", "GSMSHtml", headers, buttonsHeader, buttonsOperation, data);
                resultJs += string.Format("$( document ).ready(function() {{ $('#GSMSHtml').html('{0}'); TableReload('gsmTable', $('#gsmTable').DataTable(), 50); }});", html);
            }
            #endregion

            #region Mail
            var listEmail = _mailService.GetAllMyMails(employee.Id);

            if (listEmail.Count() == 0)
                resultJs += "$( document ).ready(function() { $('#EmailsHtml').html('Hiç mail adresi eklenmemiş.'); });";
            else
            {
                string headers = "Mail Adresi";
                string buttonsHeader = "";
                string buttonsOperation = "";

                List<string> data = new List<string>();
                foreach (var item in listEmail)
                {
                    data.Add(item.Id.ToString());
                    data.Add(item.EMailAddress);
                    
                }

                string html = TableTransactions.TableCreate_ByGuid("emailTable", "EmailsHtml", headers, buttonsHeader, buttonsOperation, data);
                resultJs += string.Format("$( document ).ready(function() {{ $('#EmailsHtml').html('{0}'); TableReload('emailTable', $('#emailTable').DataTable(), 50); }});", html);
            }
            #endregion

            resultJs += $"$('#Title').text('{employee.Name + " " + employee.Surname}');";
            resultJs += $"$('#ModalDetailEmployee').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Employees/Update")]
        public IActionResult Update(UpdateEmployeeDto employee)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Employee loginnedEmployee = _employeeService.GetEntityById(model.UserGid);

            if (loginnedEmployee.AuthorizationStatus != EnumAuthorizationStatus.SuperAdmin)
            {
                return Ok($@"ShowErrorMessage('Bu işleme yetkiniz yok.');");
            }


            Employee employeeDto = _employeeService.GetEntityById(employee.Id);

            if (employeeDto == null)
                return BadRequest();
            

            if (employee.AuthorizationStatus != employeeDto.AuthorizationStatus && employeeDto.AuthorizationStatus == EnumAuthorizationStatus.SuperAdmin)
            {
                int countTheSuperAdmins = _employeeService.GetAll().Where(x => x.AuthorizationStatus == EnumAuthorizationStatus.SuperAdmin).Count();

                if (countTheSuperAdmins == 1)
                {
                    return Ok($@"ShowErrorMessage('Sistemdeki son Süper Admin yetkisindeki kişinin yetkisini düşürüyorsunuz. Lütfen önce başkasını Süper Admin atayın.');");
                }
            }

            if (!string.IsNullOrEmpty(employee.Password))
            {
                PasswordHash passwordHash = new PasswordHash();

                employeeDto.Password = passwordHash.HashPasword(employee.Password, out var salt);
                employeeDto.PasswordSalt = Convert.ToBase64String(salt);

                employee.Password = employeeDto.Password;
                employee.PasswordSalt = employeeDto.PasswordSalt;
            }
            else
            {
                employee.Password = employeeDto.Password;
                employee.PasswordSalt = employeeDto.PasswordSalt;
            }

            ValidationResult valResult = _updateValidator.Validate(employee);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdentificationNumber", "Kimlik Numarası")
                                            .Replace("Name", "İsim")
                                            .Replace("Surname", "Soyisim")
                                            .Replace("MotherName", "Anne Adı")
                                            .Replace("FatherName", "Baba Adı")
                                            .Replace("PlaceOfBirth", "Doğum Yeri")
                                            .Replace("DateOfBirth", "Doğum Tarihi")
                                            .Replace("DateOfStart", "Başlangıç Tarihi")
                                            .Replace("Email", "E-posta")
                                            .Replace("Password", "Şifre")
                                            .Replace("PasswordSalt", "Şifre Tuzu")
                                            .Replace("TotalYearlyLeave", "Yıllık İzin Toplamı")
                                            .Replace("TotalUnpaidLeave", "Ücretsiz İzin Toplamı")
                                            .Replace("TotalExcusedLeave", "Mazeretli İzin Toplamı")
                                            .Replace("TotalEqualizationLeave", "Denkleştirme İzin Toplamı")
                                            .Replace("RegistrationNumber", "Kayıt Numarası");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            employeeDto.AuthorizationStatus = employee.AuthorizationStatus;
            employeeDto.RefresherYearlyLeave = employee.RefresherYearlyLeave;
            employeeDto.Apellation = employee.Apellation;
            employeeDto.PhoneNumber = employee.PhoneNumber;
            employeeDto.Name = employee.Name;
            employeeDto.Surname = employee.Surname;
            employeeDto.FatherName = employee.FatherName;
            employeeDto.MotherName = employee.MotherName;
            employeeDto.Chief = employee.Chief;
            employeeDto.DateOfBirth = employee.DateOfBirth;
            employeeDto.DateOfStart = employee.DateOfStart;
            employeeDto.PlaceOfBirth = employee.PlaceOfBirth;
            employeeDto.Gender = employee.Gender;
            employeeDto.BloodGroup = employee.BloodGroup;
            employeeDto.Email = employee.Email;
            employeeDto.IdentificationNumber = employee.IdentificationNumber;
            employeeDto.TotalYearlyLeave = employee.TotalYearlyLeave;
            employeeDto.TotalUnpaidLeave = employee.TotalUnpaidLeave;
            employeeDto.TotalExcusedLeave = employee.TotalExcusedLeave;
            employeeDto.TotalEqualizationLeave = employee.TotalEqualizationLeave;
            employeeDto.IdDepartmentFK = employee.IdDepartmentFK;

            _employeeService.UpdateEntity(employeeDto);

            List<string> datas = new List<string>();

            datas.Add(employeeDto.Name + " " + employee.Surname);
            datas.Add(employeeDto.IdentificationNumber);
            datas.Add(employeeDto.PhoneNumber);
            datas.Add(employeeDto.PlaceOfBirth);
            datas.Add(employeeDto.DateOfBirth.Value.ToString("yyyy.MM.dd"));
            datas.Add(StrBloodGroup.StrEnumBloodGroup((EnumBloodGroup)employeeDto.BloodGroup));
            datas.Add(employeeDto.Gender.ToString());

            if (employee.IdDepartmentFK != null)
            {
                DepartmentDto department = _departmentService.GetById(employee.IdDepartmentFK.Value);
                datas.Add(department == null ? "Belirtilmemiş" : department.Name);
            }
            else
            {
                datas.Add("Belirtilmemiş");
            }

            datas.Add(string.Format(htmlCode, employee.Id));

            resultJs += TableTransactions.UpdateTable(datas, employee.Id);

            resultJs += "$('#ModalUpdateEmployee').modal('hide');";
            resultJs += "ShowSuccessMessage('Çalışan bilgisi başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Employees/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model.Role != "SuperAdmin")
            {
                return Ok($@"ShowErrorMessage('Bu işleme yetkiniz yok.');");
            }

            Employee employee = _employeeService.GetEntityById(Id);

            if (employee.AuthorizationStatus == EnumAuthorizationStatus.SuperAdmin)
            {
                int countTheSuperAdmins = _employeeService.GetAll().Where(x => x.AuthorizationStatus == EnumAuthorizationStatus.SuperAdmin).Count();

                if (countTheSuperAdmins == 1)
                {
                    return Ok($@"ShowErrorMessage('Sistemdeki son Süper Admin yetkisindeki kişiyi siliyorsunuz. Lütfen önce başkasını Süper Admin atayın.');");
                }
            }

            if (employee != null)
            {
                employee.DataType = Domain.Enums.EnumDataType.Deleted;
                _employeeService.UpdateEntity(employee);

                resultJs += TableTransactions.DeleteTable(employee.Id);

                resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Employees/GetLaidOff")]
        public IActionResult GetLaidOff(Guid Id, DateTime date)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model.Role != "SuperAdmin")
            {
                return Ok($@"ShowErrorMessage('Bu işleme yetkiniz yok.');");
            }

            Employee employee = _employeeService.GetEntityById(Id);

            if (employee != null)
            {
                employee.DataType = Domain.Enums.EnumDataType.Draft;
                employee.ExitedDate = date;
                
                _employeeService.UpdateEntity(employee);

                resultJs += TableTransactions.DeleteTable(employee.Id);

                resultJs += $"$('#ModalDetailEmployee').modal('hide');";
                resultJs += "ShowSuccessMessage('Başarıyla işten çıkarıldı.');";

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Employees/GetBackToWork")]
        public IActionResult GetBackToWork(Guid Id)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model.Role != "SuperAdmin")
            {
                return Ok($@"ShowErrorMessage('Bu işleme yetkiniz yok.');");
            }

            Employee employee = _employeeService.GetEntityById(Id);

            if (employee != null)
            {
                employee.DataType = Domain.Enums.EnumDataType.New;
                employee.ExitedDate = null;

                _employeeService.UpdateEntity(employee);

                resultJs += TableTransactions.DeleteTable(employee.Id);

                resultJs += "ShowSuccessMessage('Başarıyla işe geri alındı.');";

                return Ok(resultJs);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Employees/ExcelExport")]
        public IActionResult ExcelExportReport(bool isExited)
        {
            string resultJs = "";

            List<EmployeeDto> allEmployees = null;

            if (isExited) 
                allEmployees = _employeeService.GetAllEmployeesExited();
            else
                allEmployees = _employeeService.GetAllEmployeesWithDepartment();

            System.Data.DataTable employees = new System.Data.DataTable();

            // EmployeeDto'ya göre sütunlar ekleniyor
            employees.Columns.Add("Çalışan Adı");
            employees.Columns.Add("Çalışan Telefonu");
            employees.Columns.Add("Çalışan Eposta");
            employees.Columns.Add("Çalışan Kimlik No");
            employees.Columns.Add("Çalışan Ünvanı");
            employees.Columns.Add("Birimi");
            employees.Columns.Add("Anne Adı");
            employees.Columns.Add("Baba Adı");
            employees.Columns.Add("Doğum Yeri");
            employees.Columns.Add("Doğum Tarihi");
            employees.Columns.Add("İşe Başlama Tarihi");
            employees.Columns.Add("Kan Grubu");
            employees.Columns.Add("Cinsiyet");
            employees.Columns.Add("Yetkilendirme Durumu");
            employees.Columns.Add("Yıllık İzin Toplamı");
            employees.Columns.Add("Ücretsiz İzin Toplamı");
            employees.Columns.Add("Mazeret İzni Toplamı");
            employees.Columns.Add("Denkleştirme İzni Toplamı");
            employees.Columns.Add("Sicil No");
            employees.Columns.Add("Amiri");
            employees.Columns.Add("İşten Çıkış Tarihi");
            employees.Columns.Add("Kıdem Gün Sayısı");

            TimeSpan seniorityTotal;
            int seniorityDay = 0;

            foreach (EmployeeDto employee in allEmployees)
            {
                System.Data.DataRow rowExcel = employees.NewRow();

                if (employee.ExitedDate != null)
                {
                    seniorityTotal = employee.ExitedDate.Value - employee.DateOfStart;
                    seniorityDay = (int)seniorityTotal.TotalDays;
                }
                else
                {
                    seniorityTotal = DateTime.Now - employee.DateOfStart;
                    seniorityDay = (int)seniorityTotal.TotalDays;
                }

                rowExcel[0] = Components.System.ExcelMethods.ChangeXMLChars(employee.Name + " " + employee.Surname);
                rowExcel[1] = Components.System.ExcelMethods.ChangeXMLChars(employee.PhoneNumber ?? string.Empty);
                rowExcel[2] = Components.System.ExcelMethods.ChangeXMLChars(employee.Email);
                rowExcel[3] = Components.System.ExcelMethods.ChangeXMLChars(employee.IdentificationNumber);
                rowExcel[4] = Components.System.ExcelMethods.ChangeXMLChars(employee.Apellation);
                rowExcel[5] = Components.System.ExcelMethods.ChangeXMLChars(employee.Department.Name);
                rowExcel[6] = Components.System.ExcelMethods.ChangeXMLChars(employee.MotherName);
                rowExcel[7] = Components.System.ExcelMethods.ChangeXMLChars(employee.FatherName);
                rowExcel[8] = Components.System.ExcelMethods.ChangeXMLChars(employee.PlaceOfBirth);
                rowExcel[9] = Components.System.ExcelMethods.ChangeXMLChars(employee.DateOfBirth.ToString("dd.MM.yyyy"));
                rowExcel[10] = Components.System.ExcelMethods.ChangeXMLChars(employee.DateOfStart.ToString("dd.MM.yyyy"));
                rowExcel[11] = Components.System.ExcelMethods.ChangeXMLChars(employee.BloodGroup?.ToString() ?? string.Empty);
                rowExcel[12] = Components.System.ExcelMethods.ChangeXMLChars(employee.Gender.ToString());
                rowExcel[13] = Components.System.ExcelMethods.ChangeXMLChars(employee.AuthorizationStatus.ToString());
                rowExcel[14] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalYearlyLeave.ToString());
                rowExcel[15] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalUnpaidLeave.ToString());
                rowExcel[16] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalExcusedLeave.ToString());
                rowExcel[17] = Components.System.ExcelMethods.ChangeXMLChars(employee.TotalEqualizationLeave.ToString());
                rowExcel[18] = Components.System.ExcelMethods.ChangeXMLChars(employee.RegistrationNumber ?? string.Empty);
                rowExcel[19] = Components.System.ExcelMethods.ChangeXMLChars(employee.Chief.HasValue ? _employeeService.GetById(employee.Chief.Value).Name + " " + _employeeService.GetById(employee.Chief.Value).Surname : "Yok");
                rowExcel[20] = Components.System.ExcelMethods.ChangeXMLChars(employee.ExitedDate == null ? "" : employee.ExitedDate.Value.ToString("dd.MM.yyyy"));
                rowExcel[21] = Components.System.ExcelMethods.ChangeXMLChars(seniorityDay.ToString());
                employees.Rows.Add(rowExcel);
            }

            string path = _webHostEnvironment.WebRootPath + "/Files/GeneralReports/Employees";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Excel dosyasını oluşturma ve indirme işlemi
            string fileName = string.Format("{0}_{1}.xlsm", DateTime.Now.ToString("dd-MM-yyyy") , isExited ? "_IstenCikanCalisanlar" : "_Calisanlar");
            string filePath = path + "/" + fileName;
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(employees, "Report");
            ws.Columns(1, 19).AdjustToContents(); // Sütun genişliklerini ayarlama
            wb.SaveAs(filePath);

            resultJs += string.Format("downloadURI('/Files/GeneralReports/Employees/{0}');", fileName);

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Employees/DetailedEmployeePage")]
        public IActionResult DetailedEmployeePage(Guid guid)
        {
            DetailedEmployeeModel detailedEmployeeModel = new DetailedEmployeeModel();

            detailedEmployeeModel.Employee = _employeeService.GetById(guid);
            detailedEmployeeModel.AcademyInfo = _academyInfoService.GetAllUserAcademyInfos(guid);
            detailedEmployeeModel.Address = _addressService.GetAllMyAddresses(guid);
            detailedEmployeeModel.GSMNumber = _numberService.GetAllMyNumbers(guid);
            detailedEmployeeModel.Emails = _mailService.GetAllMyMails(guid);

            return PartialView("DetailedEmployee", detailedEmployeeModel);
        }



        // Other methods as needed
        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Employees/OpenDetail&apos;, &apos;{0}&apos;, &apos;OpenDetail&apos;)\" href=\"\"><i class=\"mdi mdi-file text-info md20\"></i></a><a onclick=\"AjaxMethod(&apos;Employees/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Employees/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";

        public static string StrAcademyType(EnumAcademyType enumAcademyType)
        {
            switch (enumAcademyType)
            {
                case (EnumAcademyType.BachelorsDegree):
                    return "Lisans";
                case (EnumAcademyType.MastersDegree):
                    return "Yüksek Lisans";
                case (EnumAcademyType.Doctorate):
                    return "Doktora";

                default:
                    return "";
            }
        }
    }
}
