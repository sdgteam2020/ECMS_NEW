using BusinessLogicsLayer;
using BusinessLogicsLayer.Registration;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly IRegistrationBL _registrationBL;
        public HomeController(IRegistrationBL registrationBL)
        {
            _registrationBL = registrationBL;
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
    }
}
