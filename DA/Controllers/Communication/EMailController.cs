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
    public class EMailController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveEMailDto> _saveValidator;
        private readonly IValidator<UpdateEMailDto> _updateValidator;
        private readonly IEMailService _emailService;

        public EMailController(IMapper mapper,
            IValidator<SaveEMailDto> saveValidator,
            IValidator<UpdateEMailDto> updateValidator,
            IEMailService emailService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _emailService = emailService;
        }

        public IActionResult EMail()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListEMail();
            return View();
        }

        public IActionResult ListEMail()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<EMailDto> lstEMail = _emailService.GetAllMyMails(model.UserGid).OrderByDescending(x => x.RecordDate).ToList();

            return View(lstEMail);
        }

        [HttpPost]
        [Route("EMails/Save")]
        public IActionResult Save(SaveEMailDto sDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            sDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("EMailAddress", "E-posta Adresi");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _emailService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.EMailAddress);
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalEMail').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("EMails/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            EMailDto EMailDto = _emailService.GetById(guid);

            resultJs += $"$('#uEMailAddress').val('{EMailDto.EMailAddress}');";
            resultJs += $"$('#uId').val('{EMailDto.Id}');";
            resultJs += $"$('#Title').text('{EMailDto.EMailAddress}');";
            resultJs += $"$('#ModalUpdateEMail').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("EMails/Update")]
        public IActionResult Update(UpdateEMailDto uDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            uDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("EMailAddress", "E-posta Adresi");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            EMail EMail = _emailService.GetEntityById(uDto.Id);
            EMail.EMailAddress = uDto.EMailAddress;
            _emailService.UpdateEntity(EMail);

            List<string> datas = new List<string>();

            datas.Add(EMail.EMailAddress);
            datas.Add(string.Format(htmlCode, EMail.Id));

            resultJs += TableTransactions.UpdateTable(datas, EMail.Id);

            resultJs += "$('#ModalUpdateEMail').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("EMails/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            EMail EMail = _emailService.GetEntityById(Id);
            EMail.DataType = Domain.Enums.EnumDataType.Deleted;
            
            _emailService.UpdateEntity(EMail);

            resultJs += TableTransactions.DeleteTable(EMail.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;EMails/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;EMails/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
