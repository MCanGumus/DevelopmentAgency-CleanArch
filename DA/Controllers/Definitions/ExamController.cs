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
    public class ExamController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveExamDto> _saveValidator;
        private readonly IValidator<UpdateExamDto> _updateValidator;
        private readonly IExamService _examService;

        public ExamController(IMapper mapper,
            IValidator<SaveExamDto> saveValidator,
            IValidator<UpdateExamDto> updateValidator,
            IExamService examService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _examService = examService;
        }

        public IActionResult Exam()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListExam();
            return View();
        }

        public IActionResult ListExam()
        {
            List<ExamDto> lstExam = _examService.GetAll().OrderByDescending(x => x.RecordDate).ToList();
            return View(lstExam);
        }

        [HttpPost]
        [Route("Exams/Save")]
        public IActionResult Save(SaveExamDto sDto)
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

            var result = _examService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            resultJs += $"$('.dataTable').DataTable().row.add(['{result.Result.Name}', '{string.Format(htmlCode, result.Result.Id)}']).node().id='{result.Result.Id}';"; 
            resultJs += "$('.dataTable').DataTable().draw(false);";

            resultJs += "$('#ModalExam').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Exams/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            ExamDto examDto = _examService.GetById(guid);

            resultJs += $"$('#uName').val('{examDto.Name}');";
            resultJs += $"$('#Id').val('{examDto.Id}');";
            resultJs += $"$('#Title').text('{examDto.Name}');";
            resultJs += $"$('#ModalUpdateExam').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Exams/Update")]
        public IActionResult Update(UpdateExamDto uDto)
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

            Exam exam = _examService.GetEntityById(uDto.Id);
            exam.Name = uDto.Name;
            _examService.UpdateEntity(exam);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var rowData = ['{exam.Name}', '{string.Format(htmlCode, exam.Id)}'];";
            resultJs += @$"var row = table.row(""[id='{exam.Id}']"");";
            resultJs += @$"row.data(rowData).draw();";

            resultJs += "$('#ModalUpdateExam').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Exams/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            Exam exam = _examService.GetEntityById(Id);
            exam.DataType = Domain.Enums.EnumDataType.Deleted;
            _examService.UpdateEntity(exam);

            resultJs += @$"var table = $("".dataTable"").DataTable();";
            resultJs += @$"var row = table.row(""[id='{exam.Id}']"");";
            resultJs += @$"row.remove().draw();";

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Exams/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Exams/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
