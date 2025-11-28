using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Validation;
using Web.WebHelpers;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling user configuration and mapping actions. and view for user configuration.
    /// </summary>
    public class ConfigUserController : Controller
    {

        //constructor to initialize dependencies and configuration settings and user manager.
        public ConfigUserController()
        {
        }

        /// <summary>
        /// Action method to retrieve the token session details along with the user's IP address.
        /// This method fetches the session data stored for the current user and adds the IP address to the session object before returning it as a JSON response.
        /// </summary>
        /// <param name="Id">The ID used to identify the request (not used in this implementation but can be extended).</param>
        /// <returns>A JSON response containing the session data along with the user's IP address, or 0 in case of an error.</returns>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public IActionResult GetTokenArmyNo()
        {
            // Create a new session DTO object to store the session data
            DtoSession? dtoSession = new DtoSession();
            DTOGetTokenArmyNoResponse dTOGetToken = new DTOGetTokenArmyNoResponse();
            try
            {
                // Retrieve the session object "Token" from the session storage
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                if (dtoSession != null)
                {
                    // Add the user's IP address to the session object
                    dTOGetToken.IpAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "0.0.0.0";
                    dTOGetToken.RankName= dtoSession.RankName;
                    dTOGetToken.ICNO = dtoSession.ICNO;
                    dTOGetToken.Name = dtoSession.Name;
                    dTOGetToken.UnitId = dtoSession.UnitId;
                }
                // Return the updated session data as a JSON response
                return Json(dTOGetToken);
            }
            catch (Exception ex)
            {
                // In case of an exception, return 0 to indicate failure
                return Json(0);
            }
        }

    }
}
