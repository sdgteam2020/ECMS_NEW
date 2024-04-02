using DataTransferObject.Requests;
using Microsoft.AspNetCore.Hosting.Server;
using System.Security.Claims;
using Web.WebHelpers;

namespace Web.Healpers
{ 
    public class MyModule 
    {
        private readonly RequestDelegate _next;
      
        public MyModule(RequestDelegate next)
        {
            _next = next;
           
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {

           
            // Do something with context near the beginning of request processing.
            var myHeader = context.Request.Path.ToString();
            string referer = context.Request.Headers["Referer"].ToString();
            if (referer != "" && myHeader != "/")
            {
                await _next.Invoke(context);
            }
            else if (myHeader == "/" && !myHeader.Contains("DigitallysignaturePdf"))
                await _next.Invoke(context);
            else if (myHeader == "/Account/AccessDenied" || myHeader.Contains("DigitallysignaturePdf"))
                await _next.Invoke(context);
            else
                context.Response.Redirect("/Account/AccessDenied");


            // Cheak Session

            if (myHeader == "/" || myHeader.Contains("IMLogin") || myHeader.Contains("Logout") || myHeader.Contains("TokenValidate"))
            {

            }
            else
            {

                    if (SessionHeplers.GetObject<DtoSession>(context.Session, "Token") != null)
                    {
                        // Session exists, you can perform further actions
                        // await _next.Invoke(context);
                        // Do something with userName
                    }
                    else
                    {
                        context.Response.Redirect("/Account/Logout");
                    }
                }
            }
            catch (Exception ex) { }

        }
    }

    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyModule>();
        }
    }
}
