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

namespace Web.Controllers
{
    public class BasicDetailController : Controller
    {
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IService service;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<BasicDetailController> _logger;
        public const string SessionKeyStep = "_Step";
        public const string SessionKeyCurStep = "_CurStep";
        public DateTime dateTimenow;
        public BasicDetailController(IUnitOfWork unitOfWork, IService service, IMapper mapper, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger)
        {
            this.unitOfWork = unitOfWork;
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
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allrecord = await Task.Run(()=> unitOfWork.BasicDetail.GetALLBasicDetail(Convert.ToInt32(userId))) ;
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
            BasicDetail? basicDetail = await unitOfWork.BasicDetail.Get(decryptedIntId);

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
            var allrecord = await Task.Run(() => unitOfWork.BasicDetailTemp.GetALLBasicDetailTemp(Convert.ToInt32(userId)));
            _logger.LogInformation(1001, "Index Page Of Basic Detail Temp View");
            ViewBag.Title = "List of Inaccurate Data";
            return View(allrecord);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public Task<ViewResult> Registration()
        {
            //HttpContext.Session.SetInt32(SessionKeyStep, 0);
            //HttpContext.Session.SetInt32(SessionKeyCurStep, 1);
            ViewBag.OptionsRegistrationType = service.GetRegistrationType();
            ViewBag.OptionsSubmitType = service.GetSubmitType();
            return Task.FromResult(View());
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(DTORegistrationRequest model)
        {
            try
            {
                ViewBag.OptionsRegistrationType = service.GetRegistrationType();
                ViewBag.OptionsSubmitType = service.GetSubmitType();
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
                        return RedirectToActionPermanent("Create", "BasicDetail");
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
                        await unitOfWork.BasicDetailTemp.Add(basicDetailTemp);
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
                goto xyz;
            }
            catch (UniqueConstraintException ex)
            {
                _logger.LogError(1002, ex, "UniqueConstraintException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (MaxLengthExceededException ex)
            {
                _logger.LogError(1003, ex, "MaxLengthExceededException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (CannotInsertNullException ex)
            {
                _logger.LogError(1004, ex, "CannotInsertNullException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (NumericOverflowException ex)
            {
                _logger.LogError(1005, ex, "NumericOverflowException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (Exception ex)
            {
                _logger.LogError(1006, ex, "Exception");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }

        xyz:
            return View(model);
        }
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult> Part3(string Id)
        {
            return View();
        }
       
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create()
        {
            TempData.Keep("Registration");
            DTORegistrationRequest? model = new DTORegistrationRequest();
            if (TempData["Registration"]!=null)
            {
                model = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());
                if (model.SubmitType == 1)
                {
                    ViewBag.OptionsRank = service.GetRank(Convert.ToInt32(model.RegType));
                    ViewBag.OptionsArmedType = service.GetArmedType();
                    ViewBag.OptionsBloodGroup = service.GetBloodGroup();
                    DTOBasicDetailCrtRequest dTOBasicDetailCrtRequest = new DTOBasicDetailCrtRequest();
                    dTOBasicDetailCrtRequest.Name = model.Name;
                    dTOBasicDetailCrtRequest.ServiceNo = model.ServiceNo;
                    dTOBasicDetailCrtRequest.DOB = model.DOB;
                    dTOBasicDetailCrtRequest.DateOfCommissioning = model.DateOfCommissioning;
                    dTOBasicDetailCrtRequest.PermanentAddress = model.PermanentAddress;
                    dTOBasicDetailCrtRequest.RegistrationType = model.RegType;
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
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DTOBasicDetailCrtRequest model)
        {
            try
            {
                //ViewBag.OptionsRank = service.GetRank(Convert.ToInt32(model.RegistrationType));
                //ViewBag.OptionsBloodGroup = service.GetBloodGroup();
                //ViewBag.OptionsArmedType = service.GetArmedType();
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                model.Updatedby = Convert.ToInt32(userId);
                if (ModelState.IsValid)
                {
                    BasicDetail newBasicDetail = _mapper.Map<DTOBasicDetailCrtRequest, BasicDetail>(model);
                    string sourceFolderPhotoDB = "/WriteReadData/" + "Photo";
                    string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");

                    if (!Directory.Exists(sourceFolderPhotoPhy))
                        Directory.CreateDirectory(sourceFolderPhotoPhy);

                    if (model.Photo_ != null)
                    {
                        string FileName = service.ProcessUploadedFile(model.Photo_, sourceFolderPhotoPhy);

                        string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                        bool result = service.IsValidHeader (path);
                        bool imgcontentresult = service.IsImage(model.Photo_);

                        if (!result || !imgcontentresult)
                        {
                            ModelState.AddModelError("", "File format not correct");
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            goto xyz;
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
                            goto xyz;
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
                    await unitOfWork.BasicDetail.Add(newBasicDetail);
                    TempData["success"] = "Successfully created.";
                    return RedirectToAction("Index");
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
                goto xyz;
            }
            catch (UniqueConstraintException ex)
            {
                _logger.LogError(1002, ex, "UniqueConstraintException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (MaxLengthExceededException ex)
            {
                _logger.LogError(1003, ex, "MaxLengthExceededException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (CannotInsertNullException ex)
            {
                _logger.LogError(1004, ex, "CannotInsertNullException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (NumericOverflowException ex)
            {
                _logger.LogError(1005, ex, "NumericOverflowException");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }
            catch (Exception ex)
            {
                _logger.LogError(1006, ex, "Exception");
                ModelState.AddModelError("", ex.Message);
                goto xyz;
            }

        xyz:
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Edit(string Id)
        {
            ViewBag.OptionsBloodGroup = service.GetBloodGroup();
            ViewBag.OptionsArmedType = service.GetArmedType();

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
            BasicDetail? basicDetail = await unitOfWork.BasicDetail.Get(decryptedIntId);

            if (basicDetail != null)
            {
                MRank? mRank = await context.MRank.FindAsync(basicDetail.RankId);
                ViewBag.OptionsRank = service.GetRank(mRank.Type);
                DTOBasicDetailUpdRequest basicDetailUpdVM = _mapper.Map<BasicDetail, DTOBasicDetailUpdRequest>(basicDetail);
                //if (basicDetailUpdVM.AadhaarNo != null && basicDetailUpdVM.AadhaarNo.Length == 12)
                //{
                //    string p1, p2, p3;
                //    p1 = basicDetailUpdVM.AadhaarNo.Substring(0, 4);
                //    p2 = basicDetailUpdVM.AadhaarNo.Substring(4, 4);
                //    p3 = basicDetailUpdVM.AadhaarNo.Substring(8, 4);
                //    basicDetailUpdVM.AadhaarNo = p1 + " " + p2 + " " + p3;
                //}
                basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                basicDetailUpdVM.EncryptedId = Id;
                return View(basicDetailUpdVM);
            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DTOBasicDetailUpdRequest model)
        {
            ViewBag.OptionsBloodGroup = service.GetBloodGroup();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            BasicDetail basicDetail = await unitOfWork.BasicDetail.Get(model.BasicDetailId);

            if (basicDetail != null)
            {
                MRank? mRank = await context.MRank.FindAsync(basicDetail.RankId);
                ViewBag.OptionsRank = service.GetRank(mRank.Type);
                if (ModelState.IsValid)
                {
                    basicDetail.RankId = model.RankId;
                    basicDetail.ArmedId = model.ArmedId;
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
                            goto xyz;
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
                            goto xyz;
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
                    await unitOfWork.BasicDetail.Update(basicDetail);
                    TempData["success"] = "Updated Successfully.";
                    return RedirectToAction("Index");
                }
            }
            else
            {

            }


        xyz:
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
            BasicDetail basicDetail = await unitOfWork.BasicDetail.Get(decryptedIntId);
            if (basicDetail == null)
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
            else
            {
                BasicDetail deleteBasicDetail = await unitOfWork.BasicDetail.Delete(basicDetail.BasicDetailId);
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
        [HttpGet]
        public async Task<ActionResult> Part1(string Id)
        {
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

            BasicDetail? basicDetail = await unitOfWork.BasicDetail.Get(decryptedIntId);
            if (basicDetail != null)
            {
                HttpContext.Session.SetInt32(SessionKeyStep, basicDetail.Step);
                HttpContext.Session.SetInt32(SessionKeyCurStep, 1);

                if (basicDetail.Step == 0 || basicDetail.IsSubmit == true)
                    return RedirectToAction("Index");
                ViewBag.Step = basicDetail.Step;

                BasicDetailUpdVMPart1 newBasicDetail = _mapper.Map<BasicDetail, BasicDetailUpdVMPart1>(basicDetail);

                newBasicDetail.EncryptedId = Id;
                newBasicDetail.BasicDetailId = decryptedIntId;

                return View(newBasicDetail);
            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Part1(BasicDetailUpdVMPart1 model)
        {
            BasicDetail? basicDetail = await unitOfWork.BasicDetail.Get(model.BasicDetailId);
            if (basicDetail != null)
            {
                if (basicDetail.Step > 0)
                    return RedirectToAction("Part2", new { Id = model.EncryptedId });
                else
                    return RedirectToAction("Part1", new { Id = model.EncryptedId });
            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", model.BasicDetailId);
            }
        }
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult> Part2(string Id)
        {

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

            BasicDetail? basicDetail = await unitOfWork.BasicDetail.Get(decryptedIntId);
            if (basicDetail != null)
            {
                HttpContext.Session.SetInt32(SessionKeyStep, basicDetail.Step);
                HttpContext.Session.SetInt32(SessionKeyCurStep, 2);

                if (basicDetail.Step < 1 || basicDetail.IsSubmit == true)
                    return RedirectToAction("Index");
                ViewBag.OptionsRank = service.GetRank(1);
                ViewBag.OptionsArmedType = service.GetArmedType();
                ViewBag.OptionsBloodGroup = service.GetBloodGroup();


                BasicDetailUpdVMPart2 newBasicDetail = _mapper.Map<BasicDetail, BasicDetailUpdVMPart2>(basicDetail);
                if (newBasicDetail.Step == 2)
                {
                    //if (newBasicDetail.AadhaarNo != null && newBasicDetail.AadhaarNo.Length == 12)
                    //{
                    //    string p1, p2, p3;
                    //    p1 = newBasicDetail.AadhaarNo.Substring(0, 4);
                    //    p2 = newBasicDetail.AadhaarNo.Substring(4, 4);
                    //    p3 = newBasicDetail.AadhaarNo.Substring(8, 4);
                    //    newBasicDetail.AadhaarNo = p1 + " " + p2 + " " + p3;
                    //}
                    newBasicDetail.ExistingPhotoImagePath = newBasicDetail.PhotoImagePath;
                    newBasicDetail.ExistingSignatureImagePath = newBasicDetail.SignatureImagePath;
                }
                newBasicDetail.EncryptedId = Id;
                newBasicDetail.BasicDetailId = decryptedIntId;

                return View(newBasicDetail);
            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Part2(BasicDetailUpdVMPart2 model)
        {
            ViewBag.OptionsRank = service.GetRank(1);
            ViewBag.OptionsBloodGroup = service.GetBloodGroup();
            ViewBag.OptionsArmedType = service.GetArmedType();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            BasicDetail basicDetail = await unitOfWork.BasicDetail.Get(model.BasicDetailId);

            if (basicDetail != null)
            {
                if (ModelState.IsValid)
                {
                    basicDetail.RankId = model.RankId;
                    basicDetail.ArmedId = model.ArmedId;
                    basicDetail.IdentityMark = model.IdentityMark;
                    basicDetail.Height = model.Height;
                    basicDetail.BloodGroup = model.BloodGroup;
                    basicDetail.PlaceOfIssue = model.PlaceOfIssue;
                    basicDetail.DateOfIssue = model.DateOfIssue;
                    basicDetail.IssuingAuth = model.IssuingAuth;
                    basicDetail.AadhaarNo = model.AadhaarNo;
                    //if (model.AadhaarNo != null)
                    //{
                    //    basicDetail.AadhaarNo = model.AadhaarNo.Replace(" ", "");
                    //}

                    string sourceFolderPhotoDB = "/WriteReadData/" + "Photo";
                    string sourceFolderSignatureDB = "/WriteReadData/" + "Signature";
                    string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo");
                    string sourceFolderSignaturePhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature");

                    if (!Directory.Exists(sourceFolderPhotoPhy))
                        Directory.CreateDirectory(sourceFolderPhotoPhy);

                    if (!Directory.Exists(sourceFolderSignaturePhy))
                        Directory.CreateDirectory(sourceFolderSignaturePhy);

                    if (basicDetail.Step == 2)
                    {
                        if (model.Photo_ != null)
                        {
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
                                goto xyz;
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
                                goto xyz;
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
                    }
                    else
                    {
                        if (model.Photo_ == null)
                        {
                            ModelState.AddModelError("Photo_", "Photo is required.");
                            goto xyz;
                        }
                        else
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
                                goto xyz;
                            }

                            basicDetail.PhotoImagePath = sourceFolderPhotoDB + "/" + FileName;
                            ViewBag.PhotoImagePath = basicDetail.PhotoImagePath;
                        }
                        if (model.Signature_ == null)
                        {
                            ModelState.AddModelError("Signature_", "Signature is required.");
                            goto xyz;
                        }
                        else
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
                                goto xyz;
                            }

                            basicDetail.SignatureImagePath = sourceFolderSignatureDB + "/" + FileName;
                        }
                    }
                    if (basicDetail.Step < 2)
                        basicDetail.Step = 2;

                    basicDetail.Updatedby = Convert.ToInt32(userId);
                    basicDetail.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    BasicDetail updatedbasicDetail = await unitOfWork.BasicDetail.UpdateWithReturn(basicDetail);
                    TempData["success"] = "Updated Successfully.";
                    if (updatedbasicDetail.Step > 1)
                        return RedirectToAction("Part3", new { Id = model.EncryptedId });
                    else
                        return RedirectToAction("Part2", new { Id = model.EncryptedId });
                }
                else
                {
                    Response.StatusCode = 404;
                    return View("BasicDetailNotFound", model.BasicDetailId);
                }

            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", model.BasicDetailId);
            }
        xyz:
            return View(model);

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
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> GetData(string ICNumber)
        {
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
                        DTOApiDataResponse dTOApiDataResponse= new DTOApiDataResponse();
                        dTOApiDataResponse.Status = false;
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
    }
}
