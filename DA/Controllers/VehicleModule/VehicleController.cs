using AutoMapper;
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

namespace DA.Controllers.VehicleModule
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IMapper _mapper;
        private readonly IValidator<SaveVehicleDto> _saveValidator;
        private readonly IValidator<UpdateVehicleDto> _updateValidator;
        private readonly IEmployeeService _employeeService;
        public VehicleController(IVehicleService vehicleService, IValidator<UpdateVehicleDto> updateValidator, IValidator<SaveVehicleDto> saveValidator, IMapper mapper, IEmployeeService employeeService)
        {
            _vehicleService = vehicleService;
            _updateValidator = updateValidator;
            _mapper = mapper;
            _saveValidator = saveValidator;
            _employeeService = employeeService;
        }
        public IActionResult Vehicle()
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

            ListVehicle();
            return View();
        }

        public IActionResult ListVehicle()
        {
            List<VehicleDto> lst = _vehicleService.GetAll().ToList();

            return View(lst);
        }

        [HttpPost]
        [Route("Vehicle/Save")]
        public IActionResult Save(SaveVehicleDto sDto)
        {
            string resultJs = "";

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Plate", "Plaka");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            var result = _vehicleService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.Plate);
            datas.Add(result.Result.Capacity.ToString());
            datas.Add(result.Result.IsTemporary ? "Geçici" : "Geçici Değil");
            datas.Add(result.Result.IsActive ? "Aktif" : "Aktif Değil");

            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalVehicle').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Vehicle/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            VehicleDto vehicleDto = _vehicleService.GetById(guid);

            resultJs += $"$('#uPlate').val('{vehicleDto.Plate}');";
            resultJs += $"$('#uCapacity').val('{vehicleDto.Capacity}');";
            resultJs += string.Format("$('#uIsTemporary').prop('checked', '{0}');", vehicleDto.IsTemporary ? "true" : "");
            resultJs += string.Format("$('#uIsActive').prop('checked', '{0}');", vehicleDto.IsActive ? "true" : "");
            
            resultJs += $"$('#uId').val('{vehicleDto.Id}');";
            resultJs += $"$('#Title').text('{vehicleDto.Plate}');";
            resultJs += $"$('#ModalUpdateVehicle').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Vehicle/Update")]
        public IActionResult Update(UpdateVehicleDto uDto)
        {
            string resultJs = "";

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Name", "İsim");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            Vehicle vehicleDto = _vehicleService.GetEntityById(uDto.Id);

            vehicleDto.Plate = uDto.Plate;
            vehicleDto.IsTemporary = uDto.IsTemporary;
            vehicleDto.IsActive = uDto.IsActive;
            vehicleDto.Capacity = uDto.Capacity;

            _vehicleService.Update(_mapper.Map<UpdateVehicleDto>(vehicleDto));

            List<string> datas = new List<string>();

            datas.Add(vehicleDto.Plate);
            datas.Add(vehicleDto.Capacity.ToString());
            datas.Add(vehicleDto.IsTemporary ? "Geçici" : "Geçici Değil");
            datas.Add(vehicleDto.IsActive ? "Aktif" : "Aktif Değil");

            datas.Add(string.Format(htmlCode, vehicleDto.Id));

            resultJs += TableTransactions.UpdateTable(datas, vehicleDto.Id);

            resultJs += "$('#ModalUpdateVehicle').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Vehicle/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            Vehicle vehicle = _vehicleService.GetEntityById(Id);
            vehicle.DataType = Domain.Enums.EnumDataType.Deleted;

            _vehicleService.UpdateEntity(vehicle);

            resultJs += TableTransactions.DeleteTable(vehicle.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Vehicle/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Vehicle/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";

    }
}
