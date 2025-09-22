using AutoMapper;
using DA.Application.Repositories.OldEntities;
using DA.Components.System;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Domain.Entities.OldDatas;
using DA.Models;
using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers.OldEntities
{
    public class OldEmployeesController : Controller
    {
        private readonly IOldEmployeesReadRepository _readRepository;
        public OldEmployeesController(IOldEmployeesReadRepository readRepository)
        {
            _readRepository = readRepository;
           
        }
        public IActionResult OldEmployees()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }

            List<OldEmployees> allEmployees = _readRepository.GetAll().ToList();

            return View(allEmployees);
        }
    }
}
