using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.TrnMappingUnMappingLog;
using DataTransferObject.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.WebHelpers;

namespace Web.Controllers
{
    /// <summary>
    /// This controller manages user profile operations such as viewing and updating profiles, mapping units, and deregistering user IDs.
    /// And it interacts with business logic layers to perform these operations.
    /// And it ensures that only authorized users can access its actions.
    /// </summary>
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileBL _userProfileBL;// Business logic layer for user profile operations
        private readonly IUserProfileMappingBL _userProfileMappingBL;// Business logic layer for user profile mapping operations
        private readonly IDomainMapBL _iDomainMapBL;// Business logic layer for domain mapping operations
        private readonly ITrnMappingUnMappingLogBL _iTrnMappingUnMappingLogBL;// Business logic layer for transaction mapping and unmapping log operations
        private readonly ILogger<UserProfileController> _logger;// Logger for logging information and errors
        private readonly IConfiguration _configuration;// Configuration settings

        /// <summary>
        /// This constructor initializes the UserProfileController with necessary dependencies
        /// </summary>
        /// <param name="userProfileBL"></param>
        /// <param name="logger"></param>
        /// <param name="userProfileMappingBL"></param>
        /// <param name="domainMapBL"></param>
        /// <param name="trnMappingUnMappingLogBL"></param>
        /// <param name="configuration"></param>
        public UserProfileController(IUserProfileBL userProfileBL, ILogger<UserProfileController> logger, IUserProfileMappingBL userProfileMappingBL, IDomainMapBL domainMapBL,ITrnMappingUnMappingLogBL trnMappingUnMappingLogBL, IConfiguration configuration)
        {
            _userProfileBL=userProfileBL;
            _userProfileMappingBL = userProfileMappingBL;
            _iDomainMapBL = domainMapBL;
            _iTrnMappingUnMappingLogBL = trnMappingUnMappingLogBL;
            _logger = logger;
            _configuration = configuration;
        }
        /// <summary>
        /// This method retrieves the role of the currently logged-in user from the session.
        /// It checks if the session contains a valid "Token" and retrieves the user's role from it.
        /// If the token is not found, it returns an empty string.
        /// </summary>
        /// <returns>
        /// A string representing the role of the user, or an empty string if no session is found.
        /// </returns>
        private string GetSessionValue()
        {
            DtoSession? dtoSession = new DtoSession();

            // Check if session contains a valid token
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                // Retrieve the session object
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Return the role of the user from the session, or an empty string if session is not found
            string role = dtoSession != null ? dtoSession.RoleName : "";
            return role;
        }

        /// <summary>
        /// This action method is responsible for deregistering a user by removing their mapping from the domain.
        /// It updates the "TrnDomainMapping" table by setting the "UserId" field to null and logs the action in the "TrnMappingUnMapping_Log" table.
        /// </summary>
        /// <returns>
        /// Returns a Json response indicating whether the deregistration was successful or not.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DeRegisterUserId()
        {
            // Get the domain ID of the currently logged-in user
            int DomainId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get the current time in IST to update the last modified time
            DateTime UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            DtoSession? dtoSession = new DtoSession();

            // Check if the session contains a valid token
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                // Retrieve the session object
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                if (dtoSession != null)
                {
                    // Get the domain mapping based on the session's domain mapping ID
                    TrnDomainMapping trnDomainMapping = await _iDomainMapBL.Get(dtoSession.TrnDomainMappingId);

                    // Deregister the user by setting the UserId to null
                    trnDomainMapping.UserId = null;
                    trnDomainMapping.UpdatedOn = UpdatedOn;

                    // Update the domain mapping in the database
                    await _iDomainMapBL.Update(trnDomainMapping);

                    // Log the deregistration action
                    var mapping_Log = new TrnMappingUnMapping_Log()
                    {
                        TrnMappUnMapLogId = 0,
                        TDMId = dtoSession.TrnDomainMappingId,
                        UserId = dtoSession.UserId,
                        DeregisterUserId = dtoSession.UserId,
                        IsActive = true,
                        Updatedby = DomainId,
                        UpdatedOn = UpdatedOn,
                    };

                    // Add the log entry to the database
                    await _iTrnMappingUnMappingLogBL.Add(mapping_Log);

                    return Json(true);
                }
                else
                {
                    // If no session data is found, return failure
                    return Json(false);
                }
            }
            else
            {
                // If session is not valid, return failure
                return Json(false);
            }
        }

        /// <summary>
        /// This action method is used to display the user's profile information.
        /// It retrieves the user's role from the session and sets it in the ViewBag for display in the view.
        /// </summary>
        /// <returns>
        /// Returns the Profile view with the user's role set in the ViewBag.
        /// </returns>
        public IActionResult Profile()
        {
            // Get the role of the user from the session
            string role = GetSessionValue();

            // Set the role in the ViewBag to make it accessible in the view
            ViewBag.Role = role;

            // Return the Profile view
            return View();
        }

        public async Task<IActionResult> SaveUserProfile(MUserProfile dTO)
        {
            try
            {
                if (dTO.RankId != 0)
                {
                    dTO.IsActive = true;
                    dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    dTO.UpdatedOn = DateTime.Now;
                    int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (ModelState.IsValid)
                    {
                        if (dTO.UserId > 0)
                        {
                            bool? result = await _userProfileBL.FindByArmyNoWithUserId(dTO.ArmyNo, dTO.UserId);
                            if (result != null)
                            {
                                if (result == true)
                                {
                                    return Json(KeyConstants.Exists);
                                }
                                else
                                {
                                    await _userProfileBL.Update(dTO);
                                    return Json(KeyConstants.Update);
                                }
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else
                        {
                            bool? result = await _userProfileBL.FindByArmyNo(dTO.ArmyNo);
                            if (result != null)
                            {
                                if (result == true)
                                {
                                    return Json(KeyConstants.Exists);
                                }
                                else
                                {
                                    dTO = await _userProfileBL.AddWithReturn(dTO);
                                    TrnDomainMapping? trnDomainMapping = new TrnDomainMapping();
                                    trnDomainMapping.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                    trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping.AspNetUsersId);
                                    if (trnDomainMapping != null && dTO.UserId != 0)
                                    {
                                        trnDomainMapping.UserId = dTO.UserId;
                                        await _iDomainMapBL.Update(trnDomainMapping);
                                    }
                                    return Json(KeyConstants.Save);
                                }
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                    }
                    else
                    {

                        return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                    }
                }
                else
                {
                    return Json(KeyConstants.IncorrectData);
                }
            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }
        public async Task<IActionResult> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                TrnDomainMapping? trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(dTO.Updatedby);
                if (trnDomainMapping != null)
                {
                    dTO.TDMId = trnDomainMapping.Id;
                    dTO.UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0);
                }
                else
                {
                    dTO.TDMId = 0;
                    dTO.UserId = 0;
                }

                if (dTO.UserId > 0 && dTO.TDMId>0)
                {

                    dTO.UpdatedOn = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        bool? result = await _userProfileBL.UpdateProfileWithMapping(dTO);
                        if (result != null)
                        {
                            if (result == true)
                            {
                                return Json(KeyConstants.Update);
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else
                        {
                            return Json(KeyConstants.InternalServerError);
                        }
                    }
                    else
                    {

                        return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                    }
                }
                else
                {
                    return Json(KeyConstants.IncorrectData);
                }
            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }
        public async Task<IActionResult> MappingIOGSOUNIT(MMappingProfile dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                
                if (ModelState.IsValid)
                {
                    //if (!await _userProfileBL.GetByArmyNo(dTO))
                    //{
                        if (dTO.Id > 0)
                        {
                        await _userProfileMappingBL.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await _userProfileMappingBL.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    //}
                    //else
                    //{
                    //    return Json(KeyConstants.Exists);
                    //}

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAll(string Id)
        {
            try
            {
                int DomainId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetAll(DomainId, 0));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByArmyNoOrAspnetuserId(string ArmyNo,int userid)
        {
            try
            {
                if(userid==0)
                 userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                DTOUserProfileResponse dTOUserProfileResponse = await _userProfileBL.GetByArmyNo(ArmyNo, userid);
                dTOUserProfileResponse.RoleName = GetSessionValue();
                return Json(dTOUserProfileResponse);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByArmyNoIsWithoutTokenApply(string ArmyNo)
        {
            try
            {
                
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                MUserProfile dTOUserProfileResponse = await _userProfileBL.GetByIsWithoutTokenApply(userid);
                
                return Json(dTOUserProfileResponse);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize]
        public async Task<IActionResult> GetProfileByUserId(int UserId)
        {
            try
            {
                return Json(await _userProfileBL.GetProfileByUserId(UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfile->GetProfileByUserId");
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetDataForFwd(string Name,int TypeId, int StepId,int UnitId, int ISRO,int IsORO)
        {
            try
            {
                int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                //if(TypeId == 0 )
                //UnitId=SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UnitId;

                return Json(await _userProfileBL.GetDataForFwd(StepId, UnitId, Name, TypeId, ISRO, IsORO, DomainMapId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetOffrsByUnitMapId(int id,int UnitId, int IsRO,int IsORO,int IsAfsacCell, int BasicDetailsId)
        {
            try
            {
                int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if(UnitId==0)
                {
                    UnitId = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UnitId;
                    return Json(await _userProfileBL.GetOffrsByUnitMapId(UnitId, IsRO, IsORO, IsAfsacCell, BasicDetailsId, DomainMapId));
                }
                else
                {
                    return Json(await _userProfileBL.GetOffrsByUnitMapId(UnitId, IsRO, IsORO, IsAfsacCell, BasicDetailsId, DomainMapId));
                }
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByMasterArmyNo(string ArmyNo)
        {
            try
            {
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetByMArmyNo(ArmyNo, userid));

            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByAspnetUserIdBy(TrnDomainMapping Data)
        {
            try
            {
               
                return Json(await _iDomainMapBL.GetByAspnetUserIdBy(Data.AspNetUsersId));

            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByRequestId(int RequestId)
        {
            try
            {
               
                return Json(await _userProfileBL.GetByRequestId(RequestId));

            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> CheckArmyNoInUserProfile(string ArmyNo)
        {
            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DTOProfileResponse dTOProfileResponse = await _userProfileBL.CheckArmyNoInUserProfile(ArmyNo, userid);
            return Json(dTOProfileResponse);
        }
        public async Task<IActionResult> GetTopByArmyNo(string ArmyNo)
        {
            try
            {
                return Json(await _userProfileBL.GetTopByArmyNo(ArmyNo));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }


    }
}
