using BusinessLogicsLayer;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Posting;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain.Model;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    public class PostingController : Controller
    {
        private readonly IPostingBL _iPostingBL;
        public PostingController(IPostingBL postingBL)
        {
            _iPostingBL = postingBL;
        }
        public IActionResult PostingIn()
        {
            return View();
        }
        public async Task<IActionResult> GetPostingIn(string ArmyNo)
        {
            var data = await _iPostingBL.GetArmyDataForPostingOut(ArmyNo);
            return Json(data);
        }
        public async Task<IActionResult> GetAllPostingOut()
        {
            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = await _iPostingBL.GetAllPostingHistory(userid);
           
            return View(data);
        }
      
        public async Task<IActionResult> SavePoasingOut(TrnPostingOut dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    
                        if (dTO.Id > 0)
                        {
                        await _iPostingBL.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                           await _iPostingBL.Add(dTO);

                           await _iPostingBL.UpdateForPosting(dTO);

                        return Json(KeyConstants.Save);


                        }
                   

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }
    }
}
