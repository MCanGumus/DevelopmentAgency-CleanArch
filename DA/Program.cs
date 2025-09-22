using DA.Application.Mapper;
using DA.Persistence;
using DA.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DA.Application;
using DA.Application.Mapper;
using DA.Persistence;
using DA.Persistence.Context;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json.Serialization;
using DA.Components.System;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Hangfire;
using DA.Components.Middlewares;
using DA.Components.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddMvc();

#region Connection Strings - This should be removed from here.
var productionConnectionString = "Server=localhost\\SQLEXPRESS;Database=KA;Trusted_Connection=True;TrustServerCertificate=True;";
var developmentConnectionString = "Server=localhost\\SQLEXPRESS;Database=KA;Trusted_Connection=True;TrustServerCertificate=True;";
#endregion

#region Resolve Dependency Injection

builder.Services.AddContainerWithDependenciesApplication();
builder.Services.AddContainerWithDependenciesPersistence();

builder.Services.AddAutoMapper(typeof(DefinitionsProfile));

#endregion  

#region DbContext
builder.Services.AddDbContext<DAContext>(options =>
{
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
    {
        options.UseSqlServer(productionConnectionString, builder => builder.MigrationsAssembly("DA.Persistence"));
    }
    else // In Development Environment
    {
        options.UseSqlServer(developmentConnectionString, builder => builder.MigrationsAssembly("DA.Persistence"));
    }
});
#endregion  

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LoggingFilterAttribute>();
builder.Services.AddSingleton<BirthdayReminder>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(90);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(90);
    options.Cookie.MaxAge = TimeSpan.FromMinutes(90);
    options.SlidingExpiration = true;
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo> { new CultureInfo("tr-TR") };
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

#region Hangfire

builder.Services.AddHangfire((sp, config) =>
{
    config.UseSqlServerStorage(productionConnectionString);

});

builder.Services.AddHangfireServer();


builder.Services.AddRazorPages();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();

    var timezone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    var hangfireHour = builder.Configuration["HangfireHour"];
    var hangfireMinutes = builder.Configuration["HangfireMinutes"];

    // Create the CRON expression using the values from configuration
    var cronExpression = $"{hangfireMinutes} {hangfireHour} * * *";

    RecurringJob.AddOrUpdate<BirthdayReminder>(
        "birthday-reminder-job",
        x => x.CheckSpecialDates(),
        cronExpression, 
        timezone 
    );
}

//app.UseHangfireDashboard();

#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<RefreshCookieMiddleware>();

app.UseAuthorization();

app.UseSession();

var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

app.MapControllerRoute(
    name: "login",
    pattern: "{controller=Login}/{action=LoginPage}/{id?}");

#region Endpoints

app.UseEndpoints(endpoints =>
{
    endpoints.MapCustomRoutes();

});

#endregion


app.Run();
