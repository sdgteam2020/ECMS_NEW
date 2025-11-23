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
using Web.Healpers;
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
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Set the role in the ViewBag to make it accessible in the view
            ViewBag.Role = role;

            // Return the Profile view
            return View();
        }

        /// <summary>
        /// This method is responsible for saving or updating a user's profile.
        /// It checks if the user profile exists based on the ArmyNo and performs the save or update accordingly.
        /// If the user profile already exists, it returns an "Exists" message, otherwise, it saves or updates the profile.
        /// </summary>
        /// <param name="dTO">The user profile data to be saved or updated.</param>
        /// <returns>Returns a JSON response indicating the result of the operation.</returns>
        public async Task<IActionResult> SaveUserProfile(MUserProfile dTO)
        {
            try
            {
                // Check if the RankId is valid (non-zero)
                if (dTO.RankId != 0)
                {
                    dTO.IsActive = true; // Set the profile to active
                    dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); // Set the user who updated the profile
                    dTO.UpdatedOn = DateTime.Now; // Set the updated time

                    int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    if (ModelState.IsValid)
                    {
                        // If UserId is greater than 0, check if the ArmyNo is already used by the user
                        if (dTO.UserId > 0)
                        {
                            bool? result = await _userProfileBL.FindByArmyNoWithUserId(dTO.ArmyNo, dTO.UserId);
                            if (result != null)
                            {
                                if (result == true) // If the ArmyNo is already registered with the user
                                {
                                    return Json(KeyConstants.Exists); // Return message "Exists"
                                }
                                else
                                {
                                    await _userProfileBL.Update(dTO); // Update the user profile if the ArmyNo is not taken
                                    return Json(KeyConstants.Update); // Return success message "Update"
                                }
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError); // Return error message
                            }
                        }
                        else
                        {
                            // Check if the ArmyNo is already used by any user
                            bool? result = await _userProfileBL.FindByArmyNo(dTO.ArmyNo);
                            if (result != null)
                            {
                                if (result == true)
                                {
                                    return Json(KeyConstants.Exists); // Return message "Exists"
                                }
                                else
                                {
                                    dTO = await _userProfileBL.AddWithReturn(dTO); // Add the new user profile
                                                                                   // Add the domain mapping after saving the profile
                                    TrnDomainMapping? trnDomainMapping = new TrnDomainMapping();
                                    trnDomainMapping.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                    trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping.AspNetUsersId);
                                    if (trnDomainMapping != null && dTO.UserId != 0)
                                    {
                                        trnDomainMapping.UserId = dTO.UserId;
                                        await _iDomainMapBL.Update(trnDomainMapping);
                                    }
                                    return Json(KeyConstants.Save); // Return success message "Save"
                                }
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError); // Return error message
                            }
                        }
                    }
                    else
                    {
                        return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                    }
                }
                else
                {
                    return Json(KeyConstants.IncorrectData); // Return error message for invalid RankId
                }
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError); // Return error message if an exception occurs
            }
        }

        /// <summary>
        /// This method updates the user profile with additional domain mapping information.
        /// It retrieves the domain mapping based on the user and updates the profile accordingly.
        /// </summary>
        /// <param name="dTO">The update profile data with mapping to be saved.</param>
        /// <returns>Returns a JSON response indicating the result of the operation.</returns>
        public async Task<IActionResult> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO)
        {
            try
            {
                // Get the userId from the current logged-in user's claim
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Retrieve the domain mapping for the current user
                TrnDomainMapping? trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(dTO.Updatedby);
                if (trnDomainMapping != null)
                {
                    // Set the mapping details in the DTO
                    dTO.TDMId = trnDomainMapping.Id;
                    dTO.UserId = (int)(trnDomainMapping.UserId != null ? trnDomainMapping.UserId : 0);
                }
                else
                {
                    // If no domain mapping is found, set the default values
                    dTO.TDMId = 0;
                    dTO.UserId = 0;
                }

                // Ensure valid UserId and TDMId before proceeding
                if (dTO.UserId > 0 && dTO.TDMId > 0)
                {
                    dTO.UpdatedOn = DateTime.Now; // Set the updated time

                    if (ModelState.IsValid)
                    {
                        // Attempt to update the user profile with mapping
                        bool? result = await _userProfileBL.UpdateProfileWithMapping(dTO);
                        if (result != null)
                        {
                            if (result == true)
                            {
                                return Json(KeyConstants.Update); // Return success message "Update"
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError); // Return error message
                            }
                        }
                        else
                        {
                            return Json(KeyConstants.InternalServerError); // Return error message if result is null
                        }
                    }
                    else
                    {
                        return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                    }
                }
                else
                {
                    return Json(KeyConstants.IncorrectData); // Return error message if UserId or TDMId is invalid
                }
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError); // Return error message if an exception occurs
            }
        }

        /// <summary>
        /// This method maps a profile to the specified unit and updates or saves the mapping.
        /// It handles both new mappings and updates for existing profiles.
        /// </summary>
        /// <param name="dTO">The profile data to be mapped to the unit.</param>
        /// <returns>Returns a JSON response indicating the result of the operation.</returns>
        public async Task<IActionResult> MappingIOGSOUNIT(MMappingProfile dTO)
        {
            try
            {
                dTO.IsActive = true; // Set the mapping as active
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); // Set the user who is updating the mapping
                dTO.UpdatedOn = DateTime.Now; // Set the updated time

                if (ModelState.IsValid)
                {
                    if (dTO.Id > 0) // If the mapping already exists, update it
                    {
                        await _userProfileMappingBL.Update(dTO);
                        return Json(KeyConstants.Update); // Return success message "Update"
                    }
                    else
                    {
                        // If the mapping does not exist, save a new one
                        await _userProfileMappingBL.Add(dTO);
                        return Json(KeyConstants.Save); // Return success message "Save"
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                }
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError); // Return error message if an exception occurs
            }
        }

        /// <summary>
        /// This method retrieves all user profiles associated with the current domain.
        /// It fetches the profiles based on the DomainId and returns the result as JSON.
        /// </summary>
        /// <param name="Id">The identifier for the domain (unused in the method body but expected as part of the signature).</param>
        /// <returns>Returns a JSON response containing a list of user profiles.</returns>
        public async Task<IActionResult> GetAll(string Id)
        {
            try
            {
                // Retrieve the current user's DomainId from the claims
                int DomainId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch the user profiles based on DomainId
                return Json(await _userProfileBL.GetAll(DomainId, 0));
            }
            catch (Exception ex)
            {
                // In case of an exception, return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves a user profile by ArmyNo or UserId.
        /// If the UserId is not provided, it defaults to the current logged-in user's Id.
        /// </summary>
        /// <param name="ArmyNo">The army number to look for.</param>
        /// <param name="userid">The optional UserId to look for. If it's zero, the current user's Id is used.</param>
        /// <returns>Returns a JSON response containing the user profile or an error message.</returns>
        public async Task<IActionResult> GetByArmyNoOrAspnetuserId(string ArmyNo, int userid)
        {
            try
            {
                // If no UserId is passed, use the current user's Id
                if (userid == 0)
                    userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch the user profile by ArmyNo and UserId
                DTOUserProfileResponse dTOUserProfileResponse = await _userProfileBL.GetByArmyNo(ArmyNo, userid);

                // Add the role name to the response using the session value
                dTOUserProfileResponse.RoleName = SessionHelper.GetRoleFromSession(HttpContext);

                // Return the user profile as JSON
                return Json(dTOUserProfileResponse);
            }
            catch (Exception ex)
            {
                // In case of an exception, return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves a user profile for users who do not have a token applied.
        /// It fetches the profile based on the user's ID and returns the result.
        /// </summary>
        /// <param name="ArmyNo">The army number of the user (not used in this implementation).</param>
        /// <returns>Returns a JSON response containing the user profile.</returns>
        public async Task<IActionResult> GetByArmyNoIsWithoutTokenApply(string ArmyNo)
        {
            try
            {
                // Retrieve the current user's ID from the claims
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch the user profile for users who have not applied for a token
                MUserProfile dTOUserProfileResponse = await _userProfileBL.GetByIsWithoutTokenApply(userid);

                // Return the profile as JSON
                return Json(dTOUserProfileResponse);
            }
            catch (Exception ex)
            {
                // In case of an exception, return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves the profile of a user based on their UserId.
        /// It requires the user to be authorized to access this endpoint.
        /// </summary>
        /// <param name="UserId">The UserId of the profile to retrieve.</param>
        /// <returns>Returns a JSON response containing the user profile or an error message.</returns>
        [Authorize]
        public async Task<IActionResult> GetProfileByUserId(int UserId)
        {
            try
            {
                // Fetch the user profile by UserId
                return Json(await _userProfileBL.GetProfileByUserId(UserId));
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur and return an internal server error response
                _logger.LogError(1001, ex, "UserProfile->GetProfileByUserId");
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves data for forwarding based on provided parameters.
        /// It fetches data based on StepId, UnitId, Name, TypeId, ISRO, and IsORO, 
        /// along with the user's DomainMapId.
        /// </summary>
        /// <param name="Name">The name associated with the data retrieval.</param>
        /// <param name="TypeId">The type ID for filtering data.</param>
        /// <param name="StepId">The step ID to filter data.</param>
        /// <param name="UnitId">The unit ID for filtering data.</param>
        /// <param name="ISRO">Filter flag for ISRO.</param>
        /// <param name="IsORO">Filter flag for ORO.</param>
        /// <returns>Returns a JSON response with the retrieved data or an error message.</returns>
        public async Task<IActionResult> GetDataForFwd(string Name, int TypeId, int StepId, int UnitId, int ISRO, int IsORO)
        {
            try
            {
                // Retrieve the user's DomainMapId from claims
                int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Call the method to get data for forwarding based on the provided parameters
                return Json(await _userProfileBL.GetDataForFwd(StepId, UnitId, Name, TypeId, ISRO, IsORO, DomainMapId));
            }
            catch (Exception ex)
            {
                // Log the error and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves officers by UnitMapId based on various filters.
        /// The filters include UnitId, IsRO, IsORO, IsAfsacCell, and BasicDetailsId.
        /// </summary>
        /// <param name="id">The ID of the officer (not used in this method).</param>
        /// <param name="UnitId">The UnitId for filtering officers.</param>
        /// <param name="IsRO">Filter flag for RO.</param>
        /// <param name="IsORO">Filter flag for ORO.</param>
        /// <param name="IsAfsacCell">Filter flag for AfsacCell.</param>
        /// <param name="BasicDetailsId">BasicDetailsId for filtering.</param>
        /// <returns>Returns a JSON response with the officer data or an error message.</returns>
        public async Task<IActionResult> GetOffrsByUnitMapId(int id, int UnitId, int IsRO, int IsORO, int IsAfsacCell, int BasicDetailsId)
        {
            try
            {
                // Retrieve the user's DomainMapId from claims
                int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // If UnitId is 0, retrieve it from the session
                if (UnitId == 0)
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
                // Log the error and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves user profile details by ArmyNo.
        /// </summary>
        /// <param name="ArmyNo">The Army number for fetching the user profile.</param>
        /// <returns>Returns a JSON response with the user profile data.</returns>
        public async Task<IActionResult> GetByMasterArmyNo(string ArmyNo)
        {
            try
            {
                // Retrieve the user ID from claims
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch user profile by ArmyNo
                return Json(await _userProfileBL.GetByMArmyNo(ArmyNo, userid));
            }
            catch (Exception ex)
            {
                // Log the error and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves the domain mapping by AspNetUserId.
        /// </summary>
        /// <param name="Data">The domain mapping data that contains the AspNetUserId.</param>
        /// <returns>Returns a JSON response with the domain mapping data.</returns>
        public async Task<IActionResult> GetByAspnetUserIdBy(TrnDomainMapping Data)
        {
            try
            {
                // Fetch domain mapping by AspNetUserId
                return Json(await _iDomainMapBL.GetByAspnetUserIdBy(Data.AspNetUsersId));
            }
            catch (Exception ex)
            {
                // Log the error and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method retrieves the user profile based on RequestId.
        /// </summary>
        /// <param name="RequestId">The RequestId associated with the user profile.</param>
        /// <returns>Returns a JSON response with the user profile data.</returns>
        public async Task<IActionResult> GetByRequestId(int RequestId)
        {
            try
            {
                // Fetch user profile by RequestId
                return Json(await _userProfileBL.GetByRequestId(RequestId));
            }
            catch (Exception ex)
            {
                // Log the error and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method checks if the given ArmyNo exists in the user profile.
        /// </summary>
        /// <param name="ArmyNo">The Army number to be checked in the user profile.</param>
        /// <returns>Returns a JSON response indicating whether the ArmyNo exists in the profile.</returns>
        public async Task<IActionResult> CheckArmyNoInUserProfile(string ArmyNo)
        {
            // Retrieve the user ID from claims
            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check if ArmyNo exists in the user profile
            DTOProfileResponse dTOProfileResponse = await _userProfileBL.CheckArmyNoInUserProfile(ArmyNo, userid);

            return Json(dTOProfileResponse);
        }

        /// <summary>
        /// This method retrieves the top profile by ArmyNo.
        /// </summary>
        /// <param name="ArmyNo">The Army number for retrieving the profile.</param>
        /// <returns>Returns a JSON response with the top profile data.</returns>
        public async Task<IActionResult> GetTopByArmyNo(string ArmyNo)
        {
            try
            {
                // Fetch the top profile by ArmyNo
                return Json(await _userProfileBL.GetTopByArmyNo(ArmyNo));
            }
            catch (Exception ex)
            {
                // Log the error and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        } 
    }
}
