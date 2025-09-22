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
using System;
using System.Collections.Generic;
using System.Linq;

namespace DA.Controllers.Definitions
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class HolidayController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SavePublicHolidayDto> _saveValidator;
        private readonly IValidator<UpdatePublicHolidayDto> _updateValidator;
        private readonly IPublicHolidayService _publicHolidayService;
        private readonly IEmployeeService _employeeService;

        public HolidayController(IMapper mapper,
            IValidator<SavePublicHolidayDto> saveValidator,
            IValidator<UpdatePublicHolidayDto> updateValidator,
            IPublicHolidayService publicHolidayService,
            IEmployeeService employeeService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _publicHolidayService = publicHolidayService;
            _employeeService = employeeService;
        }

        public IActionResult PublicHoliday()
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

            ListPublicHoliday();
            return View();
        }

        public IActionResult ListPublicHoliday()
        {
            List<PublicHolidayDto> lstPublicHoliday = _publicHolidayService.GetAll().OrderBy(x => x.Date).ToList();

            return View(lstPublicHoliday);
        }

        [HttpPost]
        [Route("PublicHolidays/Save")]
        public IActionResult Save(SavePublicHolidayDto sDto)
        {
            string resultJs = "";

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Date", "Tarih");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _publicHolidayService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.IsNationalHoliday ? result.Result.Date.ToString("dd MMMM") : result.Result.Date.ToString("dd MMMM yyyy"));
            datas.Add(result.Result.IsNationalHoliday ? "Evet" : "Hayır");
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalPublicHoliday').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("PublicHolidays/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            PublicHolidayDto publicHolidayDto = _publicHolidayService.GetById(guid);

            resultJs += $"$('#uDate').val('{publicHolidayDto.Date.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#Id').val('{publicHolidayDto.Id}');";
            resultJs += $"$('#Title').text('{publicHolidayDto.Date.ToShortDateString()}');";
            resultJs += string.Format("$('#uIsNationalHoliday').prop('checked',{0});", publicHolidayDto.IsNationalHoliday ? "true" : "false");
            resultJs += $"$('#ModalUpdatePublicHoliday').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("PublicHolidays/Update")]
        public IActionResult Update(UpdatePublicHolidayDto uDto)
        {
            string resultJs = "";

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("Date", "Tarih");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            PublicHoliday publicHoliday = _publicHolidayService.GetEntityById(uDto.Id);
            publicHoliday.Date = uDto.Date;
            publicHoliday.IsNationalHoliday = uDto.IsNationalHoliday;
            _publicHolidayService.UpdateEntity(publicHoliday);

            List<string> datas = new List<string>();

            datas.Add(uDto.IsNationalHoliday ? uDto.Date.ToString("dd MMMM") : uDto.Date.ToString("dd MMMM yyyy"));
            datas.Add(uDto.IsNationalHoliday ? "Evet" : "Hayır");
            datas.Add(string.Format(htmlCode, uDto.Id));

            resultJs += TableTransactions.UpdateTable(datas, uDto.Id);

            resultJs += "$('#ModalUpdatePublicHoliday').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("PublicHolidays/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            PublicHoliday publicHoliday = _publicHolidayService.GetEntityById(Id);
            publicHoliday.DataType = Domain.Enums.EnumDataType.Deleted;

            _publicHolidayService.UpdateEntity(publicHoliday);

            resultJs += TableTransactions.DeleteTable(publicHoliday.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;PublicHolidays/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;PublicHolidays/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
