using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Posting;
using BusinessLogicsLayer.Service;
using DataAccessLayer;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Healpers;
using Web.WebHelpers;

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
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector _protector;
        public PostingController(IPostingBL postingBL, IApplCloseBL iApplCloseBL, ITrnICardRequestBL trnICardRequestBL, IService service, ILogger<PostingController> logger, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _iPostingBL = postingBL;
            _iApplCloseBL = iApplCloseBL;
            _iTrnICardRequestBL = trnICardRequestBL;
            this.service = service;
            _logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            _protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<IActionResult> PostingIn(string? EncId)
        {
            //var postingOutDetails = new DTOPostingOutDetailByIdResponse();
            //if (!string.IsNullOrEmpty(EncId))
            //{
            //    ViewBag.isEdit = true;
            //    string Id = _protector.Unprotect(EncId);
            //    postingOutDetails = await _iPostingBL.GetPostingDetailById(Id);
            //}
            //else
            //{
            //    ViewBag.isEdit = false;
            //}
            return View();
        }
        public async Task<IActionResult> GetPostingIn(string ArmyNo)
        {
            DTOPostingInResponse data = await _iPostingBL.GetArmyDataForPostingOut(ArmyNo);
            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
            string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", data.PhotoImagePath);

            if (System.IO.File.Exists(sourcePathPhoto))
            {
                data.PhotoImagePath = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
            }
            return Json(data);
        }
        public async Task<IActionResult> GetAllPostingOut()
        {
            int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = await _iPostingBL.GetAllPostingHistory(userid);
           
            return View(data);
        }

        public async Task<IActionResult> GetPostingOutDetails()
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
                SessionHeplers.SetObject(HttpContext.Session, "PostingType", PostingTy);
                SessionHeplers.SetObject(HttpContext.Session, "Type", t);
                ViewBag.Type = t;
                ViewBag.PostingType = PostingTy;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingController=>GetPostingOutWithType.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPostingOutWithType(DTODataTablesRequest dTO)
        {
            List<DTOPostingOutDetilsResponse> dTOPostingOutDetilsResponses = new List<DTOPostingOutDetilsResponse>();
            var responseData = new DTODataTablesResponse<DTOPostingOutDetilsResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOPostingOutDetilsResponses
            };
            try
            {
                string PostingType = SessionHeplers.GetObject<string>(HttpContext.Session, "PostingType");
                int Type = SessionHeplers.GetObject<int>(HttpContext.Session, "Type");
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                responseData = await _iPostingBL.GetPostingOutWithType(dTO, userid, Type, PostingType);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Posting->GetAllPostingOutWithType");
            }

            return Json(responseData);
        }


        public async Task<IActionResult> SavePoasingOut(TrnPostingOut dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
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

        //public async Task<IActionResult> UpdateDispatchDetails() { 
        
        //}

        public async Task<IActionResult> ApplicationClose()
        {
            return View();  
        }
        public async Task<IActionResult> SaveApplicationClose(TrnApplClose dTO)
        {
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }
                dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;
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
