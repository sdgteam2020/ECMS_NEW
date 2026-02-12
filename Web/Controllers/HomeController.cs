using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Home;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.RecordOffice;
using BusinessLogicsLayer.Registration;
using BusinessLogicsLayer.ReportReturn;
using BusinessLogicsLayer.Service;
using BusinessLogicsLayer.Unit;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Web.Healpers;
using Web.Healpers.BaseInterfaces;
using Web.WebHelpers;


namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling home-related actions and views.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRegistrationBL _registrationBL;//Interface for registration business logic layer
        private readonly IBasicDetailBL _basicDetailBL;//Interface for basic detail business logic layer
        private readonly INotificationBL _INotificationBL;//Interface for notification business logic layer
        private readonly IUserProfileBL _userProfileBL;//Interface for user profile business logic layer
        private readonly ITrnICardRequestBL _ITrnICardRequestBL;//Interface for ICard request business logic layer
        private readonly IHomeBL _home;//Interface for home business logic layer
        private readonly IRecordOfficeBL _recordOfficeBL;//Interface for record office business logic layer
        private readonly SignInManager<ApplicationUser> signInManager;//Service for managing user sign-in operations
        private readonly UserManager<ApplicationUser> userManager;//Service for managing user-related operations
        private readonly IHttpContextAccessor _httpContextAccessor;//Service for accessing the current HTTP context
        private readonly ILogger<HomeController> _logger;//Logger instance for logging information and errors
        private readonly IService service;//Interface for general service operations
        public readonly IReportReturnBL _reportReturnBL;//Interface for report return business logic layer
        private readonly IMapUnitBL _mapUnitBL;//Interface for map unit business logic layer
        private const string CounterFilePath = "wwwroot/counter.txt";// File path for storing the visitor counter
        private const string SessionKey = "SessionHit";// Session key for tracking user sessions
        private readonly string[] IgnoredIPs = { "127.0.0.2", "127.0.0.3" }; // Add IPs to ignore
        private readonly IConfiguration _configuration;//Configuration interface for accessing application settings
        private readonly IWebHostEnvironment hostingEnvironment;// For Hosting Environment
        private readonly IImageEncryptAndDecrypt imageEncryptAndDecrypt;// For Image Encrypt and Decrypt

        //constructor to initialize dependencies and configuration settings.
        public HomeController(IRegistrationBL registrationBL, IUserProfileBL userProfileBL,
            IBasicDetailBL basicDetailBL, INotificationBL notificationBL, ITrnICardRequestBL iTrnICardRequestBL,
            IHomeBL home, IRecordOfficeBL recordOfficeBL, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor,
            IReportReturnBL reportReturnBL, IService service, IConfiguration configuration, IMapUnitBL mapUnitBL, IWebHostEnvironment hostingEnvironment, IImageEncryptAndDecrypt imageEncryptAndDecrypt
            )
        {
            _userProfileBL = userProfileBL;
            _registrationBL = registrationBL;
            _basicDetailBL = basicDetailBL;
            _INotificationBL = notificationBL;
            _ITrnICardRequestBL = iTrnICardRequestBL;
            _home = home;
            _recordOfficeBL = recordOfficeBL;
            _logger = logger;
            _reportReturnBL = reportReturnBL;
            this.userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            this.service = service;
            _configuration = configuration;
            _mapUnitBL = mapUnitBL;
            this.hostingEnvironment = hostingEnvironment;
            this.imageEncryptAndDecrypt = imageEncryptAndDecrypt;
        }



        #region ContactUs / Index

        /// <summary>
        /// Action method that displays the "Contact Us" page.
        /// </summary>
        /// <returns>The "Contact Us" view.</returns>
        [HttpGet]
        public IActionResult ContactUs()
        {
            // Return the ContactUs view
            return View();
        }


        /// <summary>
        /// Action method for the Index page. It retrieves the user's role from the session
        /// and passes it to the view through the ViewBag.
        /// </summary>
        /// <returns>The Index view, with the user's role passed in ViewBag.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Pass the role to the view through ViewBag
            ViewBag.Role = role;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        #endregion


        #region Notification /GetAllNotificationData / SaveNotification / GetNotification / GetNotificationRequestId / UpdateNotification
        [HttpGet]
        public IActionResult Notification()
        {
            string role = SessionHelper.GetRoleFromSession(HttpContext);
            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        
        
        [HttpPost]
        public async Task<IActionResult> GetAllNotificationData([FromBody] DTODataTablesRequestForNotification dTORecord)
        {
            // Retrieve current userId from claims and assign it into the DTO
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            dTORecord.ReciverAspNetUsersId = userId;
            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
            try
            {
                DTODataTablesResponse<DTONotificationResponse> dTODataTablesResponse = new DTODataTablesResponse<DTONotificationResponse>();


                dTODataTablesResponse = await _INotificationBL.GetAllNotificationData(dTORecord);
                return Json(dTODataTablesResponse);
            }
            catch (Exception ex)
            {
                // Log exception with event id 1001 and return 400 Bad Request
                _logger.LogError(1001, ex, "Home->GetAllNotificationData");
                return BadRequest(new { message = "Internal Server Error" });
            }
        }

        /// <summary>
        /// Action method to save a notification. It updates any previous notifications and adds a new one for both the sender and receiver.
        /// </summary>
        /// <param name="Data">The notification data to be saved.</param>
        /// <returns>A JSON response indicating success (1) or failure (0).</returns>
        [HttpPost]
        public async Task<IActionResult> SaveNotification(DTOTrnNotificationRequest Data)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    // Retrieve the user ID from the claims
                    int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    Data.SentAspNetUsersId = userId;

                    if (Data.StepId == 5) //Export
                    {
                        // Update the previous notification data
                        await _INotificationBL.UpdatePrevious(Data);
                    }
                    else
                    {
                        // Update the previous notification data
                        await _INotificationBL.UpdatePrevious(Data);

                        // Add the new notification
                        await _INotificationBL.AddNotification(Data);
                    }

                    // Return a success response
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Home=>SaveNotification.");
                return Json(0);
            }
        }

        /// <summary>
        /// Action method to retrieve notifications based on the provided type and applyForId.
        /// </summary>
        /// <param name="TypeId">The notification type ID used to filter the notifications.</param>
        /// <param name="applyForId">The applyForId used to filter the notifications.</param>
        /// <returns>A JSON response containing a list of notifications or null if no notifications are found.</returns>
        [HttpPost]
        public async Task<IActionResult> GetNotification()
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Fetch the notifications based on the user ID, TypeId, and applyForId
            DTONotificationResult dTONotificationResponses = await _basicDetailBL.GetNotification(userId);
            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

            // Return the notifications as a JSON response, or null if no notifications are found
            if (dTONotificationResponses.Items != null)
            {
                foreach (var basicDetailUpdVM in dTONotificationResponses.Items)
                {
                    // Load existing photo and signature if files exist

                    string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", basicDetailUpdVM.PhotoImagePath);

                    if (System.IO.File.Exists(sourcePathPhoto))
                    {
                        string resultB64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                        //basicDetailUpdVM.ExistingPhotoInBase64 = resultB64;
                        basicDetailUpdVM.ExistingPhotoInBase64 = imageEncryptAndDecrypt.CompressBase64(resultB64, maxWidth: 65, jpegQuality: 10, true);
                    }
                }

                return Json(dTONotificationResponses);
            }
            else
            {
                return Json(dTONotificationResponses);
            }
        }

        /// <summary>
        /// Action method to retrieve notifications based on the request ID, type, and applyForId.
        /// </summary>
        /// <param name="TypeId">The notification type ID used to filter the notifications.</param>
        /// <param name="applyForId">The applyForId used to filter the notifications.</param>
        /// <returns>A JSON response containing a list of notifications related to the request ID or null if no notifications are found.</returns>
        [HttpPost]
        public async Task<IActionResult> GetNotificationRequestId(int TypeId, int applyForId)
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Fetch the notifications based on the user ID, TypeId, and applyForId
            List<DTONotificationResponse>? dTONotificationResponses = await _basicDetailBL.GetNotificationRequestId(userId, TypeId, applyForId);
            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

            // Return the notifications as a JSON response, or null if no notifications are found
            if (dTONotificationResponses != null)
            {

                foreach (var basicDetailUpdVM in dTONotificationResponses)
                {
                    // Load existing photo and signature if files exist

                    string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", basicDetailUpdVM.PhotoImagePath);

                    if (System.IO.File.Exists(sourcePathPhoto))
                    {
                        basicDetailUpdVM.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                    }
                }
                return Json(dTONotificationResponses);
            }
            else
            {
                return Json(null);
            }
        }

        /// <summary>
        /// Action method to update a notification as read based on the provided notification data.
        /// </summary>
        /// <param name="Data">The notification data to be updated.</param>
        /// <returns>A JSON response indicating the success or failure of the update operation.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateNotification(MTrnNotification Data)
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Set the sender ID for the notification
            Data.SentAspNetUsersId = userId;

            // Update the notification status to "read" and return the result as JSON
            return Json(await _INotificationBL.UpdateRead(Data));
        }

        #endregion


        #region DashboardUserMgt / GetDashboardUserMgtCount / RequestDashboard / GetRequestDashboardCount / RegisterUser / GetAllRegisterUser


        /// <summary>
        /// Action method for the Dashboard User Management page. It retrieves session data, checks the user's role,
        /// and performs a lookup to determine if the user has access to the specified record office (RO).
        /// </summary>
        /// <returns>The DashboardUserMgt view with appropriate session and role data passed in ViewBag.</returns>
        [HttpGet]
        public async Task<IActionResult> DashboardUserMgt()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Initialize the DTO session object
            DtoSession? dtoSession = new DtoSession();

            // Retrieve the session data if available
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve relevant session information such as UnitId, TrnDomainMappingId, and UserId
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
            int TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
            int UserId = dtoSession != null ? dtoSession.UserId : 0;

            // Retrieve the Record Office data based on the TrnDomainMappingId
            DTOGetROByTDMIdResponse? dTOGetROByUserIdResponse = await _recordOfficeBL.GetROByTDMId(TDMId);

            // Check if the user has access to the Record Office
            if (dTOGetROByUserIdResponse == null)
            {
                ViewBag.ROFound = 0;
            }
            else if (dTOGetROByUserIdResponse.IsRO == true || dTOGetROByUserIdResponse.IsORO == true || dTOGetROByUserIdResponse.TDMId == TDMId)
            {
                ViewBag.ROFound = 1;
            }
            else
            {
                ViewBag.ROFound = 0;
            }

            // Pass the UnitId and Role to the view
            ViewBag.UnitId = UnitId;
            ViewBag.Role = role;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        
        /// <summary>
        /// Action method to retrieve the dashboard user management count based on the provided UnitId and current user's session.
        /// It calls the business logic layer to get the user management count for the dashboard.
        /// </summary>
        /// <param name="UnitId">The UnitId used to filter the user management count data.</param>
        /// <returns>A JSON response containing the dashboard user management count.</returns>
        [HttpPost]
        public async Task<IActionResult> GetDashboardUserMgtCount()
        {
            try
            {
                int UnitId=0;
                // Initialize the DTO session object
                DtoSession? dtoSession = new DtoSession();

                // Retrieve the session data if available
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    // Retrieve relevant session information such as UnitId, TrnDomainMappingId, and UserId
                    UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                }

                // Retrieve the user ID from the claims
                int UserId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch the dashboard user management count from the business logic layer
                return Json(await _home.GetDashboardUserMgtCount(UnitId, UserId));
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error
                _logger.LogError(1001, ex, "Home->GetDashboardUserMgtCount");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Action method to display the Request Dashboard based on the provided base64-encoded Id.
        /// The method validates the Id, decodes it, and determines the appropriate view based on the decoded value.
        /// </summary>
        /// <param name="Id">The base64-encoded Id used to determine the dashboard type.</param>
        /// <returns>A view for the Request Dashboard with the corresponding role and previous link.</returns>
        [HttpGet]
        public IActionResult RequestDashboard(string Id)
        {
            // Validate the base64 encoded Id and check if it's valid
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                // Set an error message if the Id is invalid
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            try
            {
                // Retrieve the user's role from the session
                string role = SessionHelper.GetRoleFromSession(HttpContext);

                // Decode the base64-encoded Id
                var base64EncodedBytes = Convert.FromBase64String(Id);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

                // Pass the decoded string (dashboard type) and role to the view
                ViewBag.Type = decodedString;
                ViewBag.Role = role;

                // Set the previous link based on the decoded dashboard type
                if (decodedString == "Posting Out" || decodedString == "Posting In")
                {
                    ViewBag.PreviousLink = "DashboardUserMgt";
                }
                else
                {
                    ViewBag.PreviousLink = "SubDashboard";
                }

                if (role == "user")
                {
                    return View();
                }
                else
                {
                    TempData["error"] = "Switch to user role.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (FormatException ex)
            {
                // Log any exceptions related to invalid base64 string and return an error
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id}", Id);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                // Log any other exceptions and return an error
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = ex.Message;
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Action method to retrieve the request dashboard count based on the provided Id and session data.
        /// </summary>
        /// <param name="Id">The ID used to retrieve the request dashboard count data.</param>
        /// <returns>A JSON response containing the request dashboard count data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetRequestDashboardCount(string Id)
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve UnitMapId from the session data
            int UnitMapId = dtoSession != null ? dtoSession.UnitId : 0;

            // Fetch request dashboard count from the business logic layer
            return Json(await _home.GetRequestDashboardCount(userId, Id, UnitMapId));
        }


        /// <summary>
        /// Action method to display the "Register User" page. It retrieves the user's session data
        /// (specifically the UnitId) and passes it to the view.
        /// </summary>
        /// <returns>The "Register User" view with the UnitId passed in ViewBag.</returns>
        [Authorize]
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }


        /// <summary>
        /// Action method to retrieve all registered users based on the provided UnitId.
        /// </summary>
        /// <param name="UnitId">The UnitId used to retrieve registered users.</param>
        /// <returns>A JSON response containing the list of registered users.</returns>
        [HttpPost]
        public async Task<IActionResult> GetAllRegisterUser()
        {
            try
            {
                // Initialize a new DtoSession object
                DtoSession? dtoSession = new DtoSession();

                // Check if the session contains a valid "Token"
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    // Retrieve the session object "Token" and deserialize it into the dtoSession object
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                // Retrieve the UnitId from the session or default to 0 if not available
                int UnitId = dtoSession != null ? dtoSession.UnitId : 0;

                // Fetch all registered users based on the provided UnitId
                return Json(await _home.GetAllRegisterUser(UnitId));
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an error response
                _logger.LogError(1001, ex, "Home->GetAllRegisterUser");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion


        #region Dashboard / GetDashboardCount /SubDashboard / GetSubDashboardCount

        /// <summary>
        /// Action method for the Dashboard page. It retrieves the user's role, claims, and user information
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The Dashboard view with role and user claims passed in ViewBag.</returns>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Pass the role to the view using ViewBag
            ViewBag.Role = role;

            // Retrieve the user ID from the claims of the current user
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the user from the UserManager service using the user ID
            var user = await userManager.FindByIdAsync(userId);

            // Retrieve all claims associated with the user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Pass the user claims to the view using ViewBag
            ViewBag.UserClaims = UserClaims;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
 
        }

        /// <summary>
        /// Action method to retrieve the dashboard count based on the user's configuration settings and session data.
        /// It validates configuration values and then calls the business logic layer to get the dashboard count data.
        /// </summary>
        /// <returns>A JSON response containing the dashboard count data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetDashboardCount()
        {

            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve configuration values for ArmedIdForORO and ApplFwdCondition
            short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]);
            DTOApplFwdConditionRequest? dTOApplFwdCondition = _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>() ?? new DTOApplFwdConditionRequest
            {
                MPRSO = new MPRSO(),
                MP6F = new MP6F(),
                MP6A = new MP6A()
            };

            // Validate the configuration values
            if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) ||
                string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name) || dTOApplFwdCondition.MP6A.RankOrderby == 0 || ArmedIdForORO == 0)
            {
                // If configuration values are invalid, return an error
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Json(KeyConstants.InternalServerError);
            }

            // Fetch dashboard count from the business logic layer
            return Json(await _home.GetDashBoardCount(userId, dTOApplFwdCondition, ArmedIdForORO));
        }


        /// <summary>
        /// Action method to display the sub-dashboard. It retrieves the user's role and passes it to the view.
        /// </summary>
        /// <returns>The SubDashboard view with the user's role passed in ViewBag.</returns>
        [HttpGet]
        public IActionResult SubDashboard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Pass the role to the view
            ViewBag.Role = role;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Action method to retrieve the sub-dashboard count based on the user's session data.
        /// </summary>
        /// <returns>A JSON response containing the sub-dashboard count data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetSubDashboardCount()
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve UnitId from the session data
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;

            // Fetch sub-dashboard count from the business logic layer
            return Json(await _home.GetSubDashboardCount(userId, UnitId));
        }

        #endregion


        #region Task / GetTaskBoardCount / MyTask / GetTaskCountICardRequest

        /// <summary>
        /// Action method to display the Task page. It retrieves the user's role and claims, then passes them to the view.
        /// </summary>
        /// <returns>The Task view with the user's role and claims passed in ViewBag.</returns>
        [HttpGet]
        public async Task<IActionResult> Task()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);
            ViewBag.Role = role;

            // Retrieve the user ID from the claims
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // Get the user's claims using UserManager
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;

            if (role == "user")
            {
                // Return the Dashboard view
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Action method to retrieve the task board count based on the user's session and claims.
        /// It retrieves session data, user claims, and then calls the business logic layer to fetch the task count data.
        /// </summary>
        /// <returns>A JSON response containing the task board count data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetTaskBoardCount()
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            // Initialize necessary variables
            int MapUnitId = 0;
            int TDMId = 0;
            byte ClaimValue = 0;

            // Retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve user claims using UserManager
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Set the session data based on user claims
            if (dtoSession != null)
            {
                MapUnitId = dtoSession.UnitId;
                TDMId = dtoSession.TrnDomainMappingId;
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                {
                    ClaimValue = 1;
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                {
                    ClaimValue = 2;
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                {
                    ClaimValue = 3;
                }
                else
                {
                    ClaimValue = 0;
                }
            }

            // Fetch task board count from the business logic layer
            return Json(await _home.GetTaskBoardCount(MapUnitId, ClaimValue, TDMId));
        }

        /// <summary>
        /// Action method to display the MyTask page based on the provided base64-encoded Id.
        /// The method validates the Id, decodes it, and retrieves the user's role and claims before returning the view.
        /// </summary>
        /// <param name="Id">The base64-encoded Id used to determine the task type.</param>
        /// <returns>A view for the MyTask page with the corresponding role and user claims.</returns>
        [HttpGet]
        public async Task<IActionResult> MyTask(string Id)
        {
            // Validate the base64 encoded Id and check if it's valid
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                // Set an error message if the Id is invalid
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToActionPermanent("ContactUs", "Home");
            }

            try
            {
                // Retrieve the user's role from the session
                string role = SessionHelper.GetRoleFromSession(HttpContext);

                // Decode the base64-encoded Id
                var base64EncodedBytes = Convert.FromBase64String(Id);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

                // Pass the decoded string (task type) and role to the view
                ViewBag.Type = decodedString;
                ViewBag.Role = role;

                // Retrieve the user ID from the claims
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);

                // Get the user's claims using UserManager
                var UserClaims = await userManager.GetClaimsAsync(user);
                ViewBag.UserClaims = UserClaims;

                if (role == "user")
                {
                    return View();
                }
                else
                {
                    TempData["error"] = "Switch to user role.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (FormatException ex)
            {
                // Log any exceptions related to invalid base64 string and return an error
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id}", Id);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                // Log any other exceptions and return an error
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = ex.Message;
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Action method to retrieve the task count for the I-Card request based on the provided Id and applyForId.
        /// It fetches the task count from the business logic layer (BL) and returns the result as JSON.
        /// </summary>
        /// <param name="Id">The ID of the request.</param>
        /// <param name="applyForId">The ID of the applyFor field used to filter the task count.</param>
        /// <returns>A JSON response containing the task count for the I-Card request.</returns>
        [HttpPost]
        public async Task<IActionResult> GetTaskCountICardRequest(int Id, int applyForId)
        {
            // Retrieve the user ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Fetch the I-Card task count based on the user ID, request ID, and applyForId
            DTOICardTaskCountResponse? dTOICardTaskCountResponse = await _basicDetailBL.GetTaskCountICardRequest(userId, Id, applyForId);

            // Return the task count as a JSON response, or null if no data is found
            if (dTOICardTaskCountResponse != null)
            {
                return Json(dTOICardTaskCountResponse);
            }
            else
            {
                return Json(null);
            }
        }

        #endregion


        #region ReportAndReturn /  GetReportReturnCount


        /// <summary>
        /// Action method to display the "Report and Return" page. It retrieves the user's claims
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The ReportAndReturn view with user claims passed in ViewBag.</returns>
        [HttpGet]
        public async Task<IActionResult> ReportAndReturn()
        {
            // Retrieve the user ID from the claims of the current user
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the user from the UserManager service using the user ID
            var user = await userManager.FindByIdAsync(userId);

            // Retrieve all claims associated with the user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Pass the user claims to the view using ViewBag
            ViewBag.UserClaims = UserClaims;

            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Action method to retrieve the report return count based on the provided data.
        /// It performs validation and retrieves the report return count based on hardcoded configuration values.
        /// </summary>
        /// <param name="Data">The data used to retrieve the report return count.</param>
        /// <returns>A JSON response containing the report return count or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> GetReportReturnCount(DTOMHierarchyRequest dTORecord)
        {
            try
            {
                // Retrieve the user ID from the claims of the current user
                int UserId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Get the hardcoded value for ArmedIdForORO from the configuration
                short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]);

                // Check if the ArmedIdForORO is valid (non-zero)
                if (ArmedIdForORO == 0)
                {
                    // Set an error message in TempData and return an internal server error
                    TempData["error"] = "Invalid Input.";
                    TempData.Keep("error");
                    return Json(KeyConstants.InternalServerError);
                }


                var user = await userManager.FindByIdAsync(UserId.ToString());

                // Initialize the DTO session object and retrieve session data
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                // Retrieve the MapUnitId from the session data
                int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
                if (MapUnitId == null)
                {
                    return BadRequest(new { message = "Session expired." });
                }

                // Fetch the map unit details based on the MapUnitId
                DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

                // Retrieve the user's claims using UserManager
                var UserClaims = await userManager.GetClaimsAsync(user);

                // Conditional logic based on the user's claims to modify the request data
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
                {
                    // If user has "Army Level Reports" claim, do not modify the request data
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
                {
                    dTORecord.UnitType = dTOMap.UnitType;

                    // Modify the request data based on unit type
                    if (dTOMap.UnitType == 1)
                    {
                        dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    }
                    else if (dTOMap.UnitType == 2)
                    {
                        dTORecord.ComdId = (byte?)dTOMap.ComdId;
                        dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                    }
                    else if (dTOMap.UnitType == 3)
                    {
                        dTORecord.PsoId = (byte?)dTOMap.PsoId;
                        dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                    }
                }
                else
                {
                    // Modify request data based on unit type
                    if (MapUnitId != null)
                    {
                        dTORecord.UnitType = dTOMap.UnitType;
                        dTORecord.UnitMapId = (int)MapUnitId;
                        dTORecord.ComdId = (byte?)dTOMap.ComdId;
                        dTORecord.CorpsId = (byte?)dTOMap.CorpsId;
                        dTORecord.DivId = (byte?)dTOMap.DivId;
                        dTORecord.BdeId = (byte?)dTOMap.BdeId;
                        dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                        dTORecord.PsoId = (byte?)dTOMap.PsoId;
                        dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                    }
                }

                // Retrieve the report return count based on the provided data and ArmedIdForORO
                var ret = await _reportReturnBL.GetMstepCount(dTORecord, ArmedIdForORO);
                return Json(ret);
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error
                _logger.LogError(1001, ex, "Home->GetReportReturnCount");
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Action method to retrieve the record history based on the provided data.
        /// It processes the request and returns the record history data as a JSON response.
        /// </summary>
        /// <param name="dTORecord">The record history data to retrieve history for.</param>
        /// <returns>A JSON response containing the record history or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> GetRecordHistory([FromBody] DTORecordHistory dTORecord)
        {
            try
            {
                // Retrieve the current user's ID from the claims
                int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userManager.FindByIdAsync(userId.ToString());

                // Initialize the DTO session object and retrieve session data
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                // Retrieve the MapUnitId from the session data
                int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
                if (MapUnitId == null)
                {
                    return BadRequest(new { message = "Session expired." });
                }

                // Fetch the map unit details based on the MapUnitId
                DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

                // Retrieve the user's claims using UserManager
                var UserClaims = await userManager.GetClaimsAsync(user);

                // Conditional logic based on the user's claims to modify the request data
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
                {
                    // If user has "Army Level Reports" claim, do not modify the request data
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
                {
                    dTORecord.Data.UnitType = dTOMap.UnitType;

                    // Modify the request data based on unit type
                    if (dTOMap.UnitType == 1)
                    {
                        dTORecord.Data.ComdId = (byte?)dTOMap.ComdId;
                    }
                    else if (dTOMap.UnitType == 2)
                    {
                        dTORecord.Data.ComdId = (byte?)dTOMap.ComdId;
                        dTORecord.Data.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                    }
                    else if (dTOMap.UnitType == 3)
                    {
                        dTORecord.Data.PsoId = (byte?)dTOMap.PsoId;
                        dTORecord.Data.SubDteId = (byte?)dTOMap.SubDteId;
                    }
                }
                else
                {
                    // Modify request data based on unit type
                    if (MapUnitId != null)
                    {
                        dTORecord.Data.UnitType = dTOMap.UnitType;
                        dTORecord.Data.UnitMapId = (int)MapUnitId;
                        dTORecord.Data.ComdId = (byte?)dTOMap.ComdId;
                        dTORecord.Data.CorpsId = (byte?)dTOMap.CorpsId;
                        dTORecord.Data.DivId = (byte?)dTOMap.DivId;
                        dTORecord.Data.BdeId = (byte?)dTOMap.BdeId;
                        dTORecord.Data.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                        dTORecord.Data.PsoId = (byte?)dTOMap.PsoId;
                        dTORecord.Data.SubDteId = (byte?)dTOMap.SubDteId;
                    }
                }


                // Retrieve the record history based on the provided data
                var ret = await _reportReturnBL.GetRecordHistory(dTORecord);
                return Json(ret);
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error
                _logger.LogError(1001, ex, "Home->GetRecordHistory");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion


        #region Report / GetReportDashboardCount / GetReportData / IsValidMonthYear


        /// <summary>
        /// Action method to display the report page. It retrieves the user's role, claims, and other session data
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The Report view with role and user claims passed in ViewBag.</returns>
        [HttpGet]
        public async Task<IActionResult> Report()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Pass the role to the view using ViewBag
            ViewBag.Role = role;

            // Retrieve the user ID from the claims of the current user
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the user from the UserManager service using the user ID
            var user = await userManager.FindByIdAsync(userId);

            // Retrieve all claims associated with the user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Pass the user claims to the view using ViewBag
            ViewBag.UserClaims = UserClaims;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Action method to retrieve the report dashboard count based on the provided data.
        /// It validates the session and user claims, then fetches the required report dashboard data.
        /// </summary>
        /// <param name="dTORecord">The request data for the report dashboard count.</param>
        /// <returns>A JSON response containing the dashboard count data or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> GetReportDashboardCount([FromBody] DTOMHierarchyRequest dTORecord)
        {
            // Retrieve the current user's ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            // Initialize the DTO session object and retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve the MapUnitId from the session data
            int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
            if (MapUnitId == null)
            {
                return BadRequest(new { message = "Session expired." });
            }

            // Fetch the map unit details based on the MapUnitId
            DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

            // Retrieve the user's claims using UserManager
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Conditional logic based on the user's claims to modify the request data
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
            {
                // If user has "Army Level Reports" claim, do not modify the request data
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
            {
                dTORecord.UnitType = dTOMap.UnitType;

                // Modify the request data based on unit type
                if (dTOMap.UnitType == 1)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                }
                else if (dTOMap.UnitType == 2)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                }
                else if (dTOMap.UnitType == 3)
                {
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }
            else
            {
                // Modify request data based on unit type
                if (MapUnitId != null)
                {
                    dTORecord.UnitType = dTOMap.UnitType;
                    dTORecord.UnitMapId = (int)MapUnitId;
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.CorpsId = (byte?)dTOMap.CorpsId;
                    dTORecord.DivId = (byte?)dTOMap.DivId;
                    dTORecord.BdeId = (byte?)dTOMap.BdeId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }

            // Return the report dashboard count data as a JSON response
            return Json(await _reportReturnBL.GetReportDashboardCount(dTORecord));
        }


        /// <summary>
        /// Action method to retrieve the report data based on the provided filter criteria.
        /// It validates the session, claims, and input data before fetching the report data.
        /// </summary>
        /// <param name="dTORecord">The request data for the report.</param>
        /// <returns>A JSON response containing the report data or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> GetReportData([FromBody] DTODataTablesRequestForReport dTORecord)
        {
            // Retrieve the current user's ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            // Initialize the DTO session object and retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve the MapUnitId from the session data
            int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
            if (MapUnitId == null)
            {
                return BadRequest(new { message = "Session expired." });
            }

            // Fetch the map unit details based on the MapUnitId
            DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

            // Retrieve the user's claims using UserManager
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Conditional logic based on the user's claims to modify the request data
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
            {
                // If user has "Army Level Reports" claim, do not modify the request data
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
            {
                dTORecord.UnitType = dTOMap.UnitType;

                // Modify the request data based on unit type
                if (dTOMap.UnitType == 1)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                }
                else if (dTOMap.UnitType == 2)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                }
                else if (dTOMap.UnitType == 3)
                {
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }
            else
            {
                // Modify request data based on unit type
                if (MapUnitId != null)
                {
                    dTORecord.UnitType = dTOMap.UnitType;
                    dTORecord.UnitMapId = (int)MapUnitId;
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.CorpsId = (byte?)dTOMap.CorpsId;
                    dTORecord.DivId = (byte?)dTOMap.DivId;
                    dTORecord.BdeId = (byte?)dTOMap.BdeId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }

            // If the user selected "MonthlyProcessed", validate the MonthYear input and retrieve the report data
            if (dTORecord.Choice == "MonthlyProcessed")
            {
                if (!String.IsNullOrEmpty(dTORecord.MonthYear))
                {
                    bool isValid = IsValidMonthYear(dTORecord.MonthYear);

                    if (isValid)
                    {
                        var ret = await _reportReturnBL.GetReportData(dTORecord);
                        return Json(ret);
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid input: Month and Year are required." });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Month and Year are required." });
                }
            }

            // If no errors, retrieve the report data
            try
            {
                var ret = await _reportReturnBL.GetReportData(dTORecord);
                return Json(ret);
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error
                _logger.LogError(1001, ex, "Home->GetReportData");
                return BadRequest(new { message = "Internal Server Error" });
            }
        }


        /// <summary>
        /// Validates the provided month/year input in the "MM/yyyy" format.
        /// The method checks if the input is a valid date and falls within the range of the last 2 years and the current month.
        /// </summary>
        /// <param name="input">The month/year input to validate in "MM/yyyy" format.</param>
        /// <returns>True if the input is valid and within the allowed range, false otherwise.</returns>
        public static bool IsValidMonthYear(string input)
        {
            // Try to parse the input as a date with format "dd/MM/yyyy"
            if (!DateTime.TryParseExact("01/" + input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime selectedDate))
            {
                return false; // Invalid format
            }

            DateTime today = DateTime.Today;

            // Calculate the minimum valid date (2 years ago, January 1st)
            DateTime minDate = new DateTime(today.Year - 2, 1, 1);

            // Calculate the maximum valid date (current month)
            DateTime maxDate = new DateTime(today.Year, today.Month, 1);

            // Truncate the selected date to the start of the month (for comparison)
            DateTime selectedMonthStart = new DateTime(selectedDate.Year, selectedDate.Month, 1);

            // Return whether the selected month is within the valid range
            return selectedMonthStart >= minDate && selectedMonthStart <= maxDate;
        }

        #endregion


        #region Request / GetRegistrationApplyfor / InitiateRequest

        /// <summary>
        /// Action method for the Request page. It retrieves the current user's claims and role,
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The Request view with the user's claims and role passed in ViewBag.</returns>
        [HttpGet]
        public async Task<IActionResult> Request()
        {
            // Retrieve the user ID from the claims
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // Get all claims associated with the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;

            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);
            ViewBag.Role = role;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Action method to retrieve the registration data based on the "ApplyFor" property.
        /// It fetches data from the registration business logic layer (BL).
        /// </summary>
        /// <param name="Data">The registration data containing the "ApplyFor" property.</param>
        /// <returns>A JSON response containing the data fetched based on the "ApplyFor" value.</returns>
        [HttpPost]
        public async Task<IActionResult> GetRegistrationApplyfor(MRegistration Data)
        {
            // Fetch the registration data based on the "ApplyFor" property
            return Json(await _registrationBL.GetByApplyFor(Data));
        }

        /// <summary>
        /// Action method to initiate a new request. It retrieves the user's role and passes it to the view.
        /// </summary>
        /// <returns>The InitiateRequest view with the user's role passed in ViewBag.</returns>
        [HttpGet]
        public IActionResult InitiateRequest()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);
            ViewBag.Role = role;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        #endregion


        #region VisitorStats /InitializeCounterFile /UpdateCounterFile /UpdateStats /GetIso8601WeekOfYear /LoadStatsFromFile

        /// <summary>
        /// Action method to retrieve visitor stats, including total visitors, today's count, this week's count, and this month's count.
        /// It checks the visitor's IP address, user-agent, and session status to update or load stats.
        /// </summary>
        /// <returns>A JSON response containing the visitor stats.</returns>
        [HttpPost]
        public JsonResult VisitorStats()
        {
            var model = new DTOVisitorCounterResponse();
            var userIP = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            var currentTime = DateTime.UtcNow;

            // Check if the IP is in the ignored list
            if (IgnoredIPs.Contains(userIP))
            {
                LoadStatsFromFile(model); // Load stats without updating
                return Json(model);
            }

            // Check if the user has already visited this session
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString(SessionKey)))
            {
                // Initialize session hit count
                _httpContextAccessor.HttpContext.Session.SetString(SessionKey, "true");

                // Check if the counter file exists and initialize or update the stats accordingly
                if (!System.IO.File.Exists(CounterFilePath))
                {
                    InitializeCounterFile(currentTime, userIP);
                }
                else
                {
                    UpdateCounterFile(currentTime, userIP);
                }
            }

            // Load the updated stats from the file
            LoadStatsFromFile(model);
            return Json(model);
        }

        /// <summary>
        /// Initializes the counter file with stats for the day, week, month, and year, as well as tracking the visitor's IP and timestamp.
        /// </summary>
        /// <param name="currentTime">The current UTC time used to initialize the stats.</param>
        /// <param name="userIP">The visitor's IP address used to log the visit.</param>
        private void InitializeCounterFile(DateTime currentTime, string userIP)
        {
            var data = $"{currentTime.DayOfYear}:1||0:0||{GetIso8601WeekOfYear(currentTime)}:1||{currentTime.Month}:1||{currentTime.Year}:1||1||1||{currentTime.Ticks}\n" +
                       $"{userIP}||{currentTime.Ticks}\n";

            System.IO.File.WriteAllText(CounterFilePath, data);
        }

        /// <summary>
        /// Updates the counter file with the new visitor's stats and adds the visitor's IP and timestamp.
        /// </summary>
        /// <param name="currentTime">The current UTC time used to update the stats.</param>
        /// <param name="userIP">The visitor's IP address used to log the visit.</param>
        private void UpdateCounterFile(DateTime currentTime, string userIP)
        {
            var lines = System.IO.File.ReadAllLines(CounterFilePath);
            var stats = lines[0].Split("||");

            // Update stats with new visitor data
            var updatedStats = UpdateStats(stats, currentTime);

            // Replace the first line with updated stats
            lines[0] = string.Join("||", updatedStats);

            // Add the new IP and timestamp
            lines = lines.Concat(new[] { $"{userIP}||{currentTime.Ticks}" }).ToArray();

            // Write back to the file
            System.IO.File.WriteAllLines(CounterFilePath, lines);
        }

        /// <summary>
        /// Updates the stats with new data for the day, week, month, and total visitors.
        /// </summary>
        /// <param name="stats">The current stats array to update.</param>
        /// <param name="currentTime">The current time used to calculate the updated stats.</param>
        /// <returns>An updated stats array.</returns>
        private string[] UpdateStats(string[] stats, DateTime currentTime)
        {
            var todayStats = stats[0].Split(':');
            var weekStats = stats[2].Split(':');
            var monthStats = stats[3].Split(':');
            var allVisitors = int.Parse(stats[5]);

            // Update "today" stats
            var todayDayOfYear = int.Parse(todayStats[0]);
            var todayCount = int.Parse(todayStats[1]);

            if (todayDayOfYear == currentTime.DayOfYear)
            {
                todayCount++;
            }
            else
            {
                todayCount = 1; // Reset for a new day
            }

            // Update "week" stats
            var currentWeek = GetIso8601WeekOfYear(currentTime);
            var storedWeek = int.Parse(weekStats[0]);
            var weekCount = int.Parse(weekStats[1]);

            if (storedWeek == currentWeek)
            {
                weekCount++;
            }
            else
            {
                weekCount = 1; // Reset for a new week
            }

            // Update "month" stats
            var storedMonth = int.Parse(monthStats[0]);
            var monthCount = int.Parse(monthStats[1]);

            if (storedMonth == currentTime.Month)
            {
                monthCount++;
            }
            else
            {
                monthCount = 1; // Reset for a new month
            }

            // Update total visitor count
            allVisitors++;

            // Reassemble updated stats
            stats[0] = $"{currentTime.DayOfYear}:{todayCount}";
            stats[2] = $"{currentWeek}:{weekCount}";
            stats[3] = $"{currentTime.Month}:{monthCount}";
            stats[5] = allVisitors.ToString();

            return stats;
        }

        /// <summary>
        /// Loads the visitor stats from the file and populates the model with the data for today, this week, this month, and the total visitors.
        /// </summary>
        /// <param name="model">The DTO model to store the loaded stats.</param>
        private void LoadStatsFromFile(DTOVisitorCounterResponse model)
        {
            var lines = System.IO.File.ReadAllLines(CounterFilePath);
            var stats = lines[0].Split("||");

            var todayStats = stats[0].Split(':');
            var weekStats = stats[2].Split(':');
            var monthStats = stats[3].Split(':');

            model.Today = int.Parse(todayStats[1]);
            model.Week = int.Parse(weekStats[1]);
            model.Month = int.Parse(monthStats[1]);
            model.MonthName = new DateTime(1, int.Parse(monthStats[0]), 1).ToString("MMMM");
            model.Total = int.Parse(stats[5]);
        }

        /// <summary>
        /// Retrieves the ISO 8601 week number for a given date.
        /// </summary>
        /// <param name="time">The date for which the week number is to be calculated.</param>
        /// <returns>The ISO 8601 week number.</returns>
        private static int GetIso8601WeekOfYear(DateTime time)
        {
            var day = (int)time.DayOfWeek;
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                time,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        #endregion


        #region  GetICardProcessReport / GetApplyCardDetails

        /// <summary>
        /// Action method to retrieve the I-Card process report based on the provided request data.
        /// </summary>
        /// <param name="Data">The request data used to fetch the report.</param>
        /// <returns>A JSON response containing the I-Card process report data.</returns>
        public async Task<IActionResult> GetICardProcessReport(DTOMHierarchyRequest Data)
        {
            try
            {
                // Retrieve the report based on the provided data
                var ret = await _reportReturnBL.GetReportForm11(Data);
                return Json(ret);
            }
            catch (Exception ex)
            {
                // Return an internal server error response if any exception occurs
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Action method to retrieve the card details based on the provided data.
        /// It retrieves user ID from the session and uses it to fetch the corresponding card details.
        /// </summary>
        /// <param name="Data">The data containing information to fetch the card details.</param>
        /// <returns>A JSON response containing the card details for the current user.</returns>
        public async Task<IActionResult> GetApplyCardDetails(DTOApplyCardDetailsRequest Data)
        {
            // Retrieve the current user's ID from the claims
            Data.UserId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Fetch the card details based on the provided data
            return Json(await _registrationBL.GetApplyCardDetails(Data));
        }

        #endregion

        #region ReportCard / GetReportCardDashboardCount
        [HttpGet]
        public async Task<IActionResult> ReportCard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Pass the role to the view using ViewBag
            ViewBag.Role = role;

            // Retrieve the user ID from the claims of the current user
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the user from the UserManager service using the user ID
            var user = await userManager.FindByIdAsync(userId);

            // Retrieve all claims associated with the user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Pass the user claims to the view using ViewBag
            ViewBag.UserClaims = UserClaims;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> GetReportCardDashboardCount([FromBody] DTOMHierarchyRequest dTORecord)
        {
            // Retrieve the current user's ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            // Initialize the DTO session object and retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve the MapUnitId from the session data
            int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
            if (MapUnitId == null)
            {
                return BadRequest(new { message = "Session expired." });
            }

            // Fetch the map unit details based on the MapUnitId
            DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

            // Retrieve the user's claims using UserManager
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Conditional logic based on the user's claims to modify the request data
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
            {
                // If user has "Army Level Reports" claim, do not modify the request data
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
            {
                dTORecord.UnitType = dTOMap.UnitType;

                // Modify the request data based on unit type
                if (dTOMap.UnitType == 1)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                }
                else if (dTOMap.UnitType == 2)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                }
                else if (dTOMap.UnitType == 3)
                {
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }
            else
            {
                // Modify request data based on unit type
                if (MapUnitId != null)
                {
                    dTORecord.UnitType = dTOMap.UnitType;
                    dTORecord.UnitMapId = (int)MapUnitId;
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.CorpsId = (byte?)dTOMap.CorpsId;
                    dTORecord.DivId = (byte?)dTOMap.DivId;
                    dTORecord.BdeId = (byte?)dTOMap.BdeId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }

            // Return the report dashboard count data as a JSON response
            return Json(await _reportReturnBL.GetReportCardDashboardCount(dTORecord));
        }
        
        [HttpPost]
        public async Task<IActionResult> GetReportCardData([FromBody] DTODataTablesRequestForReportCard dTORecord)
        {
            // Retrieve the current user's ID from the claims
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            // Initialize the DTO session object and retrieve session data
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve the MapUnitId from the session data
            int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
            if (MapUnitId == null)
            {
                return BadRequest(new { message = "Session expired." });
            }

            // Fetch the map unit details based on the MapUnitId
            DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

            // Retrieve the user's claims using UserManager
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Conditional logic based on the user's claims to modify the request data
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
            {
                // If user has "Army Level Reports" claim, do not modify the request data
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
            {
                dTORecord.UnitType = dTOMap.UnitType;

                // Modify the request data based on unit type
                if (dTOMap.UnitType == 1)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                }
                else if (dTOMap.UnitType == 2)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                }
                else if (dTOMap.UnitType == 3)
                {
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }
            else
            {
                // Modify request data based on unit type
                if (MapUnitId != null)
                {
                    dTORecord.UnitType = dTOMap.UnitType;
                    dTORecord.UnitMapId = (int)MapUnitId;
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.CorpsId = (byte?)dTOMap.CorpsId;
                    dTORecord.DivId = (byte?)dTOMap.DivId;
                    dTORecord.BdeId = (byte?)dTOMap.BdeId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }

            // If no errors, retrieve the report data
            try
            {
                var ret = await _reportReturnBL.GetReportCardData(dTORecord);
                return Json(ret);
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error
                _logger.LogError(1001, ex, "Home->GetReportCardData");
                return BadRequest(new { message = "Internal Server Error" });
            }
        }

        #endregion
    }
}
