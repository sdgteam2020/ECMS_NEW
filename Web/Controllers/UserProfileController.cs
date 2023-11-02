using BusinessLogicsLayer;
using BusinessLogicsLayer.Master;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using Microsoft.AspNetCore.Mvc;

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
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await _userProfileBL.GetByArmyNo(dTO))
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
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }
        public async Task<IActionResult> MappingIOGSOUNIT(MMappingProfile dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    //if (!await _userProfileBL.GetByArmyNo(dTO))
                    //{
                        if (dTO.Id > 0)
                        {
                            _userProfileMappingBL.Update(dTO);
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
                return Json(await _userProfileBL.GetAll(Id));
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
                return Json(await _userProfileBL.GetByArmyNo(ArmyNo));
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
                return Json(await _userProfileBL.GetByMArmyNo(ArmyNo));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
    }
}
