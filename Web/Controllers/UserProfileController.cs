using BusinessLogicsLayer;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Master;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.WebHelpers;

namespace Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileBL _userProfileBL;
        private readonly IUserProfileMappingBL _userProfileMappingBL;
        public readonly IDomainMapBL _iDomainMapBL;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileBL userProfileBL, ILogger<UserProfileController> logger, IUserProfileMappingBL userProfileMappingBL, IDomainMapBL domainMapBL)
        {
            _userProfileBL=userProfileBL;
            _userProfileMappingBL = userProfileMappingBL;
            _iDomainMapBL = domainMapBL;
            _logger = logger;
        }
        public IActionResult Profile()
        {
            return View();
        }
        public async Task<IActionResult> SaveUserProfile(MUserProfile dTO)
        {
            try
            {
                if (dTO.RankId != 0)
                {
                    dTO.IsActive = true;
                    dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    dTO.UpdatedOn = DateTime.Now;
                    int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (ModelState.IsValid)
                    {
                        if (dTO.UserId > 0)
                        {
                            bool? result = await _userProfileBL.FindByArmyNoWithUserId(dTO.ArmyNo, dTO.UserId);
                            if (result != null)
                            {
                                if (result == true)
                                {
                                    return Json(KeyConstants.Exists);
                                }
                                else
                                {
                                    await _userProfileBL.Update(dTO);
                                    return Json(KeyConstants.Update);
                                }
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else
                        {
                            bool? result = await _userProfileBL.FindByArmyNo(dTO.ArmyNo);
                            if (result != null)
                            {
                                if (result == true)
                                {
                                    return Json(KeyConstants.Exists);
                                }
                                else
                                {
                                    dTO = await _userProfileBL.AddWithReturn(dTO);
                                    TrnDomainMapping? trnDomainMapping = new TrnDomainMapping();
                                    trnDomainMapping.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                    trnDomainMapping = await _iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping.AspNetUsersId);
                                    if (trnDomainMapping != null && dTO.UserId != 0)
                                    {
                                        trnDomainMapping.UserId = dTO.UserId;
                                        await _iDomainMapBL.Update(trnDomainMapping);
                                    }
                                    return Json(KeyConstants.Save);
                                }
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
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
        public async Task<IActionResult> GetByArmyNoOrAspnetuserId(string ArmyNo,int userid)
        {
            try
            {
                if(userid==0)
                 userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                return Json(await _userProfileBL.GetByArmyNo(ArmyNo, userid));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }

        [Authorize]
        public async Task<IActionResult> GetProfileByUserId(int UserId)
        {
            try
            {
                return Json(await _userProfileBL.GetProfileByUserId(UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UserProfile->GetProfileByUserId");
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetDataForFwd(string Name,int TypeId, int StepId,int UnitId)
        {
            try
            {
                //int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if(UnitId==0)
                 UnitId=SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UnitId;

                return Json(await _userProfileBL.GetDataForFwd(StepId, UnitId, Name, TypeId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetOffrsByUnitMapId(int id,int UnitId)
        {
            try
            {
                //int DomainMapId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if(UnitId==0)
                {
                     UnitId = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UnitId;
                    return Json(await _userProfileBL.GetOffrsByUnitMapId(UnitId));
                }
                else
                {
                    return Json(await _userProfileBL.GetOffrsByUnitMapId(UnitId));
                }
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
               
                return Json(await _iDomainMapBL.GetByAspnetUserIdBy(Data.AspNetUsersId));

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
        public async Task<IActionResult> CheckArmyNoInUserProfile(string ArmyNo)
        {
            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            DTOProfileResponse dTOProfileResponse = await _userProfileBL.CheckArmyNoInUserProfile(ArmyNo, userid);
            return Json(dTOProfileResponse);
        }
        public async Task<IActionResult> GetTopByArmyNo(string ArmyNo)
        {
            try
            {
                return Json(await _userProfileBL.GetTopByArmyNo(ArmyNo));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }


    }
}
