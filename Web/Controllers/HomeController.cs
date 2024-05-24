using BusinessLogicsLayer;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.Home;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.RecordOffice;
using BusinessLogicsLayer.Registration;
using BusinessLogicsLayer.User;
using DapperRepo.Core.Constants;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Security.Claims;
using Web.WebHelpers;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

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
        public HomeController(IRegistrationBL registrationBL, IUserProfileBL userProfileBL,IBasicDetailBL basicDetailBL, INotificationBL notificationBL, ITrnICardRequestBL iTrnICardRequestBL, IHomeBL home, IRecordOfficeBL recordOfficeBL, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userProfileBL = userProfileBL;
            _registrationBL = registrationBL;
            _basicDetailBL = basicDetailBL;
            _INotificationBL = notificationBL;
            _ITrnICardRequestBL = iTrnICardRequestBL;
            _home = home;
            _recordOfficeBL = recordOfficeBL;
            _logger = logger;
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
        public IActionResult Index()
        {
            string role = this.User.FindFirstValue(ClaimTypes.Role);
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
        public async Task<IActionResult> DashboardAsync()
        {
            string role = GetSessionValue();

            ViewBag.Role = role;    
            return View();
        }
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
        public async Task<IActionResult> RequestDashboardAsync(string Id)
        {
            string role = GetSessionValue();
            var base64EncodedBytes = System.Convert.FromBase64String(Id);
            var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            ViewBag.Type = ret;    
            ViewBag.Role = role;
            if(ret == "Posting Out" || ret == "Posting In")
            {
                ViewBag.PreviousLink = "DashboardUserMgt";
            }
            else
            {
                ViewBag.PreviousLink = "SubDashboard";
            }
            return View();
        }
        public async Task<IActionResult> Task()
        {
            string role = GetSessionValue();
            ViewBag.Role = role;
            return View();
        }
        public IActionResult MyTask(string Id)
        {
            string role = GetSessionValue();
            var base64EncodedBytes = System.Convert.FromBase64String(Id);
            var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            ViewBag.Type = ret;
            ViewBag.Role = role;
            return View();
        }
        public IActionResult Request()
        {
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
            return Json(await _basicDetailBL.GetTaskCountICardRequest(userId, Id, applyForId));

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
            return Json(await _basicDetailBL.GetNotification(userId, TypeId, applyForId));

        }
        public async Task<IActionResult> GetNotificationRequestId(int TypeId,int applyForId)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(await _basicDetailBL.GetNotificationRequestId(userId, TypeId,applyForId));

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
            return Json(await _home.GetDashBoardCount(userId));
        }
        public async Task<IActionResult> GetRequestDashboardCount(string Id)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(await _home.GetRequestDashboardCount(userId, Id));
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

    }
}
