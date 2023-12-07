using BusinessLogicsLayer;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Master;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.WebHelpers;

namespace Web.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileBL _userProfileBL;
        private readonly IUserProfileMappingBL _userProfileMappingBL;
        public readonly IDomainMapBL _iDomainMapBL;

        public UserProfileController(IUserProfileBL userProfileBL, IUserProfileMappingBL userProfileMappingBL, IDomainMapBL domainMapBL)
        {
            _userProfileBL=userProfileBL;
            _userProfileMappingBL = userProfileMappingBL;
            _iDomainMapBL = domainMapBL;
        }
        public IActionResult Profile()
        {
            return View();
        }
        public async Task<IActionResult> SaveUserProfile(MUserProfile dTO)
        {
            try
            {
                if (dTO.RankId != 0 && dTO.ApptId != 0)
                {
                    dTO.IsActive = true;
                    dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    dTO.UpdatedOn = DateTime.Now;
                    int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (ModelState.IsValid)
                    {
                        if (!await _userProfileBL.GetByArmyNo(dTO, userid))
                        {
                            if (dTO.UserId > 0)
                            {
                                _userProfileBL.Update(dTO);
                                return Json(KeyConstants.Update);
                            }
                            else
                            {

                                dTO= await _userProfileBL.AddWithReturn(dTO);
                                TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                                trnDomainMapping.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                trnDomainMapping= await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping);
                                if(trnDomainMapping!=null && dTO.UserId !=0)
                                {
                                    trnDomainMapping.UserId = dTO.UserId;
                                    _iDomainMapBL.Update(trnDomainMapping);
                                }

                                    return Json(KeyConstants.Save);


                            }
                        }
                        else
                        {
                            return Json(KeyConstants.Exists);
                        }

                    }
                    else
                    {

                        return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                    }
                }
                else
                {
                    return Json(KeyConstants.IncorrectData);
                }
            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }
        public async Task<IActionResult> MappingIOGSOUNIT(MMappingProfile dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                
                if (ModelState.IsValid)
                {
                    //if (!await _userProfileBL.GetByArmyNo(dTO))
                    //{
                        if (dTO.Id > 0)
                        {
                        await _userProfileMappingBL.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await _userProfileMappingBL.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    //}
                    //else
                    //{
                    //    return Json(KeyConstants.Exists);
                    //}

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAll(string Id)
        {
            try
            {
                int DomainId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetAll(DomainId, 0));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByArmyNo(string ArmyNo)
        {
            try
            {
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetByArmyNo(ArmyNo, userid));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetDataForFwd(string Name,int TypeId, int StepId)
        {
            try
            {
                //int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                int UnitId=SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UnitId;
                return Json(await _userProfileBL.GetDataForFwd(StepId, UnitId, Name, TypeId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByMasterArmyNo(string ArmyNo)
        {
            try
            {
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetByMArmyNo(ArmyNo, userid));

            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByAspnetUserIdBy(TrnDomainMapping Data)
        {
            try
            {
               
                return Json(await _iDomainMapBL.GetByAspnetUserIdBy(Data));

            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetByRequestId(int RequestId)
        {
            try
            {
               
                return Json(await _userProfileBL.GetByRequestId(RequestId));

            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }

    }
}
