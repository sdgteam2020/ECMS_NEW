using DataTransferObject.Requests;
using Web.WebHelpers;

namespace Web.Healpers
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if session exists and has a value
            if (SessionHeplers.GetObject<DtoSession>(context.Session, "Token") != null)
            {
                // Session exists, you can perform further actions
               
                // Do something with userName
            }
            else
            {
                context.Response.Redirect("/Account/Logout");
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionMiddleware>();
        }
    }
}