using DA.Application.Abstractions;
using DA.Domain.Dtos;
using DA.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace DA.Components.System
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogEntryService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingFilterAttribute(ILogEntryService logService, IHttpContextAccessor httpContextAccessor)
        {
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var user = SessionHelper.GetEmployeeLoggingIn(context.HttpContext);

                if (user != null)
                {
                    var userId = user.UserGid; // Session'dan kullanıcı Id'sini alın
                    var actionName = context.ActionDescriptor.DisplayName;
                    var arguments = JsonConvert.SerializeObject(context.ActionArguments);
                    var userName = user.UserName;

                    _httpContextAccessor.HttpContext.Items["LogUserId"] = userId;
                    _httpContextAccessor.HttpContext.Items["LogUserName"] = userName;
                    _httpContextAccessor.HttpContext.Items["LogActionName"] = actionName;
                    _httpContextAccessor.HttpContext.Items["LogArguments"] = arguments;

                    base.OnActionExecuting(context);
                }
                else { context.HttpContext.Response.Redirect("/"); }
            }
            catch (Exception)
            {
                context.HttpContext.Response.Redirect("/");
            }

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                SaveLogEntryDto save = new SaveLogEntryDto();

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                save.UserId = Guid.Parse(_httpContextAccessor.HttpContext.Items["LogUserId"]?.ToString() ?? "Unknown");
                save.UserName = _httpContextAccessor.HttpContext.Items["LogUserName"]?.ToString() ?? "Unknown";
                save.ActionName = _httpContextAccessor.HttpContext.Items["LogActionName"]?.ToString() ?? "Bilgi Yok.";
                save.Arguments = _httpContextAccessor.HttpContext.Items["LogArguments"]?.ToString() ?? "Bilgi Yok.";
                save.Result = JsonConvert.SerializeObject(context.Result == null ? "" : context.Result, settings);

                if (save.Result.Contains("\"StatusCode\":null"))
                {
                    save.Result = "Page Reloaded";
                }

                _logService.Insert(save);

                base.OnActionExecuted(context);

            }
            catch (Exception)
            {
                context.HttpContext.Response.Redirect("/");
            }
        }
    }
}
