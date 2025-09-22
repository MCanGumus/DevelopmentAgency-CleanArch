using DocumentFormat.OpenXml.InkML;
using DA.Application.Abstractions;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace DA.Controllers
{

    public class LoginController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IPermissionService _permissionService;
        private readonly IMissionService _missionService;
        public LoginController(IEmployeeService employeeService, IDepartmentService departmentService, IPermissionService permissionService, IMissionService missionService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _permissionService = permissionService;
            _missionService = missionService;
        }
        public IActionResult LoginPage()
        {
            // Oturumu tamamen temizle
            HttpContext.Session.Clear();

            // Oturumu sonlandır
            HttpContext.Session.Remove("LoginInfos");

            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.Response.Cookies.Delete(cookie);
            }

            return View();
        }

        [HttpPost]
        [Route("SignIn")]
        public IActionResult SignIn(ModelLogin modelLogin)
        {
            try
            {
                var employee = _employeeService.GetEmployeeWithMail(modelLogin.EMail);

                if (employee == null)
                {
                    return Ok("ShowErrorMessage('Kullanıcı bulunamadı!')");
                }

                PasswordHash psh = new PasswordHash();

                if (!psh.VerifyPassword(modelLogin.Password, employee.Password, Convert.FromBase64String(employee.PasswordSalt)))
                {
                    return Ok("ShowErrorMessage('Kullanıcı adı veya şifre yanlış!')");
                }

                Department department = null;
                Department departmentHelper = null;

                if (employee.AuthorizationStatus != Domain.Enums.EnumAuthorizationStatus.Employee)
                {
                    department = _departmentService.GetPresidentOfDepartment(employee.Id);
                    departmentHelper = _departmentService.GetHelperOfDepartment(employee.Id);
                }

                List<Guid> departments = new List<Guid>();

                if (employee.AuthorizationStatus == Domain.Enums.EnumAuthorizationStatus.Admin) 
                {
                    List<PermissionDto> permissions = _permissionService.GetPermissionsByProxy(employee.Id);
                    List<MissionDto> missions = _missionService.GetAllMissionsByProxy(employee.Id);

                    foreach (var permission in permissions)
                    {
                        if (!departments.Contains(permission.Department.Id))
                        {
                            departments.Add(permission.Department.Id);
                        }
                    }

                    foreach (var mission in missions)
                    {
                        if (!departments.Contains(mission.Employee.Department.Id))
                        {
                            departments.Add(mission.Employee.Department.Id);
                        }
                    }
                }

                LoginSessionModel model = new LoginSessionModel()
                {
                    UserGid = employee.Id,
                    UserName = employee.Name + " " + employee.Surname,
                    Role = employee.AuthorizationStatus == Domain.Enums.EnumAuthorizationStatus.YetkiliCalisan ? Domain.Enums.EnumAuthorizationStatus.Employee.ToString() : employee.AuthorizationStatus.ToString(),
                    ExtendedRole = employee.AuthorizationStatus == Domain.Enums.EnumAuthorizationStatus.YetkiliCalisan ? true : false,
                    DepartmentGid = employee.Department == null ? Guid.Empty : employee.Department.Id,
                    DepartmentHeadGid = department == null ? null : department.Id,
                    DepartmentHelperGid = departmentHelper == null ? null : departmentHelper.Id,
                    DepartmentAppointments = departments
                };

                SessionHelper.SetEmployeeLoggingIn(HttpContext ,model);

                #region Logging

                #endregion

                return Ok("window.location.href = '/Anasayfa';");
            }
            catch (Exception ex)
            {
                return Ok("console.log('Exception: " + ex.ToString() + "'); ShowErrorMessage('" + ex.ToString() + "');");
            }

        }

        public IActionResult UserExit(JObject obj)
        {
            // Oturumu tamamen temizle
            HttpContext.Session.Clear();

            // Oturumu sonlandır
            HttpContext.Session.Remove("LoginInfos");

            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.Response.Cookies.Delete(cookie);
            }

            return Ok("window.location.href = '/';");
        }

    }
}
