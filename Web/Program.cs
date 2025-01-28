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
using ApplicationUser = DataTransferObject.Domain.Identitytable.ApplicationUser;
using BusinessLogicsLayer.Service;
using Microsoft.SqlServer.Management.Smo.Wmi;
using DataAccessLayer.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Web.Healpers;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Rewrite;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var configration = builder.Configuration;


builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(configration.GetConnectionString("AFSACDBConnection")).UseExceptionProcessor());

//builder.Services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders =
//       //This one did not work ForwardedHeaders.XForwardedFor | 
//       ForwardedHeaders.XForwardedHost |
//       ForwardedHeaders.XForwardedProto;
//});

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
        builder => builder.WithOrigins("http://localhost", "*")
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Add services to the container.
//builder.Services.AddRazorPages();
//builder.Services.AddScoped<IGenericRepositoryDL, GenericRepositoryDL>();
builder.Services.AddScoped<IService, ServiceRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<DapperContextDb2>();
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

    //opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    //opts.Lockout.MaxFailedAccessAttempts = 3;
    //opts.User.RequireUniqueEmail = false;
    //opts.SignIn.RequireConfirmedAccount = true;
    //opts.SignIn.RequireConfirmedEmail = false;
    //opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.MaxFailedAccessAttempts = 3;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
});
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
           {
               // Configure cookie options if needed
               options.Cookie.HttpOnly = true;
      
               options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
               options.LoginPath = "/Account/IMLogin";
               options.AccessDeniedPath = "/Account/AccessDenied";
               options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
               // Add other configuration options as needed
           });


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // When the code is published on IAM, these two lines are commented out.
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //options.Cookie.SameSite = SameSiteMode.Strict;
    //------------------- End Instructions----------------------
});
builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
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
    options.LoginPath = "/Account/IMLogin";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true; 

    //options.ReturnUrlParameter=""
});

builder.Services.AddAuthorizationPolicies();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// When the code is published on IAM, these ConfigureKestrel code are uncommented.
//builder.WebHost.ConfigureKestrel(i =>
//{
//    i.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
//    i.Limits.MaxRequestLineSize = 16384;
//    i.Limits.MaxRequestHeadersTotalSize = 32768;

//});
//------------------- End Instructions----------------------

builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
builder.Services.AddSingleton<DataProtectionPurposeStrings>();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(180);
});
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddOpenIdConnect(options =>
//{
//    options.Authority = "https://localhost:7023/Account/Logout";
//    options.ClientId = "your-client-id";
//    options.ClientSecret = "your-client-secret";
//    options.ResponseType = "code";
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//    options.CallbackPath = "/signin-oidc";

//    // Additional configurations as needed
//});
var app = builder.Build();

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always // Set the Secure flag for all cookies
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.Use(async (ctx, next) =>
//{
//    ctx.Response.Headers.Add("Content-Security-Policy", "default-src *; style-src 'self' ");
//    ctx.Response.Headers.Add("Feature-Policy", "fullscreen 'none'");
//    ctx.Response.Headers.Add("Referrer-Policy", "same-origin");
//    ctx.Response.Headers.Add("X-Frame-Options", "DENY");
//    ctx.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
//    ctx.Response.Headers.Add("X-Content-Type-Options", "nosniff");
//    ctx.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
//    ctx.Response.Headers.Remove("X-Powered-By");
//    ctx.Response.Headers.Remove("x-aspnet-version");
//    //// Some headers won't remove
//    //ctx.Response.Headers.Remove("Server");
//    ctx.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
//    ctx.Response.Headers.Add("X-Content-Type-Options", "nosniff");
//    ctx.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
//    ctx.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

//    await next();

//});
//var options = new RewriteOptions()
//           // Redirect from non-www to www
//           .AddRedirectToWww()
//           // Redirect to HTTPS
//           .AddRedirectToHttps()
//           // Example of custom rewrite rule
//           .AddRewrite("^articles/(.*)", "blog/article?id=$1", skipRemainingRules: true);

//app.UseRewriter(options);

// When the code is published on IAM, these two lines are commented out.
//app.UseForwardedHeaders();
//app.UseHttpsRedirection();
//------------------- End Instructions----------------------

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = false,
});

app.UseRequestLocalization();
app.UseResponseCompression();
app.UseRouting();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//app.UseMyMiddleware();
//app.UseMiddleware<BackRestrictionMiddleware>();
//app.UseSessionMiddleware();
//app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=IMLogin}/{id?}");

app.Run();
