using AutoMapper;
using DA.Application.Abstractions;
using DA.Application.Abstractions.Authority;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Dtos.Authority.FamilyMemberDtos;
using DA.Domain.Entities.Authority;
using DA.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.Authority
{
    public class FamilyMemberController : Controller
    {
        IEmployeeService _employeeService;
        IFamilyMemberService _familyMemberService;
        IValidator<SaveFamilyMemberDto> _saveValidator;
        IValidator<UpdateFamilyMemberDto> _updateValidator;
        IMapper _mapper;
        public FamilyMemberController(IEmployeeService employeeService, IFamilyMemberService famiyMemberService, IValidator<SaveFamilyMemberDto> saveValidator, IValidator<UpdateFamilyMemberDto> updateValidator, IMapper mapper)
        {
            _employeeService = employeeService;
            _familyMemberService = famiyMemberService;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }
        public IActionResult FamilyMember()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model == null)
            {
                return Redirect("/");
            }

            ListFamilyMember();
            return View();
        }

        public IActionResult ListFamilyMember()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (_employeeService.GetById(model.UserGid) == null)
                return Redirect("/YetkiYok");

            List<FamilyMemberDto> lst = _familyMemberService.GetFamilyMembers(model.UserGid).ToList();

            return View(lst);
        }

        [HttpPost]
        [Route("FamilyMember/Save")]
        public IActionResult Save(SaveFamilyMemberDto sDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            sDto.IdEmployeeFK = model.UserGid; // IdEmployeeFK, giriş yapan kullanıcıya göre ayarlanıyor

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                          .Replace("IdEmployeeFK", "Çalışan")
                                          .Replace("NameSurname", "Ad Soyad")
                                          .Replace("NationalIdentityNumber", "T.C. Kimlik No")
                                          .Replace("DateOfBirth", "Doğum Tarihi")
                                          .Replace("RelationType", "Yakınlık Türü")
;

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            var result = _familyMemberService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.NationalIdentityNumber);
            datas.Add(result.Result.NameSurname);
            datas.Add(result.Result.RelationType.ToString());
            datas.Add(result.Result.DateOfBirth.ToString("dd.MM.yyyy"));
            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalFamilyMember').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("FamilyMember/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            FamilyMemberDto familyMemberDto = _familyMemberService.GetById(guid);

            resultJs += $"$('#uNameSurname').val('{familyMemberDto.NameSurname}');";
            resultJs += $"$('#uNationalIdentityNumber').val('{familyMemberDto.NationalIdentityNumber}');";
            resultJs += $"$('#uDateOfBirth').val('{familyMemberDto.DateOfBirth.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#uDescription').val('{familyMemberDto.Description}');";

            if (familyMemberDto.RelationType == Domain.Enums.EnumRelationType.Eş)
            {
                resultJs += "$('#uForPartner').show();";
                resultJs += "$('#uForChild').hide();";
                resultJs += $"$('#uIsWorking').prop('checked','{familyMemberDto.IsWorking}');";
                resultJs += $"$('#uHasIncome').prop('checked','{familyMemberDto.HasIncome}');";
            }
            else if(familyMemberDto.RelationType == Domain.Enums.EnumRelationType.Evlat)
            {
                resultJs += "$('#uForChild').show();";
                resultJs += "$('#uForPartner').hide();";
                resultJs += $"$('#uFatherName').val('{familyMemberDto.FatherName}');";
                resultJs += $"$('#uMotherName').val('{familyMemberDto.MotherName}');";
                resultJs += $"$('#uGender').val('{(familyMemberDto.Gender == null ? 0 : (int)familyMemberDto.Gender)}').trigger('change');";
                resultJs += $"$('#uChildState').val('{(familyMemberDto.ChildState == null ? 0 : (int)familyMemberDto.ChildState)}').trigger('change');";
                resultJs += $"$('#uDateOfStart').val('{(familyMemberDto.DateOfStart == null ? "" : familyMemberDto.DateOfStart.Value.ToString("yyyy-MM-dd"))}');";
                resultJs += $"$('#uIsInEducation').prop('checked','{familyMemberDto.IsInEducation}');";
                resultJs += $"$('#uSchoolName').val('{familyMemberDto.SchoolName}');";
                resultJs += $"$('#uClass').val('{familyMemberDto.Class}');";

            }

            resultJs += $"$('#uId').val('{familyMemberDto.Id}');";
            resultJs += $"$('#Title').text('{familyMemberDto.NameSurname + (familyMemberDto.RelationType == Domain.Enums.EnumRelationType.Eş ? " (Eşi)" : " (Çocuğu)")}');";
            resultJs += $"$('#ModalUpdateFamilyMember').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("FamilyMember/Update")]
        public IActionResult Update(UpdateFamilyMemberDto uDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            uDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                          .Replace("IdEmployeeFK", "Çalışan")
                                          .Replace("NameSurname", "Ad Soyad")
                                          .Replace("NationalIdentityNumber", "T.C. Kimlik No")
                                          .Replace("DateOfBirth", "Doğum Tarihi")
                                          .Replace("RelationType", "Yakınlık Türü")
                                          .Replace("Gender", "Cinsiyet");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            FamilyMember familyMember = _familyMemberService.GetEntityById(uDto.Id);

            familyMember.NameSurname = uDto.NameSurname;
            familyMember.NationalIdentityNumber = uDto.NationalIdentityNumber;
            familyMember.DateOfBirth = uDto.DateOfBirth;
            familyMember.Gender = uDto.Gender;
            familyMember.RelationType = uDto.RelationType;

            _familyMemberService.Update(_mapper.Map<UpdateFamilyMemberDto>(familyMember));

            List<string> datas = new List<string>();

            datas.Add(uDto.NationalIdentityNumber);
            datas.Add(uDto.NameSurname);
            datas.Add(uDto.RelationType.ToString());
            datas.Add(uDto.DateOfBirth.ToString("dd.MM.yyyy"));
            datas.Add(familyMember.RelationType.ToString());

            datas.Add(string.Format(htmlCode, familyMember.Id));

            resultJs += TableTransactions.UpdateTable(datas, familyMember.Id);

            resultJs += "$('#ModalUpdateFamilyMember').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("FamilyMember/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            FamilyMember familyMember = _familyMemberService.GetEntityById(Id);
            familyMember.DataType = Domain.Enums.EnumDataType.Deleted;

            _familyMemberService.UpdateEntity(familyMember);

            resultJs += TableTransactions.DeleteTable(familyMember.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;FamilyMember/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;FamilyMember/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";

    }
}
