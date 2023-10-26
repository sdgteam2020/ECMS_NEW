using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetIsToken(DTOTokenRequest Token)
        {
            DTOTokenResponse dTOTokenResponse = new DTOTokenResponse();
            dTOTokenResponse.IsToken = false;
            return Json(dTOTokenResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GetTokenDetails()
        {

            DTOTokenResponse dTOTokenResponse = new DTOTokenResponse();
            dTOTokenResponse.IsToken = false;
            return Json(dTOTokenResponse);
        }
    }
}
