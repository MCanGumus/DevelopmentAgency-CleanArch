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
    public class AddressController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveAddressDto> _saveValidator;
        private readonly IValidator<UpdateAddressDto> _updateValidator;
        private readonly IAddressService _addressService;

        public AddressController(IMapper mapper,
            IValidator<SaveAddressDto> saveValidator,
            IValidator<UpdateAddressDto> updateValidator,
            IAddressService addressService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _addressService = addressService;
        }

        public IActionResult Address()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            ListAddress();
            return View();
        }

        public IActionResult ListAddress()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            List<AddressDto> lstAddress = _addressService.GetAllMyAddresses(model.UserGid).OrderByDescending(x => x.RecordDate).ToList();

            return View(lstAddress);
        }

        [HttpPost]
        [Route("Addresses/Save")]
        public IActionResult Save(SaveAddressDto sDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            sDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("AddressTitle", "Adres Başlığı").Replace("FullAddress", "Tam Adres");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            var result = _addressService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.AddressTitle);
            datas.Add(result.Result.FullAddress);
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalAddress').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Addresses/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            AddressDto addressDto = _addressService.GetById(guid);

            resultJs += $"$('#uAddressTitle').val('{addressDto.AddressTitle}');";
            resultJs += $"$('#uFullAddress').val('{addressDto.FullAddress}');";
            resultJs += $"$('#uId').val('{addressDto.Id}');";
            resultJs += $"$('#Title').text('{addressDto.AddressTitle}');";
            resultJs += $"$('#ModalUpdateAddress').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Addresses/Update")]
        public IActionResult Update(UpdateAddressDto uDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            uDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString().Replace("AddressTitle", "Adres Başlığı").Replace("FullAddress", "Tam Adres");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            Address address = _addressService.GetEntityById(uDto.Id);
            address.AddressTitle = uDto.AddressTitle;
            address.FullAddress = uDto.FullAddress;
            _addressService.UpdateEntity(address);

            List<string> datas = new List<string>();

            datas.Add(address.AddressTitle);
            datas.Add(address.FullAddress);
            datas.Add(string.Format(htmlCode, address.Id));

            resultJs += TableTransactions.UpdateTable(datas, address.Id);

            resultJs += "$('#ModalUpdateAddress').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("Addresses/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";

            Address address = _addressService.GetEntityById(Id);
            address.DataType = Domain.Enums.EnumDataType.Deleted;

            _addressService.UpdateEntity(address);

            resultJs += TableTransactions.DeleteTable(address.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;Addresses/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;Addresses/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";
    }
}
