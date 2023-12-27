using BusinessLogicsLayer;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Registration;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly IRegistrationBL _registrationBL;
        private readonly IBasicDetailBL _basicDetailBL;
        public HomeController(IRegistrationBL registrationBL, IBasicDetailBL basicDetailBL)
        {
            _registrationBL = registrationBL;
            _basicDetailBL= basicDetailBL;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult MyTask()
        {
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
            return Json(await _registrationBL.GetApplyCardDetails(Data));

        }
        public async Task<IActionResult> GetTaskCountICardRequest(int Id)
        {
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(await _basicDetailBL.GetTaskCountICardRequest(userId, Id));

        }

    }
}
