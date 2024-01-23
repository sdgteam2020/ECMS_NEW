using BusinessLogicsLayer;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.Home;
using BusinessLogicsLayer.Registration;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRegistrationBL _registrationBL;
        private readonly IBasicDetailBL _basicDetailBL;
        private readonly INotificationBL _INotificationBL;
        private readonly ITrnICardRequestBL _ITrnICardRequestBL;
        private readonly IHomeBL _home;
        public HomeController(IRegistrationBL registrationBL, IBasicDetailBL basicDetailBL, INotificationBL notificationBL, ITrnICardRequestBL iTrnICardRequestBL, IHomeBL home)
        {
            _registrationBL = registrationBL;
            _basicDetailBL = basicDetailBL;
            _INotificationBL = notificationBL;
            _ITrnICardRequestBL = iTrnICardRequestBL;
            _home = home;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            string role = this.User.FindFirstValue(ClaimTypes.Role);

            ViewBag.Role = role;    
            return View();
        }
        [Authorize(Roles = "User")]
        public IActionResult MyTask()
        {
            return View();
        }
        [Authorize(Roles = "User")]
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
    }
}
