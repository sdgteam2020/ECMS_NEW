using BusinessLogicsLayer;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Token;
using BusinessLogicsLayer.Unit;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Drawing;
using System.Net;
using System.Security.Claims;
using Web.WebHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    public class ConfigUserController : Controller
    {
        public readonly iGetTokenBL _iGetTokenBL; 
        private readonly IUserProfileBL _userProfileBL;
        private readonly UserManager<ApplicationUser> userManager;
        public readonly IDomainMapBL _iDomainMapBL;
        public readonly IMapUnitBL _IMapUnitBL;
        public ConfigUserController(iGetTokenBL iGetTokenBL, IUserProfileBL userProfileBL, UserManager<ApplicationUser> userManager, IDomainMapBL domainMapBL, IMapUnitBL mapUnitBL)
        {
            _iGetTokenBL=iGetTokenBL;
            _userProfileBL=userProfileBL;
            this.userManager=userManager;
            _iDomainMapBL=domainMapBL;
            _IMapUnitBL=mapUnitBL;
        }
        public async Task<IActionResult> IndexAsync()
        {
            //this.User.FindFirstValue(ClaimTypes.NameIdentifier);


            ViewBag.DomainId = this.User.FindFirstValue(ClaimTypes.Name);
            ViewBag.Role = this.User.FindFirstValue(ClaimTypes.Role);
            TrnDomainMapping dTO = new TrnDomainMapping();
            dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            dTO = await _iDomainMapBL.GetByAspnetUserIdBy(dTO);
            if (dTO==null)
            {
                return View();
            }
            else if(dTO != null && dTO.UserId == null)
            {
                DTOMapUnitResponse dTOMapUnitResponse = await _IMapUnitBL.GetALLByUnitMapId(dTO.UnitId);
                ViewBag.TrnDomain = dTOMapUnitResponse;
                return View();
            }
            else 
            {
                
                var army = await _userProfileBL.Get(Convert.ToInt32(dTO.UserId));
                DtoSession dtoSession = new DtoSession();
                if (army!=null)
                {
                    dtoSession.ICNO = army.ArmyNo;
                    dtoSession.UserId = army.UserId;
                    dtoSession.UnitId=dTO.UnitId;

                    TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                    trnDomainMapping.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping);
                    dtoSession.TrnDomainMappingId = trnDomainMapping.Id;
                }
                SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);
                //SessionHeplers.SetObject(HttpContext.Session, "ArmyNo", dtoSession.ICNO);
                // var data=  await _iDomainMapBL.GetByDomainIdbyUnit(dTO);
                return RedirectToActionPermanent("Dashboard", "Home");
            }



        }
        [HttpPost]
        public async Task<IActionResult> CheckProfileExist(int Id)
        {
            try
            {
                if (this.User.FindFirstValue(ClaimTypes.Role) != "Admin")
                {
                    TrnDomainMapping dTO = new TrnDomainMapping();
                    dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var data = await _iDomainMapBL.GetByDomainIdbyUnit(dTO);
                    return Json(data);
                }
                else
                {
                    TrnDomainMapping dTO = new TrnDomainMapping();
                    dTO.UserId = 1;
                    return Json(dTO);
                }

               
            }
            catch (Exception ex)
            {
                return Json(null);
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> GetTokenDetails(string ApiName)
        {

            var data = await _iGetTokenBL.GetTokenDetails(ApiName);
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveMapping(TrnDomainMapping dTO,string ICNO)
        {

            dTO.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                // dTO.IsActive = true;
                // dTO.Updatedby = 1;
                //dTO.UpdatedOn = DateTime.Now;
              
                DtoSession dtoSession = new DtoSession();
               
                dtoSession.ICNO = ICNO;
                dtoSession.UnitId = dTO.UnitId;
                
                if (ModelState.IsValid)
                {
                    if (!await _iDomainMapBL.GetByDomainId(dTO))
                    {
                        if (dTO.Id > 0)
                        {
                            _iDomainMapBL.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await _iDomainMapBL.Add(dTO);
                            TrnDomainMapping trnDomainMapping1 = new TrnDomainMapping();
                            trnDomainMapping1.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                            trnDomainMapping1 = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping1);
                            if (trnDomainMapping1 != null)
                                dtoSession.TrnDomainMappingId = trnDomainMapping1.Id;

                            SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);


                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                        trnDomainMapping =await _iDomainMapBL.GetByDomainIdbyUnit(dTO);
                        trnDomainMapping.UnitId = dTO.UnitId;
                        trnDomainMapping.UserId = dTO.UserId!=null? dTO.UserId : null;
                        await _iDomainMapBL.Update(trnDomainMapping);


                        TrnDomainMapping trnDomainMapping1 = new TrnDomainMapping();
                        trnDomainMapping1.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        trnDomainMapping1 = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping1);
                        if (trnDomainMapping1 != null)
                            dtoSession.TrnDomainMappingId = trnDomainMapping1.Id;

                        SessionHeplers.SetObject(HttpContext.Session, "Token", dtoSession);
                        return Json(KeyConstants.Update);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

           // return Json(1);
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

                DtoSession dtoSession = new DtoSession();
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                return Json(dtoSession);

            }
            catch (Exception ex) {
                return Json(0);
             }
        }
       }
}
