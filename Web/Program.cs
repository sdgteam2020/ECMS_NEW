using AutoMapper;
using BusinessLogicsLayer;
using BusinessLogicsLayer.Helpers;
using DataAccessLayer;
using DataAccessLayer.Logger;
using DataTransferObject;
using DataTransferObject.Domain.Identitytable;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using Web.Data;
using ApplicationUser = DataTransferObject.Domain.Identitytable.ApplicationUser;
using BusinessLogicsLayer.Service;

var builder = WebApplication.CreateBuilder(args);
var configration = builder.Configuration;


builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(configration.GetConnectionString("AFSACDBConnection")));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
       //This one did not work ForwardedHeaders.XForwardedFor | 
       ForwardedHeaders.XForwardedHost |
       ForwardedHeaders.XForwardedProto;
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    option.Password.RequireNonAlphanumeric = true;
    option.Password.RequireUppercase = true;
    option.Password.RequireDigit = true;
    option.Password.RequiredLength = 8;
    option.Password.RequiredUniqueChars = 1;
    option.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Logging.AddDbLogger(options =>
{
    builder.Configuration.GetSection("Logging").GetSection("Database").GetSection("Options").Bind(options);
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromMinutes(20);
});

builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
    opt.ValidationInterval = TimeSpan.FromSeconds(0)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Add services to the container.
//builder.Services.AddRazorPages();
//builder.Services.AddScoped<IGenericRepositoryDL, GenericRepositoryDL>();
builder.Services.AddScoped<IService, ServiceRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITagHelperInitializer<ScriptTagHelper>, AppendVersionTagHelperInitializer>();
builder.Services.AddSingleton<ITagHelperInitializer<LinkTagHelper>, AppendVersionTagHelperInitializer>();
builder.Services.AddInfrastructure();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Use the default property (Pascal) casing
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

builder.Services.Configure<IdentityOptions>(opts =>
{
    //opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.MaxFailedAccessAttempts = 3;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
});
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    //options.Cookie.Expiration 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    //options.SlidingExpiration = true; // IS OPTION KO BAAD ME UNCOMMENT.

    //options.ReturnUrlParameter=""
});

builder.Services.AddSingleton<DataProtectionPurposeStrings>();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(180);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Add("Content-Security-Policy", "default-src *; style-src 'self' http://* 'unsafe-inline'; script-src 'self' http://* 'unsafe-inline' 'unsafe-eval'; img-src 'self' http://* data:;");
    ctx.Response.Headers.Add("Feature-Policy", "fullscreen 'none'");
    ctx.Response.Headers.Add("Referrer-Policy", "same-origin");
    ctx.Response.Headers.Add("X-Frame-Options", "DENY");
    ctx.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    ctx.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    //ctx.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    ctx.Response.Headers.Remove("X-Powered-By");
    ctx.Response.Headers.Remove("x-aspnet-version");
    // Some headers won't remove
    ctx.Response.Headers.Remove("Server");
    await next();
});

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

//app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
