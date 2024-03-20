using System.Security.Claims;
using DataTransferObject.Domain.Identitytable;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Web.Healpers
{
    public class RoleChange
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> userManager;
        public RoleChange(
            //IHttpContextAccessor httpContextAccessor, 
            UserManager<ApplicationUser> userManager)
        {
            //_httpContextAccessor = httpContextAccessor;
            this.userManager = userManager; 
        }

        //public async Task ChangeUserRoleAsync(string newRole)
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //   // var authenticationManager = httpContext.Authenticate;

        //    var currentIdentity = (ClaimsIdentity)httpContext.User.Identity;

        //    // Remove the current role claim
        //    var existingRoleClaim = currentIdentity.FindFirst(ClaimTypes.Role);
        //    if (existingRoleClaim != null)
        //    {
        //        currentIdentity.RemoveClaim(existingRoleClaim);
        //    }

        //    // Add the new role claim
        //    var newRoleClaim = new Claim(ClaimTypes.Role, newRole);
        //    currentIdentity.AddClaim(newRoleClaim);

        //    // Update the authentication ticket with the modified identity
        //    var newIdentity = new ClaimsIdentity(currentIdentity.Claims, currentIdentity.AuthenticationType);
        //    var newPrincipal = new ClaimsPrincipal(newIdentity);
        //    await httpContext.SignInAsync(httpContext.User.Identity.AuthenticationType, newPrincipal);
            
        //}
        public async Task ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Handle user not found
                return;
            }

            var roles = await userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault(); // Assuming the user has only one role

            if (currentRole != null)
            {
                // Remove the current role
                await userManager.RemoveFromRoleAsync(user, currentRole);
            }

            // Add the new role
            await userManager.AddToRoleAsync(user, newRole);
            
        }
    }
}