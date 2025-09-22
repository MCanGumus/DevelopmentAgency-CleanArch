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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DA.Controllers.Authority
{
    [ServiceFilter(typeof(LoggingFilterAttribute))]
    public class ProfileController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<SaveEmployeeDto> _saveValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;
        private readonly IEmployeeService _employeeService;

        public ProfileController(IMapper mapper,
            IValidator<SaveEmployeeDto> saveValidator,
            IValidator<UpdateEmployeeDto> updateValidator,
            IEmployeeService employeeService)
        {
            _mapper = mapper;
            _saveValidator = saveValidator;
            _updateValidator = updateValidator;
            _employeeService = employeeService;
        }
        public IActionResult Profile()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            EmployeeDto employee = _employeeService.GetById(loginnedEmployee.UserGid);

            return View(employee);
        }

        [HttpPost]
        [Route("Profile/Update")]
        public IActionResult Update(UpdateEmployeeDto employee)
        {
            string resultJs = "";

            LoginSessionModel model = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            Employee employeeDto = _employeeService.GetEntityById(model.UserGid);

            if (employeeDto == null)
                return BadRequest();

            if (!string.IsNullOrEmpty(employee.Password))
            {
                PasswordHash passwordHash = new PasswordHash();

                employeeDto.Password = passwordHash.HashPasword(employee.Password, out var salt);
                employeeDto.PasswordSalt = Convert.ToBase64String(salt);
                employee.PasswordSalt = employeeDto.PasswordSalt;
                employee.Password = employeeDto.Password;
            }
            else
            {
                employee.Password = employeeDto.Password;
                employee.PasswordSalt= employeeDto.PasswordSalt;
            }

            employee.DateOfStart = employeeDto.DateOfStart;
            employee.TotalYearlyLeave = employeeDto.TotalYearlyLeave;
            employee.TotalUnpaidLeave = employeeDto.TotalUnpaidLeave;

            ValidationResult valResult = _updateValidator.Validate(employee);

            if (!valResult.IsValid)
            {
                string message = valResult.ToString()
                                            .Replace("IdentificationNumber", "Kimlik Numarası")
                                            .Replace("Name", "İsim")
                                            .Replace("Surname", "Soyisim")
                                            .Replace("MotherName", "Anne Adı")
                                            .Replace("FatherName", "Baba Adı")
                                            .Replace("PlaceOfBirth", "Doğum Yeri")
                                            .Replace("DateOfBirth", "Doğum Tarihi")
                                            .Replace("DateOfStart", "Başlangıç Tarihi")
                                            .Replace("Email", "E-posta")
                                            .Replace("Password", "Şifre")
                                            .Replace("PasswordSalt", "Şifre Tuzu")
                                            .Replace("TotalYearlyLeave", "Yıllık İzin Toplamı")
                                            .Replace("TotalUnpaidLeave", "Ücretsiz İzin Toplamı")
                                            .Replace("TotalExcusedLeave", "Mazeretli İzin Toplamı")
                                            .Replace("RegistrationNumber", "Kayıt Numarası");

                foreach (string item in message.Split("\r\n"))
                {
                    resultJs += $@"ShowErrorMessage(""{item}"");";
                }

                return Ok(resultJs);
            }

            employeeDto.FatherName = employee.FatherName;
            employeeDto.MotherName = employee.MotherName;
            employeeDto.DateOfBirth = employee.DateOfBirth;
            employeeDto.PlaceOfBirth = employee.PlaceOfBirth;
            employeeDto.Gender = employee.Gender;
            employeeDto.BloodGroup = employee.BloodGroup;
            
            _employeeService.UpdateEntity(employeeDto);

            resultJs += $"$('#uName').val('{employee.Name}');";
            resultJs += $"$('#uSurname').val('{employee.Surname}');";
            resultJs += $"$('#uMotherName').val('{employee.MotherName}');";
            resultJs += $"$('#uFatherName').val('{employee.FatherName}');";
            resultJs += $"$('#uPlaceOfBirth').val('{employee.PlaceOfBirth}');";
            resultJs += $"$('#uDateOfBirth').val('{employee.DateOfBirth.ToString("yyyy-MM-dd")}');";
            resultJs += $"$('#uBloodGroup').val('{(int)employee.BloodGroup}').trigger('change');";
            resultJs += $"$('#uGender').val('{(int)employee.Gender}').trigger('change');";
            resultJs += $"$('#uEmail').val('{employee.Email}');";
            resultJs += $"$('#Password').val('');";

            resultJs += "ShowSuccessMessage('Çalışan bilgisi başarıyla güncellendi.');";

            return Ok(resultJs);
        }
    }

   
}
