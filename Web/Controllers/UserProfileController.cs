using BusinessLogicsLayer;
using BusinessLogicsLayer.Master;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileBL _userProfileBL;
        private readonly IUserProfileMappingBL _userProfileMappingBL;
       
       
        public UserProfileController(IUserProfileBL userProfileBL, IUserProfileMappingBL userProfileMappingBL)
        {
            _userProfileBL=userProfileBL;
            _userProfileMappingBL = userProfileMappingBL;
        }
        public IActionResult Profile()
        {
            return View();
        }
        public async Task<IActionResult> SaveUserProfile(MUserProfile dTO)
        {
            try
            {
                if (dTO.UnitId != 0 && dTO.RankId != 0 && dTO.ApptId != 0)
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

                                await _userProfileBL.Add(dTO);
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
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetAll(Id, userid));
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
        public async Task<IActionResult> GetAllByArmyNo(string ArmyNo)
        {
            try
            {
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(await _userProfileBL.GetAllByArmyNo(ArmyNo, userid));
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
    }
}
