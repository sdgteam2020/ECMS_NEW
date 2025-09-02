using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Token;
using BusinessLogicsLayer.Unit;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.WebHelpers;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling user configuration and mapping actions. and view for user configuration.
    /// </summary>
    public class ConfigUserController : Controller
    {
        public readonly iGetTokenBL _iGetTokenBL; //Interface for token business logic layer
        private readonly IUserProfileBL _userProfileBL;//Interface for user profile business logic layer
        private readonly UserManager<ApplicationUser> userManager;//User manager for handling user-related operations
        public readonly IDomainMapBL _iDomainMapBL;//Interface for domain mapping business logic layer
        public readonly IMapUnitBL _IMapUnitBL;//Interface for map unit business logic layer

        //constructor to initialize dependencies and configuration settings and user manager.
        public ConfigUserController(iGetTokenBL iGetTokenBL, IUserProfileBL userProfileBL, UserManager<ApplicationUser> userManager, IDomainMapBL domainMapBL, IMapUnitBL mapUnitBL)
        {
            _iGetTokenBL=iGetTokenBL;
            _userProfileBL=userProfileBL;
            this.userManager=userManager;
            _iDomainMapBL=domainMapBL;
            _IMapUnitBL=mapUnitBL;
        }
        /// <summary>
        /// Action method for the index page. This method retrieves user-specific domain mapping information,
        /// updates session details, and redirects to the home page based on user data.
        /// </summary>
        /// <returns>A redirect to the Home page or the current view based on the domain mapping data.</returns>
        public async Task<IActionResult> IndexAsync()
        {
            // Retrieve the domain name and role of the logged-in user from the claims
            ViewBag.DomainId = this.User.FindFirstValue(ClaimTypes.Name);
            ViewBag.Role = this.User.FindFirstValue(ClaimTypes.Role);

            // Create a DTO object to store the domain mapping data
            TrnDomainMapping? dTO = new TrnDomainMapping();
            dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Fetch the domain mapping by the logged-in user's ID
            dTO = await _iDomainMapBL.GetByAspnetUserIdBy(dTO.AspNetUsersId);

            // If no domain mapping is found, return the view without further processing
            if (dTO == null)
            {
                return View();
            }
            // If domain mapping exists but has no associated user, fetch the unit data
            else if (dTO != null && dTO.UserId == null)
            {
                // Retrieve map unit details based on the unit ID from domain mapping
                DTOMapUnitResponse dTOMapUnitResponse = await _IMapUnitBL.GetALLByUnitById(dTO.UnitId);
                // Pass the map unit response to the view
                ViewBag.TrnDomain = dTOMapUnitResponse;
                return View();
            }
            else
            {
                // Fetch user profile data based on the user ID from domain mapping
                var army = await _userProfileBL.Get(Convert.ToInt32(dTO.UserId));
                DtoSession dtoSession = new DtoSession();

                // If the user profile is found, set up session data for the user
                if (army != null)
                {
                    dtoSession.ICNO = army.ArmyNo;
                    dtoSession.Name = army.Name;
                    dtoSession.UserId = army.UserId;
                    dtoSession.UnitId = dTO.UnitId;

                    // Retrieve the domain mapping for the user and set the session ID
                    TrnDomainMapping? trnDomainMapping = new TrnDomainMapping();
                    trnDomainMapping.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping.AspNetUsersId);
                    dtoSession.TrnDomainMappingId = trnDomainMapping.Id;
                }

                // Set the session data for token and user details
                SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);

                // Redirect to the Home page after setting session data
                return RedirectToActionPermanent("Index", "Home");
            }
        }

        /// <summary>
        /// Action method to check if a profile exists for a user based on their role. 
        /// Admin or Super Admin users will receive a default response, while other users will get their domain data based on their session.
        /// </summary>
        /// <param name="Id">The ID of the user whose profile existence is being checked.</param>
        /// <returns>A JSON response indicating profile existence or data based on user role.</returns>
        [HttpPost]
        public async Task<IActionResult> CheckProfileExist(int Id)
        {
            try
            {
                // Check if the logged-in user has Admin or Super Admin role
                if (this.User.FindFirstValue(ClaimTypes.Role) == "Admin" || this.User.FindFirstValue(ClaimTypes.Role) == "Super Admin")
                {
                    // For Admin or Super Admin, return a default DTO object
                    TrnDomainMapping dTO = new TrnDomainMapping();
                    dTO.UserId = 1;  // Setting a default UserId
                    return Json(dTO); // Return the default DTO as JSON
                }
                else
                {
                    // For non-admin users, fetch domain mapping by AspNetUsersId from session claims
                    TrnDomainMapping dTO = new TrnDomainMapping();
                    dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    // Fetch the domain mapping data based on the unit ID associated with the user
                    var data = await _iDomainMapBL.GetByDomainIdbyUnit(dTO);

                    // Return the domain mapping data as JSON response
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                // In case of an exception, return null as a JSON response
                return Json(null);
            }
        }

        /// <summary>
        /// Action method to retrieve token details based on the provided token data.
        /// This method processes the incoming data and returns the corresponding token details as a JSON response.
        /// </summary>
        /// <param name="Data">The token data to retrieve details for.</param>
        /// <returns>A JSON response containing the token details retrieved from the backend.</returns>
        [HttpPost]
        //[ValidateAntiForgeryToken] // Uncomment to enable anti-forgery token validation for the request
        public IActionResult GetTokenDetails(DTOTokenResponse Data)
        {
            // Call the service to fetch token details based on the provided token data
            var data = _iGetTokenBL.GetTokenDetails(Data);

            // Return the fetched token details as a JSON response
            return Json(data);
        }

        /// <summary>
        /// Action method to save or update a domain mapping based on the provided data.
        /// This method processes the domain mapping object and handles the logic for adding or updating the mapping.
        /// It also updates session details and returns appropriate responses based on success or failure.
        /// </summary>
        /// <param name="dTO">The domain mapping data object containing the mapping details.</param>
        /// <param name="ICNO">The IC number used to associate the mapping with the user.</param>
        /// <returns>A JSON response indicating whether the mapping was successfully saved or updated.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveMapping(TrnDomainMapping dTO, string ICNO)
        {
            // Set the current user's ID (from the claims) for the domain mapping
            dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                // Initialize session DTO object to store token-related data
                DtoSession dtoSession = new DtoSession();
                dtoSession.ICNO = ICNO;
                dtoSession.UnitId = dTO.UnitId;

                // Validate the model state
                if (ModelState.IsValid)
                {
                    // Check if a domain mapping already exists for the provided data
                    if (!await _iDomainMapBL.GetByDomainId(dTO))
                    {
                        // If the ID is greater than 0, update the existing mapping
                        if (dTO.Id > 0)
                        {
                            _iDomainMapBL.Update(dTO);
                            return Json(KeyConstants.Update); // Return update response
                        }
                        else
                        {
                            // If no mapping exists, add a new mapping
                            await _iDomainMapBL.Add(dTO);

                            // Retrieve the domain mapping for the current user and update session details
                            TrnDomainMapping? trnDomainMapping1 = new TrnDomainMapping();
                            trnDomainMapping1.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                            trnDomainMapping1 = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping1.AspNetUsersId);
                            if (trnDomainMapping1 != null)
                                dtoSession.TrnDomainMappingId = trnDomainMapping1.Id;

                            // Save session data
                            SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);

                            return Json(KeyConstants.Save); // Return save response
                        }
                    }
                    else
                    {
                        // If domain mapping already exists, update the existing mapping with the new data
                        TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                        trnDomainMapping = await _iDomainMapBL.GetByDomainIdbyUnit(dTO);
                        trnDomainMapping.UnitId = dTO.UnitId;
                        trnDomainMapping.UserId = dTO.UserId != null ? dTO.UserId : null;
                        await _iDomainMapBL.Update(trnDomainMapping);

                        // Retrieve the domain mapping for the current user and update session details
                        TrnDomainMapping? trnDomainMapping1 = new TrnDomainMapping();
                        trnDomainMapping1.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        trnDomainMapping1 = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping1.AspNetUsersId);
                        if (trnDomainMapping1 != null)
                            dtoSession.TrnDomainMappingId = trnDomainMapping1.Id;

                        // Save session data
                        SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);

                        return Json(KeyConstants.Update); // Return update response
                    }
                }
                else
                {
                    // If the model state is invalid, return the validation errors as JSON
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                // In case of an exception, return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Action method to set the Army number in the session for the current user.
        /// This method stores the provided IC number in the session and returns a JSON response indicating success.
        /// </summary>
        /// <param name="ICNO">The IC number (ArmyNo) to be stored in the session.</param>
        /// <returns>A JSON response indicating success (1).</returns>
        [HttpPost]
        public async Task<IActionResult> GotoDashboard(string ICNO)
        {
            // Store the provided IC number (ArmyNo) in the session for later use
            SessionHeplers.SetObject(HttpContext.Session, "ArmyNo", ICNO);

            // Return a JSON response indicating success
            return Json(1);
        }

        [HttpPost]
        public async Task<IActionResult> GetTokenArmyNo(string Id)
        {
            try
            {

                DtoSession dtoSession = new DtoSession();
               
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                dtoSession.IpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                return Json(dtoSession);

            }
            catch (Exception ex) {
                return Json(0);
             }
        }
       }
}
