using BusinessLogicsLayer;
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
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using System.Text;
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

        //constructor to initialize dependencies and configuration settings.
        public HomeController(IRegistrationBL registrationBL, IUserProfileBL userProfileBL,
            IBasicDetailBL basicDetailBL, INotificationBL notificationBL, ITrnICardRequestBL iTrnICardRequestBL,
            IHomeBL home, IRecordOfficeBL recordOfficeBL, SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor,
            IReportReturnBL reportReturnBL, IService service, IConfiguration configuration, IMapUnitBL mapUnitBL
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
        }

        /// <summary>
        /// Retrieves the role of the user from the session.
        /// If the session contains a valid token, it retrieves the role name from the session object.
        /// </summary>
        /// <returns>The role name from the session, or an empty string if no valid session is found.</returns>
        private string GetSessionValue()
        {
            // Initialize a new DtoSession object
            DtoSession? dtoSession = new DtoSession();

            // Check if the session contains a valid "Token"
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                // Retrieve the session object "Token" and deserialize it into the dtoSession object
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Retrieve the role name from the session, or return an empty string if not available
            string role = dtoSession != null ? dtoSession.RoleName : "";
            return role;
        }

        /// <summary>
        /// Action method that displays the "Contact Us" page.
        /// </summary>
        /// <returns>The "Contact Us" view.</returns>
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
        public IActionResult Index()
        {
            // Retrieve the user's role from the session
            string role = GetSessionValue();

            // Pass the role to the view through ViewBag
            ViewBag.Role = role;

            // Return the Index view
            return View();
        }

        /// <summary>
        /// Action method to display the "Register User" page. It retrieves the user's session data
        /// (specifically the UnitId) and passes it to the view.
        /// </summary>
        /// <returns>The "Register User" view with the UnitId passed in ViewBag.</returns>
        [Authorize]
        public IActionResult RegisterUser()
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

            // Pass the UnitId to the view using ViewBag
            ViewBag.UnitId = UnitId;

            // Return the RegisterUser view
            return View();
        }

        /// <summary>
        /// Action method for the Dashboard page. It retrieves the user's role, claims, and user information
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The Dashboard view with role and user claims passed in ViewBag.</returns>
        public async Task<IActionResult> Dashboard()
        {
            // Retrieve the user's role from the session
            string role = GetSessionValue();

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

            // Return the Dashboard view
            return View();
        }

        #region Report Return
        /// <summary>
        /// Action method to display the report page. It retrieves the user's role, claims, and other session data
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The Report view with role and user claims passed in ViewBag.</returns>
        public async Task<IActionResult> Report()
        {
            // Retrieve the user's role from the session
            string role = GetSessionValue();

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

            // Return the Report view
            return View();
        }

        /// <summary>
        /// Action method to display the "Report and Return" page. It retrieves the user's claims
        /// and passes them to the view for display.
        /// </summary>
        /// <returns>The ReportAndReturn view with user claims passed in ViewBag.</returns>
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

            // Return the ReportAndReturn view
            return View();
        }

        /// <summary>
        /// Action method to retrieve the report return count based on the provided data.
        /// It performs validation and retrieves the report return count based on hardcoded configuration values.
        /// </summary>
        /// <param name="Data">The data used to retrieve the report return count.</param>
        /// <returns>A JSON response containing the report return count or an error message.</returns>
        public async Task<IActionResult> GetReportReturnCount(DTOMHierarchyRequest Data)
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

                // Retrieve the report return count based on the provided data and ArmedIdForORO
                var ret = await _reportReturnBL.GetMstepCount(Data, ArmedIdForORO);
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

        public async Task<IActionResult> GetReportDashboardCount([FromBody] DTOMHierarchyRequest dTORecord)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
            if (MapUnitId == null)
            {
                return BadRequest(new { message = "Session expired." });
            }
            DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
            {

            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
            {
                dTORecord.UnitType = dTOMap.UnitType;

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

            return Json(await _reportReturnBL.GetReportDashboardCount(dTORecord));
        }
        [HttpPost]
        public async Task<IActionResult> GetReportData([FromBody] DTODataTablesRequestForReport dTORecord)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());

            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int? MapUnitId = dtoSession != null ? dtoSession.UnitId : null;
            if (MapUnitId == null)
            {
                return BadRequest(new { message = "Session expired." });
            }
            DTOMapUnitResponse dTOMap = await _mapUnitBL.GetALLByUnitMapId((int)MapUnitId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Army Level Reports"))
            {

            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Fmn Level Reports"))
            {
                dTORecord.UnitType = dTOMap.UnitType;

                if (dTOMap.UnitType == 1)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                }
                else if (dTOMap.UnitType == 2)
                {
                    dTORecord.ComdId = (byte?)dTOMap.ComdId;
                    dTORecord.FmnBranchID = (byte?)dTOMap.FmnBranchID;
                }
                else if(dTOMap.UnitType == 3)
                {
                    dTORecord.PsoId = (byte?)dTOMap.PsoId;
                    dTORecord.SubDteId = (byte?)dTOMap.SubDteId;
                }
            }
            else
            {
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

        
            if (dTORecord.Choice == "MonthlyProcessed")
            {
                if (!String.IsNullOrEmpty( dTORecord.MonthYear))
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
            try
            {
                var ret = await _reportReturnBL.GetReportData(dTORecord);
                return Json(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Home->GetRecordHistory");
                return BadRequest(new { message = "Internal Server Error" });
            }

        }
        public static bool IsValidMonthYear(string input)
        {
            // Try parse as MM/yyyy
            if (!DateTime.TryParseExact("01/" + input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime selectedDate))
            {
                return false; // Invalid format
            }

            DateTime today = DateTime.Today;

            // Calculate min date as 2 years ago (Jan of that year)
            DateTime minDate = new DateTime(today.Year - 2, 1, 1);
            DateTime maxDate = new DateTime(today.Year, today.Month, 1); // Current month only

            // Truncate selected date to month/year
            DateTime selectedMonthStart = new DateTime(selectedDate.Year, selectedDate.Month, 1);

            return selectedMonthStart >= minDate && selectedMonthStart <= maxDate;
        }
        #endregion
        public async Task<IActionResult> SubDashboard()
        {
            string role = GetSessionValue();

            ViewBag.Role = role;
            return View();
        }
        public async Task<IActionResult> DashboardUserMgt()
        {
            string role = GetSessionValue();
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
            int TDMId = dtoSession!=null? dtoSession.TrnDomainMappingId : 0;
            int UserId = dtoSession!=null?dtoSession.UserId:0;

            DTOGetROByTDMIdResponse? dTOGetROByUserIdResponse = await _recordOfficeBL.GetROByTDMId(TDMId);
            if(dTOGetROByUserIdResponse== null)
            {
                ViewBag.ROFound = 0;
            }
            else if(dTOGetROByUserIdResponse.IsRO==true || dTOGetROByUserIdResponse.IsORO ==true || dTOGetROByUserIdResponse.TDMId == TDMId)
            {
                ViewBag.ROFound = 1;
            }
            else
            {
                ViewBag.ROFound = 0;
            }

            ViewBag.UnitId = UnitId;
            ViewBag.Role = role;
            return View();
        }
        public IActionResult InitiateRequest()
        {
            ViewBag.Role = GetSessionValue();
            return View();
        }
        public async Task<IActionResult> RequestDashboard(string Id)
        {
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            try
            {
                string role = GetSessionValue();
                var base64EncodedBytes = Convert.FromBase64String(Id);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                ViewBag.Type = decodedString;
                ViewBag.Role = role;
                if (decodedString == "Posting Out" || decodedString == "Posting In")
                {
                    ViewBag.PreviousLink = "DashboardUserMgt";
                }
                else
                {
                    ViewBag.PreviousLink = "SubDashboard";
                }
                return View();
            }
            catch (FormatException ex)
            {
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id}", Id);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = ex.Message;
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        public async Task<IActionResult> Task()
        {
            string role = GetSessionValue();
            ViewBag.Role = role;

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;

            return View();
        }
      
      
        public async Task<IActionResult> GetICardProcessReport(DTOMHierarchyRequest Data)
        {
            try
            {
                var ret = await _reportReturnBL.GetReportForm11(Data);
                return Json(ret);
            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> MyTask(string Id)
        {
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToActionPermanent("ContactUs", "Home");
            }
            try
            {
                string role = GetSessionValue();
                var base64EncodedBytes = Convert.FromBase64String(Id);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                ViewBag.Type = decodedString;
                ViewBag.Role = role;

                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);

                // UserManager service GetClaimsAsync method gets all the current claims of the user
                var UserClaims = await userManager.GetClaimsAsync(user);
                ViewBag.UserClaims = UserClaims;

                return View();
            }
            catch (FormatException ex)
            {
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id}", Id);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = ex.Message;
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        public async Task<IActionResult> Request()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;

            string role = GetSessionValue();
            ViewBag.Role = role;
            return View();
        }

        public async Task<IActionResult> GetRegistrationApplyfor(MRegistration Data)
        {
            return Json(await _registrationBL.GetByApplyFor(Data));

        }
        public async Task<IActionResult> GetApplyCardDetails(DTOApplyCardDetailsRequest Data)
        {
            Data.UserId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(await _registrationBL.GetApplyCardDetails(Data));

        }
        public async Task<IActionResult> GetTaskCountICardRequest(int Id,int applyForId)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DTOICardTaskCountResponse? dTOICardTaskCountResponse = await _basicDetailBL.GetTaskCountICardRequest(userId, Id, applyForId);
            if (dTOICardTaskCountResponse != null)
            {
                return Json(dTOICardTaskCountResponse);
            }
            else
            {
                return Json(null);
            }
        }
        public async Task<IActionResult> SaveNotification(MTrnNotification Data)
        {
            try
            {
                int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                Data.SentAspNetUsersId = userId;
               
                await _INotificationBL.UpdatePrevious(Data);

                await _INotificationBL.Add(Data);

                int requestUserId = await _ITrnICardRequestBL.GetUserIdByRequestId(Data.RequestId);
                Data.NotificationId = 0;
                Data.SentAspNetUsersId = requestUserId;
                Data.ReciverAspNetUsersId = requestUserId;

                await _INotificationBL.Add(Data);
                return Json(1);
            }
            catch (Exception ex)
            {
                return Json(0);
            }

        }
        public async Task<IActionResult> GetNotification(int TypeId, int applyForId)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<DTONotificationResponse>? dTONotificationResponses = await _basicDetailBL.GetNotification(userId, TypeId, applyForId);
            if (dTONotificationResponses != null)
            {
                return Json(dTONotificationResponses);
            }
            else
            {
                return Json(null);
            }
        }
        public async Task<IActionResult> GetNotificationRequestId(int TypeId,int applyForId)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<DTONotificationResponse>? dTONotificationResponses = await _basicDetailBL.GetNotificationRequestId(userId, TypeId, applyForId);
            if (dTONotificationResponses != null)
            {
                return Json(dTONotificationResponses);
            }
            else
            {
                return Json(null);
            }
        } 
        public async Task<IActionResult> UpdateNotification(MTrnNotification Data)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
          
            Data.SentAspNetUsersId = userId;
            return Json(await _INotificationBL.UpdateRead(Data));

        }
        public async Task<IActionResult> GetTaskBoardCount()
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());
            int MapUnitId = 0;
            int TDMId = 0;
            byte ClaimValue = 0;


            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);

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

            return Json(await _home.GetTaskBoardCount(MapUnitId, ClaimValue, TDMId));
        }
        public async Task<IActionResult> GetDashboardCount()
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]);
            //if (ArmedIdForORO == 0) ArmedIdForORO = 56;

            DTOApplFwdConditionRequest? dTOApplFwdCondition = _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>() ?? new DTOApplFwdConditionRequest
            {
                MPRSO = new MPRSO(),
                MP6F = new MP6F(),
                MP6A = new MP6A()
            };

            if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) ||
                string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name) || dTOApplFwdCondition.MP6A.RankOrderby == 0 || ArmedIdForORO == 0)
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Json(KeyConstants.InternalServerError);
            }

            return Json(await _home.GetDashBoardCount(userId, dTOApplFwdCondition, ArmedIdForORO));
        }
        public async Task<IActionResult> GetRequestDashboardCount(string Id)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int UnitMapId = dtoSession != null ? dtoSession.UnitId : 0;
            return Json(await _home.GetRequestDashboardCount(userId, Id, UnitMapId));
        }
        public async Task<IActionResult> GetSubDashboardCount()
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;

            return Json(await _home.GetSubDashboardCount(userId, UnitId));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllRegisterUser(int UnitId)
        {
            try
            {
                return Json(await _home.GetAllRegisterUser(UnitId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Home->GetAllRegisterUser");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [HttpPost]
        public async Task<IActionResult> GetDashboardUserMgtCount(int UnitId)
        {
            try
            {
                int UserId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _home.GetDashboardUserMgtCount(UnitId, UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Home->GetDashboardUserMgtCount");
                return Json(KeyConstants.InternalServerError);
            }

        }
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

                if (!System.IO.File.Exists(CounterFilePath))
                {
                    InitializeCounterFile(currentTime, userIP);
                }
                else
                {
                    UpdateCounterFile(currentTime, userIP);
                }
            }

            LoadStatsFromFile(model);
            return Json(model);
        }
        private void InitializeCounterFile(DateTime currentTime, string userIP)
        {
            var data = $"{currentTime.DayOfYear}:1||0:0||{GetIso8601WeekOfYear(currentTime)}:1||{currentTime.Month}:1||{currentTime.Year}:1||1||1||{currentTime.Ticks}\n" +
                       $"{userIP}||{currentTime.Ticks}\n";

            System.IO.File.WriteAllText(CounterFilePath, data);
        }

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

        private static int GetIso8601WeekOfYear(DateTime time)
        {
            var day = (int)time.DayOfWeek;
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                time,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

    }
}
