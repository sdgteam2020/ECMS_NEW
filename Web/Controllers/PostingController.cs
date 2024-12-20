using BusinessLogicsLayer;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Posting;
using BusinessLogicsLayer.Service;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain.Model;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Web.Controllers
{
    [Authorize]
 
    public class PostingController : Controller
    {
        private readonly IPostingBL _iPostingBL;
        private readonly IApplCloseBL _iApplCloseBL;
        private readonly ITrnICardRequestBL _iTrnICardRequestBL;
        private readonly IService service;
        private readonly ILogger<PostingController> _logger;
        public PostingController(IPostingBL postingBL, IApplCloseBL iApplCloseBL, ITrnICardRequestBL trnICardRequestBL, IService service, ILogger<PostingController> logger)
        {
            _iPostingBL = postingBL;
            _iApplCloseBL = iApplCloseBL;
            _iTrnICardRequestBL = trnICardRequestBL;
            this.service = service;
            _logger = logger;
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
        public async Task<IActionResult> GetPostingOutWithType(string Type,string PostingType)
        {
            if (string.IsNullOrEmpty(Type) || !service.IsValidBase64(Type) || string.IsNullOrEmpty(PostingType) || !service.IsValidBase64(PostingType))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(Type);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                var PostingTy = Encoding.UTF8.GetString(Convert.FromBase64String(PostingType));
                int t = Convert.ToInt32(decodedString);
                ViewBag.Type = t;
                ViewBag.PostingType = PostingTy;

                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var data = await _iPostingBL.GetPostingOutWithType(userid, t, PostingTy);

                return View(data);
            }
            catch (FormatException ex)
            {
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Type: {Type} & PostingType: {PostingType} ", Type, PostingType);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingController=>GetPostingOutWithType.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
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
                        //await _iPostingBL.Add(dTO);
                        //adding and update both done by UpdateForPosting
                        bool result = await _iPostingBL.UpdateForPosting(dTO);
                        if (result == true)
                        {
                            return Json(KeyConstants.Save);
                        }
                        else
                        {
                            return Json(KeyConstants.IncorrectData);
                        }
                    }
                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }

        public async Task<IActionResult> ApplicationClose()
        {
            return View();  
        }
        public async Task<IActionResult> SaveApplicationClose(TrnApplClose dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {

                    //if (dTO.Id > 0)
                    //{
                    //    await _iApplCloseBL.Update(dTO);
                    //    return Json(KeyConstants.Update);
                    //}
                    //else
                    //{
                    if(!await _iApplCloseBL.RequestIdExists(dTO))
                    {
                        //await _iApplCloseBL.Add(dTO);
                        //await _iTrnICardRequestBL.UpdateStatus(dTO.RequestId);
                        bool reuslt = await _iApplCloseBL.ApplCloseWithUpdateStatus(dTO);
                        if (reuslt == true)
                        {
                            return Json(KeyConstants.Save);
                        }
                        else
                        {
                            return Json(KeyConstants.IncorrectData);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }
                       


                    //}


                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }
        }
        public async Task<ActionResult> AppCloseList(string Id, string jcoor)
        {
            int retint = 0;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id) || string.IsNullOrEmpty(jcoor) || !service.IsValidBase64(jcoor))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var base64EncodedBytes = Convert.FromBase64String(Id);
                    var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                    retint = Convert.ToInt32(decodedString);
                }

                if (retint == 1)
                {
                    ViewBag.Title = "List of Closed Appl";
                }

                if (string.IsNullOrEmpty(jcoor))
                {
                    var allrecord = await Task.Run(() => _iPostingBL.GetAppClosedList(Convert.ToInt32(userId), 1));
                    return View(allrecord);
                }
                else
                {
                    var allrecord = await Task.Run(() => _iPostingBL.GetAppClosedList(Convert.ToInt32(userId), 2));
                    return View(allrecord);
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id} & jcoor: {jcoor} ", Id, jcoor);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
    }
}
