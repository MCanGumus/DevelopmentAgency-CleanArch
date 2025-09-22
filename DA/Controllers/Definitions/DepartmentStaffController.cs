using AutoMapper;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.Definitions
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class DepartmentStaffController(
        IMapper _mapper,
        IDepartmentStaffService _departmentStaffService,
        IDepartmentService _departmentService,
        IEmployeeService _employeeService,
        IApellationService _apellationService,
        IValidator<SaveDepartmentStaffDto> _saveValidator,
        IValidator<UpdateDepartmentStaffDto> _updateValidator) : Controller
    {
        public IActionResult DepartmentStaff()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListDepartmentStaff();
            return View();
        }

        public IActionResult ListDepartmentStaff()
        {
            DepartmentStaffModel DSM = new DepartmentStaffModel();

            DSM.DepartmentStaffs = new List<DepartmentStaffDto>();
            DSM.DepartmentStaffs = _departmentStaffService.GetAllDepartmentStaffs();

            DSM.DepartmentAndEmployees = new DepartmentAndEmployees();

            DSM.DepartmentAndEmployees.Employees = new List<EmployeeDto>();
            DSM.DepartmentAndEmployees.Employees = _employeeService.GetAll().ToList();

            DSM.Apellations = new List<ApellationDto>();
            DSM.Apellations = _apellationService.GetAll().ToList();

            return View(DSM);
        }

        [HttpPost]
        [Route("DepartmentStaffs/Save")]
        public IActionResult Save(SaveDepartmentStaffDto sDto)
        {
            string resultJs = "";

            DepartmentStaff departmentStaff = _departmentStaffService.GetDepartmentStaffByEmployee(sDto.IdEmployeeFK);

            if (departmentStaff != null)
            {
                return Ok("ShowErrorMessage('Bu kişi daha önce bu birime eklenmiş.')");
            }

            var result = _departmentStaffService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            EmployeeDto employee = _employeeService.GetById(sDto.IdEmployeeFK);

            if (employee == null)
            {
                return BadRequest();
            }

            ApellationDto apellation = _apellationService.GetById(sDto.IdApellationFK);

            if (apellation == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(employee.Name + " " + employee.Surname);
            datas.Add(apellation.Name);
            datas.Add(string.Format(htmlCode, result.Result.Id));
            
            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalDepartmentStaff').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("DepartmentStaffs/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            DepartmentStaff departmentStaffDto = _departmentStaffService.GetEntityById(guid);

            resultJs += $"$('#uIdEmployeeFK').val('{departmentStaffDto.IdEmployeeFK}').trigger('change');";
            resultJs += $"$('#uIdApellationFK').val('{departmentStaffDto.IdApellationFK}').trigger('change');";
            resultJs += $"$('#uId').val('{departmentStaffDto.Id}');";
            resultJs += $"$('#ModalUpdateDepartmentStaff').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("DepartmentStaffs/Update")]
        public IActionResult Update(UpdateDepartmentStaffDto uDto)
        {
            string resultJs = "";

            EmployeeDto employee = _employeeService.GetById(uDto.IdEmployeeFK);

            if (employee == null)
            {
                return BadRequest();
            }

            ApellationDto apellation = _apellationService.GetById(uDto.IdApellationFK);

            if (apellation == null)
            {
                return BadRequest();
            }

            DepartmentStaff departmentStaff = _departmentStaffService.GetDepartmentStaffById(uDto.Id);

            departmentStaff.IdEmployeeFK = uDto.IdEmployeeFK;
            departmentStaff.IdApellationFK = uDto.IdApellationFK;

            _departmentStaffService.Update(_mapper.Map<UpdateDepartmentStaffDto>(departmentStaff));

            List<string> datas = new List<string>();

            datas.Add(employee.Name + " " + employee.Surname);
            datas.Add(apellation.Name);
            datas.Add(string.Format(htmlCode, departmentStaff.Id));

            resultJs += TableTransactions.UpdateTable(datas, departmentStaff.Id);

            resultJs += "$('#ModalUpdateDepartmentStaff').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("DepartmentStaffs/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            DepartmentStaff departmentStaff = _departmentStaffService.GetEntityById(Id);
            departmentStaff.DataType = Domain.Enums.EnumDataType.Deleted;

            _departmentStaffService.Update(_mapper.Map<UpdateDepartmentStaffDto>(departmentStaff));

            resultJs += TableTransactions.DeleteTable(departmentStaff.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;DepartmentStaffs/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;DepartmentStaffs/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
