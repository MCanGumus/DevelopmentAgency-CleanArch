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
    public class MissionTypeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveMissionTypeDto> _saveValidator;
        private readonly IValidator<UpdateMissionTypeDto> _updateValidator;
        private readonly IMissionTypeService _missionTypeService;

        public MissionTypeController(IMapper mapper,
            IValidator<SaveMissionTypeDto> saveValidator,
            IValidator<UpdateMissionTypeDto> updateValidator,
            IMissionTypeService missionTypeService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _missionTypeService = missionTypeService;
        }

        public IActionResult MissionType()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }


            ListMissionType();
            return View();
        }

        public IActionResult ListMissionType()
        {
            List<MissionTypeDto> lstMissionType = _missionTypeService.GetAll().OrderByDescending(x => x.RecordDate).ToList();
            return View(lstMissionType);
        }

        [HttpPost]
        [Route("MissionTypes/Save")]
        public IActionResult Save(SaveMissionTypeDto sDto)
        {
            string resultJs = "";

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("TypeName", "Tip Adı");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _missionTypeService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            resultJs += $"$('.dataTable').DataTable().row.add(['{result.Result.TypeName}', '{string.Format(htmlCode, result.Result.Id)}']).draw(false);";
            resultJs += "$('#ModalMissionType').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("MissionTypes/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            MissionTypeDto missionTypeDto = _missionTypeService.GetById(guid);

            resultJs += $"$('#uTypeName').val('{missionTypeDto.TypeName}');";
            resultJs += $"$('#Id').val('{missionTypeDto.Id}');";
            resultJs += $"$('#Title').text('{missionTypeDto.TypeName}');";
            resultJs += $"$('#ModalUpdateMissionType').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("MissionTypes/Update")]
        public IActionResult Update(UpdateMissionTypeDto uDto)
        {
            string resultJs = "";

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("TypeName", "Tip Adı");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            MissionType missionType = _missionTypeService.GetEntityById(uDto.Id);
            missionType.TypeName = uDto.TypeName;
            _missionTypeService.UpdateEntity(missionType);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var rowData = ['{missionType.TypeName}', '{string.Format(htmlCode, missionType.Id)}'];";
            resultJs += @$"var row = table.row(""[data-id='{missionType.Id}']"");";
            resultJs += @$"row.data(rowData).draw();";

            resultJs += "$('#ModalUpdateMissionType').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("MissionTypes/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            MissionType missionType = _missionTypeService.GetEntityById(Id);
            missionType.DataType = Domain.Enums.EnumDataType.Deleted;
            _missionTypeService.UpdateEntity(missionType);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var row = table.row(""[data-id='{missionType.Id}']"");";
            resultJs += @$"row.remove().draw();";

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;MissionTypes/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;MissionTypes/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
