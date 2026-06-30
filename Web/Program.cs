using BusinessLogicsLayer;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.Service;
using DataAccessLayer;
using DataAccessLayer.Logger;
using DataAccessLayer.Security;
using DataTransferObject.Domain.Identitytable;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Web.Healpers;
using Web.Healpers.BaseInterfaces;
using Web.Services;
using ApplicationUser = DataTransferObject.Domain.Identitytable.ApplicationUser;

var builder = WebApplication.CreateBuilder(args);

var configration = builder.Configuration;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AFSACDBConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Connection string not found in environment variables.");
}

builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(connectionString).UseExceptionProcessor());

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
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromMinutes(5);
});

builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
    opt.ValidationInterval = TimeSpan.FromSeconds(0)
);

builder.Services.AddHttpClient("CdnHealthClient", client =>
{
    client.Timeout = TimeSpan.FromMilliseconds(500);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
}); ;

builder.Services.AddSingleton<ICdnHealthService, CdnHealthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            // Get allowed origins from configuration
            var allowedOrigins = Environment.GetEnvironmentVariable("CorsSettings__AllowedOrigins")?.Split(',')
                ?? Array.Empty<string>();

            // Always allow localhost in development
            if (builder.Environment.IsDevelopment() &&
                origin.StartsWith("localhost"))
            {
                return true;
            }

            // Check against configured list
            return allowedOrigins.Contains(origin) ||
                   allowedOrigins.Contains("*");
        });

        // Only allow specific methods
        policy.WithMethods("GET", "POST", "HEAD");

        // Only allow specific headers
        policy.WithHeaders("Authorization", "Content-Type", "X-Requested-With");

        // Allow credentials (cookies, auth headers)
        policy.AllowCredentials();

        // Cache preflight for 5 minutes
        policy.SetPreflightMaxAge(TimeSpan.FromMinutes(20));
    });
});




builder.Services.AddScoped<IService, ServiceRepository>();
builder.Services.AddScoped<IImageEncryptAndDecrypt, ImageEncryptAndDecrypt>();


builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<DapperContextDb2>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

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
    opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.MaxFailedAccessAttempts = 3;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // When the code is published on IAM, these two lines are commented out.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    //------------------- End Instructions----------------------
});
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = false;

    options.LoginPath = "/Account/IMLoginSelf";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

});

// TempData cookie - only needed if you use TempData
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true;
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

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100*1024*1024; // 100MB
});

var app = builder.Build();

app.UseResponseCompression();

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always // Set the Secure flag for all cookies
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();    
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.Use(async (ctx, next) =>
{
    // ========== 0) BLOCK DANGEROUS HTTP METHODS FIRST ==========
    var blockedMethods = new[] { "OPTIONS", "TRACE", "TRACK", "CONNECT" };

    if (blockedMethods.Contains(ctx.Request.Method, StringComparer.OrdinalIgnoreCase))
    {
        // Log for monitoring (optional)
        app.Logger.LogWarning($"Security: Blocked {ctx.Request.Method} request to {ctx.Request.Path}");

        ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        ctx.Response.Headers["Allow"] = "GET, HEAD, POST"; // Only allowed methods
        await ctx.Response.WriteAsync("Method Not Allowed");
        return; // Stop further processing
    }
    ctx.Response.OnStarting(() =>
    {
        // Remove Server header
        ctx.Response.Headers.Remove("Server");
        ctx.Response.Headers.Remove("Expires");
        ctx.Response.Headers.Remove("X-Powered-By");
        ctx.Response.Headers.Remove("x-aspnet-version");
        return Task.CompletedTask;
    });


    // 1) Content Security Policy
    ctx.Response.Headers["Content-Security-Policy"] =
       "default-src 'self' " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "script-src 'self' " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "style-src 'self' " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " + // allow Bootstrap inline styles
       "img-src 'self' data: " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "font-src 'self' data: " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "connect-src 'self' https://dgisapp.army.mil:55102 https://iam2.army.mil " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "frame-ancestors 'self' " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "base-uri 'self' " + Environment.GetEnvironmentVariable("AFSAC__CDN") + "; " +
       "form-action 'self' " + Environment.GetEnvironmentVariable("AFSAC__CDN") + ";";

    // 2) X-Frame-Options (align with frame-ancestors)
    ctx.Response.Headers["X-Frame-Options"] = "DENY";

    // 3) Referrer-Policy
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    // Extra good headers
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";

    // Use HSTS only on HTTPS + production
    ctx.Response.Headers["Strict-Transport-Security"] =
        "max-age=31536000; includeSubDomains; preload";

    // Hide tech details where possible
    ctx.Response.Headers.Remove("Server");
    ctx.Response.Headers.Remove("X-Powered-By");
    ctx.Response.Headers.Remove("x-aspnet-version");

    await next();
});

// When the code is published on IAM, these two lines are commented out.
app.UseForwardedHeaders();
app.UseHttpsRedirection();
//------------------- End Instructions----------------------

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.AppendCommaSeparatedValues(
            "Access-Control-Allow-Origin",
            "https://localhost:7023", "" + Environment.GetEnvironmentVariable("AFSAC__CDN") + "");
    },
    ServeUnknownFileTypes = false,
});

app.UseRequestLocalization();
app.UseResponseCompression();
app.UseRouting();
app.UseSession(); // MUST be before Authentication & Authorization

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<XssProtectionMiddleware>();


// When the code is published on IAM, these MyMiddleware code are uncommented.
//app.UseMyMiddleware();
//app.UseMiddleware<BackRestrictionMiddleware>();
//app.UseSessionMiddleware();
//app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=IMLoginSelf}/{id?}");

app.Run();
