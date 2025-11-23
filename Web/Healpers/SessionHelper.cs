using DataTransferObject.Requests;
using Web.WebHelpers;

namespace Web.Healpers
{
    public static class SessionHelper
    {
        private const string TokenKey = "Token";

        public static string GetRoleFromSession(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            // Get the raw session string (if you still need it somewhere)
            string? sessionToken = httpContext.Session.GetString(TokenKey);

            DtoSession? dtoSession = null;

            if (!string.IsNullOrEmpty(sessionToken))
            {
                // Your existing generic method to deserialize session object
                dtoSession = SessionHeplers.GetObject<DtoSession>(httpContext.Session, TokenKey);
            }

            // Fallback to empty string if RoleName is null
            return dtoSession?.RoleName ?? string.Empty;
        }
    }
}
