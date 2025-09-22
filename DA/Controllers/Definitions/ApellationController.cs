using AutoMapper;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DA.Controllers.Definitions
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class ApellationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveApellationDto> _saveValidator;
        private readonly IValidator<UpdateApellationDto> _updateValidator;
        private readonly IApellationService _apellationService;

        public ApellationController(IMapper mapper,
            IValidator<SaveApellationDto> saveValidator,
            IValidator<UpdateApellationDto> updateValidator,
            IApellationService apellationService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _apellationService = apellationService;
        }

        public IActionResult Apellation()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListApellation();
            return View();
        }

        public IActionResult ListApellation()
        {
            List<ApellationDto> lstApellation = _apellationService.GetAll().OrderByDescending(x => x.RecordDate).ToList();
            return View(lstApellation);
        }

        [HttpPost]
        [Route("Apellations/Save")]
        public IActionResult Save(SaveApellationDto sDto)
        {
            string resultJs = "";

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Name", "İsim");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _apellationService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            resultJs += $"$('.dataTable').DataTable().row.add(['{result.Result.Name}', '{string.Format(htmlCode, result.Result.Id)}']).draw(false);";
            resultJs += "$('#ModalApellation').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Apellations/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            ApellationDto apellationDto = _apellationService.GetById(guid);

            resultJs += $"$('#uName').val('{apellationDto.Name}');";
            resultJs += $"$('#Id').val('{apellationDto.Id}');";
            resultJs += $"$('#Title').text('{apellationDto.Name}');";
            resultJs += $"$('#ModalUpdateApellation').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Apellations/Update")]
        public IActionResult Update(UpdateApellationDto uDto)
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

            Apellation apellation = _apellationService.GetEntityById(uDto.Id);
            apellation.Name = uDto.Name;
            _apellationService.UpdateEntity(apellation);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var rowData = ['{apellation.Name}', '{string.Format(htmlCode, apellation.Id)}'];";
            resultJs += @$"var row = table.row(""[data-id='{apellation.Id}']"");";
            resultJs += @$"row.data(rowData).draw();";

            resultJs += "$('#ModalUpdateApellation').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Apellations/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            Apellation apellation = _apellationService.GetEntityById(Id);
            apellation.DataType = Domain.Enums.EnumDataType.Deleted;
            _apellationService.UpdateEntity(apellation);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var row = table.row(""[data-id='{apellation.Id}']"");";
            resultJs += @$"row.remove().draw();";

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Apellations/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Apellations/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
