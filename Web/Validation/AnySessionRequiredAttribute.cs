using DataTransferObject.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.WebHelpers;

namespace Web.Validation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AnySessionRequiredAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;

            // Use extension methods
            var tempSession = session.GetObject<DTOTempSession>("IMData");
            var dtoSession = session.GetObject<DtoSession>("Token");

            // If BOTH are null → no valid session
            if (tempSession == null && dtoSession == null)
            {
                // Neither IMData nor Token present → block
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        ["controller"] = "Account",
                        ["action"] = "IMLoginSelf"
                    });
            }
        }
    }
}
