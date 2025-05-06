using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Home;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.RecordOffice;
using BusinessLogicsLayer.Registration;
using BusinessLogicsLayer.ReportReturn;
using BusinessLogicsLayer.Service;
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
using System.Security.Claims;
using System.Text;
using Web.WebHelpers;


namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRegistrationBL _registrationBL;
        private readonly IBasicDetailBL _basicDetailBL;
        private readonly INotificationBL _INotificationBL;
        private readonly IUserProfileBL _userProfileBL;
        private readonly ITrnICardRequestBL _ITrnICardRequestBL;
        private readonly IHomeBL _home;
        private readonly IRecordOfficeBL _recordOfficeBL;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly IService service;
        public readonly IReportReturnBL _reportReturnBL;
        private const string CounterFilePath = "wwwroot/counter.txt";
        private const string SessionKey = "SessionHit";
        private readonly string[] IgnoredIPs = { "127.0.0.2", "127.0.0.3" }; // Add IPs to ignore
        private readonly IConfiguration _configuration;
        public HomeController(IRegistrationBL registrationBL, IUserProfileBL userProfileBL,
            IBasicDetailBL basicDetailBL, INotificationBL notificationBL, ITrnICardRequestBL iTrnICardRequestBL,
            IHomeBL home, IRecordOfficeBL recordOfficeBL, SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor,
            IReportReturnBL reportReturnBL, IService service, IConfiguration configuration
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
        }
        private string GetSessionValue()
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            string role = dtoSession != null ? dtoSession.RoleName : "";
            return role;
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Index()
        {
            string role = GetSessionValue();
            ViewBag.Role = role;
            
            return View();
        }
        [Authorize]
        public IActionResult RegisterUser()
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
            ViewBag.UnitId = UnitId;
            return View();
        }
        public async Task<IActionResult> Dashboard()
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
        #region Report Return
        public async Task<IActionResult> ReportAndReturn()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;
            return View();
        }

        public async Task<IActionResult> GetReportReturnCount(DTOMHierarchyRequest Data)
        {
            try
            {
                int UserId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]);
                //if (ArmedIdForORO == 0) ArmedIdForORO = 56;

                if (ArmedIdForORO == 0)
                {
                    TempData["error"] = "Invalid Input.";
                    TempData.Keep("error");
                    return Json(KeyConstants.InternalServerError);
                }


                var ret =await _reportReturnBL.GetMstepCount(Data, ArmedIdForORO);
                return Json(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Home->GetReportReturnCount");
                return Json(KeyConstants.InternalServerError);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetRecordHistory([FromBody] DTORecordHistory dTORecord)
        {
            try
            {
                var ret = await _reportReturnBL.GetRecordHistory(dTORecord);
                return Json(ret);
            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Home->GetRecordHistory");
                return Json(KeyConstants.InternalServerError); 
            }
            
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
        public async Task<IActionResult> GetDashboardCount()
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(userId.ToString());
            int MapUnitId = 0;
            
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;

            bool Claim = false;

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }


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

            return Json(await _home.GetDashBoardCount(userId, dTOApplFwdCondition, ArmedIdForORO, MapUnitId, Claim));
        }
        public async Task<IActionResult> GetRequestDashboardCount(string Id)
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(await _home.GetRequestDashboardCount(userId, MapUnitId, Id));
        }
        public async Task<IActionResult> GetSubDashboardCount()
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(await _home.GetSubDashboardCount(userId));
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
