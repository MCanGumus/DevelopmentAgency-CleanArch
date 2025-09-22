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


namespace DA.Controllers.Authority
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class AcademyInfoController : Controller
    {
        IEmployeeService _employeeService;
        IAcademyInfoService _academyInfoService;
        IValidator<SaveAcademyInfoDto> _saveValidator;
        IValidator<UpdateAcademyInfoDto> _updateValidator;
        IMapper _mapper;
        public AcademyInfoController(IEmployeeService employeeService, IAcademyInfoService academyInfoService, IValidator<SaveAcademyInfoDto> saveValidator, IValidator<UpdateAcademyInfoDto> updateValidator, IMapper mapper)
        {
            _employeeService = employeeService;
            _academyInfoService = academyInfoService;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }
        public IActionResult AcademyInfo()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (model == null)
            {
                return Redirect("/");
            }

            ListAcademyInfo();
            return View();
        }

        public IActionResult ListAcademyInfo()
        {
            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (_employeeService.GetById(model.UserGid) == null)
                return Redirect("/YetkiYok");

            List<AcademyInfoDto> lst = _academyInfoService.GetAllUserAcademyInfos(model.UserGid);

            return View(lst);
        }

        [HttpPost]
        [Route("AcademyInfo/Save")]
        public IActionResult Save(SaveAcademyInfoDto sDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            sDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _saveValidator.Validate(sDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdEmployeeFK", "Çalışan")
                                            .Replace("University", "Üniversite")
                                            .Replace("Faculty", "Fakülte")
                                            .Replace("Department", "Bölüm")
                                            .Replace("ThesisTopic", "Tez Konusu")
                                            .Replace("StartDate", "Başlangıç Tarihi")
                                            .Replace("EndDate", "Bitiş Tarihi");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            var result = _academyInfoService.Insert(sDto);

            if (result == null)
            {
                return BadRequest();
            }

            List<string> datas = new List<string>();

            datas.Add(result.Result.University);
            datas.Add(result.Result.Faculty);
            datas.Add(result.Result.Department);
            datas.Add(string.IsNullOrEmpty(result.Result.ThesisTopic) ? " " : result.Result.ThesisTopic);
            datas.Add(StrAcademyType((EnumAcademyType)result.Result.AcademyType));
            datas.Add(result.Result.StartDate.ToString("dd.MM.yyyy") + " / " + (result.Result.EndDate == null ? "-" :  result.Result.StartDate.ToString("dd.MM.yyyy")));

            datas.Add(string.Format(htmlCode, result.Result.Id));

            resultJs += TableTransactions.AddToTable(datas, result.Result.Id);

            resultJs += "$('#ModalAcademy').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla eklendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("AcademyInfo/OpenModal")]
        public IActionResult OpenModal(Guid guid)
        {
            string resultJs = "";

            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            AcademyInfoDto academyInfoDto = _academyInfoService.GetById(guid);

            resultJs += $"$('#uUniversity').val('{academyInfoDto.University}');";
            resultJs += $"$('#uFaculty').val('{academyInfoDto.Faculty}');";
            resultJs += $"$('#uDepartment').val('{academyInfoDto.Department}');";
            resultJs += $"$('#uThesisTopic').val('{academyInfoDto.ThesisTopic}');";
            resultJs += $"$('#uAcademyType').val('{(int)academyInfoDto.AcademyType}').trigger('change');";
            resultJs += $"$('#uStartDate').val('{academyInfoDto.StartDate.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#uEndDate').val('{academyInfoDto.EndDate.ToString("yyyy-MM-dd")}');";

            resultJs += $"$('#uId').val('{academyInfoDto.Id}');";
            resultJs += $"$('#Title').text('{academyInfoDto.University}');";
            resultJs += $"$('#ModalUpdateAcademy').modal('show');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("AcademyInfo/Update")]
        public IActionResult Update(UpdateAcademyInfoDto uDto)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            uDto.IdEmployeeFK = model.UserGid;

            ValidationResult valResult = _updateValidator.Validate(uDto);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdEmployeeFK", "Çalışan")
                                            .Replace("University", "Üniversite")
                                            .Replace("Faculty", "Fakülte")
                                            .Replace("Department", "Bölüm")
                                            .Replace("ThesisTopic", "Tez Konusu")
                                            .Replace("StartDate", "Başlangıç Tarihi")
                                            .Replace("EndDate", "Bitiş Tarihi");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }
                return Ok(resultJs);
            }

            AcademyInfo academyInfoDto = _academyInfoService.GetEntityById(uDto.Id);

            academyInfoDto.University = uDto.University;
            academyInfoDto.Faculty = uDto.Faculty;
            academyInfoDto.StartDate = uDto.StartDate;
            academyInfoDto.EndDate = uDto.EndDate;
            academyInfoDto.AcademyType = uDto.AcademyType;
            academyInfoDto.Department = uDto.Department;

            _academyInfoService.Update(_mapper.Map<UpdateAcademyInfoDto>(academyInfoDto));

            List<string> datas = new List<string>();

            datas.Add(academyInfoDto.University);
            datas.Add(academyInfoDto.Faculty);
            datas.Add(academyInfoDto.Department);
            datas.Add(string.IsNullOrEmpty(academyInfoDto.ThesisTopic) ? " " : academyInfoDto.ThesisTopic);
            datas.Add(StrAcademyType((EnumAcademyType)academyInfoDto.AcademyType));
            datas.Add(academyInfoDto.StartDate.ToString("dd.MM.yyyy") + " / " + (academyInfoDto.EndDate == null ? "-" : academyInfoDto.StartDate.ToString("dd.MM.yyyy")));

            datas.Add(string.Format(htmlCode, academyInfoDto.Id));

            resultJs += TableTransactions.UpdateTable(datas, academyInfoDto.Id);

            resultJs += "$('#ModalUpdateAcademy').modal('hide');";
            resultJs += "ShowSuccessMessage('Başarıyla güncellendi.');";

            return Ok(resultJs);
        }

        [HttpPost]
        [Route("AcademyInfo/Delete")]
        public IActionResult Delete(Guid Id)
        {
            string resultJs = "";
            AcademyInfo academyInfo = _academyInfoService.GetEntityById(Id);
            academyInfo.DataType = Domain.Enums.EnumDataType.Deleted;

            _academyInfoService.UpdateEntity(academyInfo);

            resultJs += TableTransactions.DeleteTable(academyInfo.Id);

            resultJs += "ShowSuccessMessage('Başarıyla silindi.');";

            return Ok(resultJs);
        }

        public const string htmlCode = "<a onclick=\"AjaxMethod(&apos;AcademyInfo/OpenModal&apos;, &apos;{0}&apos;, &apos;Update&apos;)\" href=\"\"><i class=\"mdi mdi-table-edit text-success md20\"></i></a><a onclick=\"AjaxMethod(&apos;AcademyInfo/Delete&apos;, &apos;{0}&apos;, &apos;Delete&apos;)\" href=\"\"><i class=\"mdi mdi-delete text-danger md20\"></i></a>";


        public static string StrAcademyType(EnumAcademyType enumAcademyType)
        {
            switch (enumAcademyType)
            {
                case (EnumAcademyType.BachelorsDegree):
                    return "Lisans";
                case (EnumAcademyType.MastersDegree):
                    return "Yüksek Lisans";
                case (EnumAcademyType.Doctorate):
                    return "Doktora";

                default:
                    return "";
            }
        }
    }
}
