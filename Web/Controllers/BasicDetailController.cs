using AutoMapper;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System;
using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Requests;
using EntityFramework.Exceptions.Common;
using DataTransferObject.Response;
using BusinessLogicsLayer.Service;
using BusinessLogicsLayer.Bde;
using Web.WebHelpers;
using DataTransferObject.ViewModels;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.Master;
using System.Text;
using BusinessLogicsLayer.Unit;
using DapperRepo.Core.Constants;
using System.IO.Compression;
using BusinessLogicsLayer.TrnLoginLog;
using Web.Healpers;
using System.Xml.Serialization;
using System.Xml.Linq;
using BusinessLogicsLayer.TrnICardHold;
using DataTransferObject.Domain.Master;

namespace Web.Controllers
{
    [Authorize]
    public class BasicDetailController : Controller
    {
        //private readonly ApplicationDbContext context, contextTransaction;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IStepCounterBL iStepCounterBL;
        private readonly ITrnICardRequestBL iTrnICardRequestBL;
        private readonly IDomainMapBL iDomainMapBL;
        private readonly ITrnFwnBL iTrnFwnBL;
        private readonly IBasicDetailBL basicDetailBL;
        private readonly IBasicUploadBL basicuploadBL;
        private readonly IBasicAddressBL basicAddressBL;
        private readonly IBasicinfoBL basicinfoBL;
        private readonly IRankBL rankBL;
        private readonly IBasicDetailTempBL basicDetailTempBL;
        private readonly IService service;
        private readonly IMapper _mapper;
        private readonly IMapUnitBL mapUnitBL;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<BasicDetailController> _logger;
        private readonly INotificationBL _INotificationBL;
        private readonly IMasterBL _IMasterBL;
        private readonly ITrnLoginLogBL _iTrnLoginLogBL;
        private readonly IICardHoldBL _iICardHoldBL;
        private readonly IConfiguration _configuration;
        public DateTime dateTimenow;
        public BasicDetailController(IConfiguration configuration,IBasicDetailBL basicDetailBL, IMapUnitBL mapUnitBL, IBasicDetailTempBL basicDetailTempBL, IService service, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger, IStepCounterBL iStepCounterBL, 
                              ITrnFwnBL iTrnFwnBL, ITrnICardRequestBL iTrnICardRequestBL, IDomainMapBL iDomainMapBL
            ,IBasicUploadBL basicUploadBL, IBasicAddressBL basicAddressBL, IBasicinfoBL basicinfoBL, IRankBL rankBL, INotificationBL notificationBL, IMasterBL masterBL
           , ITrnLoginLogBL iTrnLoginLogBL, IICardHoldBL iICardHoldBL)
        {
            _configuration = configuration;
            this.basicDetailBL = basicDetailBL;
            this.basicDetailTempBL = basicDetailTempBL;
            this.service = service;
            this._mapper = mapper;
            this.mapUnitBL= mapUnitBL;
            //this.context = context;
            //this.contextTransaction = context;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            _logger = logger;
            this.iStepCounterBL = iStepCounterBL;
            this.iTrnFwnBL = iTrnFwnBL;
            this.iTrnICardRequestBL = iTrnICardRequestBL;
            this.iDomainMapBL = iDomainMapBL;
            this.basicinfoBL = basicinfoBL;
            this.basicAddressBL = basicAddressBL;
            this.basicuploadBL = basicUploadBL;
            this.rankBL=rankBL;
            _INotificationBL = notificationBL;
            _IMasterBL = masterBL;
            _iTrnLoginLogBL = iTrnLoginLogBL;
            _iICardHoldBL = iICardHoldBL;
        }
        private string GetSessionValue()
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            string role = dtoSession != null ? dtoSession.RoleName : "";
            return role;
        }
        //[Authorize(Roles = "DteAdmin")]
        [Authorize(Policy = "FlagICardApplPolicy")]
        public async Task<IActionResult> SaveICardRequestHold(MTrnICardHold dTO)
        {
            try
            {
                DtoSession? sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                if(sessiondata!=null)
                {
                    dTO.UserId = sessiondata.UserId;
                }

                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                if (ModelState.IsValid)
                {
                    if (!await _iICardHoldBL.GetByRequestId(dTO))
                    {
                        if (dTO.ICardHoldId > 0)
                        {
                            await _iICardHoldBL.Update(dTO);
                            return Json(KeyConstants.Save);
                        }
                        else
                        {
                            await _iICardHoldBL.Add(dTO);
                            return Json(KeyConstants.Update);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->SaveICardRequestHold");
                return Json(KeyConstants.InternalServerError);
            }
                
        }
        //[Authorize(Roles = "DteAdmin")]
        [Authorize(Policy = "FlagICardApplPolicy")]
        public async Task<IActionResult> GetTopArmyNoFromICardRequest(string ArmyNo)
        {
            try
            {
                return Json(await basicDetailBL.GetTopArmyNoFromICardRequest(ArmyNo));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->GetTopArmyNoFromICardRequest");
                return Json(KeyConstants.InternalServerError);
            }
        }
        //[Authorize(Roles = "DteAdmin")]
        [Authorize(Policy = "FlagICardApplPolicy")]
        public async Task<IActionResult> GetBDetailByRequestId(int RequestId)
        {
            try
            {
                return Json(await basicDetailBL.GetBDetailByRequestId(RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->GetBDetailByRequestId");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Policy = "ViewFlaggedICardApplPolicy")]
        [HttpGet]
        public async Task<IActionResult> ICardRequestHold()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;
            return View();
        }
        [Authorize(Policy = "ViewFlaggedICardApplPolicy")]
        [HttpPost]
        public async Task<IActionResult> GetAllICardRequestHold()
        {
            try
            {
                return Json(await basicDetailBL.GetAllICardRequestHold());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->GetAllICardRequestHold");
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<ActionResult> Index(string Id,string jcoor)
        {
           
            MTrnNotification noti = new MTrnNotification();
            int retint = 0;int type = 1;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            int stepcounter = 0;
            noti.ReciverAspNetUsersId = userId;
            noti.DisplayId = 0;

            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    var decodedBytes = Convert.FromBase64String(Id);
                    var decodedString = Encoding.UTF8.GetString(decodedBytes);
                    retint = Convert.ToInt32(decodedString);
                    stepcounter = retint;
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "Invalid Base64 Id: {Id}", Id);
                    TempData["error"] = "Invalid Input.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            
            ViewBag.Id = retint;
            ViewBag.jcoor = jcoor;

            if (retint == 0)
            {
                ViewBag.Title = "List of Drafted Appl";
                // type = 2; stepcounter = 2;
            }
            else if (retint == 1)
            {
                // request from DashBoard
                ViewBag.Title = "List of Drafted Appl";
                // type = 2; stepcounter = 2;
            }
            else if (retint == 11)
            {
                retint = 1;
                stepcounter = 1;
                // request from Task Board
                ViewBag.Title = "List of Drafted Appl";
                // type = 2; stepcounter = 2;
            }

            else if (retint == 2)
            { ViewBag.Title = "I-Card Pending From IO / Superior"; type = 2; stepcounter = 2; }
            else if (retint == 22)
            {
                // request from DashBoard
                ViewBag.Title = "I-Card Rejectd From IO / Superior"; type = 1; stepcounter = 7; 
            }
            else if (retint == 2222)
            {
                // request from Task Board
                ViewBag.Title = "I-Card Rejectd From IO / Superior"; type = 1; stepcounter = 7;
            }
            else if (retint == 222)
            { ViewBag.Title = "I-Card Approved From IO / Superior"; type = 3; stepcounter = 2; }
            else if (retint == 3)
            {
                ViewBag.Title = "I-Card Pending From RO / ORO";
                type = 2; stepcounter = 3;
            }
            else if (retint == 33)
            {
                ViewBag.Title = "I-Card Rejectd From RO / ORO";
                type = 1; stepcounter = 8;
            }
            else if (retint == 333)
            {
                ViewBag.Title = "I-Card Approved From RO / ORO";
                type = 3; stepcounter = 4;
            }
            else if (retint == 4)
            { ViewBag.Title = "I-Card Pending From AFSAC Cell"; type = 2; stepcounter = 4; }
            else if (retint == 44)
            { ViewBag.Title = "I-Card Rejectd From AFSAC Cell"; type = 1; stepcounter = 9; }
            else if (retint == 444)
            { ViewBag.Title = "I-Card Approved From AFSAC Cell"; type = 3; stepcounter = 5; }
            else if (retint == 5)
            { ViewBag.Title = "I-Card Pending From HQ 54"; type = 2; stepcounter = 5; }
            else if (retint == 55)
            { ViewBag.Title = "I-Card Rejectd From HQ 54"; type = 1; stepcounter = 10; }
            else if (retint == 555)
            { ViewBag.Title = "I-Card Approved From HQ 54"; type = 2; stepcounter = 5; }
            else if (retint == 888) 
            {
                // request from DashBoard
                ViewBag.Title = "I-Card Submited"; type = 2; stepcounter = 888; 
            }
            else if (retint == 88)
            {
                // request from Task Board
                ViewBag.Title = "I-Card Submited"; type = 2; stepcounter = 888; 
            }
            else if (retint == 777)
            { 
                ViewBag.Title = "I-Card Completed"; type = 2; stepcounter = 777; 
            }
            else if (retint == 77)
            {
                ViewBag.Title = "I-Card Completed"; type = 2; stepcounter = 777;
            }
            else if (retint == 999)
            {
                // request from DashBoard 
                ViewBag.Title = "I-Card Rejectd From IO / Superior, RO / ORO and AFSAC Cell"; type = 2; stepcounter = 999; 
            }
            else if (retint == 99)
            {
                // request from Task Board
                ViewBag.Title = "I-Card Rejectd From IO / Superior, RO / ORO and AFSAC Cell"; type = 2; stepcounter = 999;
            }

            if (stepcounter==0)
            {
                var allrecord = await Task.Run(() => basicDetailBL.GetALLForIcardSttaus(Convert.ToInt32(userId), stepcounter, type, 0));

                _logger.LogInformation(1001, "Index Page Of Basic Detail View");

                return View(allrecord);
            }
           else if (string.IsNullOrEmpty(jcoor))
            {
                var allrecord = await Task.Run(() => basicDetailBL.GetALLForIcardSttaus(Convert.ToInt32(userId), stepcounter, type,1));

                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                noti.DisplayId = stepcounter;
                await _INotificationBL.UpdateRead(noti);

                return View(allrecord);
            }
            else
            {
                var allrecord = await Task.Run(() => basicDetailBL.GetALLForIcardSttaus(Convert.ToInt32(userId), stepcounter, type,2));

                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                noti.DisplayId = stepcounter+10;
                await _INotificationBL.UpdateRead(noti);

                return View(allrecord);
            }
        }
        public async Task<ActionResult> ApprovalForIO(string Id, string jcoor)
        {
            string role = GetSessionValue();
            ViewBag.Role = role;

            var UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(UserId);

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = UserClaims;

            MTrnNotification noti = new MTrnNotification();
            int type = 0; int retint = 0; int stepcounter = 0;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); //SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UserId;
            noti.ReciverAspNetUsersId = userId;
            noti.DisplayId = 0;

            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    var decodedBytes = Convert.FromBase64String(Id);
                    var decodedString = Encoding.UTF8.GetString(decodedBytes);
                    retint = Convert.ToInt32(decodedString);
                    stepcounter = retint;
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "Invalid Base64 Id: {Id}", Id);
                    TempData["error"] = "Invalid Input.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }



            if (retint == 1)
                ViewBag.Title = "List of Register I-Card";
            else if (retint == 2)
            {
                ViewBag.Title = "I-Card For Approval";
                ViewBag.Id = 1;
                type = 2;
                noti.DisplayId = 2;
                
            }
            else if (retint == 22)
            {
                ViewBag.Title = "Rejectd I-Card ";
                type = 1;
                stepcounter = 7;
            }
            else if (retint == 222)
            {
                ViewBag.Title = "Approved I-Card ";
                type = 3; stepcounter = 3;
            }

            else if (retint == 3)
            {
                ViewBag.Title = "I-Card For Approval";
                type = 2;
                ViewBag.Id = 1;
                ViewBag.StepCounter = retint;
            }
            else if (retint == 33)
            {
                ViewBag.Title = "Rejectd I-Card ";
                type = 1; stepcounter = 8;
            }
            else if (retint == 333)
            {
                ViewBag.Title = "Approved I-Card "; type = 3; stepcounter = 4;
            }
            else if (retint == 11)
            {
                ViewBag.Title = "Internal Forward I-Card "; type = 3; stepcounter = 11;
            }
            else if (retint == 4)
            { ViewBag.Title = "I-Card For Export Data"; type = 2; ViewBag.Id = 1; ViewBag.dataexport = 4; }
            else if (retint == 44)
            { ViewBag.Title = "Rejectd I-Card "; type = 1; stepcounter = 9; }
            else if (retint == 444)
            { ViewBag.Title = "Exported I-Card "; type = 3; stepcounter = 5; }
            else if (retint == 5)
            { ViewBag.Title = "Export Data"; type = 2; ViewBag.Id = 1; ViewBag.dataexport = 5; }
            else if (retint == 55)
            { ViewBag.Title = "Rejectd I-Card "; type = 1; stepcounter = 10; }
            else if (retint == 555)
            { ViewBag.Title = "Approved I-Card "; type = 3; stepcounter = 6; }
            else if (retint == 6)
            { ViewBag.Title = "Exported Data"; type = 6; ViewBag.Id = 1; ViewBag.dataexport = 6; }

            if (string.IsNullOrEmpty(jcoor))
            {
                noti.DisplayId = stepcounter;
                ViewBag.jcoor = 1;
                var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), stepcounter, type,1));
                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                await _INotificationBL.UpdateRead(noti);
                return View(allrecord);

            }
            else
            {
                ViewBag.jcoor = 0;
                noti.DisplayId = stepcounter+10;
                var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), stepcounter, type,2));
                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                await _INotificationBL.UpdateRead(noti);
                return View(allrecord);
            }
        }

        [HttpGet]
        public async Task<ActionResult> View(string Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(Id);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(decryptedIntId);
            if (basicDetailCrtAndUpdVM != null)
            {
                basicDetailCrtAndUpdVM.AadhaarNo=basicDetailCrtAndUpdVM.AadhaarNo.Substring((basicDetailCrtAndUpdVM.AadhaarNo.Length-4),4);
                return View(basicDetailCrtAndUpdVM);
            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
        }
        public async Task<IActionResult> GetICardPrintPreviewByRequestId(int RequestId)
        {
            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(RequestId);
            if (basicDetailCrtAndUpdVM != null)
            {
                return Json(basicDetailCrtAndUpdVM);
            }
            else 
            {
                return Json(null);
            }
        }
        [HttpGet]
        public async Task<ActionResult> InaccurateData(string Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(Id);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                int typeId = Convert.ToInt32(decodedString);
                if (typeId == 1 || typeId == 2)
                {
                    var allrecord = await Task.Run(() => basicDetailTempBL.GetALLBasicDetailTemp(Convert.ToInt32(userId), typeId));
                    ViewBag.Title = typeId == 1 ? "List of Inaccurate Data" : "List of Observation Raised";
                    return View(allrecord);
                }
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (FormatException ex)
            {
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id}", Id);
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        [HttpGet]
        public async Task<ActionResult> InaccurateDataView(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userIntId = Convert.ToInt32(userId); // Assuming userId is always a valid integer
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(Id);

                // Validate decrypted Id
                if (!int.TryParse(decryptedId, out decryptedIntId))
                {
                    _logger.LogWarning("Decrypted Id is not a valid integer: {DecryptedId}, UserId: {UserId}", decryptedId, userId);
                    TempData["error"] = "Invalid or tampered request.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
                // Retrieve records asynchronously
                var allRecords = await basicDetailTempBL.GetALLBasicDetailTempByBasicDetailId(userIntId, decryptedIntId);
                return View(allRecords);
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);
                TempData["error"] = ex.Message;
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        [HttpGet]
        public async Task<ActionResult> RequestType()
        {
            var allrecord = await Task.Run(() => basicDetailBL.GetAllICardType());
            return View(allrecord);
        }
        [HttpGet]
        public IActionResult Registration(string Id)
        {
            #region Old Code
            //var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //string decryptedId = string.Empty;
            //ViewBag.OptionsBloodGroup = service.GetBloodGroup();
            //ViewBag.OptionsArmedType = service.GetArmedType();
            //int decryptedIntId = 0;
            //try
            //{
            //    Decrypt the  id using Unprotect method
            //    decryptedId = protector.Unprotect(Id);
            //    decryptedIntId = Convert.ToInt32(decryptedId);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "This error occure because Id value change by user.");
            //    return RedirectToAction("Error", "Error");
            //}
            //DTORegistrationRequest dTORegistrationRequest = new DTORegistrationRequest();
            //dTORegistrationRequest.TypeId = (byte)decryptedIntId;
            //ViewBag.OptionsRegistration = service.GetRegistration();
            //return View(dTORegistrationRequest);
            #endregion End Old Code
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(DTORegistrationRequest model)
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.Updatedby = Convert.ToInt32(userId);
                if (ModelState.IsValid)
                {
                    if (model.SubmitType == 1)
                    {
                        BasicDetail? Data = new BasicDetail();
                        Data =await basicDetailBL.FindServiceNo(model.ServiceNo);
                        if (Data != null)
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            string id = protector.Protect(Data.BasicDetailId.ToString());
                            return RedirectToAction("BasicDetail", "BasicDetail", new { Id  = protector.Protect(Convert.ToString(Data.BasicDetailId)) });
                        }
                        else
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            return RedirectToAction("BasicDetail", "BasicDetail", new { Id= protector.Protect("0") });
                        }
                    }
                    else
                    {
                            BasicDetailTemp basicDetailTemp = new BasicDetailTemp();
                            basicDetailTemp.FName = model.FName;
                            basicDetailTemp.LName = model.LName;
                            basicDetailTemp.NameAsPerRecord = model.NameAsPerRecord;
                            basicDetailTemp.ServiceNo = model.ServiceNo;
                            basicDetailTemp.DOB = model.DOB;
                            basicDetailTemp.DateOfCommissioning = model.DateOfCommissioning;
                            basicDetailTemp.State = model.State;
                            basicDetailTemp.District = model.District;
                            basicDetailTemp.PS = model.PS;
                            basicDetailTemp.PO = model.PO;
                            basicDetailTemp.Tehsil = model.Tehsil;
                            basicDetailTemp.Village = model.Village;
                            basicDetailTemp.PinCode = model.PinCode;
                            basicDetailTemp.Observations = model.Observations;
                            basicDetailTemp.Updatedby = model.Updatedby;
                            basicDetailTemp.RemarksIds = model.RemarksIds;
                            basicDetailTemp.ApplyForId= model.ApplyForId;
                            basicDetailTemp.RegistrationId= model.RegistrationId;
                            basicDetailTemp.TypeId= model.TypeId;
                            basicDetailTemp.RankId= model.RankId;
                            basicDetailTemp.ArmedId = model.ArmedId;
                            basicDetailTemp.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            BasicDetailTemp temp = new BasicDetailTemp();
                            temp =await basicDetailTempBL.GetByArmyNo(model.ServiceNo);

                        if(temp != null && temp.BasicDetailTempId>0)
                        {
                            basicDetailTemp.BasicDetailTempId= temp.BasicDetailTempId;
                            await basicDetailTempBL.Update(basicDetailTemp);
                        }
                        else
                        {
                            await basicDetailTempBL.Add(basicDetailTemp);
                        }
                        TempData["success"] = "Request Submited Successfully.";
                        return RedirectToAction("InaccurateData", "BasicDetail", new {Id = "MQ=="});
                    }
                }
                else
                {
                    var error= ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                    TempData["error"] = error[0][0].ErrorMessage;
                }
            }
            catch (ReferenceConstraintException ex)
            {
                _logger.LogError(1001, ex, "ReferenceConstraintException");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }
            catch (UniqueConstraintException ex)
            {
                _logger.LogError(1002, ex, "UniqueConstraintException");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }
            catch (MaxLengthExceededException ex)
            {
                _logger.LogError(1003, ex, "MaxLengthExceededException");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }
            catch (CannotInsertNullException ex)
            {
                _logger.LogError(1004, ex, "CannotInsertNullException");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }
            catch (NumericOverflowException ex)
            {
                _logger.LogError(1005, ex, "NumericOverflowException");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }
            catch (Exception ex)
            {
                _logger.LogError(1006, ex, "Exception");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }

        end:
            return View(model);
        }
        
        [HttpGet]
        public async Task<ActionResult> BasicDetail(string? Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string decryptedId = string.Empty;
            int decryptedIntId = 0;

            if (Id != null)
            {
                try
                {
                    // Decrypt the  id using Unprotect method
                    decryptedId = protector.Unprotect(Id);

                    // Validate decrypted Id
                    if (!int.TryParse(decryptedId, out decryptedIntId))
                    {
                        _logger.LogWarning("Decrypted Id is not a valid integer: {DecryptedId}, UserId: {UserId}", decryptedId, userId);
                        TempData["error"] = "Invalid Request.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                    TempData["error"] = "Invalid or tampered request.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);
                    TempData["error"] = ex.Message;
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }

            if (Id == null || decryptedId == "0")
            {
                TempData.Keep("Registration");
                DTORegistrationRequest? model = new DTORegistrationRequest();
                if (TempData["Registration"] != null)
                {
                    model = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());
                    if (model.SubmitType == 1)
                    {
                        ViewBag.OptionsUnitId = 0;
                        BasicDetailCrtAndUpdVM dTOBasicDetailCrtRequest = new BasicDetailCrtAndUpdVM();
                        dTOBasicDetailCrtRequest.FName = model.FName;
                        dTOBasicDetailCrtRequest.LName = model.LName;
                        dTOBasicDetailCrtRequest.NameAsPerRecord= model.NameAsPerRecord;
                        dTOBasicDetailCrtRequest.ServiceNo = model.ServiceNo;
                        dTOBasicDetailCrtRequest.DOB = model.DOB;
                        dTOBasicDetailCrtRequest.DateOfCommissioning = model.DateOfCommissioning;
                        dTOBasicDetailCrtRequest.IdenMark1 = model.IdenMark1;
                        dTOBasicDetailCrtRequest.IdenMark2 = model.IdenMark2;
                        ViewBag.OptionsRankId = model.RankId;
                        ViewBag.OptionsArmedId = model.ArmedId;

                        dTOBasicDetailCrtRequest.AadhaarNo = Convert.ToInt64(model.AadhaarNo).ToString("D12");

                        dTOBasicDetailCrtRequest.ApplyForId = model.ApplyForId;
                        dTOBasicDetailCrtRequest.RegistrationId = model.RegistrationId;
                        dTOBasicDetailCrtRequest.TypeId = model.TypeId;


                        dTOBasicDetailCrtRequest.State = model.State;
                        dTOBasicDetailCrtRequest.District = model.District;
                        dTOBasicDetailCrtRequest.PS = model.PS;
                        dTOBasicDetailCrtRequest.PO = model.PO;
                        dTOBasicDetailCrtRequest.Tehsil = model.Tehsil;
                        dTOBasicDetailCrtRequest.Village = model.Village;
                        dTOBasicDetailCrtRequest.PinCode = Convert.ToInt32(model.PinCode);
                        dTOBasicDetailCrtRequest.PermanentAddress = "Village - " + model.Village + ", Post Office-" + model.PO + ", Tehsil- " + model.Tehsil + ", District- " + model.District + ", State- " + model.State + ", Pin Code- " + model.PinCode;

                        return await Task.FromResult(View(dTOBasicDetailCrtRequest));
                    }
                    else
                    {
                        return RedirectToAction("Registration");
                    }
                }
                else
                {
                    return RedirectToAction("Registration");
                }
            }
            else
            {
                BasicDetailCrtAndUpdVM? basicDetailUpdVM = await basicDetailBL.GetBesicDetailForEditById(decryptedIntId);

                if (basicDetailUpdVM != null)
                {
                    ViewBag.OptionsRankId = basicDetailUpdVM.RankId; 
                    ViewBag.OptionsUnitId = basicDetailUpdVM.UnitId; 
                    ViewBag.OptionsArmedId = basicDetailUpdVM.ArmedId;
                    ViewBag.OptionsRegimentalId = basicDetailUpdVM.RegimentalId;
                    ViewBag.OptionsBloodGroupId = basicDetailUpdVM.BloodGroupId;

                    basicDetailUpdVM.BloodGroupId = basicDetailUpdVM.BloodGroupId;
                    basicDetailUpdVM.PermanentAddress = "Village - " + basicDetailUpdVM.Village + ", Post Office-" + basicDetailUpdVM.PO + ", Tehsil- " + basicDetailUpdVM.Tehsil + ", District- " + basicDetailUpdVM.District + ", State- " + basicDetailUpdVM.State + ", Pin Code- " + basicDetailUpdVM.PinCode;



                    string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");
                    string sourcePath = Path.Combine(sourceFolderPhotoPhy, basicDetailUpdVM.PhotoImagePath);
                    string answer = basicDetailUpdVM.PhotoImagePath.Replace(".enc", string.Empty);
                    string destinationPath = Path.Combine(sourceFolderPhotoPhy, answer);

                    //ImageEncryptAndDecrypt.DecryptImageFile(sourcePath, destinationPath);

                    // Call the method to decrypt and return IFormFile
                    IFormFile decryptedFile = ImageEncryptAndDecrypt.DecryptImageToIFormFile(sourcePath, basicDetailUpdVM.ServiceNo);
                    basicDetailUpdVM.Photo_ = decryptedFile;
                    
                    //basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                    
                    
                    
                    
                    basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                    basicDetailUpdVM.EncryptedId = Id;

                    if (TempData["Registration"] != null)
                    {
                        var modelex = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());

                        basicDetailUpdVM.FName = modelex.FName;
                        basicDetailUpdVM.LName = modelex.LName;
                        basicDetailUpdVM.ServiceNo = modelex.ServiceNo;
                        basicDetailUpdVM.DOB = modelex.DOB;
                        basicDetailUpdVM.DateOfCommissioning = modelex.DateOfCommissioning;
                        basicDetailUpdVM.IdenMark1 = modelex.IdenMark1;
                        basicDetailUpdVM.IdenMark2 = modelex.IdenMark2;
                        ViewBag.OptionsRankId = modelex.RankId;
                        basicDetailUpdVM.AadhaarNo = Convert.ToInt64(modelex.AadhaarNo).ToString("D12"); ;// Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
                        basicDetailUpdVM.ApplyForId = modelex.ApplyForId;
                        basicDetailUpdVM.RegistrationId = modelex.RegistrationId;
                        basicDetailUpdVM.TypeId = modelex.TypeId;
                        basicDetailUpdVM.State = modelex.State;
                        basicDetailUpdVM.District = modelex.District;
                        basicDetailUpdVM.PS = modelex.PS;
                        basicDetailUpdVM.PO = modelex.PO;
                        basicDetailUpdVM.Tehsil = modelex.Tehsil;
                        basicDetailUpdVM.Village = modelex.Village;
                        basicDetailUpdVM.PinCode = Convert.ToInt32(modelex.PinCode);
                        basicDetailUpdVM.PermanentAddress = "Village - " + modelex.Village + ", Post Office-" + modelex.PO + ", Tehsil- " + modelex.Tehsil + ", District- " + modelex.District + ", State- " + modelex.State + ", Pin Code- " + modelex.PinCode;

                    }
                    // ViewBag.UnitName = await context.MUnit.FindAsync(basicDetailUpdVM.UnitId);

                    //MRegistration? mRegistration = await context.MRegistration.FindAsync(basicDetailUpdVM.RegistrationId);
                    //basicDetailUpdVM.Type = mRegistration != null ? mRegistration.ApplyForId : 1;

                    return View(basicDetailUpdVM);
                }
                else
                {
                    Response.StatusCode = 404;
                    return View("BasicDetailNotFound", decryptedId.ToString());
                }
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> BasicDetail(BasicDetailCrtAndUpdVM model)
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (model.BasicDetailId > 0)
                {
                    if (model.UnitId == 0)
                    {
                        ModelState.AddModelError("UnitId", "Please Enter Unit Name");
                        goto end;
                    }
                    if (model.ApplyForId != 1 && model.RegimentalId == 0)
                    {
                        ModelState.AddModelError("RegimentalId", "Please Select Regimental ");
                        goto end;
                    }
                    if (ModelState.IsValid)
                    {
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
                        newBasicDetail.DateOfIssue = null;
                        newBasicDetail.Updatedby = Convert.ToInt32(userId);
                        newBasicDetail.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        MTrnUpload mTrnUpload = new MTrnUpload();
                        mTrnUpload.UploadId = model.UploadId;
                        MTrnAddress mTrnAddress = new MTrnAddress();
                        mTrnAddress.State = model.State;
                        mTrnAddress.District = model.District;
                        mTrnAddress.PS = model.PS;
                        mTrnAddress.PO = model.PO;
                        mTrnAddress.Tehsil = model.Tehsil;
                        mTrnAddress.Village = model.Village;
                        mTrnAddress.PinCode = model.PinCode;
                        mTrnAddress.AddressId = model.AddressId;

                        MTrnIdentityInfo mTrnIdentityInfo = new MTrnIdentityInfo();
                        mTrnIdentityInfo.IdenMark1 = model.IdenMark1;
                        mTrnIdentityInfo.IdenMark2 = model.IdenMark2;
                        mTrnIdentityInfo.AadhaarNo = Convert.ToInt64(model.AadhaarNo);
                        mTrnIdentityInfo.BloodGroupId = model.BloodGroupId;
                        mTrnIdentityInfo.Height = model.Height;
                        mTrnIdentityInfo.InfoId = model.InfoId;
                        //MTrnIdentityInfo mTrnIdentityInfo = _mapper.Map<BasicDetailCrtAndUpdVM, MTrnIdentityInfo>(model);
                        //if (model.UnitId == 0)
                        //{
                        //    ModelState.AddModelError("", "Please Enter Unit Name");
                        //}
                        //if (model.ApplyForId != 1 && model.RegimentalId == 0)
                        //{
                        //    ModelState.AddModelError("", "Please Select Regimental ");
                        //}


                        //string sourceFolderPhotoDB = "/WriteReadData/" + "Photo";
                        //string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");
                        string sourceFolderPhotoPhy = Convert.ToString(GetCreateMyFolder(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo")));
                        if (!Directory.Exists(sourceFolderPhotoPhy))
                            Directory.CreateDirectory(sourceFolderPhotoPhy);

                        if (model.Photo_ != null)
                        {
                            string FileName = service.ProcessUploadedFile(model.Photo_, sourceFolderPhotoPhy, model.ServiceNo);

                            string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                            bool result = service.IsValidHeader(path);
                            bool imgcontentresult = service.IsImage(model.Photo_);

                            if (!result || !imgcontentresult)
                            {
                                ModelState.AddModelError("Photo_", "Photo File format not correct");
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                goto end;
                            }

                            mTrnUpload.PhotoImagePath = GetCreateMyFolder() + "/" + FileName;
                        }
                        else
                        {
                            mTrnUpload.PhotoImagePath = model.ExistingPhotoImagePath;
                        }

                        //string sourceFolderSignatureDB = "/WriteReadData/" + "Signature";
                        //string sourceFolderSignaturePhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature");
                        string sourceFolderSignaturePhy = Convert.ToString(GetCreateMyFolder(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature")));
                        if (!Directory.Exists(sourceFolderSignaturePhy))
                            Directory.CreateDirectory(sourceFolderSignaturePhy);

                        if (model.Signature_ != null)
                        {
                            string FileName = service.ProcessUploadedFile(model.Signature_, sourceFolderSignaturePhy, model.ServiceNo);

                            string path = Path.Combine(sourceFolderSignaturePhy, FileName);

                            bool result = service.IsValidHeader(path);
                            bool imgcontentresult = service.IsImage(model.Signature_);

                            if (!result || !imgcontentresult)
                            {
                                ModelState.AddModelError("Signature_", "Signature File format not correct");
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                goto end;
                            }

                            mTrnUpload.SignatureImagePath = GetCreateMyFolder() + "/" + FileName;
                        }
                        else
                        {
                            mTrnUpload.SignatureImagePath = model.ExistingSignatureImagePath;
                        }
                        DTOBasicDetailsSaveResponse ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload, mTrnIdentityInfo, null, null);
                        BasicDetail basicDetail = await basicDetailBL.Get(model.BasicDetailId);
                        if (ret1.Result == true)
                        {
                            bool resultforisprocess = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
                            if (!resultforisprocess)
                            {
                                MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                                mTrnICardRequest.BasicDetailId = basicDetail.BasicDetailId;
                                mTrnICardRequest.StatusId = 1;
                                mTrnICardRequest.TypeId = model.TypeId;
                                string tracid = model.DOB.Day.ToString("D2") + "" + model.DOB.Month.ToString("D2") + "" + model.DOB.Year + "" + Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
                                mTrnICardRequest.TrackingId = Convert.ToInt64(tracid);
                                mTrnICardRequest.RegistrationId = model.RegistrationId;
                                mTrnICardRequest.TrnDomainMappingId = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").TrnDomainMappingId;
                                mTrnICardRequest.UpdatedOn = DateTime.Now;
                                mTrnICardRequest.Updatedby = Convert.ToInt32(userId); //SessionHeplers.GetObject<string>(HttpContext.Session, "ArmyNo");
                                mTrnICardRequest = await iTrnICardRequestBL.AddWithReturn(mTrnICardRequest);
                                if (mTrnICardRequest.RequestId > 0)
                                {
                                    MStepCounter mStepCounter = new MStepCounter();
                                    mStepCounter.StepId = Convert.ToByte(1);
                                    mStepCounter.RequestId = mTrnICardRequest.RequestId;
                                    mStepCounter.UpdatedOn = DateTime.Now;
                                    mStepCounter.Updatedby = Convert.ToInt32(userId);
                                    mStepCounter.ApplyForId = newBasicDetail.ApplyForId;
                                    await iStepCounterBL.Add(mStepCounter);
                                }
                                //DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
                                //dTOApiDataResponse.Status = false;
                                //dTOApiDataResponse.Message = "Your I-Card is under process. Please wait.";
                                //return Ok(dTOApiDataResponse);
                            }


                            TempData["success"] = "Updated Successfully.";
                            //return RedirectToAction("Index");
                            if (newBasicDetail.ApplyForId == 1)
                                return RedirectToAction("Index", new { Id = "MQ==" });
                            else
                                return RedirectToAction("Index", new { Id = "MQ==", jcoor = "SmNvL09ycw ==" });

                        }
                        else
                        {
                            TempData["error"] = ret1.Message;
                        }

                    }
                    else
                    {
                        //var error = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        //TempData["error"] = error[0][0].ErrorMessage;
                        var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                                            .SelectMany(x => x.Value!.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();
                        if (errors.Any())
                        {
                            TempData["errors"] = string.Join("; ", errors); // Concatenate all error messages
                        }
                        
                    }
                }
                else
                {
                    model.Updatedby = Convert.ToInt32(userId);
                    model.StatusLevel = 0;

                    if (ModelState.IsValid)
                    {
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
                        newBasicDetail.DateOfIssue = null;
                        MTrnUpload mTrnUpload = new MTrnUpload();

                        MTrnAddress mTrnAddress = new MTrnAddress();
                        mTrnAddress.State = model.State;
                        mTrnAddress.District = model.District;
                        mTrnAddress.PS = model.PS;
                        mTrnAddress.PO = model.PO;
                        mTrnAddress.Tehsil = model.Tehsil;
                        mTrnAddress.Village = model.Village;
                        mTrnAddress.PinCode = model.PinCode;

                        MTrnIdentityInfo mTrnIdentityInfo = new MTrnIdentityInfo();
                        mTrnIdentityInfo.IdenMark1=model.IdenMark1;
                        mTrnIdentityInfo.IdenMark2=model.IdenMark2;
                        mTrnIdentityInfo.AadhaarNo = Convert.ToInt64(model.AadhaarNo);
                        mTrnIdentityInfo.BloodGroupId = model.BloodGroupId;
                        mTrnIdentityInfo.Height=model.Height;
                        if (model.UnitId==0)
                        {
                            ModelState.AddModelError("", "Please Enter Unit Name");
                        }
                        if(model.ApplyForId!=1 && model.RegimentalId==0)
                        {
                            ModelState.AddModelError("", "Please Select Regimental ");
                        }
                       

                        //string sourceFolderPhotoDB = "/WriteReadData/" + "Photo";
                        //string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");
                        string sourceFolderPhotoPhy = Convert.ToString(GetCreateMyFolder(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo")));
                        if (!Directory.Exists(sourceFolderPhotoPhy))
                            Directory.CreateDirectory(sourceFolderPhotoPhy);

                        if (model.Photo_ != null)
                        {
                            string FileName = service.ProcessUploadedFile(model.Photo_, sourceFolderPhotoPhy,model.ServiceNo);
                           
                            string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                            bool result = service.IsValidHeader(path);
                            bool imgcontentresult = service.IsImage(model.Photo_);

                            if (!result || !imgcontentresult)
                            {
                                ModelState.AddModelError("", "Photo File format not correct");
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                goto end;
                            }
                            else
                            {
                                string uniqueFileName = FileName + ".enc";
                                //string destinationPath = sourceFolderPhotoPhy + model.ServiceNo + ".txt";
                                string destinationPath = Path.Combine(sourceFolderPhotoPhy, uniqueFileName);
                                ImageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                //mTrnUpload.PhotoImagePath = GetCreateMyFolder() + "/" + FileName;
                                //// ViewBag.PhotoImagePath = mTrnUpload.PhotoImagePath;
                                mTrnUpload.PhotoImagePath = GetCreateMyFolder() + "/" + FileName + ".enc";
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("Photo_", "Photo is required.");
                            goto end;
                        }

                        //string sourceFolderSignatureDB = "/WriteReadData/" + "Signature";
                        //string sourceFolderSignaturePhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature");
                        string sourceFolderSignaturePhy = Convert.ToString(GetCreateMyFolder(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature")));
                        if (!Directory.Exists(sourceFolderSignaturePhy))
                            Directory.CreateDirectory(sourceFolderSignaturePhy);

                        if (model.Signature_ != null)
                        {
                            string FileName = service.ProcessUploadedFile(model.Signature_, sourceFolderSignaturePhy, model.ServiceNo);

                            string path = Path.Combine(sourceFolderSignaturePhy, FileName);

                            bool result = service.IsValidHeader(path);
                            bool imgcontentresult = service.IsImage(model.Signature_);

                            if (!result || !imgcontentresult)
                            {
                                ModelState.AddModelError("", "Signature File format not correct");
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                goto end;
                            }

                            mTrnUpload.SignatureImagePath = GetCreateMyFolder() + "/" + FileName;
                        }
                        else
                        {
                            ModelState.AddModelError("Signature_", "Signature is required.");
                            goto end;
                        }
                        MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                        mTrnICardRequest.StatusId = 1;
                        mTrnICardRequest.IsActive = true;
                        mTrnICardRequest.TypeId = model.TypeId;
                        string tracid = model.DOB.Day.ToString("D2") + "" + model.DOB.Month.ToString("D2") + "" + model.DOB.Year+""+ Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
                        mTrnICardRequest.TrackingId = Convert.ToInt64(tracid);
                        mTrnICardRequest.RegistrationId = model.RegistrationId;
                        mTrnICardRequest.TrnDomainMappingId = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").TrnDomainMappingId;
                        mTrnICardRequest.UpdatedOn = DateTime.Now;
                        mTrnICardRequest.Updatedby = Convert.ToInt32(userId);
                        //SessionHeplers.GetObject<string>(HttpContext.Session, "ArmyNo");
                                                                              // mTrnICardRequest = await iTrnICardRequestBL.AddWithReturn(mTrnICardRequest);


                        MStepCounter mStepCounter = new MStepCounter();
                        mStepCounter.StepId = Convert.ToByte(1);
                        // mStepCounter.RequestId = mTrnICardRequest.RequestId;
                        mStepCounter.UpdatedOn = DateTime.Now;
                        mStepCounter.Updatedby = Convert.ToInt32(userId);
                        mStepCounter.IsActive = true;
                        mStepCounter.ApplyForId = newBasicDetail.ApplyForId;
                        //  await iStepCounterBL.Add(mStepCounter);


                        BasicDetail ret = new BasicDetail();

                        DTOBasicDetailsSaveResponse ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload,mTrnIdentityInfo, mTrnICardRequest, mStepCounter);
                        if (ret1.Result == true)
                        {
                            await basicDetailTempBL.UpdateByArmyNo(newBasicDetail.ServiceNo);

                            TempData["success"] = "Successfully created.";
                            if (newBasicDetail.ApplyForId == 1)
                                return RedirectToAction("Index", new { Id = "MQ==" });
                            else
                                return RedirectToAction("Index", new { Id = "MQ==", jcoor = "SmNvL09ycw ==" });

                        }
                        else
                        {
                            TempData["error"] = ret1.Message;
                        }
                       
                    }
                    else
                    {
                        var error = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                                                .SelectMany(x => x.Value!.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                        if (error.Any())
                        {
                            TempData["error"] = string.Join("; ", error); // Concatenate all error messages
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1006, ex, "Exception");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }

        end:
            return View(model);

        }

        [HttpGet]
        public Task<ActionResult> DecryptZipFile(string jcoor)
        {
            if (string.IsNullOrEmpty(jcoor) || !service.IsValidBase64(jcoor))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(jcoor);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                ViewBag.jcoor = decodedString;
                return Task.FromResult<ActionResult>(View());
            }
            catch (FormatException ex)
            {
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {jcoor}", jcoor);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
        }
        [HttpPost]
        public async Task<IActionResult> DecryptZipFileData(DTODecryptZipFileRequest model)
        {
            try
            {
                model.PrivateKey = _configuration["Key:PrivateKey"];
                if (ModelState.IsValid)
                {
                    string sourceFolderPhotoPhy = Convert.ToString(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell", "Temp"));
                    if (!Directory.Exists(sourceFolderPhotoPhy))
                        Directory.CreateDirectory(sourceFolderPhotoPhy);
                    string TempFileName = Guid.NewGuid().ToString();
                    string FileName = service.ProcessUploadedFile(model.ZipFile, sourceFolderPhotoPhy, TempFileName);
                    string destinationzipfilename = Path.GetFileName(model.ZipFile.FileName);
                    string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                    bool result = service.IsValidZipHeader(path);

                    if (!result)
                    {
                        ModelState.AddModelError("ZipFile", "File format not correct");
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        return Json(KeyConstants.InternalServerError);
                    }

                    ZipDecrypt.DecryptAndUnzip(path, sourceFolderPhotoPhy, sourceFolderPhotoPhy, destinationzipfilename, model.PrivateKey); // Decrypt and unzip folder

                    return Json(model.ZipFile.FileName);
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->DecryptZipFile");
                return Json(KeyConstants.InternalServerError);
            }
        //end:
        //    return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetRegimentalListByArmedId(byte ArmedId)
        {
            var regimentals = await service.GetRegimentalListByArmedId(ArmedId);
            return Json(regimentals);
        }
        [HttpPost]
        public async Task<IActionResult> GetROListByArmedId(byte ArmedId)
        {
            List<MRecordOffice>? mRecordOffices = await basicDetailBL.GetROListByArmedId(ArmedId);
            if (mRecordOffices != null)
            {
                return Ok(mRecordOffices);
            }
            else 
            {
                return Ok(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUserData(string ICNumber)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://api.postalpincode.in/");
                client.BaseAddress = new Uri("https://localhost:7002/api/Fetch/Get/");
                //using (HttpResponseMessage response = await client.GetAsync("ICNumber/" + ICNumber))
                using (HttpResponseMessage response = await client.GetAsync(ICNumber))
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    response.EnsureSuccessStatusCode();
                    var responseData = JsonConvert.DeserializeObject(responseContent);
                    return Ok(responseData);
                }
            }
        }    
        public async Task<IActionResult> UpdateStepCounter(MStepCounter mStepCounter)
        {
            try
            {
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                DTOMapUnitResponse dTOMapUnitResponse = await mapUnitBL.GetALLByUnitMapId(sessiondata.UnitId);

                mStepCounter.UpdatedOn = DateTime.Now;
                mStepCounter.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                mStepCounter.UnitName = dTOMapUnitResponse.UnitName;
                await iStepCounterBL.UpdateStepCounter(mStepCounter);


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>IcardFwd.");
                return BadRequest();
            }
            return Ok(mStepCounter);
        }

        //[Authorize(Roles = "Coordinator")]
        [Authorize(Policy = "InternalWkDistrPolicy")]
        public async Task<IActionResult> SaveInternalFwd(DTOSaveInternalFwdRequest data)
        {
            try
            {
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsComplete = false;
                data.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId = Convert.ToByte(data.TypeId);
                if (ModelState.IsValid)
                {
                    bool? result = (bool)await iTrnFwnBL.SaveInternalFwd(data);
                    if (result != null)
                    {
                        if (result == true)
                        {
                            return Json(true);
                        }
                        else
                        {
                            return Json(false);
                        }
                    }
                    else
                    {
                        return Json(null);
                    }

                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>SaveInternalFwd");
                return BadRequest();
            }
        }
        public async Task<IActionResult> IcardFwd(MTrnFwd data)
        {
            try
            {
                DtoSession sessiondata=SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                data.FromUserId= sessiondata.UserId;
                data.UnitId= sessiondata.UnitId;
                data.FromAspNetUsersId= Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.UpdatedOn = DateTime.Now;
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId= Convert.ToByte(data.TypeId);
                //if (data.TrnFwdId > 0)
                //{
                //    await iTrnFwnBL.UpdateFieldBYTrnFwdId(data.TrnFwdId);
                //}
                if (await iTrnFwnBL.UpdateAllBYRequestId(data.RequestId))
                {
                    data.TrnFwdId = 0;
                    data = await iTrnFwnBL.AddWithReturn(data);
                    return Ok(data);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>IcardFwd.");
                return BadRequest();
            }
        }
        public async Task<IActionResult> IcardRejecte(MTrnFwd data)
        {
            try
            {
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.UpdatedOn = DateTime.Now;
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId =Convert.ToByte(1);
                TrnDomainMapping Domain = new TrnDomainMapping();
                Domain =await iDomainMapBL.GetByRequestId(data.RequestId);
                if (Domain != null) {
                    data.ToAspNetUsersId = Domain.AspNetUsersId;
                    data.ToUserId = Convert.ToInt32(Domain.UserId);

                    if (await iTrnFwnBL.UpdateAllBYRequestId(data.RequestId))
                    {
                        await iTrnFwnBL.Add(data);


                        int[] d;
                        d = new int[1];
                        d[0] = data.RequestId;
                        var dataret = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(d);
                        if (dataret != null)
                        {
                            dataret.XmlFiles = "";
                        }
                    
                   await _iTrnLoginLogBL.XmlFileDigitalSign(dataret);
                    return Ok(data);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>IcardRejecte.");
                return BadRequest();
            }




        }
        [HttpPost]
        public async Task<IActionResult> GetData(string ICNumber)
        {
            DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            if (ICNumber!=null)
            {
                BasicDetail? basicDetail = await basicDetailBL.FindServiceNo(ICNumber);
                if(basicDetail!=null) 
                {
                    bool result = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
                    if (result)
                    {
                       
                        dTOApiDataResponse.Status = false;
                        dTOApiDataResponse.Message = "Your I-Card is under process. Please wait.";
                        return Ok(dTOApiDataResponse);
                    }
                    else
                    {
                        dTOApiDataResponse.Status = true;
                       
                        return Ok(dTOApiDataResponse);
                    }
                }
                else
                {
                    dTOApiDataResponse.Status = true;

                    return Ok(dTOApiDataResponse);
                }
            }
            else
            {
                
                dTOApiDataResponse.Status = false;
                dTOApiDataResponse.Message = "Service no required.";
                return Ok(dTOApiDataResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchAllServiceNo(string ICNumber)
        {
            try
            {
                DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
                if (ICNumber != null)
                {
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var Ret = await basicDetailBL.SearchAllServiceNo(ICNumber, AspNetUsersId);
                    if (Ret != null)
                    {
                        return Ok(Ret);
                    }

                }
                return BadRequest();
            }
            catch(Exception ex) 
            {
                _logger.LogError(1001, ex, "BasicDetailController=>SearchAllServiceNo.");
                return BadRequest();
            }
        }
        public async Task<DTOApiDataResponse> GetApiData(string ICNumber)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://api.postalpincode.in/");
                client.BaseAddress = new Uri("https://localhost:7002/api/Fetch/GetData/");
                //using (HttpResponseMessage response = await client.GetAsync("ICNumber/" + ICNumber))
                using (HttpResponseMessage response = await client.GetAsync(ICNumber))
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    response.EnsureSuccessStatusCode();
                    DTOApiDataResponse? responseData = JsonConvert.DeserializeObject<DTOApiDataResponse>(responseContent);
                    return responseData;
                }
            }
        }
       
        
        public async Task<IActionResult> GetBasicDetailByRequestId(int RequestId)
        {
           return Json(await basicDetailBL.GetBasicDetailByRequestId(RequestId));
        }
        public async Task<IActionResult> GetRequestHistory(int RequestId)
        {
            List<ICardHistoryResponse>? cardHistoryResponses = await basicDetailBL.ICardHistory(RequestId);
            if (cardHistoryResponses !=null)
            {
                return Json(cardHistoryResponses);
            }
            else
            {
                return Json(null);
            }
            
        }
        
        public async Task<IActionResult> GetRemarks(DTORemarksRequest Data)
        {
            return Json(await _IMasterBL.GetRemarksByTypeId(Data));
        }
        //[Authorize(Roles = "AFSACUser")]
        [Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<IActionResult> DataExport(DTODataExportRequest Data)
        {
            try
            {
                //Data.publicKey = "MIIBCgKCAQEArhSYCF6ie0rkkXe2HSqKXQ/Sa/NwwbXQ/q1sEEL2eWGnpCa0+49DtRWtybLfK6A51Cj1TX2HnOGuPROQ46DOPI6giwDXnIimHeHAMCd4GqFuDAlDytFNls4XHCMxt1Ql2nVWVxBc2DSTGB35H+eT06rgL+j6ra0iaorAnghUzgIsgH8uLoXX9WqQZXI3rZcH6483ymh0fs/6hS0L5D/pNSaAIuMse3Jg6vcv5z/M7ZzTfiKHO0XkZE/qkm6hIR8uHi4jJwoCdHJ4Fc0wZ+ekd3h/Z2nNXbim07jX6ZcoKL5udYf5u0iFqplg6ao+qssiHF4RMCeDh1vBU5vkSpyUEQIDAQAB";
                //Data.privateKey = "MIIEpgIBAAKCAQEArhSYCF6ie0rkkXe2HSqKXQ/Sa/NwwbXQ/q1sEEL2eWGnpCa0+49DtRWtybLfK6A51Cj1TX2HnOGuPROQ46DOPI6giwDXnIimHeHAMCd4GqFuDAlDytFNls4XHCMxt1Ql2nVWVxBc2DSTGB35H+eT06rgL+j6ra0iaorAnghUzgIsgH8uLoXX9WqQZXI3rZcH6483ymh0fs/6hS0L5D/pNSaAIuMse3Jg6vcv5z/M7ZzTfiKHO0XkZE/qkm6hIR8uHi4jJwoCdHJ4Fc0wZ+ekd3h/Z2nNXbim07jX6ZcoKL5udYf5u0iFqplg6ao+qssiHF4RMCeDh1vBU5vkSpyUEQIDAQABAoIBAQCDDhgDPRPAFHsNlP1y6cLvGulEwiqiezoTcgZIG9GpQj7OUyGvvYSwwNhsYBCprF+8/PToWNgO4MynSKKs7DQ33Py6iXDJdQrytjFVT3GZQu0xfIwgFgD+xrsZQNm99kjlNa9BrpznXHVdE7upLFPbZ+qNxy1qMU0Wvs0SbJ1D1ZruXtbRqbOKzryZKa5NpboDBXIPw/o9RZS8eTFVl1SZC6asrokEepVWUsMwg/yORvKf/p/cCZBjbKQ+oclsT5ljht5j53YuYlixIYJNghmniMMEWwuyfeKZ5swL0HbGTJvkrz55mWKP7NtWGIIUzhMltPef6LNcjeMw/SOvTghNAoGBAMmSwbWRmJfXzAuq/UnnUoi8zpJuoWHh8fww0w0/bOuuVGkk/0Z+LXaWOeSRFjrwT1UU+uSW5Lj0bTRJHeGCxwaA8d2CJMUlPCBx6xukFDyaCZavtwxUMC5hOLp7DvCWyMZqQP5UAI5ukMYgljE9rvTfpXQBp04QH3xYCjXiUPffAoGBAN0VdxY+uvd7roiW1JamrzeyDCkIlWGriUd+WoO6KVKGM7Gi1E7oZcaSW9BC/qutRBuuuOFfu3btC3BlBbieXXAztAvEPD8e/JvE5FSpkcY8rELlC9Y0M3hxdJoMWH/tIwJIVKsxnGzCfRemMjvLiAGc1YSWnl5lslpQSrlJG7IPAoGBAMCXmL87ijliNRHc4L7w5vnAs/pS+5zDPerAV5ZryEzytrHzaHhY7GVGqa/KNBxCKPpY3lL0HTreR0zSo1spEbIUF4OV6j33EpjJX2J8hd1VK94uq017TsGxoHsEQsT6vIBfWxPk/NcZqveygO4xSm2rFbFeNxUt8HdkwvSy9LuvAoGBAL/W9HMVE9/ULurPFsFy+e/2S57/l8AcvQ6QkbJkQ58cXJbzmA6wkj/wmELrH1mRC9yJjFvkWiMkJhztTD2bDbFi7ASZzz1mggQYoZjlW10NIN0bK15ABbmpmWhi9hhriUldwjqa3gVx7mIrEMPaJLZhhNV8bQe0b0L3ESAeVC35AoGBAIFUQ9VziGZ2UMrDxMPU2AoMqfJe3X82CcUu/WS3KntAObSlSA3Od2Ow8gHs6KtVxMYLND9nHJ+WXMXASbv/ou1E/h8lRvg7OjFEnscgz8w5Kvf5egIoYFoMAg7TA8e/8mZ8NIli88T2/vvMZHhUSrRm43cssViI1kLFXfywzzOX";
                Data.publicKey = _configuration["Key:PublicKey"];
                Data.privateKey = _configuration["Key:PrivateKey"];
                var retdata = await basicDetailBL.GetBesicdetailsByRequestId(Data);
                string sourceFolderPhotoPhy = Convert.ToString(ForCreateFolderrandom(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell")));
                int recoff = 0;
                List<DTODataExportsResponse> lst = new List<DTODataExportsResponse>();
                string recofffolder = "";
                string recoffphotos = "";
                string recoffsing = "";
                int count = 0;
                string arryRequestId = "";
                foreach (var data in retdata)
                {
                    count++;
                    if (recoff != data.RecordOfficeId)
                    {
                        if (recoff != 0)
                        {
                            var jsonString = JsonConvert.SerializeObject(lst);
                            var jsonde = JsonConvert.DeserializeObject(jsonString);
                            System.IO.File.WriteAllText(recofffolder + "/Data.json", jsonString);
                        }

                        lst.Clear();
                        recofffolder = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice));
                        recoffphotos = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice + "/Photos"));
                        recoffsing = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice + "/Signature"));

                    }

                    System.IO.File.Copy(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo") + "/" + data.PhotoImagePath, recoffphotos + "/" + data.ServiceNo + ".png", true);
                    System.IO.File.Copy(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature") + "/" + data.SignatureImagePath, recoffsing + "/" + data.ServiceNo + ".png", true);
                    lst.Add(data);
                    recoff = data.RecordOfficeId;
                    if (count == retdata.Count())
                    {
                        var jsonString = JsonConvert.SerializeObject(lst);
                        var jsonde = JsonConvert.DeserializeObject(jsonString);
                        System.IO.File.WriteAllText(recofffolder + "/Data.json", jsonString);

                    }
                    if(count==1)
                    arryRequestId = data.RequestId+"";
                    else
                     arryRequestId = arryRequestId + "," + data.RequestId;

                }

                //CreateZipFromFolder(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip");
                
                if (Data.DataExportType == 1)
                {
                    string sourceFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell", "Temp");
                    // Check if directory exists
                    if (!Directory.Exists(sourceFolder))
                    {
                        // If directory does not exist, create it
                        Directory.CreateDirectory(sourceFolder);
                    }

                    string tempZipFilePath = Convert.ToString(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell","Temp"));

                    ZipEncrypt.EncryptAndZip(sourceFolderPhotoPhy, sourceFolderPhotoPhy+ ".zip", tempZipFilePath, Data.publicKey); // Encrypt and zip folder
                }
                else
                {
                    CreateZipFromFolder(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip");
                }


                string lastFolderName = new DirectoryInfo(sourceFolderPhotoPhy).Name;


                DtoSession dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                DTODataExported dTODataExported = new DTODataExported();
                dTODataExported.AspNetUsersId = userId;
                dTODataExported.UserId = Convert.ToInt32(dtoSession.UserId);
                dTODataExported.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                dTODataExported.CreatedBy = dtoSession.RankName + " " + dtoSession.Name + " (" + dtoSession.ICNO+")";
                dTODataExported.CreatedOn = DateTime.Now;
                dTODataExported.RequestId = arryRequestId;
                await _iTrnLoginLogBL.AddDataExport(dTODataExported);

                return Json(lastFolderName);


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataExport.");
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> CreateCSV(DTOCSVExportRequest model)
        {
            try
            {
                var UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(UserId);

                // UserManager service GetClaimsAsync method gets all the current claims of the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Internal Wk Distr"))
                {
                    //Ids is TrnFwdId.
                    model.IdsTypeRequestIdOrTrnFwdId = true;
                }
                else
                {
                    //Ids is RequestId.
                    model.IdsTypeRequestIdOrTrnFwdId = false;
                }
                
                string? csvData = await basicDetailBL.GetCSVString(model);
                if(csvData != null)
                {
                    string TempFileName = Guid.NewGuid().ToString();
                    string sourceFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CSVFile");
                    // Check if directory exists
                    if (!Directory.Exists(sourceFolder))
                    {
                        // If directory does not exist, create it
                        Directory.CreateDirectory(sourceFolder);
                    }

                    System.IO.File.WriteAllText(sourceFolder + "/"+ TempFileName + ".csv", csvData);
                    return Json(TempFileName);
                }
                else 
                {
                    return Json(KeyConstants.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>CreateCSV.");
                return Json(KeyConstants.InternalServerError);
            }
        }
        public void CreateZipFromFolder(string sourceFolder, string zipFilePath)
        {
            if (Directory.Exists(sourceFolder))
            {
                ZipFile.CreateFromDirectory(sourceFolder, zipFilePath, CompressionLevel.Fastest, true);
            }
            else
            {
                throw new DirectoryNotFoundException($"Source folder not found: {sourceFolder}");
            }
        }
     
        public async Task<IActionResult> DataDigitalXmlSign(DTODataExportRequest Data)
        {
            try
            {
                string xml = "";
                DTOXmlFilesFwdLogRequest ret=new DTOXmlFilesFwdLogRequest();
                var xmldata= await _iTrnLoginLogBL.XmlFileDigitalSignFromData(Data.Ids);
                var lastrec=await basicDetailBL.ICardFwdLastRec(Data.Ids[0]);
                XmlSerializer serializer = new XmlSerializer(typeof(DTOFwdLastRecForDigitalSign));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, lastrec);
                    xml = writer.ToString();

                }


                if (xmldata != null )
                {
                    if(xmldata.XmlFiles!="")
                    {
                        ret.Id = xmldata.Id;
                        XDocument xDoc1 = XDocument.Parse(xmldata.XmlFiles);
                        XDocument xDoc2 = XDocument.Parse(xml);
                        var newDetails = new XElement("RecForDigitalSign");
                      //  newDetails.Add(newDetails);
                       
                        foreach (XElement element in xDoc2.Root.Elements())
                        {
                            newDetails.Add(element);
                           // xDoc1.Root.Add(element);
                        }
                        xDoc1.Root.Add(newDetails);
                        ret.XmlFiles = xDoc1.ToString();// xmldata.XmlFiles;
                        return Json(ret);
                    }
                    else
                    {
                        var retdata = await basicDetailBL.GetDataDigitalXmlSign(Data);
                        var jsonString = JsonConvert.SerializeObject(retdata);
                        var jsonde = JsonConvert.DeserializeObject(jsonString);
                        DTOXmlFilesForUpdate dTOXmlFilesForUpdate = new DTOXmlFilesForUpdate();
                        dTOXmlFilesForUpdate.Id=xmldata.Id;
                        dTOXmlFilesForUpdate.jsonfile = jsonde;
                        return Json(dTOXmlFilesForUpdate);
                    }

                }
                else
                {
                    var retdata = await basicDetailBL.GetDataDigitalXmlSign(Data);

                    var jsonString = JsonConvert.SerializeObject(retdata);
                    var jsonde = JsonConvert.DeserializeObject(jsonString);

                   
                    return Json(jsonde);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataDigitalXmlSign.");
                return RedirectToAction("Error", "Error");
            }
        }
        public static DirectoryInfo GetCreateMyFolder(string baseFolder)
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MMMM");
            var dayName = now.ToString("dd");

            var folder = 
                        Path.Combine(baseFolder, 
                           Path.Combine(yearName,
                             Path.Combine(monthName,
                               dayName)));

            return Directory.CreateDirectory(folder);
        }
        public static DirectoryInfo GetCreateMyFolder()
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MMMM");
            var dayName = now.ToString("dd");

            var folder =
                        
                           Path.Combine(yearName,
                             Path.Combine(monthName,
                               dayName));

            return Directory.CreateDirectory(folder);
        }
        public static DirectoryInfo ForCreateFolderrandom(string baseFolder)
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MMMM");
            var dayName = now.ToString("dd");
            var hh = now.ToString("hh");
            var mm = now.ToString("mm");
            var ss = now.ToString("ss");
            var folder =
                        Path.Combine(baseFolder,
                           Path.Combine(yearName + "" + monthName + "" + dayName+ ""+hh + "" + mm + "" + ss));

            return Directory.CreateDirectory(folder);
        }
        public static DirectoryInfo CreateFolder(string baseFolder)
        {
            return Directory.CreateDirectory(baseFolder);
        }
        
    }
}
