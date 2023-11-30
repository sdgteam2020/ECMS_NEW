using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Token;
using DataTransferObject.Domain;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Net;
using System.Security.Claims;
using Web.WebHelpers;

namespace Web.Controllers
{
    public class ConfigUserController : Controller
    {
        public readonly iGetTokenBL _iGetTokenBL; private readonly IUserProfileBL _userProfileBL;
        public ConfigUserController(iGetTokenBL iGetTokenBL, IUserProfileBL userProfileBL)
        {
            _iGetTokenBL=iGetTokenBL;
            _userProfileBL=userProfileBL;
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
        [HttpPost]
        public async Task<IActionResult> GotoDashboard(string ICNO)
        {

            SessionHeplers.SetObject(HttpContext.Session, "ArmyNo", ICNO);
            return Json(1);
           
        }
        [HttpPost]
        public async Task<IActionResult> GetTokenArmyNo(string Id)
        {
            try
            {
                string armyno = SessionHeplers.GetObject<string>(HttpContext.Session, "ArmyNo");
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var army = await _userProfileBL.GetByMArmyNo(armyno, userid);
                DtoSession dtoSession = new DtoSession();
                if(army.Count>0l)
                {
                    dtoSession.ICNO = armyno;
                    dtoSession.UserId = army[0].UserId ;
                }
                SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);


                return Json(SessionHeplers.GetObject<string>(HttpContext.Session, "ArmyNo"));

            }
            catch (Exception ex) {
                return Json(0);
             }
        }
       }
}
