using AutoMapper;
using BusinessLogicsLayer;
using BusinessLogicsLayer.Account;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Service;
using BusinessLogicsLayer.Token;
using BusinessLogicsLayer.TrnLoginLog;
using BusinessLogicsLayer.Unit;
using DataAccessLayer;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Web.WebHelpers;
using OneLogin.Saml;
using ApplicationRole = DataTransferObject.Domain.Identitytable.ApplicationRole;
using System.Net;
using BusinessLogicsLayer.IAMSetting;
using System.Configuration;
using Humanizer;
using DataTransferObject.Constants;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAccountBL _iAccountBL;
        public readonly IDomainMapBL _iDomainMapBL;
        private readonly IUserProfileBL _userProfileBL;
        public readonly IMapUnitBL _IMapUnitBL;
        public readonly iGetTokenBL _iGetTokenBL;
        public readonly ITrnLoginLogBL _TrnLoginLogBL;
        private readonly IUnitBL _iUnitBL;
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly IDataProtector protector;
        private readonly IService service;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public const string SessionKeySalt = "_Salt";
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IIAMSettingBL _iAMSettingBL;
        private readonly IHostEnvironment _hostEnv;

        public AccountController(IConfiguration configuration,IUnitOfWork unitOfWork,IUnitBL unitBL, IAccountBL iAccountBL , IDomainMapBL iDomainMapBL, IUserProfileBL userProfileBL, IMapUnitBL mapUnitBL, RoleManager<ApplicationRole> roleManager, iGetTokenBL iGetTokenBL, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ApplicationDbContext contextTransaction,
            IDataProtectionProvider dataProtectionProvider, IService service, IMapper mapper, DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<AccountController> logger, ITrnLoginLogBL trnLoginLogBL, IHttpContextAccessor httpContextAccessor, IIAMSettingBL iAMSettingBL, IHostEnvironment hostEnv)
        {
            _configuration = configuration;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
            _iUnitBL = unitBL;
            _iAccountBL = iAccountBL;
            _iDomainMapBL = iDomainMapBL;
            _userProfileBL = userProfileBL;
            _iGetTokenBL = iGetTokenBL;
            _IMapUnitBL = mapUnitBL;
            this.context = context;
            this.contextTransaction = contextTransaction;
            this.service = service;
            this._mapper = mapper;
            this.protector = dataProtectionProvider.CreateProtector(
    dataProtectionPurposeStrings.AFSACIdRouteValue);
            _logger = logger;
            _TrnLoginLogBL= trnLoginLogBL;
            _httpContextAccessor= httpContextAccessor;
            _iAMSettingBL = iAMSettingBL;
            _hostEnv = hostEnv;
        }
        public class Log
        {
            public string NameId { get; set; }=string.Empty;
            public string SAMLRole { get; set; } = string.Empty;
            public string AppName { get; set; } = string.Empty;

        }
        /// <summary>
        /// Returns the "Access Denied" view shown when a user does not have
        /// sufficient permissions to access a protected resource.
        /// </summary>
        /// <returns>The AccessDenied view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Domain Regn.

        /// <summary>Displays the Domain Registration management page.</summary>
        /// <returns>The <c>DomainRegn</c> view.</returns>
        /// <remarks>Accessible only to users in the <c>admin</c> role.</remarks>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult DomainRegn()
        {
            return View();
        }

        /// <summary>Admin-only: returns DataTables JSON for domain registrations.</summary>
        /// <param name="dTO">DataTables request (draw/start/length/search/sort).</param>
        /// <returns>JSON result; empty set if model invalid; generic error on exception.</returns>
        /// <remarks>Validates model, delegates to BL, logs errors (eventId: 1001).</remarks>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllDomainRegn(DTODataTablesRequest dTO)
        {
            if (!ModelState.IsValid)
            {
                var empty = new DTODataTablesResponse<DTODomainRegnResponse>
                {
                    // Use dto.Draw or dto.draw depending on your DTO property name
                    draw = (dTO?.Draw) ?? 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<DTODomainRegnResponse>()
                };
                return Json(empty);
            }
            try
            {
                // Get paged/filtered/sorted domain registrations (DataTables response).
                return Json(await _iAccountBL.GetAllDomainRegn(dTO));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->GetAllDomainRegn");
                return Json(KeyConstants.InternalServerError);
            }

        }

        /// <summary>
        /// Admin-only action to upsert a domain registration.
        /// Sets <c>Updatedby</c> from the current user, <c>UpdatedOn</c> to IST,
        /// and forces <c>IsORO</c>/<c>IsRO</c> to <c>false</c>.
        /// </summary>
        /// <param name="dTO">
        /// Request DTO with domain details; if <c>Id &gt; 0</c> updates the existing record,
        /// otherwise inserts a new one. <c>DomainId</c> must be unique.
        /// </param>
        /// <returns>
        /// JSON result:
        /// <c>KeyConstants.Save</c> (insert success),
        /// <c>KeyConstants.Update</c> (update success),
        /// <c>KeyConstants.Exists</c> (duplicate <c>DomainId</c>),
        /// validation errors when <c>ModelState</c> is invalid,
        /// or <c>KeyConstants.InternalServerError</c> on failure.
        /// </returns>
        /// <remarks>
        /// Flow (compact): validate → check duplicate via <c>_iAccountBL.GetByDomainId</c> →
        /// call <c>_iAccountBL.SaveDomainRegn</c> (insert/update) → return status.  
        /// Errors are logged with eventId 1001. Requires role <c>admin</c>.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveDomainRegn(DTODomainRegnRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));// Logged in User Id
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));// IST
                dTO.IsORO = false;
                dTO.IsRO = false;

                if (ModelState.IsValid)// Server side validation check
                {
                    if (!_iAccountBL.GetByDomainId(dTO.DomainId, dTO.Id))// Check Duplicate DomainId
                    {
                        bool result;
                        if (dTO.Id > 0)// Update
                        {
                            result = (bool)await _iAccountBL.SaveDomainRegn(dTO);// Update
                            if (result==true)
                            {
                                return Json(KeyConstants.Update);// Update Success
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);// Update Failed
                            }
                            
                        }
                        else// Insert
                        {
                            result = (bool)await _iAccountBL.SaveDomainRegn(dTO);// Insert
                            if (result == true)
                            {
                                return Json(KeyConstants.Save);// Insert Success
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);// Insert Failed
                            }
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);// DomainId already exists
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());// Server side validation error
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->SaveDomainRegn");
                return Json(KeyConstants.InternalServerError);
            }

        }

        /// <summary>
        /// Returns all roles as a JSON payload.
        /// </summary>
        /// <returns>
        /// JSON result containing the list of roles from the business layer, or
        /// <c>KeyConstants.InternalServerError</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// Delegates to <c>_iAccountBL.GetAllRole()</c>. Any unhandled exceptions are logged
        /// with event ID 1001 and a generic error token is returned.
        /// </remarks>
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                return Json(await _iAccountBL.GetAllRole());// Get All Role
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->GetAllRole");// Log Error
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Returns all claim definitions as a JSON payload.
        /// </summary>
        /// <returns>
        /// JSON list from <c>_iAccountBL.GetAllClaims()</c>, or
        /// <c>KeyConstants.InternalServerError</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// Delegates to the business layer; logs exceptions with event ID 1001.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllClaims()
        {
            try
            {
                return Json(await _iAccountBL.GetAllClaims());// Get All Claims
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->GetAllClaims");
                return Json(KeyConstants.InternalServerError);
            }
        }
        
        /// Returns the count of accounts as a JSON payload.
        /// </summary>
        /// <returns>
        /// JSON result containing the account count from the business layer,
        /// or <c>KeyConstants.InternalServerError</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// Delegates to <c>_iAccountBL.AccountCount()</c>. Any unhandled exceptions are logged
        /// with event ID 1001 and a generic error token is returned. Requires <c>admin</c> role.
        /// </remarks>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AccountCount()
        {
            try
            {
                return Json(await _iAccountBL.AccountCount());// Get Account Count
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->AccountCount");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion End Domain Regn.

        #region ProfileManage

        /// <summary>
        /// Displays the Profile Management page for administrators.
        /// </summary>
        /// <returns>The ProfileManage view.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult ProfileManage()
        {
            return View();
        }
        
        /// Admin-only action to add or update a user profile.
        /// Sets <c>IsActive</c> to <c>true</c>, <c>Updatedby</c> to the current user, and <c>UpdatedOn</c> to now.
        /// If <c>UserId &gt; 0</c>, updates the existing profile after checking for duplicate ArmyNo.
        /// If <c>UserId == 0</c>, inserts a new profile after checking for duplicate ArmyNo.
        /// Returns <c>KeyConstants.Exists</c> if ArmyNo already exists, <c>KeyConstants.Update</c> or <c>KeyConstants.Save</c> on success,
        /// validation errors if <c>ModelState</c> is invalid, or <c>KeyConstants.InternalServerError</c> on failure.
        /// Errors are logged with eventId 1001.
        /// </summary>
        /// <param name="dTO">Profile data to add or update.</param>
        /// <returns>JSON result indicating success, duplicate, validation errors, or error.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveProfileManage(MUserProfile dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)// Server side validation check
                {
                    if (dTO.UserId > 0)// Update
                    {
                        bool? result = await _userProfileBL.FindByArmyNoWithUserId(dTO.ArmyNo, dTO.UserId);// Check Duplicate ArmyNo
                        if (result !=null)
                        {
                            if(result == true)
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
                        bool? result = await _userProfileBL.FindByArmyNo(dTO.ArmyNo);// Check Duplicate ArmyNo
                        if (result!=null)
                        {
                            if(result == true)
                            {
                                return Json(KeyConstants.Exists);
                            }
                            else
                            {
                                await _userProfileBL.Add(dTO);
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
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());// Server side validation error
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->SaveProfileManage");
                return Json(KeyConstants.InternalServerError);// Log Error
            }

        }
        
        /// Admin-only: returns DataTables JSON for profile management.
        /// </summary>
        /// <param name="dTO">DataTables request (draw/start/length/search/sort).</param>
        /// <returns>
        /// JSON result containing paged/filtered/sorted profile management data,
        /// or an empty set if an exception occurs.
        /// </returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllProfileManage(DTODataTablesRequest dTO)
        {
            try
            {
                return Json(await _iAccountBL.GetAllProfileManage(dTO));// Get All Profile Manage
            }
            catch (Exception ex)
            {
                List<DTOProfileManageResponse> dTOUserRegnResponses = new List<DTOProfileManageResponse>();
                var responseData = new DTODataTablesResponse<DTOProfileManageResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                _logger.LogError(1001, ex, "Account->ProfileManage");
                return Json(responseData);
            }
        }
        /// <summary>
        /// Returns the total count of user profiles as a JSON payload.
        /// </summary>
        /// <returns>
        /// JSON result containing the total profile count from the business layer,
        /// or <c>KeyConstants.InternalServerError</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// Delegates to <c>_iAccountBL.TotalProfileCount()</c>. Any unhandled exceptions are logged
        /// with event ID 1001 and a generic error token is returned. Requires <c>admin</c> role.
        /// </remarks>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> TotalProfileCount()
        {
            try
            {
                return Json(await _iAccountBL.TotalProfileCount());// Get Total Profile Count
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->TotalProfileCount");
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Admin-only action to delete a user profile.
        /// Checks for foreign key references in related tables before deletion.
        /// Returns Json(5) if references exist, otherwise deletes and returns success.
        /// </summary>
        /// <param name="dTO">User profile to delete.</param>
        /// <returns>JSON result indicating success or reference constraint.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProfile(MUserProfile dTO)
        {
            DTOProfileIdCheckInFKTableResponse? response = await _userProfileBL.ProfileIdCheckInFKTable(dTO.UserId);// Check FK Reference
            if (response.TotalTDM > 0 || response.TotalTH > 0 || response.TotalTPO_To > 0 || response.TotalTPO_From > 0 || response.TotalTFFrom > 0 || response.TotalTFTo > 0)// If reference exists
            {
                return Json(5);
            }
            else
            {
                await _userProfileBL.Delete(dTO);// Delete Profile
                return Json(KeyConstants.Success);
            }
        }

        #endregion End ProfileManage

        #region UserRegn

        /// <summary>
        /// Displays the User Registration management page for administrators.
        /// </summary>
        /// <returns>The UserRegn view.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult UserRegn()
        {
            return View();
        }

        /// <summary>
        /// Admin-only: returns DataTables JSON for user registrations.
        /// </summary>
        /// <param name="dTO">DataTables request (draw/start/length/search/sort).</param>
        /// <returns>
        /// JSON result containing paged/filtered/sorted user registration data,
        /// or an empty set if model is invalid or an exception occurs.
        /// </returns>
        /// <remarks>
        /// Validates model, delegates to business layer, logs errors (eventId: 1001).
        /// </remarks>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllUserRegn(DTODataTablesRequest dTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Json(await _iAccountBL.GetAllUserRegn(dTO));// Get All User Regn
                }
                else
                {
                    List<DTOUserRegnResponse> dTOUserRegns = new List<DTOUserRegnResponse>();
                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUserRegns
                    };
                    return Json(responseData);
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->GetAllDomainRegn");
                return Json(KeyConstants.InternalServerError);
            }

        }

        /// <summary>
        /// Admin-only: returns DataTables JSON for user registration mapping.
        /// </summary>
        /// <param name="dTO">DataTables request (draw/start/length/search/sort).</param>
        /// <returns>
        /// JSON result containing paged/filtered/sorted user registration mapping data,
        /// or an empty set if model is invalid or an exception occurs.
        /// </returns>
        /// <remarks>
        /// Validates model, delegates to business layer, logs errors (eventId: 1001).
        /// </remarks>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetDataForDataTable(DTODataTablesRequest dTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Json(await _iAccountBL.GetDataForDataTable(dTO));// Get All User Regn
                }
                else
                {
                    List<DTOUserRegnResponse> dTOUserRegnResponses = new List<DTOUserRegnResponse>();
                    var responseData = new DTODataTablesResponse<DTOUserRegnResponse>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUserRegnResponses
                    };
                    return Json(responseData);
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {   
                _logger.LogError(1001, ex, "Account->GetDataForDataTable");
                return Json(KeyConstants.InternalServerError);
            }

        }

        /// <summary>
        /// Admin-only action to save user registration mapping (domain/user role/claims mapping).
        /// </summary>
        /// <param name="dTO">Request DTO containing user registration mapping details.</param>
        /// <returns>
        /// JSON result indicating success or failure:
        /// - Success: Serialized <see cref="DTOUserRegnResultResponse"/>.
        /// - Failure: Error message and <see cref="DTOUserRegnResultResponse"/> with <c>Result = false</c>.
        /// </returns>
        /// <remarks>
        /// Validates model state before delegating to <c>_iAccountBL.SaveMapping</c>.
        /// If the mapping is saved successfully, it returns the serialized result. Otherwise, 
        /// it returns a failure message.
        /// In case of validation errors, the model state errors are returned as a JSON array.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveMapping(DTOUserRegnMappingRequest dTO)
        {
            DTOUserRegnResultResponse dTOUserRegnResult = new DTOUserRegnResultResponse();
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                if (ModelState.IsValid)
                {
                    DTOUserRegnResultResponse? dTOUserRegnResultResponse = await _iAccountBL.SaveMapping(dTO);// Save Mapping
                    if (dTOUserRegnResultResponse != null)
                    {
                        string json = JsonConvert.SerializeObject(dTOUserRegnResultResponse);
                        return Json(json);
                    }
                    else
                    {
                        dTOUserRegnResult.Result = false;
                        dTOUserRegnResult.Message = "Something went wrong or Invalid Entry!";
                        return Json(dTOUserRegnResult);
                    }
                }
                else
                {
                    //working pending 
                    string json = JsonConvert.SerializeObject(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                    dTOUserRegnResult.Result = false;
                    dTOUserRegnResult.Message = json;
                    return Json(dTOUserRegnResult);
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->SaveProfileManage");
                dTOUserRegnResult.Result = false;
                dTOUserRegnResult.Message = "Something went wrong or Invalid Entry!";
                return Json(dTOUserRegnResult);
            }

        }

        /// <summary>
        /// Admin-only action to update the domain flag for a user registration.
        /// </summary>
        /// <param name="dTO">Request DTO containing the details to update the domain flag.</param>
        /// <returns>
        /// JSON result:
        /// <list type="bullet">
        ///   <item><c>KeyConstants.Update</c> on successful update.</item>
        ///   <item><c>KeyConstants.InternalServerError</c> if an error occurs or the update fails.</item>
        ///   <item>Validation errors if <paramref name="ModelState"/> is invalid.</item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method checks the validity of the input model, calls the business layer to update the domain flag,
        /// and returns the appropriate result. Errors are logged with event ID 1001 if an exception occurs.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDomainFlag(DTOUserRegnUpdateDomainFlagRequest dTO)
        {
            DTOUserRegnResultResponse dTOUserRegnResult = new DTOUserRegnResultResponse();
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (ModelState.IsValid)
                {
                    bool? result = (bool)await _iAccountBL.UpdateDomainFlag(dTO);// Update Domain Flag
                    if (result!=null)
                    {
                        if(result == true)
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
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->UpdateDomainFlag");
                return Json(KeyConstants.InternalServerError);
            }

        }

        #endregion End UserRegn

        #region IMLogin

        /// <summary>
        /// Handles the self-login for IM (Identity Management) based on the environment setting and IAM configuration.
        /// Redirects to an IAM login page if the login is enabled; otherwise, it clears session data and renders the view.
        /// </summary>
        /// <returns>
        /// A view of the IM login page. If the environment is development, a different IAM login URL is used.
        /// </returns>
        /// <remarks>
        /// - Checks whether IAM login is enabled by reading settings from the <see cref="IAMSetting"/> with the appropriate environment byte value.
        /// - If IAM login is enabled, redirects the user to the IAM login page (https://iam2.army.mil/IAM/User).
        /// - If IAM login is disabled, it clears any session data related to the IM and sets a footer value in the view.
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> IMLoginSelf()
        {
            byte id = 0;
            if (_hostEnv.IsDevelopment())
            {
                id = 2;
            }
            else
            {
                id = 1;
            }
            IAMSetting iAMSetting = await _iAMSettingBL.GetByByte(id);

            if (iAMSetting.WithIAMLogin)
            {
                try
                {
                    Response.Redirect("https://iam2.army.mil/IAM/User", true);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                string? Footer = _configuration["Footer:Test"];
                ViewBag.Footer = Footer;

                DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "Token");
                DTOTempSession? dTOTempSession1 = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");
                if (dTOTempSession != null)
                {
                    HttpContext.Session.Remove("Token");
                }

                if (dTOTempSession1 != null)
                {
                    HttpContext.Session.Remove("IMData");
                }
            }

            return View();
        }
        /// <summary>
        /// Handles the self-login process for IM (Identity Management) by validating the user's domain and role information.
        /// </summary>
        /// <param name="model">The request DTO containing the domain and role information for login validation.</param>
        /// <returns>
        /// A redirection to the <see cref="TokenValidate"/> action if the domain and role are valid. 
        /// If any validation fails, the method handles it by setting the appropriate session status and error message.
        /// </returns>
        /// <remarks>
        /// This method checks several conditions based on the <paramref name="model"/> (such as role and domain ID):
        /// <list type="bullet">
        ///     <item>
        ///         <description>If the user has an active domain mapping and a valid role, the user is redirected to the <see cref="TokenValidate"/> action.</description>
        ///     </item>
        ///     <item>
        ///         <description>If the user is not mapped or their role is invalid, the method sets the status to 6 and returns an error message.</description>
        ///     </item>
        ///     <item>
        ///         <description>If the user is new, the status is set to 2 and they are redirected for further validation.</description>
        ///     </item>
        /// </list>
        /// Session variables are set to store important data like user ID, domain ID, and mapping status, and these are passed along for use in subsequent actions.
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IMLoginSelf(DTOIMLoginRequest model)
        {
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;
           
            if (ModelState.IsValid)
            {
                DTOTempSession dTOTempSession = new DTOTempSession();
                TrnDomainMapping? _trnDomainMapping = await _iDomainMapBL.GetAllRelatedDataByDomainId(model.DomainId,model.Role);
                if (_trnDomainMapping != null && _trnDomainMapping.ApplicationUser.AdminFlag == true && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId != null)
                {
                    dTOTempSession.NewUser = false;
                    dTOTempSession.AdminFlag = _trnDomainMapping.ApplicationUser.AdminFlag;
                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                    dTOTempSession.RoleName = model.Role;
                    dTOTempSession.ICNO = _trnDomainMapping.MUserProfile.ArmyNo;
                    dTOTempSession.Name = _trnDomainMapping.MUserProfile.Name;
                    dTOTempSession.RankAbbreviation = _trnDomainMapping.Rank.RankAbbreviation;
                    dTOTempSession.UserId = _trnDomainMapping.MUserProfile.UserId;
                    dTOTempSession.TDMId = _trnDomainMapping.Id;
                    dTOTempSession.TDMUnitMapId = _trnDomainMapping.UnitId;
                    dTOTempSession.TDMApptId = _trnDomainMapping.ApptId;
                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;
                    dTOTempSession.IsIO = _trnDomainMapping.IsIO;
                    dTOTempSession.IsCO = _trnDomainMapping.IsCO;
                    dTOTempSession.IsRO = _trnDomainMapping.IsRO;
                    dTOTempSession.IsORO = _trnDomainMapping.IsORO;
                    dTOTempSession.DialingCode = _trnDomainMapping.DialingCode;
                    dTOTempSession.Extension = _trnDomainMapping.Extension;
                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;


                    if (_trnDomainMapping.Role !=null)
                    {
                        dTOTempSession.Status = 5;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }
                    else
                    {
                        TempData["error"] = "Role not authorized.";
                        dTOTempSession.Status = 6;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }

                }
                else if (_trnDomainMapping != null && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId == null)
                {
                    /*Get UserId from ProfileTable (Based on Input ArmyNo with token authorise.) and Update in TrnDomainMapping Table*/
                    dTOTempSession.NewUser = false;
                    dTOTempSession.AdminFlag = _trnDomainMapping.ApplicationUser.AdminFlag;
                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                    dTOTempSession.RoleName = model.Role;
                    dTOTempSession.TDMId = _trnDomainMapping.Id;
                    dTOTempSession.TDMUnitMapId = _trnDomainMapping.UnitId;
                    dTOTempSession.TDMApptId = _trnDomainMapping.ApptId;
                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;
                    dTOTempSession.IsIO = _trnDomainMapping.IsIO;
                    dTOTempSession.IsCO = _trnDomainMapping.IsCO;
                    dTOTempSession.IsRO = _trnDomainMapping.IsRO;
                    dTOTempSession.IsORO = _trnDomainMapping.IsORO;
                    dTOTempSession.DialingCode = _trnDomainMapping.DialingCode;
                    dTOTempSession.Extension = _trnDomainMapping.Extension;
                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;
                    if (_trnDomainMapping.Role != null)
                    {
                        dTOTempSession.Status = 4;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }
                    else
                    {
                        TempData["error"] = "Role not authorized.";
                        dTOTempSession.Status = 6;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }

                }
                else if (_trnDomainMapping != null && _trnDomainMapping.Id == 0)
                {
                    /*Create TrnDomainMapping using AspnetUserId,UnitId,UserId from Profile Table.*/
                    dTOTempSession.NewUser = false;
                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                    dTOTempSession.RoleName = model.Role;
                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;

                    if (_trnDomainMapping.Role != null)
                    {
                        dTOTempSession.Status = 3;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }
                    else
                    {
                        TempData["error"] = "Role not authorized.";
                        dTOTempSession.Status = 6;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }

                }
                else if (_trnDomainMapping != null && _trnDomainMapping.ApplicationUser.AdminFlag == false && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId != null) 
                {
                    dTOTempSession.NewUser = false;
                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                    dTOTempSession.RoleName = model.Role;
                    dTOTempSession.ICNO = _trnDomainMapping.MUserProfile.ArmyNo;
                    dTOTempSession.Name = _trnDomainMapping.MUserProfile.Name;
                    dTOTempSession.UserId = _trnDomainMapping.MUserProfile.UserId;
                    dTOTempSession.TDMId = _trnDomainMapping.Id;
                    dTOTempSession.TDMUnitMapId = _trnDomainMapping.UnitId;
                    dTOTempSession.TDMApptId = _trnDomainMapping.ApptId;
                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;
                    dTOTempSession.IsIO = _trnDomainMapping.IsIO;
                    dTOTempSession.IsCO = _trnDomainMapping.IsCO;
                    dTOTempSession.IsRO = _trnDomainMapping.IsRO;
                    dTOTempSession.IsORO = _trnDomainMapping.IsORO;
                    dTOTempSession.DialingCode = _trnDomainMapping.DialingCode;
                    dTOTempSession.Extension = _trnDomainMapping.Extension;
                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;
                    if (_trnDomainMapping.Role != null)
                    {
                        dTOTempSession.Status = 1;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        //TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval..<br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence. <br/>Contact Admin.";
                        if (_trnDomainMapping.ApplicationUser.AdminMsg != null)
                        {
                            TempData["error"] = _trnDomainMapping.ApplicationUser.AdminMsg;
                        }
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }
                    else
                    {
                        TempData["error"] = "Role not authorized.";
                        dTOTempSession.Status = 6;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }

                }
                else if (_trnDomainMapping == null)
                {
                    /*Create DomainId in AspNetUser Table , Assign Role.,Create Mapping with add profile id.*/
                    dTOTempSession.NewUser = true;
                    dTOTempSession.DomainId = model.DomainId;
                    dTOTempSession.RoleName = model.Role;
                    dTOTempSession.Status = 2;
                    SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                    return RedirectToActionPermanent("TokenValidate", "Account");
                }

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TokenValidate()
        {
            ///////Cookie With Secure Flag at this time not use////////////////////////
            //var cookieOptions = new CookieOptions
            //{
            //    Secure = true, // Ensures the cookie is only transmitted over HTTPS
            //    HttpOnly = true, // Makes the cookie inaccessible via client-side scripts
            //    SameSite = SameSiteMode.Strict, // Limits cookie to same-site requests to prevent CSRF
            //    Expires = DateTimeOffset.UtcNow.AddHours(1) // Cookie expiration
            //};

            //Response.Cookies.Append("MySecureCookie", "cookieValue", cookieOptions);
            /////////////end Cookie With Secure Flag//////////////

            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;


            string dd = AESEncrytDecry.GetSalt();  // "8080808080808080"; //protector.Protect("1");
            HttpContext.Session.SetString(SessionKeySalt, dd);
            ViewBag.hdns = dd;



            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "Token");
            List<string> RoleNameList = new List<string>() { "user" };


            if (userid == 0)
            {
                DTOTempSession? dTOTempSession1 = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");

                if (dTOTempSession1 != null)
                {
                    if (dTOTempSession1.Status == 1)
                    {
                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "You are not authorized this page.";
                    return View();
                }
            }
            else
            {
                if (dTOTempSession != null)
                {

                    if (RoleNameList.Contains(dTOTempSession.RoleName))
                    {
                        return RedirectToActionPermanent("Index", "Home");
                    }
                    else if (dTOTempSession.RoleName == "admin")
                    {
                        return RedirectToActionPermanent("DashboardMaster", "Master");
                    }
                    else if (dTOTempSession.RoleName == "super admin")
                    {
                        return RedirectToActionPermanent("Index", "Account");
                    }
                    return View();
                }
                else
                {
                    return View();
                }

            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> TokenValidate(DTOTokenRequest model)
        {
            try
            {
                string? Footer = _configuration["Footer:Test"];
                ViewBag.Footer = Footer;

                string? dd = HttpContext.Session.GetString(SessionKeySalt);
                if (dd != null)
                {
                    csConst.cSalt = dd;
                    string Password = AESEncrytDecry.DecryptStringAES(model.Password);
                    model.Password = Password;
                }



                ///////Cookie With Secure Flag at this time not use////////////////////////
                //var cookieOptions = new CookieOptions
                //{
                //    Secure = true, // Ensures the cookie is only transmitted over HTTPS
                //    HttpOnly = true, // Makes the cookie inaccessible via client-side scripts
                //    SameSite = SameSiteMode.Strict, // Limits cookie to same-site requests to prevent CSRF
                //    Expires = DateTimeOffset.UtcNow.AddHours(1) // Cookie expiration
                //};

                //Response.Cookies.Append("MySecureCookie", Guid.NewGuid().ToString(), cookieOptions);
                /////////////end Cookie With Secure Flag//////////////

                DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");
                List<string> RoleNameList = new List<string>() { "user" };
                if (dTOTempSession != null)
                {
                    if (dTOTempSession.NewUser == false)
                    {
                        //model.ConfirmPassword= model.Password;
                        // Remove ConfirmPassword validation
                        ModelState.Remove("ConfirmPassword");
                    }

                    model.ICNo = model.ICNo.Trim();

                    if (ModelState.IsValid)
                    {
                        if (dTOTempSession.Status == 5 && dTOTempSession.ICNO == model.ICNo)
                        {
                            var usera = await userManager.FindByIdAsync(dTOTempSession.AspNetUsersId.ToString());

                            HttpContext.Session.Remove("Token");
                            //await signInManager.SignOutAsync();
                            //await userManager.UpdateSecurityStampAsync(usera);
                            if (usera != null)
                            {
                                //default Password - Admin123#
                                var result = await signInManager.PasswordSignInAsync(usera.UserName, model.Password, false, lockoutOnFailure: true);
                                // var rolelist = await signInManager.UserManager.GetRolesAsync(usera);
                                //var user1 = await signInManager.UserManager.IsInRoleAsync(usera, "User");
                                if (result.Succeeded)
                                {
                                    //var army = await _userProfileBL.Get(Convert.ToInt32(dTOTempSession.UserId));

                                    //  await userManager.RemoveClaimAsync(usera, new Claim("Roles", dTOTempSession.RoleName));
                                    //  await userManager.AddClaimAsync(usera, new Claim("Roles", dTOTempSession.RoleName));





                                    DtoSession dtoSession = new DtoSession();
                                    dtoSession.ICNO = dTOTempSession.ICNO;
                                    dtoSession.RoleName = dTOTempSession.RoleName.Trim();
                                    dtoSession.UserId = dTOTempSession.UserId;
                                    dtoSession.UnitId = dTOTempSession.TDMUnitMapId;
                                    dtoSession.Name = dTOTempSession.Name.ToUpper();
                                    dtoSession.RankName = dTOTempSession.RankAbbreviation.ToUpper();
                                    dtoSession.TrnDomainMappingId = dTOTempSession.TDMId;
                                    dtoSession.RoleName = dTOTempSession.RoleName;
                                    dtoSession.DoaminId = dTOTempSession.DomainId;
                                    ///////////////login log//////////////////////
                                    TrnLogin_Log log = new TrnLogin_Log();
                                    log.AspNetUsersId = Convert.ToInt32(usera.Id);
                                    var Role = await roleManager.FindByNameAsync(dTOTempSession.RoleName);
                                    log.RoleId = Convert.ToInt32(Role.Id);
                                    log.UserId = Convert.ToInt32(dTOTempSession.UserId);
                                    log.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                                    log.IsActive = true;
                                    log.Updatedby = Convert.ToInt32(usera.Id);
                                    log.UpdatedOn = DateTime.Now;
                                    await _TrnLoginLogBL.Add(log);
                                    ////////////////End Log////////////////////////

                                    SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);



                                    if (RoleNameList.Contains(dTOTempSession.RoleName))
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        return RedirectToActionPermanent("Index", "Home");
                                    }
                                    else if (dTOTempSession.RoleName.ToUpper() == "ADMIN")
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        return RedirectToActionPermanent("DashboardMaster", "Master");
                                    }
                                    else if (dTOTempSession.RoleName == "Super Admin")
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        return RedirectToActionPermanent("Index", "Account");
                                    }
                                }
                                else if (result.IsLockedOut)
                                {
                                    TempData["error"] = "Account Locked Out Please Try after 10 minutes.";
                                    goto End;
                                }
                                else if (result.IsNotAllowed)
                                {
                                    TempData["error"] = "Already Login " + usera.UserName + " Please Try Some Time";
                                    goto End;
                                }
                                else
                                {

                                    TempData["error"] = "Not Valid User / Password. Access Failed Count " + usera.AccessFailedCount + " Max Access Attempts 3";
                                    goto End;
                                }
                            }

                        }
                        else
                        {
                            DTOAllRelatedDataByArmyNoResponse? _dTOProfileResponse = await _userProfileBL.GetAllRelatedDataByArmyNo(model.ICNo);
                            if (dTOTempSession.Status == 1)
                            {
                                //TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. <br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/> Contact Admin.";
                                if (_dTOProfileResponse != null && _dTOProfileResponse.AdminMsg != null)
                                {
                                    TempData["error"] = _dTOProfileResponse.AdminMsg;
                                }
                                return View();
                            }
                            else if (dTOTempSession.Status == 6)
                            {
                                TempData["error"] = "Role not authorized.";
                                return View();
                            }
                            else if (dTOTempSession.Status == 5 && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId > 0 && model.ICNo != dTOTempSession.ICNO)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.ICNoDomainId = _dTOProfileResponse.DomainId;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNoTDMUnitMapId = _dTOProfileResponse.UnitId;
                                dTOTempSession.ICNoTDMId = _dTOProfileResponse.TrnDomainMappingId;
                                dTOTempSession.ICNoTDMApptId = _dTOProfileResponse.ApptId;
                                //TempData["error"] = "Not Authorized to access the current profile because Domain Id - " + dTOTempSession.DomainId + " is presently mapped to Profile Id - " + dTOTempSession.UserId + " ( IC No- " + dTOTempSession.ICNO + ") .<br/>Pl change Token and try again!";
                                
                                
                                
                                
                                
                                
                                
                                TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId > 0)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNoDomainId = _dTOProfileResponse.DomainId;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNoTDMUnitMapId = _dTOProfileResponse.UnitId;
                                dTOTempSession.ICNoTDMId = _dTOProfileResponse.TrnDomainMappingId;
                                dTOTempSession.ICNoTDMApptId = _dTOProfileResponse.ApptId;

                                if (dTOTempSession.Status == 2)
                                    //TempData["error"] = "Your Profile Id -" + _dTOProfileResponse.UserId + " is mapped to Domain Id - " + _dTOProfileResponse.DomainId + " in Sys.<br/>Pl get yourself relieved first    and try again.";
                                    TempData["error"] = "Invalid Army No / Password.";
                                else if (dTOTempSession.Status == 3)
                                    //TempData["error"] = "Your Profile Id - " + _dTOProfileResponse.UserId + " is already mapped to Domain Id -" + _dTOProfileResponse.DomainId + ".<br/>Pl get yourself relieved first..Domain Id - " + dTOTempSession.DomainId + "(regd) is not mapped to any profile.";
                                    TempData["error"] = "Invalid Army No / Password.";
                                else if (dTOTempSession.Status == 4)
                                    //TempData["error"] = "You are presently mapped to Domain Id -" + _dTOProfileResponse.DomainId + ".<br/>Pl relieve yourself and get your profile mapped to new domain ID - " + dTOTempSession.DomainId + ".";
                                    TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId == 0)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNO = _dTOProfileResponse.ArmyNo;
                                dTOTempSession.UserId = _dTOProfileResponse.UserId;
                                SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                return RedirectToActionPermanent("Profile", "Account");
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse == null)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNO = model.ICNo;
                                SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                return RedirectToActionPermanent("Profile", "Account");
                            }
                            else if (dTOTempSession.Status == 5 && dTOTempSession.ICNO != model.ICNo)
                            {
                                //TempData["error"] = "Not Authorized to access the current profile because Domain Id - " + dTOTempSession.DomainId + " is presently mapped to Profile Id - " + dTOTempSession.UserId + " ( IC No " + dTOTempSession.ICNO + ") .<br/>Pl change Token and try again!";
                                TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                        }
                    }
                    else
                    {
                        var error = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        TempData["error"] = error[0][0].ErrorMessage;
                        goto End;
                    }
                }
                else
                {
                    TempData["error"] = "You are not authorized this page.";
                    goto End;
                }
            End:
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Profile()
        {
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;
            ///////Cookie With Secure Flag at this time not use////////////////////////
            //var cookieOptions = new CookieOptions
            //{
            //    Secure = true, // Ensures the cookie is only transmitted over HTTPS
            //    HttpOnly = true, // Makes the cookie inaccessible via client-side scripts
            //    SameSite = SameSiteMode.Strict, // Limits cookie to same-site requests to prevent CSRF
            //    Expires = DateTimeOffset.UtcNow.AddHours(1) // Cookie expiration
            //};

            //Response.Cookies.Append("MySecureCookie", Guid.NewGuid().ToString(), cookieOptions);
            /////////////end Cookie With Secure Flag//////////////

            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");
            if (dTOTempSession != null)
            {
                DTOProfileAndMappingRequest dTOProfileAndMappingRequest = new DTOProfileAndMappingRequest();
                if (dTOTempSession.Status == 1)
                {
                    TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/>Contact Admin.";
                    return View();
                }
                else if(dTOTempSession.Status == 6)
                {
                    TempData["error"] = "Role not authorized.";
                    return View();
                }
                else
                {
                    if (dTOTempSession.Status == 4)
                    {
                        dTOProfileAndMappingRequest.TDMId = dTOTempSession.TDMId;
                        dTOProfileAndMappingRequest.ApptId = dTOTempSession.TDMApptId;
                        dTOProfileAndMappingRequest.UnitMapId = dTOTempSession.TDMUnitMapId;
                        //dTOProfileAndMappingRequest.DialingCode = dTOTempSession.DialingCode;
                        //dTOProfileAndMappingRequest.Extension = dTOTempSession.Extension;
                        //dTOProfileAndMappingRequest.IsRO = dTOTempSession.IsRO;
                        dTOProfileAndMappingRequest.IsCO = dTOTempSession.IsCO;
                        dTOProfileAndMappingRequest.IsIO = dTOTempSession.IsIO;
                        //dTOProfileAndMappingRequest.IsORO = dTOTempSession.IsORO;
                    }
                    ViewBag.OptionsRank = service.GetRank(1);
                    ViewBag.OptionsArmedType = service.GetArmedType();

                    if (dTOTempSession.UserId > 0)
                    {
                        try
                        {

                            //Get ArmyNo from UserProfile Table
                            MUserProfile mUserProfile = await _userProfileBL.Get(dTOTempSession.UserId);

                            dTOProfileAndMappingRequest.UserId = mUserProfile.UserId;
                            dTOProfileAndMappingRequest.ArmyNo = mUserProfile.ArmyNo;
                            dTOProfileAndMappingRequest.RankId = mUserProfile.RankId;
                            dTOProfileAndMappingRequest.Name = mUserProfile.Name;
                            //dTOProfileAndMappingRequest.MobileNo = mUserProfile.MobileNo;
                            dTOProfileAndMappingRequest.ArmedId = mUserProfile.ArmedId;
                            dTOProfileAndMappingRequest.ReasonTokenWaiver = mUserProfile.ReasonTokenWaiver;
                            dTOProfileAndMappingRequest.IsTokenWaiver = mUserProfile.IsTokenWaiver;

                            return View(dTOProfileAndMappingRequest);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(1001, ex, "Session variable timeout.");
                            TempData["error"] = "Session time out.";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                    }
                    else
                    {
                        dTOProfileAndMappingRequest.ArmyNo = dTOTempSession.ICNO;
                        return View(dTOProfileAndMappingRequest);
                    }
                }
            }
            else
            {
                TempData["error"] = "You are not authorized this page.";
                return RedirectToActionPermanent("TokenValidate", "Account");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(DTOProfileAndMappingRequest model)
        {
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;

            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");
            model.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            if (dTOTempSession != null)
            {
                if(model.IsTokenWaiver==true)
                {
                    if(model.ReasonTokenWaiver == null)
                    {
                        ModelState.AddModelError("ReasonTokenWaiver", "Reason for IACA Token Waiver is required.");
                    }
                }
                if (ModelState.IsValid)
                {
                    DTOTempSession? resultfinal = await _iAccountBL.ProfileAndMappingSaving(model, dTOTempSession);
                    if (dTOTempSession.Status == 2)
                    {
                        if(resultfinal!=null)
                        {
                            dTOTempSession.AspNetUsersId = resultfinal.AspNetUsersId;
                            dTOTempSession.TDMId = resultfinal.TDMId;
                            dTOTempSession.TDMUnitMapId = resultfinal.TDMUnitMapId;
                            dTOTempSession.UserId = resultfinal.UserId;
                            dTOTempSession.Status = 1;
                            dTOTempSession.IsToken = true;


                            SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                            TempData["success"] = "Domian Id - " + dTOTempSession.DomainId + " & Profile Id- " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. <br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/>Contact Admin or try login after 24 Hrs.";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                        else
                        {
                            TempData["error"] = "Your regn request was not placed.<br/>Contact Admin.";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                    }
                    else if (dTOTempSession.Status == 3)
                    {
                        
                        if (resultfinal != null)
                        {
                            dTOTempSession.TDMId = resultfinal.TDMId;
                            dTOTempSession.TDMUnitMapId = resultfinal.TDMUnitMapId;
                            dTOTempSession.UserId = resultfinal.UserId;
                            dTOTempSession.Status = 1;
                            dTOTempSession.IsToken = true;

                            SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                            if(model.IsTokenWaiver == true)
                            {
                                TempData["success"] = "Your Profile Id - " + dTOTempSession.UserId + " has been successfully mapped to Domain Id - " + dTOTempSession.DomainId + ". > Your token request was successfully placed with Admin for necy Approval.";
                            }
                            else
                            {
                                TempData["success"] = "Your Profile Id - " + dTOTempSession.UserId + " has been successfully mapped to Domain Id - " + dTOTempSession.DomainId + ". > DB ";
                            }
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                        else
                        {
                            TempData["error"] = "Your Profile Id -" + dTOTempSession.UserId + " has not mappe to Domain Id - " + dTOTempSession.DomainId + ". > DB ";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                    }
                    else if (dTOTempSession.Status == 4)
                    {
                        if (resultfinal != null)
                        {
                            dTOTempSession.Status = 1;
                            dTOTempSession.IsToken = true;
                            dTOTempSession.UserId = resultfinal.UserId;
                            SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                            if (model.IsTokenWaiver == true)
                            {
                                TempData["success"] = "Your Profile Id - " + dTOTempSession.UserId + " has been successfully mapped to Domain Id - " + dTOTempSession.DomainId + ". > Your token request was successfully placed with Admin for necy Approval.";
                            }
                            else
                            {
                                TempData["success"] = "Your Profile Id - " + dTOTempSession.UserId + " has been successfully mapped to Domain Id - " + dTOTempSession.DomainId + ". > DB ";
                            }
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                        else
                        {
                            TempData["error"] = "Your Profile Id -" + dTOTempSession.UserId + " has not mappe to Domain Id - " + dTOTempSession.DomainId + ". > DB ";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                    }
                }
            }
            else
            {
                TempData["error"] = "You are not authorized this page.";
                return RedirectToActionPermanent("TokenValidate", "Account");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveUnitWithMapping(DTOSaveUnitWithMappingRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                if (ModelState.IsValid)
                {
                    string SUSNo = dTO.Sus_no + dTO.Suffix.ToUpper();
                    DTOCheckUnitMappedInMapUnitResponse? response = await _IMapUnitBL.CheckUnitMappedInMapUnit(SUSNo);
                    if(response != null)
                    {
                        if(response.UnitMapId != null)
                        {
                            //Unit already mapped
                            return Json(KeyConstants.Exists);
                        }
                        else if (response.IsVerify == false)
                        {
                            //Unit not verify
                            return Json(5);
                        }
                        else
                        {
                            bool result = (bool)await _iAccountBL.SaveUnitWithMapping(dTO);
                            if (result == true)
                            {
                                return Json(KeyConstants.Save);
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                    }
                    else
                    {
                        bool result = (bool)await _iAccountBL.SaveUnitWithMapping(dTO);
                        if (result == true)
                        {
                            return Json(KeyConstants.Save);
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
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->SaveUnitWithMapping");
                return Json(KeyConstants.InternalServerError);
            }

        }

        //now switchrole method is implementaion stage
        [HttpPost]
        [Authorize]
        public IActionResult SwitchRole(string Id)
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                dtoSession.RoleName = Id;
                SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);
            }
            if (Id == "admin")
            {
                return RedirectToActionPermanent("DashboardMaster", "Master");
            }
            else if (Id == "super admin")
            {
                return RedirectToActionPermanent("Index", "Account");
            }
            else
            {
                return RedirectToActionPermanent("Index", "Home");
            }
        }

        #endregion End IMLogin

        #region Logout
        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.Remove("Token");
            await signInManager.SignOutAsync();
            return View();
        }
        #endregion End Logout

        #region Commented Code for for Login Purpose

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Login()
        //{
        //    DTOLoginRequest loginVM = new DTOLoginRequest();
        //    string dd = AESEncrytDecry.GetSalt();  // "8080808080808080"; //protector.Protect("1");
        //    HttpContext.Session.SetString(SessionKeySalt, dd);
        //    loginVM.hdns = dd;

        //    return View(loginVM);

        //}

        //[HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        //[HttpPost, AllowAnonymous]
        //public async Task<IActionResult> Login(DTOLoginRequest model, string? returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string dd = HttpContext.Session.GetString(SessionKeySalt);
        //        csConst.cSalt = dd;
        //        string Password = AESEncrytDecry.DecryptStringAES(model.Password);

        //        string ipAddress;
        //        ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

        //        var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
        //        var url = location.AbsoluteUri;

        //        string user_name = string.Empty;

        //        var selecteduser = await userManager.FindByNameAsync(model.UserName);
        //        if (selecteduser == null)
        //        {
        //            user_name = model.UserName;

        //            ModelState.AddModelError(string.Empty, "Invalid User Id or Password");
        //            goto xyz;
        //        }
        //        else
        //        {
        //            //var result = await signInManager.PasswordSignInAsync(model.UserId, Password, model.RememberMe, true);
        //            var result = await signInManager.PasswordSignInAsync(model.UserName, Password, false, true);
        //            if (result.Succeeded)
        //            {
        //                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //                {
        //                    return Redirect(returnUrl);
        //                }
        //                else
        //                {
        //                    var user = await userManager.FindByNameAsync(model.UserName);

        //                    user_name = model.UserName;


        //                    var roles = await userManager.GetRolesAsync(user);
        //                    ViewBag.Message = "Sucessfully Logged In.";
        //                roll_check:
        //                    if (roles[0] == "Admin")
        //                    {
        //                        return RedirectToActionPermanent("Dashboard", "Home");

        //                    }
        //                    else if (roles[0] == "User")
        //                    {
        //                        return RedirectToActionPermanent("index", "ConfigUser");
        //                    }
        //                    else if (roles[0] == "Super Admin")
        //                    {
        //                        return RedirectToActionPermanent("Index", "Account");
        //                    }
        //                }
        //            }
        //            if (result.IsLockedOut)
        //                ModelState.AddModelError("", "Your account is locked out. Kindly wait for 10 minutes and try again");
        //            else
        //                ModelState.AddModelError(string.Empty, "Invalid User Id or Password");
        //        }
        //    }
        //    ViewBag.Message = "Sucessfully Logged In.";
        //xyz:
        //    return View(model);
        //}

        #endregion End Login

        #region IAM Code

        [AllowAnonymous]
        public async Task<IActionResult> IMLogin()
        {
            try
            {
                var EncryptedResponse = "";
                byte id = 0;
                if (_hostEnv.IsDevelopment())
                {
                    id = 2;
                }
                else 
                {
                    id = 1;
                }
                    
                IAMSetting iAMSetting = await _iAMSettingBL.GetByByte(id);

                if (iAMSetting.DebugWithIAM == true)
                {
                    if (iAMSetting.LocalHostActive == 0)
                    {
                        EncryptedResponse = Request.Form["SAMLResponse"];
                        iAMSetting.DebugWithIAM = false;
                        await _iAMSettingBL.Update(iAMSetting);

                        using (HttpClient client = new HttpClient())
                        {
                            var values = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("SAMLResponse", EncryptedResponse)
                            });

                            HttpResponseMessage response = await client.PostAsync("https://localhost:7023/Account/IMLogin", values);

                            string responseString = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("Server Response: " + responseString);
                        }
                    }
                    else if (iAMSetting.LocalHostActive == 1)
                    {
                        EncryptedResponse = Request.Form["SAMLResponse"];
                        iAMSetting.HardSAMLResonoce = EncryptedResponse;
                        iAMSetting.LocalHostActive = 2;
                        await _iAMSettingBL.Update(iAMSetting);
                        //stop code directelly from here to debug code with IAM, and Run this on this URL "https://localhost:7023/Account/IMLogin". 
                    }
                    else
                    {
                        EncryptedResponse = iAMSetting.HardSAMLResonoce;
                    }
                }
                else
                {
                    EncryptedResponse = Request.Form["SAMLResponse"];
                }



                DTOIMLoginRequest model = new DTOIMLoginRequest();

                DTOTempSession dTOTempSession = new DTOTempSession();


                if (!string.IsNullOrEmpty(EncryptedResponse))
                {

                    string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\eisac.army.mil.pfx", "Abc@2022");

                    AccountSettings accountSettings = new AccountSettings();
                    OneLogin.Saml.Response samlResponse = new OneLogin.Saml.Response(accountSettings);

                    samlResponse.LoadXmlFromBase64(decryptedsamlresponse);
                    //if (samlResponse.IsValid_sign())
                    if (samlResponse.GetNameID() != null)
                    {
                        Log log = new Log();
                        log.NameId = samlResponse.GetNameID();//"Admin";//samlResponse.GetNameID();
                        log.SAMLRole = samlResponse.GetSAMLRole(); //"Admin";//samlResponse.GetSAMLRole();
                        //log.NameId = "Admin";
                        //log.SAMLRole = "Admin";
                        log.AppName = samlResponse.GetSAMLAppName();



                        if (log.NameId != null)
                        {
                            model.DomainId = log.NameId;
                            model.Role = log.SAMLRole;
                            dTOTempSession.AppName = log.AppName;
                            HttpContext.Session.SetString("AppName", dTOTempSession.AppName);

                            string? Footer = _configuration["Footer:Test"];
                            ViewBag.Footer = Footer;
                            //if (ModelState.IsValid)
                            {

                                TrnDomainMapping? _trnDomainMapping = await _iDomainMapBL.GetAllRelatedDataByDomainId(model.DomainId, model.Role);
                                if (_trnDomainMapping != null && _trnDomainMapping.ApplicationUser.AdminFlag == true && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId != null)
                                {
                                    dTOTempSession.NewUser = false;
                                    dTOTempSession.AdminFlag = _trnDomainMapping.ApplicationUser.AdminFlag;
                                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                                    dTOTempSession.RoleName = model.Role;
                                    dTOTempSession.ICNO = _trnDomainMapping.MUserProfile.ArmyNo;
                                    dTOTempSession.Name = _trnDomainMapping.MUserProfile.Name;
                                    dTOTempSession.RankAbbreviation = _trnDomainMapping.Rank.RankAbbreviation;
                                    dTOTempSession.UserId = _trnDomainMapping.MUserProfile.UserId;
                                    dTOTempSession.TDMId = _trnDomainMapping.Id;
                                    dTOTempSession.TDMUnitMapId = _trnDomainMapping.UnitId;
                                    dTOTempSession.TDMApptId = _trnDomainMapping.ApptId;
                                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;
                                    dTOTempSession.IsIO = _trnDomainMapping.IsIO;
                                    dTOTempSession.IsCO = _trnDomainMapping.IsCO;
                                    dTOTempSession.IsRO = _trnDomainMapping.IsRO;
                                    dTOTempSession.IsORO = _trnDomainMapping.IsORO;
                                    dTOTempSession.DialingCode = _trnDomainMapping.DialingCode;
                                    dTOTempSession.Extension = _trnDomainMapping.Extension;
                                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;


                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 5;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }
                                    else
                                    {
                                        TempData["error"] = "Role not authorized.";
                                        dTOTempSession.Status = 6;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }


                                }
                                else if (_trnDomainMapping != null && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId == null)
                                {
                                    /*Get UserId from ProfileTable (Based on Input ArmyNo with token authorise.) and Update in TrnDomainMapping Table*/
                                    dTOTempSession.NewUser = false;
                                    dTOTempSession.AdminFlag = _trnDomainMapping.ApplicationUser.AdminFlag;
                                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                                    dTOTempSession.RoleName = model.Role;
                                    dTOTempSession.TDMId = _trnDomainMapping.Id;
                                    dTOTempSession.TDMUnitMapId = _trnDomainMapping.UnitId;
                                    dTOTempSession.TDMApptId = _trnDomainMapping.ApptId;
                                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;
                                    dTOTempSession.IsIO = _trnDomainMapping.IsIO;
                                    dTOTempSession.IsCO = _trnDomainMapping.IsCO;
                                    dTOTempSession.IsRO = _trnDomainMapping.IsRO;
                                    dTOTempSession.IsORO = _trnDomainMapping.IsORO;
                                    dTOTempSession.DialingCode = _trnDomainMapping.DialingCode;
                                    dTOTempSession.Extension = _trnDomainMapping.Extension;
                                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;
                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 4;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }
                                    else
                                    {
                                        TempData["error"] = "Role not authorized.";
                                        dTOTempSession.Status = 6;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }

                                }
                                else if (_trnDomainMapping != null && _trnDomainMapping.Id == 0)
                                {
                                    /*Create TrnDomainMapping using AspnetUserId,UnitId,UserId from Profile Table.*/
                                    dTOTempSession.NewUser = false;
                                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                                    dTOTempSession.RoleName = model.Role;
                                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;

                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 3;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }
                                    else
                                    {
                                        TempData["error"] = "Role not authorized.";
                                        dTOTempSession.Status = 6;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }

                                }
                                else if (_trnDomainMapping != null && _trnDomainMapping.ApplicationUser.AdminFlag == false && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId != null)
                                {
                                    dTOTempSession.NewUser = false;
                                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                                    dTOTempSession.RoleName = model.Role;
                                    dTOTempSession.ICNO = _trnDomainMapping.MUserProfile.ArmyNo;
                                    dTOTempSession.Name = _trnDomainMapping.MUserProfile.Name;
                                    dTOTempSession.UserId = _trnDomainMapping.MUserProfile.UserId;
                                    dTOTempSession.TDMId = _trnDomainMapping.Id;
                                    dTOTempSession.TDMUnitMapId = _trnDomainMapping.UnitId;
                                    dTOTempSession.TDMApptId = _trnDomainMapping.ApptId;
                                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;
                                    dTOTempSession.IsIO = _trnDomainMapping.IsIO;
                                    dTOTempSession.IsCO = _trnDomainMapping.IsCO;
                                    dTOTempSession.IsRO = _trnDomainMapping.IsRO;
                                    dTOTempSession.IsORO = _trnDomainMapping.IsORO;
                                    dTOTempSession.DialingCode = _trnDomainMapping.DialingCode;
                                    dTOTempSession.Extension = _trnDomainMapping.Extension;
                                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;
                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 1;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval..<br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence. <br/>Contact Admin.";
                                        if (_trnDomainMapping.ApplicationUser.AdminMsg != null)
                                        {
                                            TempData["error"] = _trnDomainMapping.ApplicationUser.AdminMsg;
                                        }
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }
                                    else
                                    {
                                        TempData["error"] = "Role not authorized.";
                                        dTOTempSession.Status = 6;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }

                                }
                                else if (_trnDomainMapping == null)
                                {
                                    /*Create DomainId in AspNetUser Table , Assign Role.,Create Mapping with add profile id.*/
                                    dTOTempSession.NewUser = true;
                                    dTOTempSession.DomainId = model.DomainId;
                                    dTOTempSession.RoleName = model.Role;
                                    dTOTempSession.Status = 2;
                                    SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);


                                    return RedirectToAction("TokenValidate", "Account");
                                    // return RedirectToAction("UnAuthUser", "Account");
                                }

                            }
                        }

                        else
                        {

                            Response.Redirect("https://iam2.army.mil/IAM/User", true);
                        }
                    }
                    else
                    {

                        Response.Redirect("https://iam2.army.mil/IAM/User", true);
                    }
                }
                else
                {

                    Response.Redirect("https://iam2.army.mil/IAM/User", true);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("https://iam2.army.mil/IAM/User", true);
            }
            return RedirectToAction("IMLoginSelf", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TokenValidate_()  //__ForIAM
        {
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;

            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "Token");
            List<string> RoleNameList = new List<string>() { "user" };


            if (userid == 0)
            {
                DTOTempSession? dTOTempSession1 = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");

                ViewBag.AppName = dTOTempSession1.AppName;
                ViewBag.DomainId = dTOTempSession1.DomainId;
                ViewBag.RoleName = dTOTempSession1.RoleName;

                if (dTOTempSession1 != null)
                {
                    if (dTOTempSession1.Status == 1)
                    {
                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "You are not authorized to this page.";
                    return View();
                }
            }
            else
            {
                if (dTOTempSession != null)
                {
                    ViewBag.AppName = dTOTempSession.AppName;
                    ViewBag.DomainId = dTOTempSession.DomainId;
                    ViewBag.RoleName = dTOTempSession.RoleName;

                    if (RoleNameList.Contains(dTOTempSession.RoleName))
                    {
                        return RedirectToActionPermanent("Index", "Home");
                    }
                    else if (dTOTempSession.RoleName == "admin")
                    {
                        return RedirectToActionPermanent("DashboardMaster", "Master");
                    }
                    else if (dTOTempSession.RoleName == "super admin")
                    {
                        return RedirectToActionPermanent("Index", "Account");
                    }
                    return View();
                }
                else
                {
                    return View();
                }

            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> TokenValidate_(DTOTokenRequestForIAM model)  //__ForIAM
        {
            try
            {
                string? Footer = _configuration["Footer:Test"];
                ViewBag.Footer = Footer;

                DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");
                List<string> RoleNameList = new List<string>() { "user" };
                if (dTOTempSession != null)
                {
                    model.ICNo = model.ICNo.Trim();
                    model.Password = "Admin123#";


                    if (ModelState.IsValid)
                    {
                        if (dTOTempSession.Status == 5 && dTOTempSession.ICNO == model.ICNo)
                        {
                            var usera = await userManager.FindByIdAsync(dTOTempSession.AspNetUsersId.ToString());

                            HttpContext.Session.Remove("Token");
                            await signInManager.SignOutAsync();
                            await userManager.UpdateSecurityStampAsync(usera);
                            if (usera != null)
                            {
                                //default Password - Admin123#
                                var result = await signInManager.PasswordSignInAsync(usera.UserName, model.Password, false, lockoutOnFailure: true);
                                // var rolelist = await signInManager.UserManager.GetRolesAsync(usera);
                                //var user1 = await signInManager.UserManager.IsInRoleAsync(usera, "User");
                                if (result.Succeeded)
                                {
                                    //var army = await _userProfileBL.Get(Convert.ToInt32(dTOTempSession.UserId));

                                    //  await userManager.RemoveClaimAsync(usera, new Claim("Roles", dTOTempSession.RoleName));
                                    //  await userManager.AddClaimAsync(usera, new Claim("Roles", dTOTempSession.RoleName));

                                    DtoSession dtoSession = new DtoSession();
                                    dtoSession.ICNO = dTOTempSession.ICNO;
                                    dtoSession.RoleName = dTOTempSession.RoleName.Trim();
                                    dtoSession.UserId = dTOTempSession.UserId;
                                    dtoSession.UnitId = dTOTempSession.TDMUnitMapId;
                                    dtoSession.Name = dTOTempSession.Name.ToUpper();
                                    dtoSession.RankName = dTOTempSession.RankAbbreviation.ToUpper();
                                    dtoSession.TrnDomainMappingId = dTOTempSession.TDMId;
                                    dtoSession.RoleName = dTOTempSession.RoleName;
                                    dtoSession.DoaminId = dTOTempSession.DomainId;
                                    ///////////////login log//////////////////////
                                    TrnLogin_Log log = new TrnLogin_Log();
                                    log.AspNetUsersId = Convert.ToInt32(usera.Id);
                                    var Role = await roleManager.FindByNameAsync(dTOTempSession.RoleName);
                                    log.RoleId = Convert.ToInt32(Role.Id);
                                    log.UserId = Convert.ToInt32(dTOTempSession.UserId);
                                    log.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                                    log.IsActive = true;
                                    log.Updatedby = Convert.ToInt32(usera.Id);
                                    log.UpdatedOn = DateTime.Now;
                                    await _TrnLoginLogBL.Add(log);
                                    ////////////////End Log////////////////////////

                                    SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);



                                    if (RoleNameList.Contains(dTOTempSession.RoleName))
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        return RedirectToActionPermanent("Index", "Home");
                                    }
                                    else if (dTOTempSession.RoleName.ToUpper() == "ADMIN")
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        return RedirectToActionPermanent("DashboardMaster", "Master");
                                    }
                                    else if (dTOTempSession.RoleName == "Super Admin")
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        return RedirectToActionPermanent("Index", "Account");
                                    }
                                }
                                else if (result.IsLockedOut)
                                {
                                    TempData["error"] = "Account Locked Out Please Try after 10 minutes.";
                                    goto End;
                                }
                                else if (result.IsNotAllowed)
                                {
                                    TempData["error"] = "Already Login " + usera.UserName + " Please Try Some Time";
                                    goto End;
                                }
                                else
                                {

                                    TempData["error"] = "Not Valid User / Password. Access Failed Count " + usera.AccessFailedCount + " Max Access Attempts 3";
                                    goto End;
                                }
                            }

                        }
                        else
                        {
                            DTOAllRelatedDataByArmyNoResponse? _dTOProfileResponse = await _userProfileBL.GetAllRelatedDataByArmyNo(model.ICNo);
                            if (dTOTempSession.Status == 1)
                            {
                                //TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. <br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/> Contact Admin.";
                                if (_dTOProfileResponse != null && _dTOProfileResponse.AdminMsg != null)
                                {
                                    TempData["error"] = _dTOProfileResponse.AdminMsg;
                                }
                                return View();
                            }
                            else if (dTOTempSession.Status == 6)
                            {
                                TempData["error"] = "Role not authorized.";
                                return View();
                            }
                            else if (dTOTempSession.Status == 5 && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId > 0 && model.ICNo != dTOTempSession.ICNO)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.ICNoDomainId = _dTOProfileResponse.DomainId;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNoTDMUnitMapId = _dTOProfileResponse.UnitId;
                                dTOTempSession.ICNoTDMId = _dTOProfileResponse.TrnDomainMappingId;
                                dTOTempSession.ICNoTDMApptId = _dTOProfileResponse.ApptId;
                                //TempData["error"] = "Not Authorized to access the current profile because Domain Id - " + dTOTempSession.DomainId + " is presently mapped to Profile Id - " + dTOTempSession.UserId + " ( IC No- " + dTOTempSession.ICNO + ") .<br/>Pl change Token and try again!";
                                TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId > 0)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNoDomainId = _dTOProfileResponse.DomainId;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNoTDMUnitMapId = _dTOProfileResponse.UnitId;
                                dTOTempSession.ICNoTDMId = _dTOProfileResponse.TrnDomainMappingId;
                                dTOTempSession.ICNoTDMApptId = _dTOProfileResponse.ApptId;

                                if (dTOTempSession.Status == 2)
                                    //TempData["error"] = "Your Profile Id -" + _dTOProfileResponse.UserId + " is mapped to Domain Id - " + _dTOProfileResponse.DomainId + " in Sys.<br/>Pl get yourself relieved first    and try again.";
                                    TempData["error"] = "Invalid Army No / Password.";
                                else if (dTOTempSession.Status == 3)
                                    //TempData["error"] = "Your Profile Id - " + _dTOProfileResponse.UserId + " is already mapped to Domain Id -" + _dTOProfileResponse.DomainId + ".<br/>Pl get yourself relieved first..Domain Id - " + dTOTempSession.DomainId + "(regd) is not mapped to any profile.";
                                    TempData["error"] = "Invalid Army No / Password.";
                                else if (dTOTempSession.Status == 4)
                                    //TempData["error"] = "You are presently mapped to Domain Id -" + _dTOProfileResponse.DomainId + ".<br/>Pl relieve yourself and get your profile mapped to new domain ID - " + dTOTempSession.DomainId + ".";
                                    TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId == 0)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNO = _dTOProfileResponse.ArmyNo;
                                dTOTempSession.UserId = _dTOProfileResponse.UserId;
                                SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                return RedirectToActionPermanent("Profile", "Account");
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse == null)
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNO = model.ICNo;
                                SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                return RedirectToActionPermanent("Profile", "Account");
                            }
                            else if (dTOTempSession.Status == 5 && dTOTempSession.ICNO != model.ICNo)
                            {
                                //TempData["error"] = "Not Authorized to access the current profile because Domain Id - " + dTOTempSession.DomainId + " is presently mapped to Profile Id - " + dTOTempSession.UserId + " ( IC No " + dTOTempSession.ICNO + ") .<br/>Pl change Token and try again!";
                                TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                        }
                    }
                    else
                    {
                        var error = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        TempData["error"] = error[0][0].ErrorMessage;
                        goto End;
                    }
                }
                else
                {
                    TempData["error"] = "You are not authorized this page.";
                    goto End;
                }
            End:
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
        }

        public async Task<ActionResult> FinalLogout()
        {
            HttpContext.Session.Remove("Token");
            await signInManager.SignOutAsync();
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> IMLogout()
        {
            // if(HttpContext.Request.Query.Count()>0)
            // {
            string? SAMLRequest = HttpContext.Request.Query["SAMLRequest"];
            string? SAMLResponse = HttpContext.Request.Query["SAMLResponse"];
            // }

            //string ss = Convert.ToString(HttpContext.Request.QueryString);

            var dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            if (SAMLResponse != null && !string.IsNullOrEmpty(Convert.ToString(SAMLResponse)))
            {
                // Response.Redirect("https://localhost:7023/Account/FinalLogout");
                Response.Redirect("https://eisac.army.mil/Account/FinalLogout");
            }
            else if (SAMLRequest != null && !string.IsNullOrEmpty(Convert.ToString(SAMLRequest)))
            {
                string EncryptedResponse = Convert.ToString(SAMLRequest);
                if (!string.IsNullOrEmpty(EncryptedResponse))
                {
                    AccountSettings accountSettings = new AccountSettings();
                    OneLogin.Saml.Response samlResponse = new OneLogin.Saml.Response(accountSettings);

                    string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\eisac.army.mil.pfx", "Abc@2022");
                    samlResponse.LoadXmlFromBase64(decryptedsamlresponse);



                    string nameid = string.Empty;
                    string issuer = string.Empty;
                    samlResponse.GetLogoutParameter(out nameid, out issuer);
                    HttpContext.Session.Remove("Token");
                    await signInManager.SignOutAsync();
                    try
                    {
                        //SendResponseToIAM("https://localhost:7023/Account/FinalLogout", accountSettings.entityId, nameid);
                        SendResponseToIAM("https://eisac.army.mil/Account/FinalLogout", accountSettings.entityId, nameid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(1001, ex, "Account->IMLogout");
                    }
                }
            }

            else
            {
                AccountSettings acs = new AccountSettings();

                string NameId = dtoSession.DoaminId;
                string userRole = dtoSession.RoleName;

                LogoutRequesttoIAM(userRole, acs.entityId, NameId);
            }

            if (SAMLRequest == null && SAMLResponse == null)
            {
                AccountSettings acs = new AccountSettings();
                string NameId = dtoSession.DoaminId;
                string userRole = dtoSession.RoleName; ;


                //HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);



                LogoutRequesttoIAM(userRole, acs.entityId, NameId);
            }
            return View();
        }
        public IActionResult UnAuthUser()
        {
            return View();
        }
        [AllowAnonymous]
        public string DecryptSAmlResponseNew(string Encryptedtext, string certificatepath, string password)
        {

            string result = "True";
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("alpha");

                string[] spearator = { Convert.ToBase64String(plainTextBytes) };

                // using the method
                string[] newstring = Encryptedtext.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                //string[] newstring = encryptedvalue.Split();
                string key = newstring[1].ToString();
                string plain = newstring[0].ToString();
                #region decryptkeyusingprivatekey
                try
                {
                    byte[] byteData = Convert.FromBase64String(key);
                    //   byte[] decryptedkey = new byte[16];
                    byte[] decryptedkey = new byte[32];
                    X509Certificate2 myCert2 = null;
                    RSACryptoServiceProvider rsa = null;

                    try
                    {
                        myCert2 = new X509Certificate2(@"C:\\Cert\\App Certificate\\eisac.army.mil.pfx", "Abc@2022");
                        // rsa = (RSACryptoServiceProvider)myCert2.PrivateKey;
                        #region test
                        using (RSA rs = myCert2.GetRSAPrivateKey())
                        {
                            // rs.KeySize = 16;
                            decryptedkey = rs.Decrypt(byteData, RSAEncryptionPadding.Pkcs1);

                        }
                        #endregion
                    }
                    catch (Exception e)
                    {

                    }
                    // byte[] iv = new byte[16];
                    byte[] iv = new byte[32];


                    byte[] iv1 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };


                    // result = DecryptString0705222_Final(plain, rsa.Decrypt(byteData, RSAEncryptionPadding.Pkcs1), iv1);
                    result = DecryptString0705222_Final(plain, decryptedkey, iv1);
                }
                catch (Exception exxx)
                {
                    result = exxx.Message;
                }
                #endregion

            }
            catch (Exception exx)
            {
                result = exx.Message;
            }

            return result;
        }


        [AllowAnonymous]
        public void SendResponseToIAM(string issueurl, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();

            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            //string ReuestXML = req.GetRequest(AuthRequest.AuthRequestFormat.Base64);
            //string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");
            string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");

            //Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);
            Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);

        }
        [AllowAnonymous]
        public void LogoutRequesttoIAM(string role, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();
            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            string ReuestXML = req.SingleLogoutRequest(AuthRequest.AuthRequestFormat.Base64, entityid, role, usernam);
            //Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);
            Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);
        }

        [AllowAnonymous]
        private string DecryptString0705222_Final(string cipherText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.ECB;

            // Set key and IV
            //  byte[] aesKey = new byte[16];
            byte[] aesKey = new byte[32];
            //Array.Copy(key, 0, aesKey, 0, 16);
            Array.Copy(key, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = iv;
            encryptor.Padding = PaddingMode.PKCS7;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = string.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            catch (Exception exx)
            {

            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;

        }
        #endregion IAM Code

        #region Claims
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Claims()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllClaims(DTODataTablesRequest dTO)
        {
            try
            {
                return Json(await _iAccountBL.GetAllClaimsOrderBy(dTO));
            }
            catch (Exception ex)
            {
                List<DTOClaimsStoreResponse> dTOClaimsStoreResponses = new List<DTOClaimsStoreResponse>();
                var responseData = new DTODataTablesResponse<DTOClaimsStoreResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOClaimsStoreResponses
                };
                _logger.LogError(1001, ex, "Account->Claims");
                return Json(responseData);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost()]
        public async Task<IActionResult> GetAllUsersByClaim(DTODataTablesRequest dTO)
        {
            try
            {
                return Json(await _iAccountBL.GetAllUsersByClaim(dTO));
            }
            catch (Exception ex)
            {
                List<DTOUsersByClaim> dTOClaimsStoreResponses = new List<DTOUsersByClaim>();
                var responseData = new DTODataTablesResponse<DTOUsersByClaim>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOClaimsStoreResponses
                };
                _logger.LogError(1001, ex, "Account->UsersByClaim");
                return Json(responseData);
            }
        }
        #endregion Claims
    }
}
