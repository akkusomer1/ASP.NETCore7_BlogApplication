using Microsoft.EntityFrameworkCore;
using NLog.Web;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Data.Context;
using ProgrammersBlog.Mvc.Extantions;
using ProgrammersBlog.Mvc.Filter;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Mvc.Helpers.Concrete;
using ProgrammersBlog.Mvc.Mapping;
using ProgrammersBlog.Services.Extantion;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProgrammersBlogContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("localDb"), opt =>
    {
        opt.MigrationsAssembly(Assembly.GetAssembly(typeof(ProgrammersBlogContext))?.GetName().Name);
    }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Host.ConfigureAppConfiguration((hostingcontext, config) =>
{
    config.Sources.Clear();
    var environment = hostingcontext.HostingEnvironment;
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true);

    config.AddEnvironmentVariables();
    if (args != null)
    {
        config.AddCommandLine(args);
    }
});
builder.Logging.ClearProviders();
builder.Host.UseNLog();


builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<WebSiteInfo>(builder.Configuration.GetSection("WebsiteInfo"));
builder.Services.Configure<AboutUsPageInfo>(builder.Configuration.GetSection("AboutUsPageInfo"));
builder.Services.Configure<ArticleRightSideBarWidgetOptions>(builder.Configuration.GetSection("ArticleRightSideBarWidgetOptions"));


builder.Services.ConfigureWritable<AboutUsPageInfo>(builder.Configuration.GetSection("AboutUsPageInfo"));
builder.Services.ConfigureWritable<WebSiteInfo>(builder.Configuration.GetSection("WebsiteInfo"));
builder.Services.ConfigureWritable<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.ConfigureWritable<ArticleRightSideBarWidgetOptions>(builder.Configuration.GetSection("ArticleRightSideBarWidgetOptions"));


builder.Services.AddAutoMapper(typeof(ViewModelMapProfile));
builder.Services.AddSession();

builder.Services.LoadMyService();
builder.Services.AddScoped<IImageHelper, ImageHelper>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Admin/Auth/Login");
    options.LogoutPath = new PathString("/Admin/User/Logout");
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);

    options.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");
    options.Cookie = new CookieBuilder()
    {
        Name = "ProgrammersBlog",
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        SecurePolicy = CookieSecurePolicy.SameAsRequest
    };

});

builder.Services.AddControllersWithViews(opt =>
{
    opt.Filters.Add<ExceptionFilter>();
    opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Bu alan boþ geçilmemelidir.");
}).AddNToastNotifyToastr().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseSession();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseNToastNotify();
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
        );

    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "article",
        pattern: "{title}/{articleId}",
        defaults: new { controller = "Article", action = "Detail" }
    );
});


app.Run();
