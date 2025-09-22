using DA.Components.System;
using DA.Models;
using Microsoft.AspNetCore.Mvc;

namespace Orjeen.Controllers.Errors
{
    public class ErrorController : Controller
    {
        [ServiceFilter(typeof(LoggingFilterAttribute))]
        public IActionResult AccessDenied()
        {
            LoginSessionModel loginnedEmployee = SessionHelper.GetEmployeeLoggingIn(HttpContext);

            if (loginnedEmployee == null)
            {
                return Redirect("/");
            }


            return View();
        }
    }
}
