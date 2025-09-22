using AutoMapper;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Models;
using DA.Persistence.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DA.Controllers.Definitions
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class EducationalInstitutionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveEducationalInstitutionDto> _saveValidator;
        private readonly IValidator<UpdateEducationalInstitutionDto> _updateValidator;
        private readonly IEducationalInstitutionService _educationalInstitutionService;

        public EducationalInstitutionController(IMapper mapper,
            IValidator<SaveEducationalInstitutionDto> saveValidator,
            IValidator<UpdateEducationalInstitutionDto> updateValidator,
            IEducationalInstitutionService educationalInstitutionService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _educationalInstitutionService = educationalInstitutionService;
        }

        public IActionResult EducationalInstitution()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListEducationalInstitution();
            return View();
        }

        public IActionResult ListEducationalInstitution()
        {
            List<EducationalInstitutionDto> lstEducationalInstitution = _educationalInstitutionService.GetAll().OrderByDescending(x => x.RecordDate).ToList();
            return View(lstEducationalInstitution);
        }

        [HttpPost]
        [Route("EducationalInstitutions/Save")]
        public IActionResult Save(SaveEducationalInstitutionDto sDto)
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

            var result = _educationalInstitutionService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            resultJs += $"$('.dataTable').DataTable().row.add(['{result.Result.Name}', '{string.Format(htmlCode, result.Result.Id)}']).draw(false);";
            resultJs += "$('#ModalEducationalInstitution').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("EducationalInstitutions/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            EducationalInstitutionDto educationalInstitutionDto = _educationalInstitutionService.GetById(guid);

            resultJs += $"$('#uName').val('{educationalInstitutionDto.Name}');";
            resultJs += $"$('#Id').val('{educationalInstitutionDto.Id}');";
            resultJs += $"$('#Title').text('{educationalInstitutionDto.Name}');";
            resultJs += $"$('#ModalUpdateEducationalInstitution').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("EducationalInstitutions/Update")]
        public IActionResult Update(UpdateEducationalInstitutionDto uDto)
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

            EducationalInstitution educationalInstitution = _educationalInstitutionService.GetEntityById(uDto.Id);
            educationalInstitution.Name = uDto.Name;
            _educationalInstitutionService.UpdateEntity(educationalInstitution);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var rowData = ['{educationalInstitution.Name}', '{string.Format(htmlCode, educationalInstitution.Id)}'];";
            resultJs += @$"var row = table.row(""[data-id='{educationalInstitution.Id}']"");";
            resultJs += @$"row.data(rowData).draw();";

            resultJs += "$('#ModalUpdateEducationalInstitution').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("EducationalInstitutions/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            EducationalInstitution educationalInstitution = _educationalInstitutionService.GetEntityById(Id);
            educationalInstitution.DataType = Domain.Enums.EnumDataType.Deleted;
            _educationalInstitutionService.UpdateEntity(educationalInstitution);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var row = table.row(""[data-id='{educationalInstitution.Id}']"");";
            resultJs += @$"row.remove().draw();";

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;EducationalInstitutions/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;EducationalInstitutions/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
