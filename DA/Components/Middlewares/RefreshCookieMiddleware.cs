namespace DA.Components.Middlewares
{
    public class RefreshCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public RefreshCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Çerezin süresini her istek geldiğinde uzat
            if (context.Request.Cookies.ContainsKey("LoginInfos"))
            {
                string cookieValue = context.Request.Cookies["LoginInfos"];
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(60), // Süreyi uzat
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                context.Response.Cookies.Append("LoginInfos", cookieValue, option);
            }

            await _next(context);
        }
    }
}
