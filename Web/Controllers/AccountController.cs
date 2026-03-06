using AutoMapper;
using BusinessLogicsLayer;
using BusinessLogicsLayer.Account;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.IAMSetting;   
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Service;
using BusinessLogicsLayer.TrnLoginLog;
using BusinessLogicsLayer.Unit;
using DataAccessLayer;
using DataTransferObject.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OneLogin.Saml;
using System.Configuration;
using System.Data;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Web.Validation;
using Web.WebHelpers;
using ApplicationRole = DataTransferObject.Domain.Identitytable.ApplicationRole;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAccountBL _iAccountBL;
        public readonly IDomainMapBL _iDomainMapBL;
        private readonly IUserProfileBL _userProfileBL;
        public readonly IMapUnitBL _IMapUnitBL;
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
        private static readonly Regex armyNoRegex = new Regex(@"^[A-Z]{2}\d{5,6}[A-Z]$", RegexOptions.IgnoreCase);

        public AccountController(IConfiguration configuration,IUnitOfWork unitOfWork,IUnitBL unitBL, IAccountBL iAccountBL , IDomainMapBL iDomainMapBL, IUserProfileBL userProfileBL, IMapUnitBL mapUnitBL, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ApplicationDbContext contextTransaction,
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
        [IgnoreAntiforgeryToken]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Domain Regn.

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
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllClaimsForDD()
        {
            try
            {
                return Json(await _iAccountBL.GetAllClaims());// Get All Claims
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Account->GetAllClaimsForDD");
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

        #region UserRegn/GetAllUserRegn/GetDataForDataTable/SaveMapping/UpdateDomainFlag

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
            IAMSetting iAMSetting = await _iAMSettingBL.GetByByte(id); // Get IAM Setting by Environment

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

                DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "Token"); // Get Session Object
                DTOTempSession? dTOTempSession1 = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData"); // Get Session Object
                if (dTOTempSession != null)
                {
                    HttpContext.Session.Remove("Token");
                }

                if (dTOTempSession1 != null)
                {
                    HttpContext.Session.Remove("IMData");
                }
                string? dd = HttpContext.Session.GetString(SessionKeySalt); // Get Salt from Session
                if (dd != null)
                {
                    HttpContext.Session.Remove(SessionKeySalt);
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
        /// If any validation fails, the method sets the appropriate session status and error message.
        /// </returns>
        /// <remarks>
        /// This method evaluates several conditions based on the <paramref name="model"/> (such as role and domain ID):
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///         If the user has an active domain mapping and a valid role, they are redirected to the <see cref="TokenValidate"/> action.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Status 1: AdminFlag is false in the AspNetUsers table, so an error message (AdminMsg from the AspNetUsers table) is displayed.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Status 2: Create a DomainId in the AspNetUsers table, assign a role, create a mapping with the profile ID, and redirect the user for further validation.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Status 3: If no domain mapping is found, a TrnDomainMapping record is created using AspNetUserId, UnitId, and UserId from the Profile table; status is set to 3, and the user is redirected for further validation.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Status 4: If the UserId is null in the TrnDomainMapping table (based on the input ArmyNo with token authorization), the status is set to 4 and the TrnDomainMapping table is updated.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Status 5: If the DomainId exists in the AspNetUsers table and the domain mapping exists in the TrnDomainMapping table with a non-null UserId, the status is set to 5 and the user is redirected for further validation.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Status 6: If the user is not mapped or their role is invalid, the status is set to 6, and an error message is returned.
        ///         </description>
        ///     </item>
        /// </list>
        /// Session variables are used to store important data such as user ID, domain ID, and mapping status, which are then used in subsequent actions.
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
                TrnDomainMapping? _trnDomainMapping = await _iDomainMapBL.GetAllRelatedDataByDomainId(model.DomainId,model.Role); // Get Domain Mapping by DomainId and Role
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
                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;


                    if (_trnDomainMapping.Role !=null)
                    {
                        dTOTempSession.Status = 5;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        HttpContext.Session.CommitAsync().Wait(); // Force write session
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }
                    else
                    {
                        TempData["error"] = "Role not authorized.";
                        dTOTempSession.Status = 6;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        HttpContext.Session.CommitAsync().Wait(); // Force write session
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
                        HttpContext.Session.CommitAsync().Wait(); // Force write session
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
                        HttpContext.Session.CommitAsync().Wait(); // Force write session
                        return RedirectToActionPermanent("TokenValidate", "Account");
                    }
                    else
                    {
                        TempData["error"] = "Role not authorized.";
                        dTOTempSession.Status = 6;
                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                        HttpContext.Session.CommitAsync().Wait(); // Force write session
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
                        HttpContext.Session.CommitAsync().Wait(); // Force write session
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
                    HttpContext.Session.CommitAsync().Wait(); // Force write session
                    return RedirectToActionPermanent("TokenValidate", "Account");
                }

            }
            return View(model);
        }


        /// <summary>
        /// Validates the user's token and manages role-based access.
        /// </summary>
        /// <remarks>
        /// Retrieves a cryptographic salt for session security, checks the authenticated user's ID,
        /// and fetches session data to determine authorization.  
        /// Unauthenticated users are checked for temporary IM session data ("IMData").  
        /// Authenticated users are redirected based on their role:
        /// "user" → Home/Index, "admin" → Master/DashboardMaster.  
        /// ViewBag and session variables store temporary security information.
        /// </remarks>
        /// <returns>
        /// Returns the token validation view if access is not redirected,
        /// or a redirect action result based on user role.
        /// </returns>
        [HttpGet]
        [AnySessionRequired]
        [AllowAnonymous]
        public IActionResult TokenValidate()
        {
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;


            string dd = AESEncrytDecry.GetSalt();
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
                    return View();
                }
                else
                {
                    return View();
                }

            }

        }


        /// <summary>
        /// Validates the user's token and performs login.
        /// </summary>
        /// <param name="model">The <see cref="DTOTokenRequest"/> object containing ICNo and Password.</param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/>:
        /// <list type="bullet">
        /// <item>
        /// <description>Redirects user to <c>Home/Index</c> for normal users</description>
        /// </item>
        /// <item>
        /// <description>Redirects user to <c>Master/DashboardMaster</c> for admin users</description>
        /// </item>
        /// <item>
        /// <description>Returns the same view with <c>TempData["error"]</c> message if validation fails or user is unauthorized</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The method performs the following operations:
        /// <list type="number">
        /// <item>
        /// <description>Retrieves footer information and assigns it to <c>ViewBag.Footer</c></description>
        /// </item>
        /// <item>
        /// <description>Decrypts password from session salt if available</description>
        /// </item>
        /// <item>
        /// <description>Retrieves temporary session (<c>IMData</c>) to verify token status</description>
        /// </item>
        /// <item>
        /// <description>Checks token status (1–6) and validates credentials accordingly</description>
        /// </item>
        /// <item>
        /// <description>Signs in the user via <c>SignInManager.PasswordSignInAsync</c></description>
        /// </item>
        /// <item>
        /// <description>Logs login attempts to <c>TrnLogin_Log</c> table</description>
        /// </item>
        /// <item>
        /// <description>Removes session objects when login is complete</description>
        /// </item>
        /// <item>
        /// <description>Handles exceptions by logging them and redirecting to <c>Error/Error</c> page</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="Exception">Catches all exceptions and logs using <c>_logger.LogError</c></exception>
        [HttpPost]
        [AnySessionRequired]
        [AllowAnonymous]
        public async Task<IActionResult> TokenValidate(DTOTokenRequest model)
        {
            try
            {
                string? Footer = _configuration["Footer:Test"];
                ViewBag.Footer = Footer;

                string? dd = HttpContext.Session.GetString(SessionKeySalt); // Get Salt from Session
                if (dd != null)
                {
                    ViewBag.hdns = dd;
                    string Password = AESEncrytDecry.DecryptAES(model.Password, dd);  //decrypt password
                    string ICNo = AESEncrytDecry.DecryptAES(model.ICNo, dd);  //decrypt ICNo
                    model.ICNo = ICNo;
                    model.Password = Password;

                    if (string.IsNullOrWhiteSpace(model.ICNo))
                    {
                        ModelState.AddModelError("ICNo", "Army No is required.");
                        goto End;
                    }
                    if (model.ICNo.Length < 8 || model.ICNo.Length > 9)
                    {
                        ModelState.AddModelError("ICNo", "Invalid Army No.");
                        goto End;
                    }

                    if (!armyNoRegex.IsMatch(model.ICNo))
                    {
                        ModelState.AddModelError("ICNo", "Invalid Army No.");
                        goto End;
                    }

                }

                DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData"); // Get Session Object
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

                            // 1) Kill any existing auth + session first (old session id)

                            HttpContext.Session.Remove("Token"); // Remove Session Object
                            await signInManager.SignOutAsync(); // Sign out any existing user
                            await userManager.UpdateSecurityStampAsync(usera); // Update security stamp to invalidate old tokens

                            if (usera != null)
                            {
                                var result = await signInManager.PasswordSignInAsync(usera.UserName, model.Password, false, lockoutOnFailure: true); // Sign in user
                                if (result.Succeeded)
                                {
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

                                    SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession); // Set session object



                                    if (RoleNameList.Contains(dTOTempSession.RoleName))
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        HttpContext.Session.Remove(SessionKeySalt);
                                        return RedirectToActionPermanent("Index", "Home");
                                    }
                                    else if (dTOTempSession.RoleName.ToUpper() == "ADMIN")
                                    {
                                        HttpContext.Session.Remove("IMData");
                                        HttpContext.Session.Remove(SessionKeySalt);
                                        return RedirectToActionPermanent("DashboardMaster", "Master");
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
                            DTOAllRelatedDataByArmyNoResponse? _dTOProfileResponse = await _userProfileBL.GetAllRelatedDataByArmyNo(model.ICNo); // Get Profile by ArmyNo
                            if (dTOTempSession.Status == 1)
                            {
                                //TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. <br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/> Contact Admin.";
                                if (_dTOProfileResponse != null && _dTOProfileResponse.AdminMsg != null)
                                {
                                    TempData["error"] = _dTOProfileResponse.AdminMsg;
                                }
                                return View();
                            }
                            else if (dTOTempSession.Status == 6) // Not mapped or Invalid Role
                            {
                                TempData["error"] = "Role not authorized.";
                                return View();
                            }
                            else if (dTOTempSession.Status == 5 && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId > 0 && model.ICNo != dTOTempSession.ICNO) // DomainId mapped with other Profile
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
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId > 0) // DomainId mapped with other Profile
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNoDomainId = _dTOProfileResponse.DomainId;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNoTDMUnitMapId = _dTOProfileResponse.UnitId;
                                dTOTempSession.ICNoTDMId = _dTOProfileResponse.TrnDomainMappingId;
                                dTOTempSession.ICNoTDMApptId = _dTOProfileResponse.ApptId;

                                if (dTOTempSession.Status == 2) // New DomainId mapped with other Profile
                                    //TempData["error"] = "Your Profile Id -" + _dTOProfileResponse.UserId + " is mapped to Domain Id - " + _dTOProfileResponse.DomainId + " in Sys.<br/>Pl get yourself relieved first    and try again.";
                                    TempData["error"] = "Invalid Army No / Password.";
                                else if (dTOTempSession.Status == 3) // Existing DomainId mapped with other Profile
                                    //TempData["error"] = "Your Profile Id - " + _dTOProfileResponse.UserId + " is already mapped to Domain Id -" + _dTOProfileResponse.DomainId + ".<br/>Pl get yourself relieved first..Domain Id - " + dTOTempSession.DomainId + "(regd) is not mapped to any profile.";
                                    TempData["error"] = "Invalid Army No / Password.";
                                else if (dTOTempSession.Status == 4) // Existing DomainId mapped with other Profile
                                    //TempData["error"] = "You are presently mapped to Domain Id -" + _dTOProfileResponse.DomainId + ".<br/>Pl relieve yourself and get your profile mapped to new domain ID - " + dTOTempSession.DomainId + ".";
                                    TempData["error"] = "Invalid Army No / Password.";
                                goto End;
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse != null && _dTOProfileResponse.TrnDomainMappingId == 0) // Valid Case
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNoUserId = _dTOProfileResponse.UserId;
                                dTOTempSession.ICNO = _dTOProfileResponse.ArmyNo;
                                dTOTempSession.UserId = _dTOProfileResponse.UserId;
                                SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                HttpContext.Session.CommitAsync().Wait(); // Force write session
                                return RedirectToActionPermanent("Profile", "Account");
                            }
                            else if ((dTOTempSession.Status == 2 || dTOTempSession.Status == 3 || dTOTempSession.Status == 4) && _dTOProfileResponse == null) // Valid Case
                            {
                                dTOTempSession.ICNOInput = model.ICNo;
                                dTOTempSession.Password = model.Password;
                                dTOTempSession.ICNO = model.ICNo;
                                SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                HttpContext.Session.CommitAsync().Wait(); // Force write session
                                return RedirectToActionPermanent("Profile", "Account");
                            }
                            else if (dTOTempSession.Status == 5 && dTOTempSession.ICNO != model.ICNo) // DomainId mapped with other Profile
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


        /// <summary>
        /// Displays the profile page for the currently logged-in user based on session data.
        /// </summary>
        /// <remarks>
        /// - Retrieves temporary session data using <see cref="SessionHeplers.GetObject{T}"/>.
        /// - Sets footer information from configuration.
        /// - Handles various session statuses:
        ///   - Status 1: Registration request placed, shows message and returns view.
        ///   - Status 4: Loads domain mapping data into DTOProfileAndMappingRequest.
        ///   - Status 6: Role not authorized, shows error message.
        /// - Populates ViewBag with rank and armed type options.
        /// - If session contains a valid UserId, fetches user profile data from database and populates DTOProfileAndMappingRequest.
        /// - Handles session timeout or missing session data by redirecting to TokenValidate action.
        /// </remarks>
        /// <returns>
        /// Returns a <see cref="ViewResult"/> displaying the profile page with <see cref="DTOProfileAndMappingRequest"/> model, 
        /// or a redirect to "TokenValidate" if session is invalid or unauthorized.
        /// </returns>
        [HttpGet]
        [AnySessionRequired]
        [AllowAnonymous]
        public async Task<IActionResult> Profile()
        {
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;

            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData"); // Get Session Object
            if (dTOTempSession != null)
            {
                DTOProfileAndMappingRequest dTOProfileAndMappingRequest = new DTOProfileAndMappingRequest();
                if (dTOTempSession.Status == 1) // Registration request placed
                {
                    TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/>Contact Admin.";
                    return View();
                }
                else if(dTOTempSession.Status == 6) // Not mapped or Invalid Role
                {
                    TempData["error"] = "Role not authorized.";
                    return View();
                }
                else
                {
                    if (dTOTempSession.Status == 4) // Existing DomainId mapped with other Profile
                    {
                        dTOProfileAndMappingRequest.TDMId = dTOTempSession.TDMId;
                        dTOProfileAndMappingRequest.ApptId = dTOTempSession.TDMApptId;
                        dTOProfileAndMappingRequest.UnitMapId = dTOTempSession.TDMUnitMapId;
                        //dTOProfileAndMappingRequest.IsRO = dTOTempSession.IsRO;
                        dTOProfileAndMappingRequest.IsCO = dTOTempSession.IsCO;
                        dTOProfileAndMappingRequest.IsIO = dTOTempSession.IsIO;
                        //dTOProfileAndMappingRequest.IsORO = dTOTempSession.IsORO;
                    }
                    ViewBag.OptionsRank = service.GetRank(1); // Get Officer Ranks only
                    ViewBag.OptionsArmedType = service.GetArmedType(); // Get Armed Types

                    if (dTOTempSession.UserId > 0) // Valid UserId
                    {
                        try
                        {

                            //Get ArmyNo from UserProfile Table
                            MUserProfile mUserProfile = await _userProfileBL.Get(dTOTempSession.UserId); // Get User Profile by UserId

                            dTOProfileAndMappingRequest.UserId = mUserProfile.UserId;
                            dTOProfileAndMappingRequest.ArmyNo = mUserProfile.ArmyNo;
                            dTOProfileAndMappingRequest.RankId = mUserProfile.RankId;
                            dTOProfileAndMappingRequest.Name = mUserProfile.Name;
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


        /// <summary>
        /// Handles the submission of the user profile and domain mapping form.
        /// Saves or updates profile and mapping information in the database, 
        /// manages token waiver requests, updates session data, and sets 
        /// success or error messages accordingly.
        /// </summary>
        /// <param name="model">An instance of <see cref="DTOProfileAndMappingRequest"/> containing profile and mapping data submitted by the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the form submission:
        /// - Redirects to "TokenValidate" action on success or failure.
        /// - Returns the view with validation errors if model state is invalid.
        /// </returns>
        /// <remarks>
        /// - Updates session object <see cref="DTOTempSession"/> with new profile mapping information.
        /// - Handles different user statuses (Status 2, 3, 4) to determine workflow and messaging.
        /// - Ensures "ReasonTokenWaiver" is provided if token waiver is requested.
        /// - Logs errors in case of exceptions during processing.
        /// </remarks>
        [HttpPost]
        [AnySessionRequired]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(DTOProfileAndMappingRequest model)
        {
            string? Footer = _configuration["Footer:Test"]; // Get Footer from config
            ViewBag.Footer = Footer;

            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData"); // Get Session Object
            model.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            if (dTOTempSession != null) // Valid Session
            {
                if(model.IsTokenWaiver==true) // Token Waiver requested
                {
                    if(model.ReasonTokenWaiver == null) // Reason for Token Waiver is required
                    {
                        ModelState.AddModelError("ReasonTokenWaiver", "Reason for IACA Token Waiver is required."); // Add Model Error
                    }
                }
                if (ModelState.IsValid) // Valid Model State
                {
                    DTOTempSession? resultfinal = await _iAccountBL.ProfileAndMappingSaving(model, dTOTempSession); // Save or Update Profile and Mapping
                    if (dTOTempSession.Status == 2) // New DomainId mapped with other Profile
                    {
                        if(resultfinal!=null) // Successfully saved
                        {
                            dTOTempSession.AspNetUsersId = resultfinal.AspNetUsersId;
                            dTOTempSession.TDMId = resultfinal.TDMId;
                            dTOTempSession.TDMUnitMapId = resultfinal.TDMUnitMapId;
                            dTOTempSession.UserId = resultfinal.UserId;
                            dTOTempSession.Status = 1;
                            dTOTempSession.IsToken = true;


                            SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession); // Update Session Object
                            HttpContext.Session.CommitAsync().Wait(); // Force write session
                            TempData["success"] = "Domian Id - " + dTOTempSession.DomainId + " & Profile Id- " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval.. <br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence.<br/>Contact Admin or try login after 24 Hrs.";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                        else
                        {
                            TempData["error"] = "Your regn request was not placed.<br/>Contact Admin.";
                            return RedirectToActionPermanent("TokenValidate", "Account");
                        }
                    }
                    else if (dTOTempSession.Status == 3) // Existing DomainId mapped with other Profile
                    {
                        
                        if (resultfinal != null) // Successfully saved
                        {
                            dTOTempSession.TDMId = resultfinal.TDMId;
                            dTOTempSession.TDMUnitMapId = resultfinal.TDMUnitMapId;
                            dTOTempSession.UserId = resultfinal.UserId;
                            dTOTempSession.Status = 1;
                            dTOTempSession.IsToken = true;

                            SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession); // Update Session Object
                            if (model.IsTokenWaiver == true) // Token Waiver requested
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
                    else if (dTOTempSession.Status == 4) // Existing DomainId mapped with other Profile
                    {
                        if (resultfinal != null)
                        {
                            dTOTempSession.Status = 1;
                            dTOTempSession.IsToken = true;
                            dTOTempSession.UserId = resultfinal.UserId;
                            SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession); // Update Session Object
                            if (model.IsTokenWaiver == true) // Token Waiver requested
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


        /// <summary>
        /// Handles the saving of a unit along with its mapping information.
        /// Validates the input model, checks if the unit is already mapped or verified, 
        /// and then saves the mapping through the account business layer.
        /// Returns JSON responses indicating success, existence, verification status, 
        /// or validation errors.
        /// </summary>
        /// <param name="dTO">An instance of <see cref="DTOSaveUnitWithMappingRequest"/> containing the unit and mapping details to be saved.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating:
        /// - <c>KeyConstants.Exists</c> if the unit is already mapped.
        /// - <c>5</c> if the unit has not been verified.
        /// - <c>KeyConstants.Save</c> if the unit mapping is successfully saved.
        /// - <c>KeyConstants.InternalServerError</c> in case of failure or exceptions.
        /// - ModelState errors if the submitted model is invalid.
        /// </returns>
        /// <remarks>
        /// - Updates <see cref="Updatedby"/> and <see cref="UpdatedOn"/> fields before saving.
        /// - Checks for duplicate mapping using <see cref="_IMapUnitBL.CheckUnitMappedInMapUnit"/>.
        /// - Invokes <see cref="_iAccountBL.SaveUnitWithMapping"/> to persist the mapping.
        /// - Catches exceptions and logs them with <see cref="_logger"/>.
        /// - Designed for AJAX calls returning JSON results.
        /// </remarks>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> SaveUnitWithMapping(DTOSaveUnitWithMappingRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                if (ModelState.IsValid) // Valid Model State
                {
                    string SUSNo = dTO.Sus_no + dTO.Suffix.ToUpper();
                    DTOCheckUnitMappedInMapUnitResponse? response = await _IMapUnitBL.CheckUnitMappedInMapUnit(SUSNo); // Check if Unit is already mapped
                    if (response != null)
                    {
                        if(response.UnitMapId != null) // Unit already mapped
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
                            bool result = (bool)await _iAccountBL.SaveUnitWithMapping(dTO); // Save Unit with Mapping
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
                        bool result = (bool)await _iAccountBL.SaveUnitWithMapping(dTO); // Save Unit with Mapping
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

        /// <summary>
        /// Switches the current user's role within the session and redirects to the corresponding page.
        /// </summary>
        /// <param name="Id">The role identifier to switch to (e.g., "admin" or other role names).</param>
        /// <returns>
        /// Redirects permanently to:
        /// <list type="bullet">
        /// <item>If <paramref name="Id"/> is "admin", redirects to the <c>DashboardMaster</c> action of the <c>Master</c> controller.</item>
        /// <item>Otherwise, redirects to the <c>Index</c> action of the <c>Home</c> controller.</item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// - Updates the <c>RoleName</c> property of the <c>DtoSession</c> object stored in the session under the key "Token".
        /// - If no session exists with key "Token", no session update occurs and the redirection still happens based on <paramref name="Id"/>.
        /// - This method requires the user to be authenticated due to the <c>[Authorize]</c> attribute.
        /// </remarks>
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
            else
            {
                return RedirectToActionPermanent("Index", "Home");
            }
        }

        #endregion End IMLogin

        #region Logout

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// - Removes the "Token" object from the current session to clear user-specific session data.
        /// - Signs out the user from the authentication system using <see cref="SignInManager{TUser}.SignOutAsync"/>.
        /// - After logout, renders the default logout view.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="ViewResult"/> for the logout confirmation page.
        /// </returns>
        public async Task<ActionResult> Logout()
        {
            // Remove the session token to clear user-specific session data
            HttpContext.Session.Remove("Token");

            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                await userManager.UpdateSecurityStampAsync(user); // invalidate all cookies
            }

            // Sign out the user from ASP.NET Identity authentication
            await signInManager.SignOutAsync();

            //Clear server-side session state
            HttpContext.Session.Clear();

            // Delete session + auth cookies explicitly (good for audits)
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Session");

            // Return the logout confirmation view to the user
            return View();
        }

        #endregion End Logout

        #region IAM Code


        /// <summary>
        /// Handles the Identity Management (IM) login process using SAML responses.
        /// </summary>
        /// <remarks>
        /// - Checks environment (Development or Production) to set the configuration ID.
        /// - Retrieves IAM settings from the database to determine debug mode and local host handling.
        /// - Reads the SAMLResponse from the HTTP form or stored hardcoded values in debug scenarios.
        /// - Decrypts and validates the SAML response using a certificate.
        /// - Extracts user and role information from the SAML response.
        /// - Fetches or creates domain mapping and temporary session data for the user.
        /// - Redirects to <c>TokenValidate</c> action for further processing based on session status.
        /// - Handles new users, existing users with or without admin privileges, and unauthorized roles.
        /// - Fallback redirects to the IAM login page in case of errors or invalid responses.
        /// </remarks>
        /// <returns>
        /// Returns an <see cref="IActionResult"/>:
        /// - Redirects to <c>TokenValidate</c> action for valid SAML responses.
        /// - Redirects to IAM login page if the response is missing, invalid, or an exception occurs.
        /// </returns>
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> IMLogin()
        {
            try
            {
                var EncryptedResponse = ""; // Holds the encrypted SAML response
                byte id = 0;

                // Set configuration ID based on environment
                if (_hostEnv.IsDevelopment())
                {
                    id = 2;
                }
                else 
                {
                    id = 1;
                }

                // Fetch IAM settings based on environment                    
                IAMSetting iAMSetting = await _iAMSettingBL.GetByByte(id);

                // Handle debug mode with IAM
                if (iAMSetting.DebugWithIAM == true)
                {
                    if (iAMSetting.LocalHostActive == 0)
                    {
                        // Capture SAMLResponse from HTTP request form
                        EncryptedResponse = Request.Form["SAMLResponse"];

                        // Disable debug after fetching
                        iAMSetting.DebugWithIAM = false;
                        await _iAMSettingBL.Update(iAMSetting);

                        // Post the response to localhost for debugging
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
                        // Store SAMLResponse for hardcoded debug usage
                        EncryptedResponse = Request.Form["SAMLResponse"];
                        iAMSetting.HardSAMLResonoce = EncryptedResponse;
                        iAMSetting.LocalHostActive = 2;
                        await _iAMSettingBL.Update(iAMSetting);
                        //stop code directelly from here to debug code with IAM, and Run this on this URL "https://localhost:7023/Account/IMLogin". 
                    }
                    else
                    {
                        // Use stored hardcoded SAML response
                        EncryptedResponse = iAMSetting.HardSAMLResonoce;
                    }
                }
                else
                {
                    // Production scenario: fetch SAMLResponse from request
                    EncryptedResponse = Request.Form["SAMLResponse"];
                }



                DTOIMLoginRequest model = new DTOIMLoginRequest();

                DTOTempSession dTOTempSession = new DTOTempSession();


                if (!string.IsNullOrEmpty(EncryptedResponse))
                {
                    // Decrypt SAML response using the certificate
                    string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\eisac.army.mil.pfx", "Abc@2022");

                    AccountSettings accountSettings = new AccountSettings();
                    OneLogin.Saml.Response samlResponse = new OneLogin.Saml.Response(accountSettings);

                    samlResponse.LoadXmlFromBase64(decryptedsamlresponse);
                    //if (samlResponse.IsValid_sign())
                    // Validate the response and extract NameID
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
                            // Populate model and session objects
                            model.DomainId = log.NameId;
                            model.Role = log.SAMLRole;
                            dTOTempSession.AppName = log.AppName;
                            HttpContext.Session.SetString("AppName", dTOTempSession.AppName);

                            string? Footer = _configuration["Footer:Test"];
                            ViewBag.Footer = Footer;
                            //if (ModelState.IsValid)
                            {
                                // Retrieve domain mapping based on DomainId and Role
                                TrnDomainMapping? _trnDomainMapping = await _iDomainMapBL.GetAllRelatedDataByDomainId(model.DomainId, model.Role);

                                // Case 1: Mapping exists, AdminFlag is true, and UserId is present
                                if (_trnDomainMapping != null && _trnDomainMapping.ApplicationUser.AdminFlag == true && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId != null)
                                {
                                    // Populate session with existing user/admin details
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
                                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;

                                    // Check if Role is valid
                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 5;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }
                                    else
                                    {
                                        // Role not authorized
                                        TempData["error"] = "Role not authorized.";
                                        dTOTempSession.Status = 6;
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        return RedirectToActionPermanent("TokenValidate", "Account");
                                    }


                                }
                                // Case 2: Mapping exists, UserId not present
                                else if (_trnDomainMapping != null && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId == null)
                                {
                                    /*Get UserId from ProfileTable (Based on Input ArmyNo with token authorise.) and Update in TrnDomainMapping Table*/
                                    // Populate session, UserId will be updated later
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
                                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;
                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 4; // Status for existing mapping but missing UserId
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
                                // Case 3: Mapping exists but Id == 0 (probably new entry to be created)
                                else if (_trnDomainMapping != null && _trnDomainMapping.Id == 0)
                                {
                                    /*Create TrnDomainMapping using AspnetUserId,UnitId,UserId from Profile Table.*/
                                    // Populate session for creating new mapping
                                    dTOTempSession.NewUser = false;
                                    dTOTempSession.DomainId = _trnDomainMapping.ApplicationUser.DomainId;
                                    dTOTempSession.RoleName = model.Role;
                                    dTOTempSession.AspNetUsersId = _trnDomainMapping.ApplicationUser.Id;

                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 3; // Status for mapping creation
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
                                // Case 4: Mapping exists, AdminFlag is false, and UserId present
                                else if (_trnDomainMapping != null && _trnDomainMapping.ApplicationUser.AdminFlag == false && _trnDomainMapping.Id > 0 && _trnDomainMapping.UserId != null)
                                {
                                    // Populate session for non-admin user
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
                                    dTOTempSession.IsToken = _trnDomainMapping.IsToken;
                                    if (_trnDomainMapping.Role != null)
                                    {
                                        dTOTempSession.Status = 1; // Your regn request was successfully placed with Admin for necy Approval
                                        SessionHeplers.SetObject(HttpContext.Session, "IMData", dTOTempSession);
                                        TempData["error"] = "Domain Id - " + dTOTempSession.DomainId + " & Profile Id - " + dTOTempSession.UserId + ".<br/>Your regn request was successfully placed with Admin for necy Approval..<br/>Pl note regn No - " + dTOTempSession.AspNetUsersId + " for future correspondence. <br/>Contact Admin.";

                                        // Override message if AdminMsg exists
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
                                // Case 5: No mapping exists (completely new user)
                                else if (_trnDomainMapping == null)
                                {
                                    /*Create DomainId in AspNetUser Table , Assign Role.,Create Mapping with add profile id.*/
                                    // Handle completely new user
                                    // Set session for new user
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
                    // On exception, redirect to IAM login page
                    Response.Redirect("https://iam2.army.mil/IAM/User", true);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("https://iam2.army.mil/IAM/User", true);
            }
            // Fallback redirect to self-login if all else fails
            return RedirectToAction("IMLoginSelf", "Account");
        }


        /// <summary>
        /// Handles token validation for IAM users and redirects based on session and role information.
        /// </summary>
        /// <remarks>
        /// - Retrieves footer from configuration and sets ViewBag.Footer.
        /// - Checks current logged-in user by Claims.
        /// - Retrieves session objects ("Token" and "IMData") to determine user status.
        /// - Redirects users based on role: "user" goes to Home/Index, "admin" goes to Master/DashboardMaster.
        /// - Handles new users, pending verification, and unauthorized access gracefully.
        /// </remarks>
        /// <returns>
        /// - Returns the appropriate View() for token validation if user is pending or not fully authorized.
        /// - Redirects to Home or Admin dashboard for authorized users based on role.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [AnySessionRequired]
        public IActionResult TokenValidate_()  //__ForIAM
        {
            // Get footer text from configuration and pass to ViewBag
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;

            // Get the current logged-in user's Id from Claims
            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get session object for token-based validation
            DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "Token");

            // Default role list for basic user
            List<string> RoleNameList = new List<string>() { "user" };

            // Case: User not logged in (userid == 0)
            if (userid == 0)
            {
                // Retrieve temporary session object set during initial login
                DTOTempSession? dTOTempSession1 = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");

                ViewBag.AppName = dTOTempSession1.AppName;
                ViewBag.DomainId = dTOTempSession1.DomainId;
                ViewBag.RoleName = dTOTempSession1.RoleName;

                if (dTOTempSession1 != null)
                {
                    // Status check can be extended for pending verification or new user logic
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
                    // Session missing, show unauthorized error
                    TempData["error"] = "You are not authorized to this page.";
                    return View();
                }
            }
            else
            {
                // Case: User is logged in
                if (dTOTempSession != null)
                {
                    ViewBag.AppName = dTOTempSession.AppName;
                    ViewBag.DomainId = dTOTempSession.DomainId;
                    ViewBag.RoleName = dTOTempSession.RoleName;

                    // Redirect based on role
                    if (RoleNameList.Contains(dTOTempSession.RoleName))
                    {
                        return RedirectToActionPermanent("Index", "Home");
                    }
                    else if (dTOTempSession.RoleName == "admin")
                    {
                        return RedirectToActionPermanent("DashboardMaster", "Master");
                    }
                    return View();
                }
                else
                {
                    // Session object not found, display default view
                    return View();
                }

            }

        }

        /// <summary>
        /// Handles POST request for token validation for IAM users.
        /// Validates the user credentials, updates session, and redirects based on role and verification status.
        /// </summary>
        /// <param name="model">The <see cref="DTOTokenRequestForIAM"/> object containing ICNo and password inputs from the user.</param>
        /// <returns>
        /// - Returns the same view with error messages if validation fails or user is unauthorized.
        /// - Redirects to Home/Index for basic users ("user") or Master/DashboardMaster for admin users upon successful login.
        /// </returns>
        /// <remarks>
        /// - Retrieves footer from configuration and sets ViewBag.Footer.
        /// - Fetches temporary session data ("IMData") to determine the current status of the IAM user.
        /// - Handles multiple verification statuses:
        ///   1: Registration pending approval, 2-4: Domain mapping issues, 5: Approved, 6: Role not authorized.
        /// - Uses ASP.NET Identity <see cref="UserManager"/> and <see cref="SignInManager"/> for authentication.
        /// - Creates a login log entry using <see cref="TrnLogin_Log"/>.
        /// - Updates session with <see cref="DtoSession"/> object upon successful login.
        /// - Provides detailed error messages using TempData for the view.
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        [AnySessionRequired]
        public async Task<IActionResult> TokenValidate_(DTOTokenRequestForIAM model)  //__ForIAM
        {
            try
            {
                // Set footer from configuration
                string? Footer = _configuration["Footer:Test"];
                ViewBag.Footer = Footer;

                // Retrieve temporary session for IAM data
                DTOTempSession? dTOTempSession = SessionHeplers.GetObject<DTOTempSession>(HttpContext.Session, "IMData");

                // Default role list for basic user redirection
                List<string> RoleNameList = new List<string>() { "user" };


                // Check if session exists
                if (dTOTempSession != null)
                {
                    // Clean up ICNo input and set default password
                    model.ICNo = model.ICNo.Trim();
                    model.Password = Environment.GetEnvironmentVariable("Common__Password") ?? string.Empty;

                    // Validate model
                    if (ModelState.IsValid)
                    {
                        // Case: Approved user attempting login
                        if (dTOTempSession.Status == 5 && dTOTempSession.ICNO == model.ICNo)
                        {
                            // Fetch user by Id
                            var usera = await userManager.FindByIdAsync(dTOTempSession.AspNetUsersId.ToString());

                            // Clear old session and sign out any existing user
                            HttpContext.Session.Remove("Token");
                            await signInManager.SignOutAsync();

                            // Update security stamp to invalidate previous sessions
                            await userManager.UpdateSecurityStampAsync(usera);
                            if (usera != null)
                            {
                                // Attempt sign-in with default password
                                var result = await signInManager.PasswordSignInAsync(usera.UserName, model.Password, false, lockoutOnFailure: true);
                                if (result.Succeeded)
                                {
                                    // Create session object
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
                                    // Log the login attempt
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

                                    // Set session for successful login
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
                            // Handle other statuses: pending approval, domain mapping issues, etc
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
                            // Handle Status 2,3,4 with existing domain mapping
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
                            // Handle new profiles or unmapped statuses
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
                        // Model state invalid, return first validation error
                        var error = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        TempData["error"] = error[0][0].ErrorMessage;
                        goto End;
                    }
                }
                else
                {
                    // Session missing
                    TempData["error"] = "You are not authorized this page.";
                    goto End;
                }
            End:
                // Return the same view with model and TempData errors
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
        }


        /// <summary>
        /// Logs out the currently signed-in user and clears the session token.
        /// </summary>
        /// <remarks>
        /// This action performs a complete sign-out by removing the "Token" object from the session
        /// and calling <see cref="SignInManager{TUser}.SignOutAsync"/> to invalidate the authentication cookie.
        /// After successful logout, the user is redirected to the logout confirmation view.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="ViewResult"/> representing the logout confirmation page.
        /// </returns>
        public async Task<ActionResult> FinalLogout()
        {
            // Remove the session object named "Token" to clear user-specific session data
            HttpContext.Session.Remove("Token");

            // Sign out the user from ASP.NET Identity authentication
            await signInManager.SignOutAsync();

            // Return the logout confirmation view
            return View();
        }

        /// <summary>
        /// Handles logout requests coming from the IAM (Identity and Access Management) system.
        /// </summary>
        /// <remarks>
        /// This method checks for SAMLRequest or SAMLResponse parameters in the query string to determine
        /// whether the request is a logout request from the identity provider or a logout response. 
        /// Depending on the request type, it either decrypts the SAML request, signs the user out, 
        /// and sends a logout response back to the IAM, or simply removes the session and signs out the user.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="ViewResult"/> representing the logout confirmation page.
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> IMLogout()
        {
            // if(HttpContext.Request.Query.Count()>0)
            // {
            // Read SAMLRequest and SAMLResponse from query string
            string? SAMLRequest = HttpContext.Request.Query["SAMLRequest"];
            string? SAMLResponse = HttpContext.Request.Query["SAMLResponse"];
            // }

            //string ss = Convert.ToString(HttpContext.Request.QueryString);

            // Retrieve current session details
            var dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            // If there is a SAMLResponse, redirect to the final logout page
            if (SAMLResponse != null && !string.IsNullOrEmpty(Convert.ToString(SAMLResponse)))
            {
                // Response.Redirect("https://localhost:7023/Account/FinalLogout");
                Response.Redirect("https://eisac.army.mil/Account/FinalLogout");
            }
            // If there is a SAMLRequest, process logout request from IAM
            else if (SAMLRequest != null && !string.IsNullOrEmpty(Convert.ToString(SAMLRequest)))
            {
                string EncryptedResponse = Convert.ToString(SAMLRequest);
                if (!string.IsNullOrEmpty(EncryptedResponse))
                {
                    AccountSettings accountSettings = new AccountSettings();
                    
                    // Create a SAML response object
                    OneLogin.Saml.Response samlResponse = new OneLogin.Saml.Response(accountSettings);

                    var certPath = Environment.GetEnvironmentVariable("Cert__Path");
                    var certPassword = Environment.GetEnvironmentVariable("Cert__Password");

                    if (string.IsNullOrWhiteSpace(certPath))
                    {
                        throw new Exception("Certificate path not found in environment variable.");
                    }

                    // Decrypt the SAML request using the specified certificate and password
                    string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, certPath, certPassword);
                    samlResponse.LoadXmlFromBase64(decryptedsamlresponse);


                    // Extract logout parameters from the SAML response
                    string nameid = string.Empty;
                    string issuer = string.Empty;
                    samlResponse.GetLogoutParameter(out nameid, out issuer);

                    // Remove the session token and sign out from Identity
                    HttpContext.Session.Remove("Token");
                    await signInManager.SignOutAsync();
                    try
                    {
                        // Remove the session token and sign out from Identity
                        //SendResponseToIAM("https://localhost:7023/Account/FinalLogout", accountSettings.entityId, nameid);
                        SendResponseToIAM("https://eisac.army.mil/Account/FinalLogout", accountSettings.entityId, nameid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(1001, ex, "Account->IMLogout");
                    }
                }
            }
            // If neither SAMLRequest nor SAMLResponse is present
            else
            {
                AccountSettings acs = new AccountSettings();

                string NameId = dtoSession.DoaminId;
                string userRole = dtoSession.RoleName;

                // Send logout request to IAM for the current user
                LogoutRequesttoIAM(userRole, acs.entityId, NameId);
            }
            // Final fallback: if no SAMLRequest or SAMLResponse, send logout request to IAM
            if (SAMLRequest == null && SAMLResponse == null)
            {
                AccountSettings acs = new AccountSettings();
                string NameId = dtoSession.DoaminId;
                string userRole = dtoSession.RoleName; ;


                //HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);



                LogoutRequesttoIAM(userRole, acs.entityId, NameId);
            }
            // Return the logout confirmation view
            return View();
        }
        
        public IActionResult UnAuthUser()
        {
            return View();
        }


        /// <summary>
        /// Decrypts a SAML response string using a specified certificate and password.
        /// </summary>
        /// <param name="Encryptedtext">The SAML response text, which is expected to be base64-encoded and encrypted.</param>
        /// <param name="certificatepath">The file path to the X.509 certificate (.pfx) used for decryption.</param>
        /// <param name="password">The password for the certificate file.</param>
        /// <returns>
        /// Returns the decrypted SAML response as a <see cref="string"/>.
        /// If an error occurs during decryption, returns the exception message.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Splits the encrypted text to extract the encrypted key and encrypted payload.
        /// 2. Uses the private key from the specified certificate to decrypt the encrypted key.
        /// 3. Uses the decrypted key along with a fixed IV to decrypt the payload using a custom method <see cref="DecryptString0705222_Final"/>.
        /// 
        /// Important: The current implementation uses hard-coded certificate path and password inside the try-block.
        /// Ensure that the certificate and password are secured appropriately in production.
        /// </remarks>
        [AllowAnonymous]
        public string DecryptSAmlResponseNew(string Encryptedtext, string certificatepath, string password)
        {

            string result = "True"; // Default return value
            try
            {
                // Prepare a separator based on a base64-encoded string "alpha"
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("alpha");

                string[] spearator = { Convert.ToBase64String(plainTextBytes) };

                // Split the encrypted text into payload and encrypted key
                string[] newstring = Encryptedtext.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                //string[] newstring = encryptedvalue.Split();
                string key = newstring[1].ToString();
                string plain = newstring[0].ToString();
                #region decryptkeyusingprivatekey
                try
                {
                    byte[] byteData = Convert.FromBase64String(key);
                    //   byte[] decryptedkey = new byte[16];
                    byte[] decryptedkey = new byte[32]; // Placeholder for decrypted key
                    X509Certificate2 myCert2 = null;
                    RSACryptoServiceProvider rsa = null;

                    try
                    {
                        var certPath = Environment.GetEnvironmentVariable("Cert__Path");
                        var certPassword = Environment.GetEnvironmentVariable("Cert__Password");

                        if (string.IsNullOrWhiteSpace(certPath))
                        {
                            throw new Exception("Certificate path not found in environment variable.");
                        }

                        // Load certificate from specified path and password
                        myCert2 = new X509Certificate2(certPath, certPassword);
                        // rsa = (RSACryptoServiceProvider)myCert2.PrivateKey;
                        #region test
                        // Decrypt the key using RSA private key
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
                    // Initialization vector for payload decryption
                    byte[] iv = new byte[32];


                    byte[] iv1 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };


                    // result = DecryptString0705222_Final(plain, rsa.Decrypt(byteData, RSAEncryptionPadding.Pkcs1), iv1);
                    // Decrypt the payload using the decrypted symmetric key and IV
                    result = DecryptString0705222_Final(plain, decryptedkey, iv1);
                }
                catch (Exception exxx)
                {
                    // Return any errors encountered during key decryption
                    result = exxx.Message;
                }
                #endregion

            }
            catch (Exception exx)
            {
                // Return any errors encountered during the overall decryption process
                result = exx.Message;
            }

            return result;
        }


        /// <summary>
        /// Sends a SAML logout response to the IAM (Identity and Access Management) system.
        /// </summary>
        /// <param name="issueurl">The URL to which the IAM system should redirect after logout.</param>
        /// <param name="entityid">The unique entity ID of the service provider in the SAML configuration.</param>
        /// <param name="usernam">The username (NameID) of the user to be logged out.</param>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Initializes account and application settings for the SAML request.
        /// 2. Generates a SAML LogoutRequest in Base64 format.
        /// 3. Redirects the user to the IAM logout URL with the SAMLResponse query parameter.
        ///
        /// Important: The redirection URL is hardcoded to "https://iam2.army.mil/IAM/logout".
        /// Ensure that the endpoint and parameters are updated appropriately for production environments.
        /// </remarks>
        [AllowAnonymous]
        public void SendResponseToIAM(string issueurl, string entityid, string usernam)
        {
            // Initialize account settings for the SAML request
            AccountSettings accountSettings = new AccountSettings();

            // Create a new SAML AuthRequest object using application and account settings
            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            //string ReuestXML = req.GetRequest(AuthRequest.AuthRequestFormat.Base64);
            //string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");
            
            // Generate a Base64-encoded SAML LogoutRequest for the IAM system
            string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");

            //Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);

            // Redirect the user to the IAM logout endpoint with the SAMLResponse parameter
            Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);

        }


        /// <summary>
        /// Sends a SAML single logout request to the IAM (Identity and Access Management) system for a specific user and role.
        /// </summary>
        /// <param name="role">The role of the user initiating the logout (e.g., "Admin", "User").</param>
        /// <param name="entityid">The unique entity ID of the service provider in the SAML configuration.</param>
        /// <param name="usernam">The username (NameID) of the user to be logged out.</param>
        /// <remarks>
        /// This method performs the following actions:
        /// 1. Initializes account and application settings required for SAML communication.
        /// 2. Generates a Base64-encoded SAML Single LogoutRequest using the user's role and username.
        /// 3. Redirects the user's browser to the IAM Single Logout endpoint with the SAMLRequest as a query parameter.
        ///
        /// Notes:
        /// - The IAM endpoint URL is hardcoded to "https://iam2.army.mil/IAM/singleAppLogout".
        /// - Ensure that the entity ID, role, and username are correct to avoid logout failures.
        /// </remarks>
        [AllowAnonymous]
        public void LogoutRequesttoIAM(string role, string entityid, string usernam)
        {
            // Initialize account settings for SAML operations
            AccountSettings accountSettings = new AccountSettings();

            // Create a new SAML AuthRequest object using application and account settings
            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            // Generate a Base64-encoded SAML Single LogoutRequest for the IAM system
            string ReuestXML = req.SingleLogoutRequest(AuthRequest.AuthRequestFormat.Base64, entityid, role, usernam);
            //Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);

            // Redirect the user's browser to the IAM Single Logout endpoint
            // with the SAMLRequest appended as a URL-encoded query parameter
            Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);
        }


        /// <summary>
        /// Decrypts a Base64-encoded AES-encrypted string using a specified key and initialization vector (IV).
        /// </summary>
        /// <param name="cipherText">The Base64-encoded encrypted string to be decrypted.</param>
        /// <param name="key">The byte array representing the AES decryption key.</param>
        /// <param name="iv">The byte array representing the initialization vector (IV) for decryption.</param>
        /// <returns>
        /// Returns the decrypted plaintext string. If decryption fails, an empty string is returned.
        /// </returns>
        /// <remarks>
        /// This method performs symmetric AES decryption using ECB mode and PKCS7 padding:
        /// 1. Converts the Base64-encoded ciphertext to a byte array.
        /// 2. Configures AES with the provided key and IV.
        /// 3. Uses a CryptoStream to perform decryption into a MemoryStream.
        /// 4. Converts the decrypted byte array to an ASCII string.
        /// </remarks>
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

        /// <summary>
        /// Displays the Claims view for users in the "admin" role.
        /// </summary>
        /// <remarks>
        /// This action is restricted to users authorized with the "admin" role via the <see cref="AuthorizeAttribute"/>.
        /// The method responds to HTTP GET requests and returns the corresponding view.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="ViewResult"/> that renders the Claims page.
        /// </returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Claims()
        {
            return View();
        }

        /// <summary>
        /// Retrieves all claims data for the DataTables grid, ordered based on the request parameters.
        /// </summary>
        /// <remarks>
        /// This action is restricted to users in the "admin" role and responds to HTTP POST requests.
        /// The method expects a <see cref="DTODataTablesRequest"/> object containing pagination, sorting, and filtering parameters.
        /// Returns a JSON response compatible with jQuery DataTables.
        /// </remarks>
        /// <param name="dTO">The DataTables request containing pagination, sorting, and search parameters.</param>
        /// <returns>
        /// Returns a <see cref="JsonResult"/> containing either:
        /// - The claims data from <see cref="_iAccountBL.GetAllClaimsOrderBy"/> if successful.
        /// - An empty response with 0 records if an exception occurs.
        /// </returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllClaims(DTODataTablesRequest dTO)
        {
            List<DTOClaimsStoreResponse> dTOClaims = new List<DTOClaimsStoreResponse>();
            try
            {
                if (ModelState.IsValid)
                {
                    // Attempt to retrieve all claims ordered as per DataTables request parameters
                    return Json(await _iAccountBL.GetAllClaimsOrderBy(dTO));
                }
                else
                {
                    var responseData = new DTODataTablesResponse<DTOClaimsStoreResponse>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOClaims
                    };
                    return Json(responseData);
                }
            }
            catch (Exception ex)
            {
                var responseData = new DTODataTablesResponse<DTOClaimsStoreResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOClaims
                };
                _logger.LogError(1001, ex, "Account->GetAllClaims");
                return Json(responseData);
            }
        }

        /// <summary>
        /// Retrieves all users associated with claims and returns them in a format suitable for DataTables.
        /// </summary>
        /// <remarks>
        /// The method supports server-side pagination, sorting, and filtering for DataTables.
        /// In case of an exception, an empty response is returned and the error is logged.
        /// This endpoint is restricted to users in the "admin" role.
        /// </remarks>
        /// <param name="dTO">A <see cref="DTODataTablesRequest"/> object containing paging, sorting, and draw parameters from the client.</param>
        /// <returns>
        /// Returns a <see cref="JsonResult"/> containing a <see cref="DTODataTablesResponse{DTOUsersByClaim}"/> object with:
        /// - <c>draw</c>: The draw counter from the client request.
        /// - <c>recordsTotal</c>: Total number of records without filtering.
        /// - <c>recordsFiltered</c>: Total number of records after applying filtering.
        /// - <c>data</c>: The paginated list of users grouped by their claims.
        /// </returns>
        [Authorize(Roles = "admin")]
        [HttpPost()]
        public async Task<IActionResult> GetAllUsersByClaim(DTODataTablesRequest dTO)
        {
            List<DTOUsersByClaim> dTOUsers = new List<DTOUsersByClaim>();
            try
            {
                if (ModelState.IsValid)
                {
                    // Call the business logic layer to fetch users grouped by claims
                    // Returns data in a structure compatible with DataTables
                    return Json(await _iAccountBL.GetAllUsersByClaim(dTO));
                }
                else
                {
                    var responseData = new DTODataTablesResponse<DTOUsersByClaim>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUsers
                    };
                    return Json(responseData);
                }
            }
            catch (Exception ex)
            {
                // Prepare an empty DataTables response
                var responseData = new DTODataTablesResponse<DTOUsersByClaim>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUsers
                };
                // Log the exception with a unique event ID for troubleshooting
                _logger.LogError(1001, ex, "Account->UsersByClaim");
                
                // Return empty response to maintain DataTables compatibility
                return Json(responseData);
            }
        }
        
        #endregion Claims
    }
}
