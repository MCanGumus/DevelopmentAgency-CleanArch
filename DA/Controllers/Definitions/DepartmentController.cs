using AutoMapper;
using FastReport.Barcode;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Enums;
using DA.Models;
using DA.Persistence.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;

namespace DA.Controllers.Definitions
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class DepartmentController(IMapper _mapper, IValidator<SaveDepartmentDto> _saveValidator, IValidator<UpdateDepartmentDto> _updateValidator, IDepartmentService _departmentService, IEmployeeService _employeeService) : Controller
    {
        public IActionResult Department()
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

            ListDepartment();

            return View();
        }

        public IActionResult ListDepartment()
        {
            DepartmentAndEmployees DE = new DepartmentAndEmployees();

            DE.Departments = _departmentService.GetAllDepartmentWithEmployee().OrderByDescending(x => x.RecordDate).ToList();
            DE.Employees = _employeeService.GetAll().ToList();

            return View(DE);
        }

        [HttpPost]
        [Route("Birimler/Save")]
        public IActionResult Save(SaveDepartmentDto sDto)
        {
            string resultJs = "";

            #region Validate Entity

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult
                    .ToString()
                    .Replace("Name", "İsim");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            #endregion

            var result = _departmentService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            Employee employee = _employeeService.GetEntityById(sDto.IdEmployeeFK);

            if (employee == null)
            {
                return BadRequest();
            }

            Employee employeeBackup = null;

            if (sDto.IdBackupManager != Guid.Empty)
            {
                employeeBackup = _employeeService.GetEntityById(sDto.IdBackupManager);

                if (employeeBackup == null)
                {
                    return BadRequest();
                }
            }


            List<string> datas = new List<string>();

            datas.Add(result.Result.Name);
            datas.Add(employee.Name + " " + employee.Surname);
            datas.Add(sDto.IdBackupManager == Guid.Empty ? "Yok" : employeeBackup.Name + " " + employeeBackup.Surname);
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalDepartment').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Birimler/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Employee employee = _employeeService.GetEntityById(loginnedEmployee.UserGid);

            Department departmentDto = _departmentService.GetDepartmentWithEmployee(guid);

            if ((departmentDto.Name == "İnsan Kaynakları" || departmentDto.Name == "Genel Sekreterlik") && employee.AuthorizationStatus != EnumAuthorizationStatus.SuperAdmin)
            {
                return Ok("ShowErrorMessage('Bu departman ile alakalı güncelleme yapılmamaktadır. Lütfen sistem yöneticisiyle görüşün.')");
            }

            resultJs += $"$('#uName').val('{departmentDto.Name}');";
            resultJs += $"$('#uIdEmployeeFK').val('{departmentDto.IdEmployeeFK}').trigger('change');";

            if (departmentDto.IdBackupManager != Guid.Empty)
            {
                resultJs += $"$('#uIdBackupManager').val('{departmentDto.IdBackupManager}').trigger('change');";
            }
            else
            {
                resultJs += $"$('#uIdBackupManager').val('0').trigger('change');";
            }

            resultJs += $"$('#Id').val('{departmentDto.Id}');";
            resultJs += $"$('#Title').text('{departmentDto.Name}');";
            resultJs += $"$('#ModalUpdateDepartment').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Birimler/Update")]
        public IActionResult Update(UpdateDepartmentDto uDto)
        {
            string resultJs = "";
            #region Validate Entity

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult
                    .ToString()
                    .Replace("Name", "İsim");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            #endregion

            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Employee employeeLoginnedIn = _employeeService.GetEntityById(loginnedEmployee.UserGid);

            Department department = _departmentService.GetDepartmentWithEmployee(uDto.Id);

            if ((department.Name == "İnsan Kaynakları" || department.Name == "Genel Sekreterlik") && employeeLoginnedIn.AuthorizationStatus != EnumAuthorizationStatus.SuperAdmin)
            {
                return Ok("ShowErrorMessage('Bu departman ile alakalı güncelleme yapılmamaktadır. Lütfen sistem yöneticisiyle görüşün.')");
            }
            
            Employee employee = _employeeService.GetEntityById(uDto.IdEmployeeFK);

            if (employee == null)
            {
                return BadRequest();
            }


            Employee employeeBackup = null;

            if (uDto.IdBackupManager != Guid.Empty)
            {
                employeeBackup = _employeeService.GetEntityById(uDto.IdBackupManager);

                if (employeeBackup == null)
                {
                    return BadRequest();
                }
            }

            department.Name = uDto.Name;
            department.IdBackupManager = uDto.IdBackupManager;
            department.IdEmployeeFK = uDto.IdEmployeeFK;

            UpdateDepartmentDto uDepartmentDto = _mapper.Map<UpdateDepartmentDto>(department);

            _departmentService.Update(uDepartmentDto);

            List<string> datas = new List<string>();

            datas.Add(department.Name);
            datas.Add(employee.Name + " " + employee.Surname);
            datas.Add(uDto.IdBackupManager == Guid.Empty ? "Yok" : employeeBackup.Name + " " + employeeBackup.Surname);
            datas.Add(string.Format(htmlCode, department.Id));

            resultJs += TableTransactions.UpdateTable(datas, department.Id);

            resultJs += "$('#ModalUpdateDepartment').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Birimler/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            Department department = _departmentService.GetEntityById(Id);

            if (department.Name == "İnsan Kaynakları" || department.Name == "Genel Sekreterlik")
            {
                return Ok("ShowErrorMessage('Bu departman silinemez. Lütfen sistem yöneticisiyle görüşün.')");
            }

            department.DataType = Domain.Enums.EnumDataType.Deleted;

            _departmentService.UpdateEntity(department);

            resultJs += TableTransactions.DeleteTable(department.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Birimler/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Birimler/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
