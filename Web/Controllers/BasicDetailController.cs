using AutoMapper;
using BusinessLogicsLayer.Helpers;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Newtonsoft.Json;
using System.Security.Claims;
using System;
using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Requests;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Common;
using DataTransferObject.Response;
using BusinessLogicsLayer.Service;
using BusinessLogicsLayer;
using DataTransferObject.Constants;
using BusinessLogicsLayer.Bde;
using Web.WebHelpers;
using DataTransferObject.ViewModels;
using System.Data.Entity;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.BdeCate;
using Microsoft.IdentityModel.Tokens;
using Azure;
using BusinessLogicsLayer.Master;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static NuGet.Packaging.PackagingConstants;
using System.Diagnostics;
using System.Drawing.Printing;
using BusinessLogicsLayer.Unit;
using DapperRepo.Core.Constants;
using System.Text.Json.Nodes;
using System.IO.Compression;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Logging;
using BusinessLogicsLayer.TrnLoginLog;
using NuGet.Packaging;
using Web.Healpers;

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
        public DateTime dateTimenow;
        public BasicDetailController(IBasicDetailBL basicDetailBL, IMapUnitBL mapUnitBL, IBasicDetailTempBL basicDetailTempBL, IService service, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger, IStepCounterBL iStepCounterBL, 
                              ITrnFwnBL iTrnFwnBL, ITrnICardRequestBL iTrnICardRequestBL, IDomainMapBL iDomainMapBL
            ,IBasicUploadBL basicUploadBL, IBasicAddressBL basicAddressBL, IBasicinfoBL basicinfoBL, IRankBL rankBL, INotificationBL notificationBL, IMasterBL masterBL
           , ITrnLoginLogBL iTrnLoginLogBL)
        {
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

        public async Task<ActionResult> Index(string Id,string jcoor)
        {
            MTrnNotification noti = new MTrnNotification();
            int retint = 0;int type = 1;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            int stepcounter = 0;
            noti.ReciverAspNetUsersId = userId;
            noti.DisplayId = 0;
            if (!string.IsNullOrEmpty(Id))
            { 
            var base64EncodedBytes = System.Convert.FromBase64String(Id);
            var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            retint = Convert.ToInt32(ret);
            stepcounter = retint;
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

            MTrnNotification noti=new MTrnNotification();
            int type = 0; int retint = 0; int stepcounter = 0;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); //SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UserId;
            noti.ReciverAspNetUsersId = userId;
            noti.DisplayId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                var base64EncodedBytes = System.Convert.FromBase64String(Id);
                var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                retint = Convert.ToInt32(ret);
                stepcounter = retint;
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
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            
            
           // BasicDetail? basicDetail = await basicDetailBL.Get(decryptedIntId);
            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetByBasicDetailsId(decryptedIntId);
            if (basicDetailCrtAndUpdVM != null)
            {
                //DTOBasicDetailRequest basicDetailVM = _mapper.Map<BasicDetailCrtAndUpdVM, DTOBasicDetailRequest>(basicDetailCrtAndUpdVM);
                //MRank? mRank = await context.MRank.FindAsync(basicDetail.RankId);
                //if(mRank!=null)
                //{
                //    basicDetailVM.MRank = mRank;
                //}
                //MArmedType? mArmedType = await context.MArmedType.FindAsync(basicDetail.ArmedId);
                //if(mArmedType!=null)
                //{
                //    basicDetailVM.MArmedType = mArmedType;
                //}
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
            return Json(await basicDetailBL.GetByBasicDetailsId(RequestId));
        }
        [HttpGet]
        public async Task<ActionResult> InaccurateData(string Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int TypeId;
            if (!string.IsNullOrEmpty(Id))
            {
                var base64EncodedBytes = System.Convert.FromBase64String(Id);
                var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                TypeId = Convert.ToInt32(ret);
                if(TypeId == 1)
                {
                    var allrecord = await Task.Run(() => basicDetailTempBL.GetALLBasicDetailTemp(Convert.ToInt32(userId), TypeId));
                    ViewBag.Title = "List of Inaccurate Data";
                    return View(allrecord);
                }
                else if(TypeId == 2)
                {
                    var allrecord = await Task.Run(() => basicDetailTempBL.GetALLBasicDetailTemp(Convert.ToInt32(userId), TypeId));
                    ViewBag.Title = "List of Observation Raised";
                    return View(allrecord);
                }
                else
                {
                    TempData["error"] = "Invalid Input.";
                    return RedirectToActionPermanent("Dashboard", "Home");
                }
            }
            else
            {
                TempData["error"] = "Invalid Input.";
                return RedirectToActionPermanent("Dashboard", "Home");
            }
        }
        [HttpGet]
        public async Task<ActionResult> InaccurateDataView(string Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
           string decryptedId = protector.Unprotect(Id);
           int decryptedIntId = Convert.ToInt32(decryptedId);
            var allrecord = await Task.Run(() => basicDetailTempBL.GetALLBasicDetailTempByBasicDetailId(Convert.ToInt32(userId), decryptedIntId));
            _logger.LogInformation(1001, "Index Page Of Basic Detail Temp View");
          
            return View(allrecord);
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
                    //DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
                    //dTOApiDataResponse =await GetApiData(model.ServiceNo);
                    if (model.SubmitType == 1)
                    {
                        //BasicDetail basicDetail = new BasicDetail();
                        //basicDetail.Name = dTOApiDataResponse.Name;
                        //basicDetail.ServiceNo = dTOApiDataResponse.ServiceNo;
                        //basicDetail.DOB = dTOApiDataResponse.DOB;
                        //basicDetail.DateOfCommissioning = dTOApiDataResponse.DateOfCommissioning;
                        //basicDetail.PermanentAddress = dTOApiDataResponse.PermanentAddress;
                        //basicDetail.Updatedby = model.Updatedby;
                        //basicDetail.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        //basicDetail.Step = 1;
                        //BasicDetail insertedBasicdetail = await unitOfWork.BasicDetail.AddWithReturn(basicDetail);
                        //// Encrypt the ID value and store in EncryptedId property
                        //insertedBasicdetail.EncryptedId = protector.Protect(insertedBasicdetail.BasicDetailId.ToString());
                        //TempData["success"] = "Successfully created.";
                        //return RedirectToActionPermanent("Create", "BasicDetail", new { Id = insertedBasicdetail.EncryptedId });
                        BasicDetail Data = new BasicDetail();
                        Data =await basicDetailBL.FindServiceNo(model.ServiceNo);
                        if (Data != null)
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            string id = protector.Protect(Data.BasicDetailId.ToString());
                            return RedirectToActionPermanent("BasicDetail", "BasicDetail", new { Id  = protector.Protect(Convert.ToString(Data.BasicDetailId)) });
                        }
                        else
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            return RedirectToActionPermanent("BasicDetail", "BasicDetail", new { Id= protector.Protect("0") });
                        }

                        
                    }
                    else
                    {
                            BasicDetailTemp basicDetailTemp = new BasicDetailTemp();
                            basicDetailTemp.Name = model.Name;
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
                            return RedirectToAction("Registration");
                    }
                }
                else
                {
                    TempData["error"] = "Operation failed.";
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

            if (Id == null || protector.Unprotect(Id)== "0")
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
                        dTOBasicDetailCrtRequest.Name = model.Name;
                        dTOBasicDetailCrtRequest.NameAsPerRecord= model.NameAsPerRecord;
                        dTOBasicDetailCrtRequest.ServiceNo = model.ServiceNo;
                        dTOBasicDetailCrtRequest.DOB = model.DOB;
                        dTOBasicDetailCrtRequest.DateOfCommissioning = model.DateOfCommissioning;
                        dTOBasicDetailCrtRequest.IdenMark1 = model.IdenMark1;
                        dTOBasicDetailCrtRequest.IdenMark2 = model.IdenMark2;
                        ViewBag.OptionsRankId = model.RankId;
                        ViewBag.OptionsArmedId = model.ArmedId;

                        //dTOBasicDetailCrtRequest.Height = model.Height;

                        // dTOBasicDetailCrtRequest.AadhaarNo = Convert.ToString(model.AadhaarNo);
                        dTOBasicDetailCrtRequest.AadhaarNo = Convert.ToInt64(model.AadhaarNo).ToString("D12"); ;// Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");


                        //dTOBasicDetailCrtRequest.BloodGroup = model.BloodGroup;

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
                        //dTOBasicDetailCrtRequest.PermanentAddress = model.PermanentAddress;
                        // dTOBasicDetailCrtRequest.RegistrationId = model.RegId;
                        // dTOBasicDetailCrtRequest.Type = mRegistration.ApplyForId;

                        //dTOBasicDetailCrtRequest.DateOfIssue = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
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
                try
                {
                    // Decrypt the  id using Unprotect method
                    //string s = protector.Protect("0");
                    decryptedId = protector.Unprotect(Id);
                    decryptedIntId = Convert.ToInt32(decryptedId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                    return RedirectToAction("Error", "Error");
                }
                BasicDetailCrtAndUpdVM? basicDetailUpdVM = await basicDetailBL.GetByBasicDetailsId(decryptedIntId);

                if (basicDetailUpdVM != null)
                {
                    ViewBag.OptionsRankId = basicDetailUpdVM.RankId; 
                    ViewBag.OptionsUnitId = basicDetailUpdVM.UnitId; 
                    ViewBag.OptionsArmedId = basicDetailUpdVM.ArmedId;
                    ViewBag.OptionsRegimentalId = basicDetailUpdVM.RegimentalId;
                    ViewBag.OptionsBloodGroupId = basicDetailUpdVM.BloodGroupId;

                    basicDetailUpdVM.BloodGroupId = basicDetailUpdVM.BloodGroupId;
                    basicDetailUpdVM.PermanentAddress = "Village - " + basicDetailUpdVM.Village + ", Post Office-" + basicDetailUpdVM.PO + ", Tehsil- " + basicDetailUpdVM.Tehsil + ", District- " + basicDetailUpdVM.District + ", State- " + basicDetailUpdVM.State + ", Pin Code- " + basicDetailUpdVM.PinCode;
                    basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                    basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                    basicDetailUpdVM.EncryptedId = Id;
                    //ViewBag.OptionsRegimental = service.GetRegimentalDDLIdSelected(basicDetailUpdVM.ArmedId);

                    ///////////////////////for close appl
                    ///
                    if (TempData["Registration"] != null)
                    {
                        var modelex = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());

                        basicDetailUpdVM.Name = modelex.Name;
                        basicDetailUpdVM.ServiceNo = modelex.ServiceNo;
                        basicDetailUpdVM.DOB = modelex.DOB;
                        basicDetailUpdVM.DateOfCommissioning = modelex.DateOfCommissioning;
                        basicDetailUpdVM.IdenMark1 = modelex.IdenMark1;
                        basicDetailUpdVM.IdenMark2 = modelex.IdenMark2;
                        ViewBag.OptionsRankId = modelex.RankId;
                        //dTOBasicDetailCrtRequest.Height = model.Height;

                        // dTOBasicDetailCrtRequest.AadhaarNo = Convert.ToString(model.AadhaarNo);
                        basicDetailUpdVM.AadhaarNo = Convert.ToInt64(modelex.AadhaarNo).ToString("D12"); ;// Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");


                        //dTOBasicDetailCrtRequest.BloodGroup = model.BloodGroup;

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
                        //newBasicDetail.RankId = model.RankId;
                        //newBasicDetail.ArmedId = model.ArmedId;
                        //newBasicDetail.UnitId= model.UnitId;
                        // basicDetail.IdentityMark = model.IdentityMark;
                        //basicDetail.Height = model.Height;
                        // basicDetail.BloodGroup = model.BloodGroup;
                        //basicDetail.PlaceOfIssue = model.PlaceOfIssue;
                        //newBasicDetail.DateOfIssue = model.DateOfIssue;
                        //newBasicDetail.IssuingAuth = model.IssuingAuth;

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
                                ModelState.AddModelError("", "File format not correct");
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                goto end;
                            }

                            mTrnUpload.PhotoImagePath = GetCreateMyFolder() + "/" + FileName;
                            // ViewBag.PhotoImagePath = mTrnUpload.PhotoImagePath;
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
                                ModelState.AddModelError("", "File format not correct");
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
                        //if (model.AadhaarNo != null)
                        //{
                        //    newBasicDetail.AadhaarNo = model.AadhaarNo.Replace(" ", "");
                        //}

                        // await basicDetailBL.Update(basicDetail);
                        var ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload, mTrnIdentityInfo, null, null);
                        BasicDetail basicDetail = await basicDetailBL.Get(model.BasicDetailId);
                        if (ret1 == true)
                        {
                            bool resultforisprocess = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
                            if (!resultforisprocess)
                            {
                                MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                                mTrnICardRequest.BasicDetailId = basicDetail.BasicDetailId;
                                mTrnICardRequest.Status = 0;
                                mTrnICardRequest.TypeId = model.TypeId;
                                string tracid = model.DOB.Day.ToString("D2") + "" + model.DOB.Month.ToString("D2") + "" + model.DOB.Year + "" + Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
                                mTrnICardRequest.TrackingId = Convert.ToInt64(tracid);
                                mTrnICardRequest.RegistrationId = model.RegistrationId;
                                //TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                                // trnDomainMapping.AspNetUsersId= Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                //trnDomainMapping=await iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping);
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
                            TempData["success"] = "Updated Not Successfully.";
                            if (newBasicDetail.ApplyForId == 1)
                                return RedirectToAction("Index", new { Id = "MQ==" });
                            else
                                return RedirectToAction("Index", new { Id = "MQ==", jcoor = "SmNvL09ycw ==" });
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
                        //MTrnIdentityInfo mTrnIdentityInfo = _mapper.Map<BasicDetailCrtAndUpdVM, MTrnIdentityInfo>(model);
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
                                ModelState.AddModelError("", "File format not correct");
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                goto end;
                            }

                            mTrnUpload.PhotoImagePath = GetCreateMyFolder() + "/" + FileName;
                            // ViewBag.PhotoImagePath = mTrnUpload.PhotoImagePath;
                        }
                        else
                        {
                            ModelState.AddModelError("Photo_", "Photo is required.");
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
                                ModelState.AddModelError("", "File format not correct");
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
                        }
                        //if (model.AadhaarNo != null)
                        //{
                        //    newBasicDetail.AadhaarNo = model.AadhaarNo.Replace(" ", "");
                        //}

                        MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                        mTrnICardRequest.Status = 0;
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

                       var ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload,mTrnIdentityInfo, mTrnICardRequest, mStepCounter);
                       // ret = await basicDetailBL.AddWithReturn(newBasicDetail);
                        if (ret1 == true)
                        {
                            //mTrnUpload.BasicDetailId = ret.BasicDetailId;
                            //await basicuploadBL.Add(mTrnUpload);

                            //mTrnAddress.BasicDetailId = ret.BasicDetailId;
                            //await basicAddressBL.Add(mTrnAddress);

                            //mTrnIdentityInfo.BasicDetailId = ret.BasicDetailId;
                            //await basicinfoBL.Add(mTrnIdentityInfo);


                            //TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                            // trnDomainMapping.AspNetUsersId= Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                            //trnDomainMapping=await iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping);
                            await basicDetailTempBL.UpdateByArmyNo(newBasicDetail.ServiceNo);

                            TempData["success"] = "Successfully created.";
                            if (newBasicDetail.ApplyForId == 1)
                                return RedirectToAction("Index", new { Id = "MQ==" });
                            else
                                return RedirectToAction("Index", new { Id = "MQ==", jcoor = "SmNvL09ycw ==" });

                        }
                        else
                        {
                            TempData["error"] = "Data Not Inserted.";
                        }
                       
                    }
                    else
                    {
                        TempData["error"] = "Operation failed.";
                    }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string Id)
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
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            BasicDetail basicDetail = await basicDetailBL.Get(decryptedIntId);
            if (basicDetail == null)
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
            else
            {
                BasicDetail deleteBasicDetail = await basicDetailBL.Delete(basicDetail.BasicDetailId);
                //if (deleteBasicDetail != null)
                //{
                //    if (deleteBasicDetail.PhotoImagePath != null)
                //    {
                //        string sourcePhotoImagePathPhy = Path.Join(hostingEnvironment.WebRootPath, deleteBasicDetail.PhotoImagePath.Replace('/', '\\').ToString());
                //        if (System.IO.File.Exists(sourcePhotoImagePathPhy))
                //        {
                //            System.IO.File.Delete(sourcePhotoImagePathPhy);
                //        }
                //    }
                //    if (deleteBasicDetail.SignatureImagePath != null)
                //    {
                //        string sourceSignatureImagePath = Path.Join(hostingEnvironment.WebRootPath, deleteBasicDetail.SignatureImagePath.Replace('/', '\\').ToString());
                //        if (System.IO.File.Exists(sourceSignatureImagePath))
                //        {
                //            System.IO.File.Delete(sourceSignatureImagePath);
                //        }
                //    }
                //}

                TempData["success"] = "Deleted Successfully.";
                return RedirectToAction("index");
            }
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
            var ro = await basicDetailBL.GetROListByArmedId(ArmedId);
            return Ok(ro);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsServiceNoInUse(string ServiceNo, string initialServiceNo)
        {
            //var user = await context.BasicDetails.FirstOrDefaultAsync(x => x.ServiceNo == ServiceNo);

            //if (ServiceNo == initialServiceNo)
            //{
            //    return Json(true);
            //}
            //else if (user == null)
            //{
            //    return Json(true);
            //}
            //else
            //{
            //    return Json($"Service No {ServiceNo} is already in use.");
            //}
            return Json(true);
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
       
        [Authorize(Roles = "Coordinator")]
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
                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
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
            //api:
            //using (var client = new HttpClient())
            //{
            //    //client.BaseAddress = new Uri("https://api.postalpincode.in/");
            //    client.BaseAddress = new Uri("https://localhost:7002/api/Fetch/GetData/");
            //    //using (HttpResponseMessage response = await client.GetAsync("ICNumber/" + ICNumber))
            //    using (HttpResponseMessage response = await client.GetAsync(ICNumber))
            //    {
            //        if (response.IsSuccessStatusCode)
            //        {
            //            var responseContent = response.Content.ReadAsStringAsync().Result;
            //            response.EnsureSuccessStatusCode();
            //            DTOApiDataResponse? responseData = JsonConvert.DeserializeObject<DTOApiDataResponse>(responseContent);
            //            DateTime DOB, DOC;
            //            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 0, 0);
            //            DOB = responseData.DOB.Date + timeSpan;
            //            DOC = responseData.DateOfCommissioning.Date + timeSpan;
            //            responseData.DOB = DOB;
            //            responseData.DateOfCommissioning = DOC;
            //            responseData.Status = true;
            //            return Ok(responseData);
            //        }
            //        else
            //        {
            //            DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            //            dTOApiDataResponse.Status = false;
            //            dTOApiDataResponse.Message = "Data Not Found.";
            //            return Ok(dTOApiDataResponse);
            //        }
            //    }
            //}

        }

        [HttpPost]
        public async Task<IActionResult> SearchAllServiceNo(string ICNumber)
        {
            DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            if (ICNumber != null)
            {
                int AspNetUsersId= Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var Ret = await basicDetailBL.SearchAllServiceNo(ICNumber, AspNetUsersId);
                if (Ret != null)
                {
                    return Ok(Ret);
                }
               
            }
            return BadRequest();
           

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
       
        
        public async Task<IActionResult> GetDataByBasicDetailsId(int Id)
        {
           return Json(await basicDetailBL.GetByBasicDetailsId(Id));
        }
        public async Task<IActionResult> GetRequestHistory(int RequestId)
        {
            return Json(await basicDetailBL.ICardHistory(RequestId));
        }
        
        public async Task<IActionResult> GetRemarks(DTORemarksRequest Data)
        {
            return Json(await _IMasterBL.GetRemarksByTypeId(Data));
        }
        public async Task<IActionResult> DataExport(DTODataExportRequest Data)
        {
            try
            {
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

                CreateZipFromFolder(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip");
                //Encrypt.EncryptParameter(jsonde.ToString())
                ZipEncryptionService zipEncryptionService = new ZipEncryptionService();
                zipEncryptionService.EncryptFile(sourceFolderPhotoPhy + ".zip", sourceFolderPhotoPhy);

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
                var retdata = await basicDetailBL.GetDataDigitalXmlSign(Data);

                var jsonString = JsonConvert.SerializeObject(retdata);
                var jsonde = JsonConvert.DeserializeObject(jsonString);


                return Json(jsonde);


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataExport.");
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
