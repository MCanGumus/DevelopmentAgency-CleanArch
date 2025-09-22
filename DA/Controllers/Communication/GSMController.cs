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
    public class GSMController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveGSMNumberDto> _saveValidator;
        private readonly IValidator<UpdateGSMNumberDto> _updateValidator;
        private readonly IGSMNumberService _gsmNumberService;

        public GSMController(IMapper mapper,
            IValidator<SaveGSMNumberDto> saveValidator,
            IValidator<UpdateGSMNumberDto> updateValidator,
            IGSMNumberService gsmNumberService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _gsmNumberService = gsmNumberService;
        }

        public IActionResult GSM()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListGSMNumber();
            return View();
        }

        public IActionResult ListGSMNumber()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<GSMNumberDto> lstGSMNumber = _gsmNumberService.GetAllMyNumbers(model.UserGid).OrderByDescending(x => x.RecordDate).ToList();

            return View(lstGSMNumber);
        }

        [HttpPost]
        [Route("GSMNumbers/Save")]
        public IActionResult Save(SaveGSMNumberDto sDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            sDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("GSM", "Cep Telefonu Numarası");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _gsmNumberService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.GSM);
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalGSM').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("GSMNumbers/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            GSMNumberDto gsmNumberDto = _gsmNumberService.GetById(guid);

            resultJs += $"$('#uGSM').val('{gsmNumberDto.GSM}');";
            resultJs += $"$('#uId').val('{gsmNumberDto.Id}');";
            resultJs += $"$('#Title').text('{gsmNumberDto.GSM}');";
            resultJs += $"$('#ModalUpdateGSM').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("GSMNumbers/Update")]
        public IActionResult Update(UpdateGSMNumberDto uDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            uDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("GSM", "Cep Telefonu Numarası");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            GSMNumber gsmNumber = _gsmNumberService.GetEntityById(uDto.Id);
            gsmNumber.GSM = uDto.GSM;
            _gsmNumberService.UpdateEntity(gsmNumber);

            List<string> datas = new List<string>();

            datas.Add(gsmNumber.GSM);
            datas.Add(string.Format(htmlCode, gsmNumber.Id));

            resultJs += TableTransactions.UpdateTable(datas, gsmNumber.Id);

            resultJs += "$('#ModalUpdateGSM').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("GSMNumbers/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            GSMNumber gsmNumber = _gsmNumberService.GetEntityById(Id);
            gsmNumber.DataType = Domain.Enums.EnumDataType.Deleted;

            _gsmNumberService.UpdateEntity(gsmNumber);

            resultJs += TableTransactions.DeleteTable(gsmNumber.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;GSMNumbers/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;GSMNumbers/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
