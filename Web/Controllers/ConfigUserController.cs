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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult GetTokenDetails(DTOTokenResponse Data)
        {

            var data = _iGetTokenBL.GetTokenDetails(Data);
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveMapping(TrnDomainMapping dTO,string ICNO)
        {

            dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                // dTO.IsActive = true;
                // dTO.Updatedby = 1;
                //dTO.UpdatedOn = DateTime.Now;
              
                DtoSession dtoSession = new DtoSession();
               
                dtoSession.ICNO = ICNO;
                dtoSession.UnitId = dTO.UnitId;
                
                if (ModelState.IsValid)
                {
                    if (!await _iDomainMapBL.GetByDomainId(dTO))
                    {
                        if (dTO.Id > 0)
                        {
                            _iDomainMapBL.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await _iDomainMapBL.Add(dTO);
                            TrnDomainMapping? trnDomainMapping1 = new TrnDomainMapping();
                            trnDomainMapping1.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                            trnDomainMapping1 = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping1.AspNetUsersId);
                            if (trnDomainMapping1 != null)
                                dtoSession.TrnDomainMappingId = trnDomainMapping1.Id;

                            SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);


                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                        trnDomainMapping =await _iDomainMapBL.GetByDomainIdbyUnit(dTO);
                        trnDomainMapping.UnitId = dTO.UnitId;
                        trnDomainMapping.UserId = dTO.UserId!=null? dTO.UserId : null;
                        await _iDomainMapBL.Update(trnDomainMapping);


                        TrnDomainMapping? trnDomainMapping1 = new TrnDomainMapping();
                        trnDomainMapping1.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        trnDomainMapping1 = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping1.AspNetUsersId);
                        if (trnDomainMapping1 != null)
                            dtoSession.TrnDomainMappingId = trnDomainMapping1.Id;

                        SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);
                        return Json(KeyConstants.Update);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

           // return Json(1);
        }
        [HttpPost]
        public async Task<IActionResult> GotoDashboard(string ICNO)
        {

            SessionHeplers.SetObject(HttpContext.Session, "ArmyNo", ICNO);
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
