using AutoMapper;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
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
using System.IO.Compression;
using BusinessLogicsLayer.TrnLoginLog;
using Web.Healpers;
using System.Xml.Serialization;
using System.Xml.Linq;
using BusinessLogicsLayer.TrnICardHold;
using DataTransferObject.Domain.Master;
using DataAccessLayer.Healpers;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DataTransferObject.Constants;
using CsvHelper;
using CsvHelper.Configuration;
using BusinessLogicsLayer.CSVImports;
using BusinessLogicsLayer.FaultyCard;
using Humanizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;
using BusinessLogicsLayer.HotlistCard;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        private readonly string[] _expectedColumns = { "RequestId", "RankName", "FName", "LName", "ServiceNo", "ChipNo", "CardSerialNo" };
        private readonly  ICSVImportBL _iCSVImportBL;
        private readonly IFaultyCardBL faultyCardBL;
        private readonly IHotlistCardBL _hotlistCardBL;

        public BasicDetailController(IConfiguration configuration,IBasicDetailBL basicDetailBL, IMapUnitBL mapUnitBL, IBasicDetailTempBL basicDetailTempBL, IService service, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger, IStepCounterBL iStepCounterBL, 
                              ITrnFwnBL iTrnFwnBL, ITrnICardRequestBL iTrnICardRequestBL, IDomainMapBL iDomainMapBL
            ,IBasicUploadBL basicUploadBL, IBasicAddressBL basicAddressBL, IBasicinfoBL basicinfoBL, IRankBL rankBL, INotificationBL notificationBL, IMasterBL masterBL
           , ITrnLoginLogBL iTrnLoginLogBL, IICardHoldBL iICardHoldBL, ICSVImportBL iCSVImportBL, IFaultyCardBL _faultyCardBL,IHotlistCardBL hotlistCardBL)
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
            _iCSVImportBL = iCSVImportBL;
            faultyCardBL = _faultyCardBL;
            _hotlistCardBL = hotlistCardBL;
        }

        #region Index/ApprovalForIO/View/InaccurateData/InaccurateDataView/RequestType

        public async Task<ActionResult> Index(string Id, string jcoor)
        {

            MTrnNotification noti = new MTrnNotification();
            int retint = 0; int type = 1;
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
                ViewBag.Title = "Status of Appl Approved & Fwd"; type = 2; stepcounter = 888;
            }
            else if (retint == 88)
            {
                // request from Task Board
                ViewBag.Title = "Status of Appl Approved & Fwd"; type = 2; stepcounter = 888;
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
                ViewBag.Title = "Appl rejected by Approver, Verifier"; type = 2; stepcounter = 999;
            }
            else if (retint == 99)
            {
                // request from Task Board
                ViewBag.Title = "Appl rejected by Approver, Verifier"; type = 2; stepcounter = 999;
            }

            if (stepcounter == 0)
            {
                var allrecord = await Task.Run(() => basicDetailBL.GetALLForIcardSttaus(Convert.ToInt32(userId), stepcounter, type, 0));

                _logger.LogInformation(1001, "Index Page Of Basic Detail View");

                return View(allrecord);
            }
            else if (string.IsNullOrEmpty(jcoor))
            {
                var allrecord = await Task.Run(() => basicDetailBL.GetALLForIcardSttaus(Convert.ToInt32(userId), stepcounter, type, 1));

                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                noti.DisplayId = stepcounter;
                await _INotificationBL.UpdateRead(noti);

                return View(allrecord);
            }
            else
            {
                var allrecord = await Task.Run(() => basicDetailBL.GetALLForIcardSttaus(Convert.ToInt32(userId), stepcounter, type, 2));

                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                noti.DisplayId = stepcounter + 10;
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
                var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), stepcounter, type, 1));
                _logger.LogInformation(1001, "Index Page Of Basic Detail View");
                await _INotificationBL.UpdateRead(noti);
                return View(allrecord);

            }
            else
            {
                ViewBag.jcoor = 0;
                noti.DisplayId = stepcounter + 10;
                var allrecord = await Task.Run(() => basicDetailBL.GetALLBasicDetail(Convert.ToInt32(userId), stepcounter, type, 2));
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
                basicDetailCrtAndUpdVM.AadhaarNo = basicDetailCrtAndUpdVM.AadhaarNo.Substring((basicDetailCrtAndUpdVM.AadhaarNo.Length - 4), 4);
                return View(basicDetailCrtAndUpdVM);
            }
            else
            {
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
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
                    short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]);
                    //if (ArmedIdForORO == 0) ArmedIdForORO = 56;

                    DTOApplFwdConditionRequest? dTOApplFwdCondition = _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>() ?? new DTOApplFwdConditionRequest
                    {
                        MPRSO = new MPRSO(),
                        MP6F = new MP6F(),
                        MP6A = new MP6A()
                    };

                    if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                        string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) ||
                        string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name) || dTOApplFwdCondition.MP6A.RankOrderby == 0 || ArmedIdForORO == 0)
                    {
                        TempData["error"] = "Invalid Input.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                    else
                    {
                        var allrecord = await Task.Run(() => basicDetailTempBL.GetALLBasicDetailTemp(Convert.ToInt32(userId), typeId, dTOApplFwdCondition, ArmedIdForORO));
                        ViewBag.Title = typeId == 1 ? "Requests pending due to Incorrect Details/Data" : "List of Observation Raised";
                        return View(allrecord);
                    }

                    //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name)) dTOApplFwdCondition.MPRSO.Name = "MPRSO";
                    //if (dTOApplFwdCondition.MPRSO.ArmedAbbreviation == null || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0)
                    //    dTOApplFwdCondition.MPRSO.ArmedAbbreviation = new List<string> { "ADC", "AMC", "MNS" };
                    //if (dTOApplFwdCondition.MPRSO.RecordOfficeId == 0) dTOApplFwdCondition.MPRSO.RecordOfficeId = 135;

                    //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name)) dTOApplFwdCondition.MP6F.Name = "MP 6F";
                    //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix)) dTOApplFwdCondition.MP6F.ArmyNoPrefix = "SL";
                    //if (dTOApplFwdCondition.MP6F.RecordOfficeId == 0) dTOApplFwdCondition.MP6F.RecordOfficeId = 132;

                    //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name)) dTOApplFwdCondition.MP6A.Name = "MP 6A";
                    //if (dTOApplFwdCondition.MP6A.RankOrderby == 0) dTOApplFwdCondition.MP6A.RankOrderby = 4;
                    //if (dTOApplFwdCondition.MP6A.RecordOfficeId == 0) dTOApplFwdCondition.MP6A.RecordOfficeId = 126;
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
                DTOBasicDetailTempRequest? dTOBasicDetail = await basicDetailTempBL.GetALLBasicDetailTempByBasicDetailId(userIntId, decryptedIntId);
                if (dTOBasicDetail != null)
                {
                    return View(dTOBasicDetail);
                }
                else
                {
                    TempData["error"] = "Id not found.";
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

        [HttpGet]
        public async Task<ActionResult> RequestType()
        {
            var allrecord = await Task.Run(() => basicDetailBL.GetAllICardType());
            return View(allrecord);
        }

        #endregion

        #region Registration/BasicDetail/GetApiData/GetUserData

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
                        Data = await basicDetailBL.FindServiceNo(model.ServiceNo);
                        if (Data != null)
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            string id = protector.Protect(Data.BasicDetailId.ToString());
                            return RedirectToAction("BasicDetail", "BasicDetail", new { Id = protector.Protect(Convert.ToString(Data.BasicDetailId)) });
                        }
                        else
                        {
                            TempData["Registration"] = JsonConvert.SerializeObject(model);
                            return RedirectToAction("BasicDetail", "BasicDetail", new { Id = protector.Protect("0") });
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
                        basicDetailTemp.ApplyForId = model.ApplyForId;
                        basicDetailTemp.RegistrationId = model.RegistrationId;
                        basicDetailTemp.TypeId = model.TypeId;
                        basicDetailTemp.RankId = model.RankId;
                        basicDetailTemp.ArmedId = model.ArmedId;
                        basicDetailTemp.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        BasicDetailTemp? temp = await basicDetailTempBL.GetByArmyNo(model.ServiceNo);

                        if (temp != null && temp.BasicDetailTempId > 0)
                        {
                            basicDetailTemp.BasicDetailTempId = temp.BasicDetailTempId;
                            await basicDetailTempBL.Update(basicDetailTemp);
                        }
                        else
                        {
                            await basicDetailTempBL.Add(basicDetailTemp);
                        }
                        TempData["success"] = "Request Submited Successfully.";
                        return RedirectToAction("InaccurateData", "BasicDetail", new { Id = "MQ==" });
                    }
                }
                else
                {
                    var error = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
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
                //TempData.Keep("Registration"); // not required for keep
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
                        dTOBasicDetailCrtRequest.NameAsPerRecord = model.NameAsPerRecord;
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
                        dTOBasicDetailCrtRequest.PermanentAddress = "Village - " + model.Village + ", Post Office-" + model.PO + ", Tehsil- " + model.Tehsil + ", District- " + model.District + ", State- " + model.State + ", Pin Code- " + (model.PinCode == 0 ?"" : model.PinCode);

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
                    basicDetailUpdVM.PermanentAddress = "Village - " + basicDetailUpdVM.Village + ", Post Office-" + basicDetailUpdVM.PO + ", Tehsil- " + basicDetailUpdVM.Tehsil + ", District- " + basicDetailUpdVM.District + ", State- " + basicDetailUpdVM.State + ", Pin Code- " + (basicDetailUpdVM.PinCode == 0 ? "" : basicDetailUpdVM.PinCode);

                    string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
                    string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", basicDetailUpdVM.PhotoImagePath);
                    string sourcePathSignature = Path.Combine(sourceFolderPhotoPhy, "Signature", basicDetailUpdVM.SignatureImagePath);
                    
                    if (System.IO.File.Exists(sourcePathPhoto))
                    {
                        basicDetailUpdVM.ExistingPhotoInBase64 = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                        basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                    }

                    if (System.IO.File.Exists(sourcePathSignature))
                    {
                        basicDetailUpdVM.ExistingSignatureInBase64 = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
                        basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                    }


                    basicDetailUpdVM.EncryptedId = Id;

                    if (TempData["Registration"] != null)
                    {
                        var modelex = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());

                        basicDetailUpdVM.FName = modelex.FName;
                        basicDetailUpdVM.LName = modelex.LName;
                        basicDetailUpdVM.ServiceNo = modelex.ServiceNo;
                        basicDetailUpdVM.DOB = modelex.DOB;
                        basicDetailUpdVM.DateOfCommissioning = modelex.DateOfCommissioning;
                        //basicDetailUpdVM.IdenMark1 = modelex.IdenMark1;
                        //basicDetailUpdVM.IdenMark2 = modelex.IdenMark2;
                        ViewBag.OptionsRankId = modelex.RankId;
                        //basicDetailUpdVM.AadhaarNo = Convert.ToInt64(modelex.AadhaarNo).ToString("D12"); ;// Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
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
                        basicDetailUpdVM.PermanentAddress = "Village - " + modelex.Village + ", Post Office-" + modelex.PO + ", Tehsil- " + modelex.Tehsil + ", District- " + modelex.District + ", State- " + modelex.State + ", Pin Code- " + (modelex.PinCode == 0 ? "" : modelex.PinCode);

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
                    ViewBag.OptionsRankId = model.RankId;
                    ViewBag.OptionsUnitId = model.UnitId;
                    ViewBag.OptionsArmedId = model.ArmedId;
                    ViewBag.OptionsRegimentalId = model.RegimentalId;
                    ViewBag.OptionsBloodGroupId = model.BloodGroupId;

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
                    if (string.IsNullOrEmpty(model.AadhaarNo) || model.AadhaarNo.Length != 12 || !model.AadhaarNo.All(char.IsDigit) || model.AadhaarNo == "000000000000")
                    {
                        ModelState.AddModelError("AadhaarNo", "Aadhaar number must be exactly 12 digits.");
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
                        string sourceFolderPhotoPhy_Old = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
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
                            else
                            {
                                string uniqueFileName = FileName + ".enc";
                                //string destinationPath = sourceFolderPhotoPhy + model.ServiceNo + ".txt";
                                string destinationPath = Path.Combine(sourceFolderPhotoPhy, uniqueFileName);

                                // Old image delete before new image created
                                if(model.ExistingPhotoImagePath != null)
                                {
                                    string ExitImagePath = Path.Combine(sourceFolderPhotoPhy_Old,"Photo" ,model.ExistingPhotoImagePath);
                                    if (System.IO.File.Exists(ExitImagePath))
                                    {
                                        System.IO.File.Delete(ExitImagePath);
                                    }
                                }

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
                            else
                            {
                                string uniqueFileName = FileName + ".enc";
                                //string destinationPath = sourceFolderSignaturePhy + model.ServiceNo + ".txt";
                                string destinationPath = Path.Combine(sourceFolderSignaturePhy, uniqueFileName);

                                // Old signature image delete before new image created
                                if (model.ExistingSignatureImagePath != null)
                                {
                                    string ExitImagePath = Path.Combine(sourceFolderPhotoPhy_Old, "Signature", model.ExistingSignatureImagePath);
                                    if (System.IO.File.Exists(ExitImagePath))
                                    {
                                        System.IO.File.Delete(ExitImagePath);
                                    }
                                }

                                ImageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                // mTrnUpload.SignatureImagePath = GetCreateMyFolder() + "/" + FileName;
                                mTrnUpload.SignatureImagePath = GetCreateMyFolder() + "/" + FileName + ".enc";
                            }
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
                        mTrnIdentityInfo.IdenMark1 = model.IdenMark1;
                        mTrnIdentityInfo.IdenMark2 = model.IdenMark2;
                        mTrnIdentityInfo.AadhaarNo = Convert.ToInt64(model.AadhaarNo);
                        mTrnIdentityInfo.BloodGroupId = model.BloodGroupId;
                        mTrnIdentityInfo.Height = model.Height;
                        if (model.UnitId == 0)
                        {
                            ModelState.AddModelError("", "Please Enter Unit Name");
                        }
                        if (model.ApplyForId != 1 && model.RegimentalId == 0)
                        {
                            ModelState.AddModelError("", "Please Select Regimental ");
                        }
                        if (string.IsNullOrEmpty(model.AadhaarNo) || model.AadhaarNo.Length != 12 || !model.AadhaarNo.All(char.IsDigit) || model.AadhaarNo == "000000000000")
                        {
                            ModelState.AddModelError("AadhaarNo", "Aadhaar number must be exactly 12 digits.");
                            goto end;
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
                            else
                            {
                                string uniqueFileName = FileName + ".enc";
                                //string destinationPath = sourceFolderSignaturePhy + model.ServiceNo + ".txt";
                                string destinationPath = Path.Combine(sourceFolderSignaturePhy, uniqueFileName);
                                ImageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                // mTrnUpload.SignatureImagePath = GetCreateMyFolder() + "/" + FileName;
                                mTrnUpload.SignatureImagePath = GetCreateMyFolder() + "/" + FileName + ".enc";
                            }
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
                        string tracid = model.DOB.Day.ToString("D2") + "" + model.DOB.Month.ToString("D2") + "" + model.DOB.Year + "" + Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
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

                        DTOBasicDetailsSaveResponse ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload, mTrnIdentityInfo, mTrnICardRequest, mStepCounter);
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

        #endregion

        #region DecryptZipFile/DecryptZipFileData

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

        #endregion

        #region CSVFileUpload/UploadCsv/GetHeaderMap/UploadChipAndSerial
        [HttpGet]
        public Task<ActionResult> CSVFileUpload(string jcoor)
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
                _logger.LogError(1001, ex, "BasicDetailsController=>CSVFileUpload.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
        }
        [HttpPost]
        public IActionResult UploadCsv(DTOCSVFileRequest model)
        {
            if (model.CSVFile == null || model.CSVFile.Length == 0)
            {
                return BadRequest(new { message = "File is not uploaded or is empty." });
            }

            var records = new List<object>();

            try
            {
                using (var stream = new StreamReader(model.CSVFile.OpenReadStream()))
                {
                    string? headerLine = stream.ReadLine();
                    if (headerLine == null)
                    {
                        return BadRequest(new { message = "File is empty or missing headers." });
                    }

                    var headers = headerLine.Split(',');
                    var headerMap = GetHeaderMap(headers);

                    if (headerMap == null)
                    {
                        return BadRequest(new
                        {
                            message = $"Invalid column names. Expected: {string.Join(", ", _expectedColumns)}"
                        });
                    }

                    string? line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        var values = line.Split(',');

                        var record = new
                        {
                            ServiceNo = values[headerMap["ServiceNo"]],
                            RankName = values[headerMap["RankName"]],
                            FName = values[headerMap["FName"]],
                            LName = values[headerMap["LName"]],
                            RequestId = int.TryParse(values[headerMap["RequestId"]], out var requestId) ? requestId : -1,
                            ChipNo = values[headerMap["ChipNo"]],
                            CardSerialNo = values[headerMap["CardSerialNo"]],
                            IsValid = true
                        };

                        if (record.RequestId == -1 || record.ChipNo.Length != 12 || !long.TryParse(record.ChipNo, out _))
                        {
                            records.Add(new
                            {
                                record.RequestId,
                                record.ServiceNo,
                                record.RankName,
                                record.FName,
                                record.LName,
                                record.ChipNo,
                                record.CardSerialNo,
                                IsValid = false
                            });
                        }
                        else
                        {
                            records.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Return a JSON response with status code 500 and detailed error message
                return StatusCode(500, new { message = $"An error occurred while processing the file: {ex.Message}" });
            }

            return Ok(records);
        }

        private Dictionary<string, int>? GetHeaderMap(string[] headers)
        {
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headers.Length; i++)
            {
                headerMap[headers[i]] = i;
            }

            if (!_expectedColumns.All(expected => headerMap.ContainsKey(expected)))
            {
                return null;
            }

            return headerMap;
        }

        [HttpPost]
        public async Task<ActionResult> UploadChipAndSerial([FromBody] List<DTOUploadChipAndSerialRequest> data)
        {
            // Validate that lstUpdate contains at least one record
            if (data == null || data.Count == 0)
            {
                return BadRequest(new { message = "No records received. Please select at least one record to process." });
            }
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();

            if (ModelState.IsValid)
            {

                response = await basicDetailBL.UploadChipAndSerial(data);
                if (response.Result == true)
                {
                    return Json(response);
                }
                else
                {
                    return Json(response);
                }
            }
            else
            {
                response.Result=false; 
                response.Message = ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToString();
                //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                return Json(response);
            }
        }


        [HttpPost]
        public async Task<ActionResult> GetCSVFileUploadsHistory([FromForm] DTODataTablesRequest dTO)
        {
            try
            {
                return Json(await _iCSVImportBL.GetDataTableResponse(dTO));
            }
            catch (Exception ex)
            {
                List<CSVImport> dTOClaimsStoreResponses = new List<CSVImport>();
                var responseData = new DTODataTablesResponse<CSVImport>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOClaimsStoreResponses
                };
                _logger.LogError(1001, ex, "BasicDetail->GetCSVFileUploadsHistory");
                return Json(responseData);
            }
        }

        #endregion

        #region SaveInternalFwd/IcardFwd/IcardRejecte/UpdateStepCounter/SaveICardRequestHold/DataExport/DataDigitalXmlSign/GenerateLastRecordXml/MergeXmlDocuments/GenerateJsonResponse

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
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.UpdatedOn = DateTime.Now;
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId = Convert.ToByte(data.TypeId);
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
                DTOBasicDetailsSaveResponse response = new DTOBasicDetailsSaveResponse();

                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.UpdatedOn = DateTime.Now;
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId = Convert.ToByte(1);
                TrnDomainMapping Domain = new TrnDomainMapping();
                Domain = await iDomainMapBL.GetByRequestId(data.RequestId);
                if (Domain != null)
                {
                    data.ToAspNetUsersId = Domain.AspNetUsersId;
                    data.ToUserId = Domain.UserId.GetValueOrDefault();

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
        
        public async Task<IActionResult> UpdateStepCounter(MStepCounter mStepCounter)
        {
            DTOBasicDetailsSaveResponse response = new DTOBasicDetailsSaveResponse();
            try
            {
                if (mStepCounter.Flag == "R")
                {
                    TrnDomainMapping Domain = new TrnDomainMapping();
                    Domain = await iDomainMapBL.GetByRequestId(mStepCounter.RequestId);

                    if (Domain?.UserId.GetValueOrDefault() == 0)
                    {
                        response.Message = "Profile is not mapped with domain Id!";
                        response.Result = false;
                        return Ok(response);
                    }
                }
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                DTOMapUnitResponse dTOMapUnitResponse = await mapUnitBL.GetALLByUnitMapId(sessiondata.UnitId);

                mStepCounter.UpdatedOn = DateTime.Now;
                mStepCounter.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                mStepCounter.UnitName = dTOMapUnitResponse.UnitName;
                await iStepCounterBL.UpdateStepCounter(mStepCounter);
                response.Result = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>IcardFwd.");
                response.Message = "Internal Server Error!";
            }
            return Ok(response);
        }
        
        //[Authorize(Roles = "DteAdmin")]
        [Authorize(Policy = "FlagICardApplPolicy")]
        public async Task<IActionResult> SaveICardRequestHold(MTrnICardHold dTO)
        {
            try
            {
                DtoSession? sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                if (sessiondata != null)
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

                DTOApplFwdConditionRequest? dTOApplFwdCondition = _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>() ?? new DTOApplFwdConditionRequest
                {
                    MPRSO = new MPRSO(),
                    MP6F = new MP6F(),
                    MP6A = new MP6A()
                };

                //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name)) dTOApplFwdCondition.MPRSO.Name = "MPRSO";
                //if (dTOApplFwdCondition.MPRSO.ArmedAbbreviation == null || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0)
                //    dTOApplFwdCondition.MPRSO.ArmedAbbreviation = new List<string> { "ADC", "AMC", "MNS" };
                //if (dTOApplFwdCondition.MPRSO.RecordOfficeId == 0) dTOApplFwdCondition.MPRSO.RecordOfficeId = 135;

                //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name)) dTOApplFwdCondition.MP6F.Name = "MP 6F";
                //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix)) dTOApplFwdCondition.MP6F.ArmyNoPrefix = "SL";
                //if (dTOApplFwdCondition.MP6F.RecordOfficeId == 0) dTOApplFwdCondition.MP6F.RecordOfficeId = 132;

                //if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name)) dTOApplFwdCondition.MP6A.Name = "MP 6A";
                //if (dTOApplFwdCondition.MP6A.RecordOfficeId == 0) dTOApplFwdCondition.MP6A.RecordOfficeId = 126;
                //if (dTOApplFwdCondition.MP6A.RankOrderby == 0) dTOApplFwdCondition.MP6A.RankOrderby = 4;
                if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                    dTOApplFwdCondition.MPRSO.RecordOfficeId == 0 || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) || 
                    string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) || dTOApplFwdCondition.MP6F.RecordOfficeId == 0 ||
                    string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name) || dTOApplFwdCondition.MP6A.RecordOfficeId == 0 || dTOApplFwdCondition.MP6A.RankOrderby == 0)
                {
                    return Json(KeyConstants.InternalServerError);
                }

                List<DTODataExportsResponse> retdata = await basicDetailBL.GetBesicdetailsByRequestId(Data, dTOApplFwdCondition);
                if(retdata.Count() > 0)
                {
                    DtoSession dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    string sourceFolderPhotoPhy = Convert.ToString(ForCreateFolderrandom(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell"), dtoSession.DoaminId));
                    string lastFolderName = new DirectoryInfo(sourceFolderPhotoPhy).Name;
                    int recoff = 0;
                    List<DTODataExportsResponse> lst = new List<DTODataExportsResponse>();
                    List<DTODataExportsResponse> csvlst = new List<DTODataExportsResponse>();
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

                                CsvService csvService = new CsvService();
                                string csvData = csvService.GenerateCsv(lst);
                                System.IO.File.WriteAllText(recofffolder + "/Data.csv", csvData);
                            }

                            lst.Clear();
                            recofffolder = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice));
                            recoffphotos = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice + "/Photos/"));
                            recoffsing = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice + "/Signature"));

                        }

                        //System.IO.File.Copy(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo") + "/" + data.PhotoImagePath, recoffphotos + "/" + data.ServiceNo + ".png", true);
                        //System.IO.File.Copy(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature") + "/" + data.SignatureImagePath, recoffsing + "/" + data.ServiceNo + ".png", true);
                        string temp = data.PhotoImagePath.Replace(".enc", string.Empty);
                        string[] parts = temp.Split('.');
                        string extenstionImage = parts[parts.Length - 1];

                        temp = data.SignatureImagePath.Replace(".enc", string.Empty);
                        parts = temp.Split('.');
                        string extenstionSign = parts[parts.Length - 1];

                        ImageEncryptAndDecrypt.DecryptImageFile(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo", data.PhotoImagePath), recoffphotos + "/" + data.ServiceNo + "." + extenstionImage);
                        ImageEncryptAndDecrypt.DecryptImageFile(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature", data.SignatureImagePath), recoffsing + "/" + data.ServiceNo + "." + extenstionSign);

                        lst.Add(data);
                        csvlst.Add(data);
                        recoff = data.RecordOfficeId;
                        if (count == retdata.Count())
                        {
                            var jsonString = JsonConvert.SerializeObject(lst);
                            var jsonde = JsonConvert.DeserializeObject(jsonString);
                            System.IO.File.WriteAllText(recofffolder + "/Data.json", jsonString);

                            CsvService csvService = new CsvService();
                            string csvData = csvService.GenerateCsv(lst);
                            System.IO.File.WriteAllText(recofffolder + "/Data.csv", csvData);

                        }
                        if (count == 1)
                            arryRequestId = data.RequestId + "";
                        else
                            arryRequestId = arryRequestId + "," + data.RequestId;

                    }
                    if (count != 0 && count == retdata.Count())
                    {
                        CsvService csvService = new CsvService();
                        string csvData = csvService.GenerateCsv(csvlst);
                        System.IO.File.WriteAllText(sourceFolderPhotoPhy + "/" + lastFolderName + ".csv", csvData);
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

                        string tempZipFilePath = Convert.ToString(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell", "Temp"));

                        ZipEncrypt.EncryptAndZip(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip", tempZipFilePath, Data.publicKey); // Encrypt and zip folder
                    }
                    else
                    {
                        CreateZipFromFolder(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip");
                    }

                    var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    DTODataExported dTODataExported = new DTODataExported();
                    dTODataExported.AspNetUsersId = userId;
                    dTODataExported.UserId = Convert.ToInt32(dtoSession.UserId);
                    dTODataExported.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    dTODataExported.CreatedBy = dtoSession.RankName + " " + dtoSession.Name + " (" + dtoSession.ICNO + ")";
                    dTODataExported.CreatedOn = DateTime.Now;
                    dTODataExported.RequestId = arryRequestId;
                    await _iTrnLoginLogBL.AddDataExport(dTODataExported);

                    return Json(lastFolderName);
                }
                else
                {
                    return Json(KeyConstants.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataExport.");
                return Json(KeyConstants.InternalServerError);
            }
        }

        public async Task<IActionResult> DataDigitalXmlSign(DTODataExportRequest Data)
        {
            try
            {
                DTOXmlFilesFwdLogRequest ret = new DTOXmlFilesFwdLogRequest();

                // Fetch XML data for digital sign
                var xmldata = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(Data.Ids);
                if (xmldata != null && !string.IsNullOrEmpty(xmldata.XmlFiles))
                {
                    ret.Id = xmldata.Id;

                    // Create XML structure
                    string xml = await GenerateLastRecordXml(Data.Ids[0]);
                    ret.XmlFiles = MergeXmlDocuments(xmldata.XmlFiles, xml);

                    return Json(ret);
                }
                else
                {
                    // If xmldata.XmlFiles is empty or null, prepare JSON response
                    return await GenerateJsonResponse(xmldata, Data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataDigitalXmlSign.");
                return RedirectToAction("Error", "Error");
            }
        }
        
        // Method to serialize last record to XML
        private async Task<string> GenerateLastRecordXml(int id)
        {
            var lastRec = await basicDetailBL.ICardFwdLastRec(id);
            XmlSerializer serializer = new XmlSerializer(typeof(DTOFwdLastRecForDigitalSign));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, lastRec);
                return writer.ToString();
            }
        }
       
        // Method to merge two XML documents
        private string MergeXmlDocuments(string xmlData, string lastRecordXml)
        {
            XmlDocument xmlDoc1 = new XmlDocument();
            xmlDoc1.LoadXml(xmlData);

            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(lastRecordXml);

            XmlDocument xmlDoc3 = new XmlDocument();
            XmlElement rootElement = xmlDoc3.CreateElement("RecForDigitalSign");
            xmlDoc3.AppendChild(rootElement);

            foreach (XmlNode node in xmlDoc2.DocumentElement.ChildNodes)
            {
                XmlNode importedNode = xmlDoc3.ImportNode(node, true);
                rootElement.AppendChild(importedNode);
            }

            XmlNode importedRoot = xmlDoc3.ImportNode(xmlDoc1.DocumentElement, true);
            rootElement.AppendChild(importedRoot);

            return xmlDoc3.OuterXml;
        }
       
        // Method to generate JSON response when XML signing data is unavailable
        private async Task<IActionResult> GenerateJsonResponse(DTOXmlFilesFwdLogRequest xmldata, DTODataExportRequest Data)
        {
            var retData = await basicDetailBL.GetDataDigitalXmlSign(Data);
            var jsonString = JsonConvert.SerializeObject(retData);
            var jsonResponse = JsonConvert.DeserializeObject(jsonString);

            if (xmldata != null)
            {
                DTOXmlFilesForUpdate updateResponse = new DTOXmlFilesForUpdate
                {
                    Id = xmldata.Id,
                    jsonfile = jsonResponse
                };
                return Json(updateResponse);
            }

            return Json(jsonResponse);
        }

        #endregion

        #region FaultyCard
        public async Task<ViewResult> FaultyCardAsync()
        {
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
            bool Claim = false;

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }
            ViewBag.Claim = Claim;
            return View();
        }
        public async Task<IActionResult> GetAllFaulty()
        {
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            int MapUnitId = 0;
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;


            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
            bool Claim = false;

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }

            return Json(await faultyCardBL.GetAllFaulty(Claim, MapUnitId));
        }
        [HttpPost]
        public async Task<IActionResult> GetTrnFaultyCardDetail(int TrnFaultyCardId)
        {
            return Json(await faultyCardBL.GetTrnFaultyCardDetail(TrnFaultyCardId));
        }
        public async Task<ActionResult> FaultyCardRequestAsync(string? Id)
        {
            bool Claim = false;
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

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
                        _logger.LogWarning("Decrypted Id is not a valid integer: {DecryptedId}, UserId: {UserId}", decryptedId, AspNetUsersId);
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


            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }

            ViewBag.Claim = Claim;
            ViewBag.TrnFaultyCardId = decryptedIntId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetBasicDetailForParitalViewByRequestId(int RequestId)
        {
            DTOBasicDetailForParitalViewResponse data = await basicDetailBL.GetBasicDetailForParitalViewByRequestId(RequestId);
            string sourceFolderPathPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
            string sourcePathPhoto = Path.Combine(sourceFolderPathPhy, "Photo", data.PhotoImagePath);
            string sourcePathSignature = Path.Combine(sourceFolderPathPhy, "Signature", data.SignatureImagePath);

            if (System.IO.File.Exists(sourcePathPhoto))
            {
                data.PhotoImagePath = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
            }
            if (System.IO.File.Exists(sourcePathSignature))
            {
                data.SignatureImagePath = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
            }
            return PartialView("_BasicDetail_ParitalView", data);
        }
        [Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<IActionResult> SaveFaultyCard(DTOFaultyCardRequest dTO)
        {
            MTrnFwd? mTrnFwd = new MTrnFwd();
            DtoSession? dtoSession = new DtoSession();
            DTOFaultyCardSaveResponse dTOFaulty = new DTOFaultyCardSaveResponse();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;
            //Reject Case
            if (dTO.Choice  == 3)
            {
                mTrnFwd.RequestId = dTO.RequestId;
                mTrnFwd.FromUserId = dtoSession != null ? dtoSession.UserId : 0;
                mTrnFwd.UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                mTrnFwd.Remark = dTO.ToRemark;
                mTrnFwd.FwdStatusId = Convert.ToByte(3); //Reject
                mTrnFwd.TypeId = Convert.ToByte(1);
                mTrnFwd.StepId = Convert.ToByte(9);
                mTrnFwd.IsComplete = false;
                mTrnFwd.RemarksIds = dTO.RemarksIds;
                mTrnFwd.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                mTrnFwd.UpdatedOn = DateTime.Now;
                mTrnFwd.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                mTrnFwd.IsActive = true;

                TrnDomainMapping Domain = new TrnDomainMapping();
                Domain = await iDomainMapBL.GetByRequestId(dTO.RequestId);
                if (Domain != null)
                {
                    if (Domain.UserId.GetValueOrDefault() == 0)
                    {
                        dTOFaulty.Message = "Profile is not mapped with domain Id!";
                        dTOFaulty.Result = false;
                        return Ok(dTOFaulty);
                    }
                    else
                    {
                        mTrnFwd.ToAspNetUsersId = Domain.AspNetUsersId;
                        mTrnFwd.ToUserId = Convert.ToInt32(Domain.UserId);
                    }
                }
            }

            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
                dTO.UpdatedOn = DateTime.Now;

                

                if (ModelState.IsValid)
                {
                    if (dTO.TrnFaultyCardId > 0)
                    {
                        TrnFaultyCard? trnFaultyCard = await faultyCardBL.Get(dTO.TrnFaultyCardId);
                        if (trnFaultyCard != null && trnFaultyCard.IsEditAction)
                        {
                            dTOFaulty.Result = false;
                            dTOFaulty.Message = "This action has already been completed by you.";
                            return Json(dTOFaulty);
                        }
                        else
                        {
                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd);
                            return Json(dTOFaulty);
                        }
                    }
                    else
                    {
                        bool checkduplicate = await faultyCardBL.FindRequestId(dTO.RequestId);
                        if (checkduplicate)
                        {
                            dTOFaulty.Result = false;
                            dTOFaulty.Message = "The faulty request already exists!";
                            return Json(dTOFaulty);
                        }
                        else
                        {
                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd);
                            return Json(dTOFaulty);
                        }
                    }
                }
                else
                {
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    if (errors.Any())
                    {
                        dTOFaulty.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOFaulty.Result = false;
                    return Json(dTOFaulty);
                }

            }
            catch (Exception ex)
            {
                dTOFaulty.Result = false;
                dTOFaulty.Message = ex.Message;
                return Json(dTOFaulty);
            }
        }
        public async Task<IActionResult> SaveFaultyCardRequest(DTOFaultyCardRequest dTO)
        {
            MTrnFwd? mTrnFwd = new MTrnFwd();
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;

            DTOFaultyCardSaveResponse dTOFaulty = new DTOFaultyCardSaveResponse();
            try
            {
                dTO.IsActive = true;
                dTO.IsComplete = false;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (dTO.TrnFaultyCardId > 0)
                    {
                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "This action is not allowed for you. Please check.";
                        return Json(dTOFaulty);
                    }
                    else
                    {
                        bool checkduplicate = await faultyCardBL.FindRequestId(dTO.RequestId);
                        if (checkduplicate)
                        {
                            dTOFaulty.Result = false;
                            dTOFaulty.Message = "The faulty request already exists!";
                            return Json(dTOFaulty);
                        }
                        else
                        {
                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd);
                            return Json(dTOFaulty);
                        }
                    }
                }
                else
                {
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    if (errors.Any())
                    {
                        dTOFaulty.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOFaulty.Result = false;
                    return Json(dTOFaulty);
                }

            }
            catch (Exception ex) 
            {
                dTOFaulty.Result = false;
                dTOFaulty.Message = ex.Message;
                return Json(dTOFaulty); 
            }
        }

        #endregion

        #region HotlistCard
        public async Task<ViewResult> HotlistCardAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllHotlist(DTODataTablesRequest dTO)
        {
            return Json(await _hotlistCardBL.GetAllHotlist(dTO));
        }

        [HttpPost]
        public async Task<IActionResult> HotlistDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            DTOFaultyCardSaveResponse dTOFaulty = new DTOFaultyCardSaveResponse();
            try
            {
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");
                var records = await _hotlistCardBL.GetDetailsByRequestIds(req);
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTOHotlistCardExportResponse>(true, CsvClassMapTypeEnum.HotlistExport));
                    try
                    {
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->HotlistDataExport");
                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "Internal Server Error!";
                        goto ReturnSt;
                    }
                }
                dTOFaulty.Result = true;
                dTOFaulty.Message = Path.GetFileName(tempFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->HotlistDataExport");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }
            ReturnSt:
            return Json(dTOFaulty);
        }

        [HttpGet]
        public IActionResult DownloadCsv(string fileName)
        {
            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var mimeType = "text/csv";
                return PhysicalFile(filePath, mimeType, "E-ISAC_HotlistExportData.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->DownloadCsv");
                return BadRequest();
            }
        }


        public async Task<ActionResult> HotListCardRequestAsync()
        {
            return View();
        }

        public async Task<IActionResult> SaveHotlistCardRequest(TrnHotlistCard model)
        {
            DTOFaultyCardSaveResponse dTOFaulty = new DTOFaultyCardSaveResponse();
            try
            {
                model.IsActive = true;
                model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); ;
                model.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                   bool checkduplicate = await _hotlistCardBL.FindRequestId(model.RequestId);
                   if (checkduplicate)
                   {
                       dTOFaulty.Result = false;
                       dTOFaulty.Message = "The hotlist request already exists!";
                   }
                   else
                   {
                       var result = await _hotlistCardBL.AddWithReturn(model);
                       dTOFaulty.Result = true;
                       dTOFaulty.Message = "Record created!";
                       dTOFaulty.CurrentTime = result.UpdatedOn.GetValueOrDefault();
                       dTOFaulty.Id = result.HotlistCardId.ToString();
                   }
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    if (errors.Any())
                    {
                        dTOFaulty.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOFaulty.Result = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->SaveHotlistCardRequest");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

            return Json(dTOFaulty);
        }
        #endregion HotlistCard

        #region GetSessionValue/GetData/SearchAllServiceNo/GetBasicDetailByRequestId/GetRequestHistory/GetRegimentalListByArmedId/GetROListByArmedId/GetRemarks/CreateCSV/GetICardPrintPreviewByRequestId/GetBDetailByRequestId/GetTopArmyNoFromICardRequest/ICardRequestHold/GetAllICardRequestHold

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

        [HttpPost]
        public async Task<IActionResult> GetData(string ICNumber)
        {
            DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            if (ICNumber != null)
            {
                BasicDetail? basicDetail = await basicDetailBL.FindServiceNo(ICNumber);
                if (basicDetail != null)
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
        public async Task<IActionResult> SearchAllServiceNo([FromForm] DTOSearchArmyNoRequest dto)
        {
            try
            {
                dto.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dto.MapUnitId = 0;

                if (dto.TypeId == KeyConstants.FaultyCardRequest)
                {
                    DtoSession? dtoSession = new DtoSession();
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                    {
                        dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                    }
                    dto.MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;

                    var user = await userManager.FindByIdAsync(dto.AspNetUsersId.ToString());

                    // UserManager service GetClaimsAsync method gets all the current claims of the user
                    var UserClaims = await userManager.GetClaimsAsync(user);
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dto.Claim = true;
                    }
                    else
                    {
                        dto.Claim = false;
                    }
                }
                else
                {
                    dto.Claim = true;
                }


                if (ModelState.IsValid)
                {

                    var Ret = await basicDetailBL.SearchAllServiceNo(dto);
                    if (Ret != null)
                    {
                        foreach (var item in Ret)
                        {
                            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
                            string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", item.Image);

                            if (System.IO.File.Exists(sourcePathPhoto))
                            {
                                item.Image = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                            }
                        }

                        return Ok(Ret);
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailController=>SearchAllServiceNo.");
                return BadRequest();
            }
        }
        public async Task<IActionResult> GetBasicDetailByRequestId(int RequestId)
        {
            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(RequestId);
            if (basicDetailCrtAndUpdVM != null)
            {
                string sourceFolderPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

                string sourcePathPhoto = Path.Combine(sourceFolderPhy, "Photo", basicDetailCrtAndUpdVM.PhotoImagePath);
                basicDetailCrtAndUpdVM.ExistingPhotoInBase64 = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);

                string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", basicDetailCrtAndUpdVM.SignatureImagePath);
                basicDetailCrtAndUpdVM.ExistingSignatureInBase64 = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
                return Json(basicDetailCrtAndUpdVM);
            }
            else
            {
                return Json(null);
            }
        }
        
        public async Task<IActionResult> GetRequestHistory(int RequestId)
        {
            ICardHistoryResponseAll? cardHistoryResponses = await basicDetailBL.ICardHistory(RequestId);
            if (cardHistoryResponses != null)
            {
                return Json(cardHistoryResponses);
            }
            else
            {
                return Json(null);
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

        public async Task<IActionResult> GetRemarks(DTORemarksRequest Data)
        {
            return Json(await _IMasterBL.GetRemarksByTypeId(Data));
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
                if (csvData != null)
                {
                    string TempFileName = Guid.NewGuid().ToString();
                    string sourceFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CSVFile");
                    // Check if directory exists
                    if (!Directory.Exists(sourceFolder))
                    {
                        // If directory does not exist, create it
                        Directory.CreateDirectory(sourceFolder);
                    }

                    System.IO.File.WriteAllText(sourceFolder + "/" + TempFileName + ".csv", csvData);
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

        public async Task<IActionResult> GetICardPrintPreviewByRequestId(int RequestId)
        {
            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(RequestId);
            if (basicDetailCrtAndUpdVM != null)
            {
                string sourceFolderPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

                string sourcePathPhoto = Path.Combine(sourceFolderPhy, "Photo", basicDetailCrtAndUpdVM.PhotoImagePath);
                basicDetailCrtAndUpdVM.ExistingPhotoInBase64 = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);

                string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", basicDetailCrtAndUpdVM.SignatureImagePath);
                basicDetailCrtAndUpdVM.ExistingSignatureInBase64 = ImageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);

                return Json(basicDetailCrtAndUpdVM);
            }
            else
            {
                return Json(null);
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

        #endregion

        #region CreateFolder:-GetCreateMyFolder/GetCreateMyFolder/ForCreateFolderrandom/CreateFolder/CreateZipFromFolder
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
        public static DirectoryInfo ForCreateFolderrandom(string baseFolder,string DoaminId)
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MM");
            var dayName = now.ToString("dd");
            var hh = now.ToString("HH");
            var mm = now.ToString("mm");
            var ss = now.ToString("ss");
            var folder =
                        Path.Combine(baseFolder,
                           Path.Combine(dayName+ ""+ monthName + "" + yearName + "_" + hh + "" + mm + "" + ss + "_"+ DoaminId));

            return Directory.CreateDirectory(folder);
        }
        public static DirectoryInfo CreateFolder(string baseFolder)
        {
            return Directory.CreateDirectory(baseFolder);
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
        #endregion

        #region Card Distribution

        [Authorize(Policy = "ViewFlaggedICardApplPolicy")]
        [HttpGet]
        public async Task<IActionResult> ICardDistribution()
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
        public async Task<IActionResult> GetAllICardDistribution()
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
        #endregion Card Distribution

        #region ICard Printing
        [HttpPost]
        public async Task<IActionResult> ICardPrintUploadCsv(DTOCSVFileRequest model)
        {
            var response = new DTOCsvUploadValResponse();
            if (model.CSVFile == null || model.CSVFile.Length == 0)
            {
                response.Message = "File is not uploaded or is empty.";
                goto Returnstm;
            }
            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            try
            {
                var records = new List<DTOCardPriningRequest>();
                using (var reader = new StreamReader(model.CSVFile.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTOCardPriningRequest>(true));
                    try
                    {
                        records = csv.GetRecords<DTOCardPriningRequest>().ToList();
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->ICardPrintUploadCsv");
                        response.Result = false;
                        response.Message = "Internal Server Error!";
                        goto Returnstm;
                    }
                }

                #region Upload File Without Remarks
                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "CardPrinitngCSVs", "CSVWithoutRemarks");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CSVFile.CopyToAsync(stream);
                }
                #endregion Upload User File

                var validateResult = await basicDetailBL.ValidateCardPrinitng(records);
                response.Result = true;
                response.TotalRecords = validateResult.Count();
                response.ValidRecords = validateResult.Where(x => x.IsValid).Count();
                response.SheetInValidRecords = validateResult.Where(x => x.Status == "SheetInValid").Count();
                response.DbInValidRecords = validateResult.Where(x => x.Status == "DbInvalid").Count();

                #region Upload File With Remarks
                uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "CardPrinitngCSVs", "CSVWithRemarks");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CSVFile.CopyToAsync(stream);
                }
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTOCardPriningRequest>(false));
                    try
                    {
                        csv.WriteRecords(validateResult);
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->ICardPrintUploadCsv");
                        response.Result = false;
                        response.Message = "Internal Server Error!";
                        goto Returnstm;
                    }
                }
                #endregion Upload User File
                #region Insert record
                var cSVImport = new CSVImport()
                {
                    FileName = fileName,
                    TotalRecords = response.TotalRecords,
                    ValidRecords = response.ValidRecords,
                    DbInvalidRecords = response.DbInValidRecords,
                    SheetInvalidRecords = response.SheetInValidRecords,
                    DBUpdated = false,
                    ImportedBy = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    ImportedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
                };

                var csvImportInsert = await _iCSVImportBL.AddWithReturn(cSVImport);
                SessionHeplers.SetObject(HttpContext.Session, "CsvImportId", csvImportInsert.Id);
                #endregion Insert record

                SessionHeplers.SetObject(HttpContext.Session, "ValidRecordsCardUpload", validateResult.Where(v => v.IsValid).ToList());
                //var bytes = memoryStream.ToArray();
                //var base64 = Convert.ToBase64String(bytes);
                response.FileName = fileName;
                //response.File = base64;
                //memoryStream.Position = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->ICardDistibutionUploadCsv");
                response.Message = "Internal Server Error!";
            }
            Returnstm:
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> ICardPrintValidRecordsUpload()
        {
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            try
            {
                var records = SessionHeplers.GetObject<List<DTOCardPriningRequest>>(HttpContext.Session, "ValidRecordsCardUpload");
                if (records?.Count() > 0)
                {
                    response = await basicDetailBL.CardPrinitngCSVUpload(records);
                }
                else
                {
                    response.Message = "There are no valid records!";
                }
                var csvImportId = SessionHeplers.GetObject<int>(HttpContext.Session, "CsvImportId");
                var getCsvDetById = await _iCSVImportBL.Get(csvImportId);
                getCsvDetById.DBUpdated = true;
                await _iCSVImportBL.Update(getCsvDetById);
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "BasicDetail->ICardPrintValidRecordsUpload");
                response.Message = "Internal Server Error!";
            }
            return Json(response);
        }
        #endregion ICard Printing
    }
}