﻿using AutoMapper;
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

namespace Web.Controllers
{
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
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<BasicDetailController> _logger;
        private readonly INotificationBL _INotificationBL;
        private readonly IMasterBL _IMasterBL;

        public DateTime dateTimenow;
        public BasicDetailController(IBasicDetailBL basicDetailBL, IBasicDetailTempBL basicDetailTempBL, IService service, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger, IStepCounterBL iStepCounterBL, 
                              ITrnFwnBL iTrnFwnBL, ITrnICardRequestBL iTrnICardRequestBL, IDomainMapBL iDomainMapBL
            ,IBasicUploadBL basicUploadBL, IBasicAddressBL basicAddressBL, IBasicinfoBL basicinfoBL, IRankBL rankBL, INotificationBL notificationBL, IMasterBL masterBL
            )
        {
            this.basicDetailBL = basicDetailBL;
            this.basicDetailTempBL = basicDetailTempBL;
            this.service = service;
            this._mapper = mapper;
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
        }

        [Authorize(Roles = "Admin,User")]
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

            if (retint == 1)
            {
                ViewBag.Title = "List of Register I-Card";
                // type = 2; stepcounter = 2;
            }

            else if (retint == 2)
            { ViewBag.Title = "I-Card Pending From Io"; type = 2; stepcounter = 2; }
            else if (retint == 22)
            { ViewBag.Title = "I-Card Rejectd From Io"; type = 1; stepcounter = 7; }
            else if (retint == 222)
            { ViewBag.Title = "I-Card Approved From Io"; type = 3; stepcounter = 2; }
            else if (retint == 3)
            {
                ViewBag.Title = "I-Card Pending From GSO";
                type = 2; stepcounter = 3;
            }
            else if (retint == 33)
            {
                ViewBag.Title = "I-Card Rejectd From GSO";
                type = 1; stepcounter = 8;
            }
            else if (retint == 333)
            {
                ViewBag.Title = "I-Card Approved From GSO";
                type = 3; stepcounter = 4;
            }
            else if (retint == 4)
            { ViewBag.Title = "I-Card Pending From MI 11"; type = 2; stepcounter = 4; }
            else if (retint == 44)
            { ViewBag.Title = "I-Card Rejectd From MI 11"; type = 1; stepcounter = 9; }
            else if (retint == 444)
            { ViewBag.Title = "I-Card Approved From MI 11"; type = 3; stepcounter = 5; }
            else if (retint == 5)
            { ViewBag.Title = "I-Card Pending From HQ 54"; type = 2; stepcounter = 5; }
            else if (retint == 55)
            { ViewBag.Title = "I-Card Rejectd From HQ 54"; type = 1; stepcounter = 10; }
            else if (retint == 555)
            { ViewBag.Title = "I-Card Approved From HQ 54"; type = 2; stepcounter = 5; }
            else if (retint == 888)
            { ViewBag.Title = "I-Card Submited"; type = 2; stepcounter = 888; }
            else if (retint == 999)
            { ViewBag.Title = "I-Card Rejectd From IO,MI11 and HQ 54"; type = 2; stepcounter = 999; }

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
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> ApprovalForIO(string Id, string jcoor)
        {
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
            else if (retint == 4)
            { ViewBag.Title = "I-Card For Approval"; type = 2; ViewBag.Id = 1; ViewBag.dataexport = 4; }
            else if (retint == 44)
            { ViewBag.Title = "Rejectd I-Card "; type = 1; stepcounter = 9; }
            else if (retint == 444)
            { ViewBag.Title = "Approved I-Card "; type = 3; stepcounter = 5; }
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
                var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), stepcounter, type,1));
                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                await _INotificationBL.UpdateRead(noti);
                return View(allrecord);
            }
            else
            {
                noti.DisplayId = stepcounter+10;
                var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), stepcounter, type,2));
                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                await _INotificationBL.UpdateRead(noti);
                return View(allrecord);
            }
                

           
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
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
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> InaccurateData()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allrecord = await Task.Run(() => basicDetailTempBL.GetALLBasicDetailTemp(Convert.ToInt32(userId)));
            _logger.LogInformation(1001, "Index Page Of Basic Detail Temp View");
            ViewBag.Title = "List of Inaccurate Data";
            return View(allrecord);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> RequestType()
        {
            var allrecord = await Task.Run(() => basicDetailBL.GetAllICardType());
            return View(allrecord);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Registration(string Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string decryptedId = string.Empty;
            ViewBag.OptionsBloodGroup = service.GetBloodGroup();
            ViewBag.OptionsArmedType = service.GetArmedType();
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                //decryptedId = protector.Unprotect(Id);
                //decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            DTORegistrationRequest dTORegistrationRequest = new DTORegistrationRequest();
            //dTORegistrationRequest.TypeId =(byte) decryptedIntId;
            ViewBag.OptionsRegistration = service.GetRegistration();
            return View(dTORegistrationRequest);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(DTORegistrationRequest model)
        {
            try
            {
                //ViewBag.OptionsRegistration = service.GetRegistration();
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
                            return RedirectToActionPermanent("BasicDetail", "BasicDetail", new { Id  = protector.Protect("0") });
                        }
                        else
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            return RedirectToActionPermanent("BasicDetail", "BasicDetail", new { Id= protector.Protect("0") });
                        }

                        
                    }
                    else
                    {
                        //if (model.Observations == null)
                        //{
                        //    ModelState.AddModelError("Observations", "Observations is required.");
                        //    goto end;
                        //}
                        //else
                        //{
                            BasicDetailTemp basicDetailTemp = new BasicDetailTemp();
                            basicDetailTemp.Name = model.Name;
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
                        //}
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
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> BasicDetail(string? Id)
        {

            
            ViewBag.OptionsBloodGroup = service.GetBloodGroup();
            ViewBag.OptionsArmedType = service.GetArmedType();

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
                        // MRegistration? mRegistration = await context.MRegistration.FindAsync(model.RegId);
                        //if(mRegistration.ApplyForId == (int)RegistrationType.Officer)
                        //{
                        //    ViewBag.OptionsRank = service.GetRank(1);
                        //}
                        //else
                        //{
                        //    ViewBag.OptionsRank = service.GetRank(2);
                        //}

                       
                        ViewBag.OptionsUnitId = 0;
                        ViewBag.OptionsArmedType = service.GetArmedType();
                        ViewBag.OptionsBloodGroup = service.GetBloodGroup();
                        BasicDetailCrtAndUpdVM dTOBasicDetailCrtRequest = new BasicDetailCrtAndUpdVM();
                        dTOBasicDetailCrtRequest.Name = model.Name;
                        dTOBasicDetailCrtRequest.ServiceNo = model.ServiceNo;
                        dTOBasicDetailCrtRequest.DOB = model.DOB;
                        dTOBasicDetailCrtRequest.DateOfCommissioning = model.DateOfCommissioning;
                        dTOBasicDetailCrtRequest.IdenMark1 = model.IdenMark1;
                        dTOBasicDetailCrtRequest.IdenMark2 = model.IdenMark2;
                        ViewBag.OptionsRank = model.RankId;
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
                    //MRank? mRank = await rankBL.GetAll().FindAsync(basicDetailUpdVM.RankId);
                    ViewBag.OptionsRank = basicDetailUpdVM.RankId; //service.GetRank(mRank.Type);
                    ViewBag.OptionsUnitId = basicDetailUpdVM.UnitId; //service.GetRank(mRank.Type);
                    // BasicDetailCrtAndUpdVM basicDetailUpdVM = _mapper.Map<BasicDetail, BasicDetailCrtAndUpdVM>(basicDetail);
                    //if (basicDetailUpdVM.AadhaarNo != null && basicDetailUpdVM.AadhaarNo.Length == 12)
                    //{
                    //    string p1, p2, p3;
                    //    p1 = basicDetailUpdVM.AadhaarNo.Substring(0, 4);
                    //    p2 = basicDetailUpdVM.AadhaarNo.Substring(4, 4);
                    //    p3 = basicDetailUpdVM.AadhaarNo.Substring(8, 4);
                    //    basicDetailUpdVM.AadhaarNo = p1 + " " + p2 + " " + p3;
                    //}
                    //basicDetailUpdVM.RegistrationType = basicDetailUpdVM.RegistrationType;
                    //basicDetailUpdVM.RegimentalId = basicDetailUpdVM.RegimentalId;
                    basicDetailUpdVM.PermanentAddress = "Village - " + basicDetailUpdVM.Village + ", Post Office-" + basicDetailUpdVM.PO + ", Tehsil- " + basicDetailUpdVM.Tehsil + ", District- " + basicDetailUpdVM.District + ", State- " + basicDetailUpdVM.State + ", Pin Code- " + basicDetailUpdVM.PinCode;
                    basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                    basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                    basicDetailUpdVM.EncryptedId = Id;
                    ViewBag.OptionsRegimental = service.GetRegimentalDDLIdSelected(basicDetailUpdVM.ArmedId);
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
        [Authorize(Roles = "Admin,User")]
        
        public async Task<IActionResult> BasicDetail(BasicDetailCrtAndUpdVM model)
        {
            try
            {
                ViewBag.OptionsBloodGroup = service.GetBloodGroup();
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (model.BasicDetailId > 0)
                {
                    BasicDetail basicDetail = await basicDetailBL.Get(model.BasicDetailId);

                    if (basicDetail != null)
                    {
                      //  MRegistration? mRegistration = await context.MRegistration.FindAsync(model.RegistrationId);
                        //if (mRegistration.ApplyForId == 1)
                        //{
                        //    ViewBag.OptionsRank = service.GetRank(1);
                        //}
                        //else
                        //{
                        //    ViewBag.OptionsRank = service.GetRank(2);
                        //}
                        if (model.Type==2)
                        {
                            if(model.RegimentalId == null)
                            {
                                ModelState.AddModelError("RegimentalId", "Regimental is required.");
                                goto end;
                            }
                            else
                            {
                                basicDetail.RegimentalId= model.RegimentalId;
                            }
                        }
                        if (ModelState.IsValid)
                        {
                            BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
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
                            newBasicDetail.UpdatedOn =  TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            MTrnUpload mTrnUpload = new MTrnUpload();
                            mTrnUpload.UploadId=model.UploadId;
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
                            mTrnIdentityInfo.BloodGroupId =model.BloodGroupId;
                            mTrnIdentityInfo.Height = model.Height;
                            mTrnIdentityInfo.InfoId = model.InfoId;
                            //MTrnIdentityInfo mTrnIdentityInfo = _mapper.Map<BasicDetailCrtAndUpdVM, MTrnIdentityInfo>(model);
                            if (model.UnitId == 0)
                            {
                                ModelState.AddModelError("", "Please Enter Unit Name");
                            }
                            if (model.ApplyForId != 1 && model.RegimentalId == 0)
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
                            basicDetail.Updatedby = Convert.ToInt32(userId);
                            basicDetail.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            // await basicDetailBL.Update(basicDetail);
                            var ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload, mTrnIdentityInfo,null,null);
                            if(ret1==true)
                            {
                                bool resultforisprocess = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
                                if (!resultforisprocess)
                                {
                                    MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                                    mTrnICardRequest.BasicDetailId = basicDetail.BasicDetailId;
                                    mTrnICardRequest.Status = false;
                                    mTrnICardRequest.TypeId = model.TypeId;
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
                                return RedirectToAction("Index", new { Id = "MQ==" });

                            }else
                            {
                                TempData["success"] = "Updated Not Successfully.";
                                return RedirectToAction("Index", new { Id = "MQ==" });
                            }
                           
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    //MRegistration? mRegistration = await context.MRegistration.FindAsync(model.RegistrationId);
                    //if (mRegistration.ApplyForId == (int)RegistrationType.Officer)
                    //{
                    //    ViewBag.OptionsRank = service.GetRank(1);
                    //}
                    //else
                    //{
                    //    ViewBag.OptionsRank = service.GetRank(2);
                    //}
                    ViewBag.OptionsArmedType = service.GetArmedType();

                    model.Updatedby = Convert.ToInt32(userId);
                    model.StatusLevel = 0;
                    if (ModelState.IsValid)
                    {
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);

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
                        mTrnICardRequest.Status = false;
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
                            return RedirectToAction("Index", new { Id = "MQ==" });

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
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<JsonResult> GetRegimentalListByArmedId(int RegimentalId)
        {
            var regimentals = await service.GetRegimentalListByArmedId(RegimentalId);
            return Json(regimentals);
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
        [Authorize(Roles = "Admin,User")]
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
                mStepCounter.UpdatedOn = DateTime.Now;
                mStepCounter.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                await iStepCounterBL.UpdateStepCounter(mStepCounter);


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>IcardFwd.");
                return BadRequest();
            }
            return Ok(mStepCounter);
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
                if(await iTrnFwnBL.UpdateAllBYRequestId(data.RequestId))
                {
                    data= await iTrnFwnBL.AddWithReturn(data);
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
        [Authorize(Roles = "Admin,User")]
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

        [Authorize(Roles = "Admin,User")]
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
        //[Authorize(Roles = "Admin,User")]
        //[HttpPost]
        //public async Task<IActionResult> DummyData()
        //{
        //    Random rnd1 = new Random();
        //    String[] first = new String[] {"Yogendra", "Ajay", "Dilshad", "Vijay", "Balbeer", "Hari", "Chandra", "Ram",
        //                                    "Rahul", "Roy", "Nikhil", "Manoj", "Ankit", "Pradeep", "Vishkrma", "Ushman" };
        //    String[] last = new String[] {"Kumar", "Gupta", "Garg", "Rajvanshi", "Gujjar", "Singh", "Pal", "Kaushik",
        //                                      "Patel", "Modi", "Sharma", "Vasisth", "Khan", "Babra", "Hussain", "Tyagi" };

        //    String[] BloodGroup = new string[] { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };
        //    String[] Rank = new string[] { "Lt", "Capt", "Maj", "Lt Col", "Col", "Brig", "Maj Gen" };
        //    String[] ArmService = new string[] { "INF", "ARTY", "INT", "EME" };
        //    String[] IdentityMark = new string[] { "Cut mark on thumb", "Cut mark on left leg", "Cut markon right leg" };
        //    String[] PlaceOfIssue = new string[] { "New Mandi", "Ring Road", "Mal Road" };
        //    String[] PermanentAddress = new string[] { "New Mandi", "Ring Road", "Masl Road" };
        //    DateTime DOB = new DateTime(1980, 1, 1);
        //    Random rnd = new Random();
        //    int x = -1;
        //    int y = -1;
        //    int z = -1;
        //    int r = -1;
        //    int s = -1;
        //    int i = -1;
        //    int p = -1;
        //    int a = -1;

        //    x = rnd.Next(0, first.Length);
        //    y = rnd.Next(0, last.Length);
        //    r = rnd.Next(0, Rank.Length);
        //    s = rnd.Next(0, ArmService.Length);
        //    i = rnd.Next(0, IdentityMark.Length);
        //    z = rnd.Next(0, BloodGroup.Length);
        //    p = rnd.Next(0, PlaceOfIssue.Length);
        //    a = rnd.Next(0, PermanentAddress.Length);


        //    DTOBasicDetailRequest basicDetail = new DTOBasicDetailRequest();
        //    basicDetail.Name = first[x] + " " + last[y];
        //    basicDetail.ServiceNo = "IC" + Random.Shared.Next(50000, 99999) + "X";
        //    basicDetail.IdentityMark = IdentityMark[i];
        //    basicDetail.DOB = DOB.AddDays(Random.Shared.Next(1, 365));
        //    basicDetail.Height = Random.Shared.Next(60, 84);
        //    basicDetail.AadhaarNo = rnd1.Next(1000, 9999).ToString() + " " + rnd1.Next(1000, 9999).ToString() + " " + rnd1.Next(1000, 9999).ToString();
        //    basicDetail.BloodGroup = BloodGroup[z];
        //    basicDetail.PlaceOfIssue = PlaceOfIssue[p];
        //    basicDetail.DateOfIssue = DateTime.Now;
        //    basicDetail.DateOfCommissioning = DateTime.Now;
        //    basicDetail.PermanentAddress = "House No.-" + Random.Shared.Next(50, 999) + ", " + PermanentAddress[a];
        //    return Ok(basicDetail);
        //}
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetDataByBasicDetailsId(int Id)
        {
           return Json(await basicDetailBL.GetByBasicDetailsId(Id));
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetRequestHistory(int RequestId)
        {
            return Json(await basicDetailBL.ICardHistory(RequestId));
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetRemarks(DTORemarksRequest Data)
        {
            return Json(await _IMasterBL.GetRemarksByTypeId(Data));
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DataExport(DTODataExportRequest Data)
        {
            try
            {
                var retdata = await basicDetailBL.GetBesicdetailsByRequestId(Data);
             
                var jsonString = JsonConvert.SerializeObject(retdata);
                var jsonde=JsonConvert.DeserializeObject(jsonString);
                
                
                return Json(Encrypt.EncryptParameter(jsonde.ToString()));


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataExport.");
                return RedirectToAction("Error", "Error");
            }
        }
        [Authorize(Roles = "User")]
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


        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "User")]
        public static DirectoryInfo CreateFolder(string baseFolder)
        {
            return Directory.CreateDirectory(baseFolder);
        }
        
    }
}
