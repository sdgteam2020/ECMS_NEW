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

namespace Web.Controllers
{
    public class BasicDetailController : Controller
    {
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IStepCounterBL iStepCounterBL;
        private readonly ITrnICardRequestBL iTrnICardRequestBL;
        private readonly IDomainMapBL iDomainMapBL;
        private readonly ITrnFwnBL iTrnFwnBL;
        private readonly IBasicDetailBL basicDetailBL;
        private readonly IBasicDetailTempBL basicDetailTempBL;
        private readonly IService service;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<BasicDetailController> _logger;
        public DateTime dateTimenow;
        public BasicDetailController(IBasicDetailBL basicDetailBL, IBasicDetailTempBL basicDetailTempBL, IService service, IMapper mapper, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger, IStepCounterBL iStepCounterBL, ITrnFwnBL iTrnFwnBL, ITrnICardRequestBL iTrnICardRequestBL, IDomainMapBL iDomainMapBL )
        {
            this.basicDetailBL = basicDetailBL;
            this.basicDetailTempBL = basicDetailTempBL;
            this.service = service;
            this._mapper = mapper;
            this.context = context;
            this.contextTransaction = context;
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
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Index(string Id)
        {
            int retint = 0;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            int stepcounter = 0;

            if (!string.IsNullOrEmpty(Id))
            { 
            var base64EncodedBytes = System.Convert.FromBase64String(Id);
            var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            retint = Convert.ToInt32(ret);
                stepcounter = retint;
            }
            ViewBag.Id = retint;
            var allrecord = await Task.Run(()=> basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId),stepcounter , retint)) ;
            _logger.LogInformation(1001, "Index Page Of Basic Detail View");

            if(retint==1)
                  ViewBag.Title = "List of Register I-Card";
            else if(retint == 2)
                ViewBag.Title = "I-Card Pending From Io";
            else if (retint == 22)
                 ViewBag.Title = "I-Card Rejectd From Io";
            else if(retint == 3)
                ViewBag.Title = "I-Card Pending From GSO";
            else if (retint == 33)
                ViewBag.Title = "I-Card Rejectd From GSO";
            else if (retint == 4)
                ViewBag.Title = "I-Card Pending From MI 11";
            else if (retint == 44)
                ViewBag.Title = "I-Card Rejectd From MI 11";
            else if (retint == 5)
                ViewBag.Title = "I-Card Pending From HQ 54";
            else if (retint == 44)
                ViewBag.Title = "I-Card Rejectd From HQ 54";
            return View(allrecord);
        }
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> ApprovalForIO(int Id)
        {
            int type = 0;
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); //SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UserId;
            if (Id == 22)
            {
                Id = 2;
                type = 2;
            }
            else if (Id == 33)
            {
                Id = 3;
                type = 3;
            }
            else if (Id == 44)
            {
                Id = 4;
                type = 4;
                userId = 101;
            }
            else if (Id == 55)
            {
                Id = 5;
                type = 5;
                userId = 29;
            }

            var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), Id, type));
            _logger.LogInformation(1001, "Index Page Of Basic Detail View");
            ViewBag.Title = "List of Register I-Card";
            return View(allrecord);
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
            
            
            BasicDetail? basicDetail = await basicDetailBL.Get(decryptedIntId);

            if (basicDetail != null)
            {
                DTOBasicDetailRequest basicDetailVM = _mapper.Map<BasicDetail, DTOBasicDetailRequest>(basicDetail);
                MRank? mRank = await context.MRank.FindAsync(basicDetail.RankId);
                if(mRank!=null)
                {
                    basicDetailVM.MRank = mRank;
                }
                MArmedType? mArmedType = await context.MArmedType.FindAsync(basicDetail.ArmedId);
                if(mArmedType!=null)
                {
                    basicDetailVM.MArmedType = mArmedType;
                }
                return View(basicDetailVM);
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
            DTORegistrationRequest dTORegistrationRequest = new DTORegistrationRequest();
            dTORegistrationRequest.TypeId =(byte) decryptedIntId;
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
                ViewBag.OptionsRegistration = service.GetRegistration();
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
                        TempData["Registration"] = JsonConvert.SerializeObject(model);
                        return RedirectToActionPermanent("BasicDetail", "BasicDetail");
                    }
                    else
                    {
                        if (model.Observations == null)
                        {
                            ModelState.AddModelError("Observations", "Observations is required.");
                            goto end;
                        }
                        else
                        {
                            BasicDetailTemp basicDetailTemp = new BasicDetailTemp();
                            basicDetailTemp.Name = model.Name;
                            basicDetailTemp.ServiceNo = model.ServiceNo;
                            basicDetailTemp.DOB = model.DOB;
                            basicDetailTemp.DateOfCommissioning = model.DateOfCommissioning;
                            basicDetailTemp.PermanentAddress = model.PermanentAddress;
                            basicDetailTemp.Observations = model.Observations;
                            basicDetailTemp.Updatedby = model.Updatedby;
                            basicDetailTemp.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            await basicDetailTempBL.Add(basicDetailTemp);
                            TempData["success"] = "Request Submited Successfully.";
                            return RedirectToAction("Registration");
                        }
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

            if (Id == null)
            {
                TempData.Keep("Registration");
                DTORegistrationRequest? model = new DTORegistrationRequest();
                if (TempData["Registration"] != null)
                {
                    model = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());
                    if (model.SubmitType == 1)
                    {
                        MRegistration? mRegistration = await context.MRegistration.FindAsync(model.RegId);
                        if(mRegistration.Type== (int)RegistrationType.Officer)
                        {
                            ViewBag.OptionsRank = service.GetRank(1);
                        }
                        else
                        {
                            ViewBag.OptionsRank = service.GetRank(2);
                        }

                        ViewBag.OptionsArmedType = service.GetArmedType();
                        ViewBag.OptionsBloodGroup = service.GetBloodGroup();
                        BasicDetailCrtAndUpdVM dTOBasicDetailCrtRequest = new BasicDetailCrtAndUpdVM();
                        dTOBasicDetailCrtRequest.Name = model.Name;
                        dTOBasicDetailCrtRequest.ServiceNo = model.ServiceNo;
                        dTOBasicDetailCrtRequest.DOB = model.DOB;
                        dTOBasicDetailCrtRequest.DateOfCommissioning = model.DateOfCommissioning;
                        dTOBasicDetailCrtRequest.PermanentAddress = model.PermanentAddress;
                        dTOBasicDetailCrtRequest.RegistrationId = model.RegId;
                        dTOBasicDetailCrtRequest.Type = mRegistration.Type;
                        dTOBasicDetailCrtRequest.TypeId = model.TypeId;
                        dTOBasicDetailCrtRequest.DateOfIssue = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
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
                    decryptedId = protector.Unprotect(Id);
                    decryptedIntId = Convert.ToInt32(decryptedId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                    return RedirectToAction("Error", "Error");
                }
                BasicDetail? basicDetail = await basicDetailBL.Get(decryptedIntId);

                if (basicDetail != null)
                {
                    MRank? mRank = await context.MRank.FindAsync(basicDetail.RankId);
                    ViewBag.OptionsRank = service.GetRank(mRank.Type);
                    BasicDetailCrtAndUpdVM basicDetailUpdVM = _mapper.Map<BasicDetail, BasicDetailCrtAndUpdVM>(basicDetail);
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
                    basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                    basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                    basicDetailUpdVM.EncryptedId = Id;
                    ViewBag.OptionsRegimental = service.GetRegimentalDDLIdSelected(basicDetailUpdVM.ArmedId);
                    ViewBag.UnitName = await context.MUnit.FindAsync(basicDetailUpdVM.UnitId);
                    
                    MRegistration? mRegistration = await context.MRegistration.FindAsync(basicDetailUpdVM.RegistrationId);
                    basicDetailUpdVM.Type = mRegistration != null ? mRegistration.Type : 1;

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
        [ValidateAntiForgeryToken]
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
                        MRegistration? mRegistration = await context.MRegistration.FindAsync(model.RegistrationId);
                        if (mRegistration.Type == 1)
                        {
                            ViewBag.OptionsRank = service.GetRank(1);
                        }
                        else
                        {
                            ViewBag.OptionsRank = service.GetRank(2);
                        }
                        if(model.Type==2)
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
                            basicDetail.RankId = model.RankId;
                            basicDetail.ArmedId = model.ArmedId;
                            basicDetail.UnitId= model.UnitId;
                            basicDetail.IdentityMark = model.IdentityMark;
                            basicDetail.Height = model.Height;
                            basicDetail.BloodGroup = model.BloodGroup;
                            basicDetail.PlaceOfIssue = model.PlaceOfIssue;
                            basicDetail.DateOfIssue = model.DateOfIssue;
                            basicDetail.IssuingAuth = model.IssuingAuth;

                            if (model.Photo_ != null)
                            {
                                string sourceFolderPhotoDB = "/WriteReadData/" + "Photo";
                                string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");

                                string FileName = service.ProcessUploadedFile(model.Photo_, sourceFolderPhotoPhy);
                                string filePath = Path.Combine(sourceFolderPhotoPhy, FileName);

                                bool imgcontentresult = service.IsImage(model.Photo_);

                                bool result = service.IsValidHeader(filePath);

                                if (!result || !imgcontentresult)
                                {
                                    ModelState.AddModelError("", "File format not correct");
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }
                                    goto end;
                                }

                                if (model.ExistingPhotoImagePath != null)
                                {
                                    string f = Path.Join(hostingEnvironment.WebRootPath, basicDetail.PhotoImagePath.Replace('/', '\\').ToString());
                                    if (System.IO.File.Exists(f))
                                    {
                                        System.IO.File.Delete(f);
                                    }
                                }
                                basicDetail.PhotoImagePath = sourceFolderPhotoDB + "/" + FileName;
                            }

                            if (model.Signature_ != null)
                            {
                                string sourceFolderSignatureDB = "/WriteReadData/" + "Signature";
                                string sourceFolderSignaturePhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature");

                                string FileName = service.ProcessUploadedFile(model.Signature_, sourceFolderSignaturePhy);
                                string filePath = Path.Combine(sourceFolderSignaturePhy, FileName);

                                bool imgcontentresult = service.IsImage(model.Signature_);

                                bool result = service.IsValidHeader(filePath);

                                if (!result || !imgcontentresult)
                                {
                                    ModelState.AddModelError("", "File format not correct");
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }
                                    goto end;
                                }

                                if (model.ExistingSignatureImagePath != null)
                                {
                                    string f = Path.Join(hostingEnvironment.WebRootPath, basicDetail.SignatureImagePath.Replace("/", "\\"));
                                    if (System.IO.File.Exists(f))
                                    {
                                        System.IO.File.Delete(f);
                                    }
                                }
                                basicDetail.SignatureImagePath = sourceFolderSignatureDB + "/" + FileName;
                            }
                            //if (model.AadhaarNo != null)
                            //{
                            //    basicDetail.AadhaarNo = model.AadhaarNo.Replace(" ", "");
                            //}
                            basicDetail.AadhaarNo = model.AadhaarNo;
                            basicDetail.Updatedby = Convert.ToInt32(userId);
                            basicDetail.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            await basicDetailBL.Update(basicDetail);
                            TempData["success"] = "Updated Successfully.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    MRegistration? mRegistration = await context.MRegistration.FindAsync(model.RegistrationId);
                    if (mRegistration.Type == (int)RegistrationType.Officer)
                    {
                        ViewBag.OptionsRank = service.GetRank(1);
                    }
                    else
                    {
                        ViewBag.OptionsRank = service.GetRank(2);
                    }
                    ViewBag.OptionsArmedType = service.GetArmedType();

                    model.Updatedby = Convert.ToInt32(userId);
                    model.StatusLevel = 0;
                    if (ModelState.IsValid)
                    {
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
                        string sourceFolderPhotoDB = "/WriteReadData/" + "Photo";
                        string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");

                        if (!Directory.Exists(sourceFolderPhotoPhy))
                            Directory.CreateDirectory(sourceFolderPhotoPhy);

                        if (model.Photo_ != null)
                        {
                            string FileName = service.ProcessUploadedFile(model.Photo_, sourceFolderPhotoPhy);

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

                            newBasicDetail.PhotoImagePath = sourceFolderPhotoDB + "/" + FileName;
                            ViewBag.PhotoImagePath = newBasicDetail.PhotoImagePath;
                        }
                        else
                        {
                            ModelState.AddModelError("Photo_", "Photo is required.");
                        }

                        string sourceFolderSignatureDB = "/WriteReadData/" + "Signature";
                        string sourceFolderSignaturePhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature");

                        if (!Directory.Exists(sourceFolderSignaturePhy))
                            Directory.CreateDirectory(sourceFolderSignaturePhy);

                        if (model.Signature_ != null)
                        {
                            string FileName = service.ProcessUploadedFile(model.Signature_, sourceFolderSignaturePhy);

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

                            newBasicDetail.SignatureImagePath = sourceFolderSignatureDB + "/" + FileName;
                        }
                        else
                        {
                            ModelState.AddModelError("Signature_", "Signature is required.");
                        }
                        //if (model.AadhaarNo != null)
                        //{
                        //    newBasicDetail.AadhaarNo = model.AadhaarNo.Replace(" ", "");
                        //}
                        BasicDetail ret = new BasicDetail();
                        ret = await basicDetailBL.AddWithReturn(newBasicDetail);
                        if (ret != null)
                        {

                            MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                            mTrnICardRequest.BasicDetailId = ret.BasicDetailId;
                            mTrnICardRequest.Status = false;
                            mTrnICardRequest.TypeId = model.TypeId;
                            //TrnDomainMapping trnDomainMapping = new TrnDomainMapping();
                            // trnDomainMapping.AspNetUsersId= Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                            //trnDomainMapping=await iDomainMapBL.GetByAspnetUserIdBy(trnDomainMapping);
                            mTrnICardRequest.TrnDomainMappingId= SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").TrnDomainMappingId;
                            mTrnICardRequest.UpdatedOn = DateTime.Now;
                            mTrnICardRequest.Updatedby = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").UserId; //SessionHeplers.GetObject<string>(HttpContext.Session, "ArmyNo");
                            mTrnICardRequest = await iTrnICardRequestBL.AddWithReturn(mTrnICardRequest);
                            if (mTrnICardRequest.RequestId > 0)
                            {
                                MStepCounter mStepCounter = new MStepCounter();
                                mStepCounter.StepId = Convert.ToByte(1);
                                mStepCounter.RequestId = mTrnICardRequest.RequestId;
                                mStepCounter.UpdatedOn = DateTime.Now;
                                mStepCounter.Updatedby = 1;
                                await iStepCounterBL.Add(mStepCounter);
                            }

                        }
                        TempData["success"] = "Successfully created.";
                        return RedirectToAction("Index", new { Id = 1 });
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
                if (deleteBasicDetail != null)
                {
                    if (deleteBasicDetail.PhotoImagePath != null)
                    {
                        string sourcePhotoImagePathPhy = Path.Join(hostingEnvironment.WebRootPath, deleteBasicDetail.PhotoImagePath.Replace('/', '\\').ToString());
                        if (System.IO.File.Exists(sourcePhotoImagePathPhy))
                        {
                            System.IO.File.Delete(sourcePhotoImagePathPhy);
                        }
                    }
                    if (deleteBasicDetail.SignatureImagePath != null)
                    {
                        string sourceSignatureImagePath = Path.Join(hostingEnvironment.WebRootPath, deleteBasicDetail.SignatureImagePath.Replace('/', '\\').ToString());
                        if (System.IO.File.Exists(sourceSignatureImagePath))
                        {
                            System.IO.File.Delete(sourceSignatureImagePath);
                        }
                    }
                }

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
            var user = await context.BasicDetails.FirstOrDefaultAsync(x => x.ServiceNo == ServiceNo);

            if (ServiceNo == initialServiceNo)
            {
                return Json(true);
            }
            else if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Service No {ServiceNo} is already in use.");
            }
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
                mStepCounter.Updatedby = 1;
                await iStepCounterBL.Update(mStepCounter);

                
            }
            catch (Exception ex) { }
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
                    await iTrnFwnBL.Add(data);
                    return Ok(data);
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex) 
            { 
                
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

                return BadRequest();
            }




        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> GetData(string ICNumber)
        {
            if(ICNumber!=null)
            {
                BasicDetail? basicDetail = await basicDetailBL.FindServiceNo(ICNumber);
                if(basicDetail!=null) 
                {
                    bool result = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
                    if (result)
                    {
                        DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
                        dTOApiDataResponse.Status = false;
                        dTOApiDataResponse.Message = "Your I-Card is under process. Please wait.";
                        return Ok(dTOApiDataResponse);
                    }
                    else
                    {
                        goto api;
                    }
                }
                else
                {
                    goto api;
                }
            }
            else
            {
                DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
                dTOApiDataResponse.Status = false;
                dTOApiDataResponse.Message = "Service no required.";
                return Ok(dTOApiDataResponse);
            }
            api:
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://api.postalpincode.in/");
                client.BaseAddress = new Uri("https://localhost:7002/api/Fetch/GetData/");
                //using (HttpResponseMessage response = await client.GetAsync("ICNumber/" + ICNumber))
                using (HttpResponseMessage response = await client.GetAsync(ICNumber))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        response.EnsureSuccessStatusCode();
                        DTOApiDataResponse? responseData = JsonConvert.DeserializeObject<DTOApiDataResponse>(responseContent);
                        DateTime DOB, DOC;
                        TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 0, 0);
                        DOB = responseData.DOB.Date + timeSpan;
                        DOC = responseData.DateOfCommissioning.Date + timeSpan;
                        responseData.DOB = DOB;
                        responseData.DateOfCommissioning = DOC;
                        responseData.Status = true;
                        return Ok(responseData);
                    }
                    else
                    {
                        DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
                        dTOApiDataResponse.Status = false;
                        dTOApiDataResponse.Message = "Data Not Found.";
                        return Ok(dTOApiDataResponse);
                    }
                }
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
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> DummyData()
        {
            Random rnd1 = new Random();
            String[] first = new String[] {"Yogendra", "Ajay", "Dilshad", "Vijay", "Balbeer", "Hari", "Chandra", "Ram",
                                            "Rahul", "Roy", "Nikhil", "Manoj", "Ankit", "Pradeep", "Vishkrma", "Ushman" };
            String[] last = new String[] {"Kumar", "Gupta", "Garg", "Rajvanshi", "Gujjar", "Singh", "Pal", "Kaushik",
                                              "Patel", "Modi", "Sharma", "Vasisth", "Khan", "Babra", "Hussain", "Tyagi" };

            String[] BloodGroup = new string[] { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };
            String[] Rank = new string[] { "Lt", "Capt", "Maj", "Lt Col", "Col", "Brig", "Maj Gen" };
            String[] ArmService = new string[] { "INF", "ARTY", "INT", "EME" };
            String[] IdentityMark = new string[] { "Cut mark on thumb", "Cut mark on left leg", "Cut markon right leg" };
            String[] PlaceOfIssue = new string[] { "New Mandi", "Ring Road", "Mal Road" };
            String[] PermanentAddress = new string[] { "New Mandi", "Ring Road", "Masl Road" };
            DateTime DOB = new DateTime(1980, 1, 1);
            Random rnd = new Random();
            int x = -1;
            int y = -1;
            int z = -1;
            int r = -1;
            int s = -1;
            int i = -1;
            int p = -1;
            int a = -1;

            x = rnd.Next(0, first.Length);
            y = rnd.Next(0, last.Length);
            r = rnd.Next(0, Rank.Length);
            s = rnd.Next(0, ArmService.Length);
            i = rnd.Next(0, IdentityMark.Length);
            z = rnd.Next(0, BloodGroup.Length);
            p = rnd.Next(0, PlaceOfIssue.Length);
            a = rnd.Next(0, PermanentAddress.Length);


            DTOBasicDetailRequest basicDetail = new DTOBasicDetailRequest();
            basicDetail.Name = first[x] + " " + last[y];
            basicDetail.ServiceNo = "IC" + Random.Shared.Next(50000, 99999) + "X";
            basicDetail.IdentityMark = IdentityMark[i];
            basicDetail.DOB = DOB.AddDays(Random.Shared.Next(1, 365));
            basicDetail.Height = Random.Shared.Next(60, 84);
            basicDetail.AadhaarNo = rnd1.Next(1000, 9999).ToString() + " " + rnd1.Next(1000, 9999).ToString() + " " + rnd1.Next(1000, 9999).ToString();
            basicDetail.BloodGroup = BloodGroup[z];
            basicDetail.PlaceOfIssue = PlaceOfIssue[p];
            basicDetail.DateOfIssue = DateTime.Now;
            basicDetail.DateOfCommissioning = DateTime.Now;
            basicDetail.PermanentAddress = "House No.-" + Random.Shared.Next(50, 999) + ", " + PermanentAddress[a];
            return Ok(basicDetail);
        }

        public async Task<IActionResult> GetDataByBasicDetailsId(int Id)
        {
           return Json(await basicDetailBL.GetByBasicDetailsId(Id));
        }
        public async Task<IActionResult> GetRequestHistory(int RequestId)
        {
            return Json(await basicDetailBL.ICardHistory(RequestId));
        }
    }
}
