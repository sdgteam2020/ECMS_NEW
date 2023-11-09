using BusinessLogicsLayer.Token;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ConfigUserController : Controller
    {
        public readonly iGetTokenBL _iGetTokenBL; 
        public ConfigUserController(iGetTokenBL iGetTokenBL)
        {
            _iGetTokenBL=iGetTokenBL;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetTokenDetails(string ApiName)
        {

            var data = await _iGetTokenBL.GetTokenDetails(ApiName);
            return Json(data);
        }
    }
}
