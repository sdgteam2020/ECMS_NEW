using AutoMapper;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.CSVImports;
using BusinessLogicsLayer.DestructionCard;
using BusinessLogicsLayer.DispatchCard;
using BusinessLogicsLayer.DispatchCardMapping;
using BusinessLogicsLayer.DistributeCard;
using BusinessLogicsLayer.EncryptionSetting;
using BusinessLogicsLayer.FaultyCard;
using BusinessLogicsLayer.HotlistCard;
using BusinessLogicsLayer.LostCard;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Service;
using BusinessLogicsLayer.TrnICardHold;
using BusinessLogicsLayer.TrnLoginLog;
using BusinessLogicsLayer.Unit;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccessLayer;
using DataAccessLayer.Healpers;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using DataTransferObject.ViewModels;
using EntityFramework.Exceptions.Common;
using Humanizer;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.IO.Font.Cmap;
using iText.Layout.Renderer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Newtonsoft.Json;
using NuGet.Packaging;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Web.Healpers;
using Web.Healpers.BaseInterfaces;
using Web.WebHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    /// <summary>
    /// This controller manages basic details related to I-Card requests, including viewing, approval, and data accuracy checks.
    /// And it handles various user roles and permissions for accessing and modifying I-Card data.
    /// Also, it provides functionalities for exporting and importing I-Card related data.
    /// Moreover, it integrates with notification and logging systems to track user actions and system events.
    /// </summary>
    [Authorize]
    public class BasicDetailController : Controller
    {
        //private readonly ApplicationDbContext context, contextTransaction;
        private readonly UserManager<ApplicationUser> userManager;// For Identity
        private readonly IStepCounterBL iStepCounterBL;// For Step Counter
        private readonly ITrnICardRequestBL iTrnICardRequestBL;// For ICard Request
        private readonly IDomainMapBL iDomainMapBL;// For Domain Mapping
        private readonly ITrnFwnBL iTrnFwnBL;// For Forwarding
        private readonly IBasicDetailBL basicDetailBL;// For Basic Detail
        private readonly IBasicUploadBL basicuploadBL;// For Basic Upload
        private readonly IBasicAddressBL basicAddressBL;// For Basic Address
        private readonly IBasicinfoBL basicinfoBL;// For Basic Info
        private readonly IRankBL rankBL;// For Rank and Type
        private readonly IBasicDetailTempBL basicDetailTempBL;// For Basic Detail Temp
        private readonly IService service;// For Service
        private readonly IMapper _mapper;// For Auto Mapper
        private readonly IMapUnitBL mapUnitBL;// For Map Unit
        private readonly IWebHostEnvironment hostingEnvironment;// For Hosting Environment
        private readonly IDataProtector protector;// For Data Protection
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<BasicDetailController> _logger;// For Logging
        private readonly INotificationBL _INotificationBL;// For Notification
        private readonly IMasterBL _IMasterBL;// For Master Data
        private readonly ITrnLoginLogBL _iTrnLoginLogBL;// For Login Log
        private readonly IICardHoldBL _iICardHoldBL;// For I-Card Hold
        private readonly IConfiguration _configuration;// For Configuration
        public DateTime dateTimenow;
        private readonly string[] _expectedColumns = { "RequestId", "RankName", "FName", "LName", "ServiceNo", "ChipNo", "CardSerialNo" };
        private readonly IcsvImportBl _iCSVImportBL;// For CSV Import
        private readonly IFaultyCardBL faultyCardBL;// For Faulty Card
        private readonly IHotlistCardBL _hotlistCardBL;// For Hotlist Card
        private readonly ILostCardBL _lostCardBL;// For Lost Card
        private readonly IDistributeCardBL _distributeCardBL;// For Distribute Card
        private readonly IDestructionCardBL _destructionCardBL;// For Destruction Card
        private readonly IDispatchCardBL dispatchCardBL;// For Dispatch Card
        private readonly IDispatchCardMappingBL dispatchCardMappingBL;// For Dispatch Card Mapping
        private readonly IImageEncryptAndDecrypt imageEncryptAndDecrypt;// For Image Encrypt and Decrypt
        private readonly IEncryptionSettingBL encryptionSettingBL;// For Encryption Setting

        /// <summary>
        /// This is the constructor for the BasicDetailController class, 
        /// which initializes various services and dependencies required for handling basic details related to I-Card requests.
        /// Via dependency injection, it sets up services for user management, data protection, 
        /// logging, and business logic layers for managing I-Card details, notifications, and other related functionalities.
        /// </summary>        
        public BasicDetailController(IConfiguration configuration, IBasicDetailBL basicDetailBL, IMapUnitBL mapUnitBL, IBasicDetailTempBL basicDetailTempBL, IService service, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailController> logger, IStepCounterBL iStepCounterBL,
                              ITrnFwnBL iTrnFwnBL, ITrnICardRequestBL iTrnICardRequestBL, IDomainMapBL iDomainMapBL
            , IBasicUploadBL basicUploadBL, IBasicAddressBL basicAddressBL, IBasicinfoBL basicinfoBL, IRankBL rankBL, INotificationBL notificationBL, IMasterBL masterBL
           , ITrnLoginLogBL iTrnLoginLogBL, IICardHoldBL iICardHoldBL, IcsvImportBl iCSVImportBL, IFaultyCardBL _faultyCardBL, IHotlistCardBL hotlistCardBL, ILostCardBL lostCardBL, IDistributeCardBL distributeCardBL, IDestructionCardBL destructionCardBL, IDispatchCardBL dispatchCardBL, IDispatchCardMappingBL dispatchCardMappingBL, IImageEncryptAndDecrypt imageEncryptAndDecrypt, IEncryptionSettingBL encryptionSettingBL)
        {
            _configuration = configuration;
            this.basicDetailBL = basicDetailBL;
            this.basicDetailTempBL = basicDetailTempBL;
            this.service = service;
            this._mapper = mapper;
            this.mapUnitBL = mapUnitBL;
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
            this.rankBL = rankBL;
            _INotificationBL = notificationBL;
            _IMasterBL = masterBL;
            _iTrnLoginLogBL = iTrnLoginLogBL;
            _iICardHoldBL = iICardHoldBL;
            _iCSVImportBL = iCSVImportBL;
            faultyCardBL = _faultyCardBL;
            _hotlistCardBL = hotlistCardBL;
            _lostCardBL = lostCardBL;
            _distributeCardBL = distributeCardBL;
            _destructionCardBL = destructionCardBL;
            this.dispatchCardBL = dispatchCardBL;
            this.dispatchCardMappingBL = dispatchCardMappingBL;
            this.imageEncryptAndDecrypt= imageEncryptAndDecrypt;
            this.encryptionSettingBL = encryptionSettingBL;
        }

        #region Index/ApprovalForIO/View/InaccurateData/InaccurateDataView/RequestType
        /// <summary>
        /// Handles requests to display the Index view for I-Card application statuses.
        /// Accepts a Base64 encoded Id string and an optional coordinate string (jcoor),
        /// decodes and validates the Id, determines the status, step counter, and title
        /// based on predefined mappings, and sets the corresponding ViewBag properties.
        /// </summary>
        /// <param name="Id">
        /// A Base64 encoded string representing a numeric status code for the I-Card process.
        /// </param>
        /// <param name="jcoor">
        /// An optional parameter (e.g., coordinates or external reference) passed through to the view.
        /// </param>
        /// <returns>
        /// Returns an <see cref="ActionResult"/> rendering the Index view with relevant ViewBag data,
        /// or redirects to the ContactUs page if input is invalid.
        /// </returns>
        public async Task<ActionResult> Index(string Id, string jcoor)
        {
            // Initialize notification object (example usage, not persisted here)
            MTrnNotification noti = new MTrnNotification
            {
                // Convert logged-in UserId (NameIdentifier claim) to int
                ReciverAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                DisplayId = 0
            };

            // Validate Id: must not be null/empty and must be valid Base64
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            int retint;              // decoded integer identifier
            int type = 1;            // default type (status group)
            int stepcounter = 0;     // step tracker for workflow
            string title = "List of Drafted Appl"; // default title

            try
            {
                // Decode Base64 string into integer
                var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(Id));
                retint = Convert.ToInt32(decodedString);
                stepcounter = retint; // default step counter equals decoded int
            }
            catch (FormatException ex)
            {
                // Log error and redirect if Base64 decoding or int conversion fails
                _logger.LogError(ex, "Invalid Base64 Id: {Id}", Id);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            // Map decoded integer Ids to application statuses, step counters, and titles
            switch (retint)
            {
                case 0:
                case 1:  // Request from Dashboard
                    stepcounter = retint;
                    break;

                case 11: // Request from Task Board → maps to Dashboard (1)
                    stepcounter = retint = 1;
                    break;

                case 2:
                    title = "I-Card Pending From IO / Superior";
                    type = 2; stepcounter = 2;
                    break;

                case 22:    // Request from Dashboard
                case 2222:  // Request from Task Board
                    title = "I-Card Rejectd From IO / Superior";
                    type = 1; stepcounter = 7;
                    break;

                case 222:
                    title = "I-Card Approved From IO / Superior";
                    type = 3; stepcounter = 2;
                    break;

                case 3:
                    title = "I-Card Pending From RO / ORO";
                    type = 2; stepcounter = 3;
                    break;

                case 33:
                    title = "I-Card Rejectd From RO / ORO";
                    type = 1; stepcounter = 8;
                    break;

                case 333:
                    title = "I-Card Approved From RO / ORO";
                    type = 3; stepcounter = 4;
                    break;

                case 4:
                    title = "I-Card Pending From AFSAC Cell";
                    type = 2; stepcounter = 4;
                    break;

                case 44:
                    title = "I-Card Rejectd From AFSAC Cell";
                    type = 1; stepcounter = 9;
                    break;

                case 444:
                    title = "I-Card Approved From AFSAC Cell";
                    type = 3; stepcounter = 5;
                    break;

                case 5:
                    title = "I-Card Pending From HQ 54";
                    type = 2; stepcounter = 5;
                    break;

                case 55:
                    title = "I-Card Rejectd From HQ 54";
                    type = 1; stepcounter = 10;
                    break;

                case 555:
                    title = "I-Card Approved From HQ 54";
                    type = 2; stepcounter = 5;
                    break;

                case 88:   // Request from Task Board
                case 888:  // Request from Dashboard
                    title = "Status of Appl Approved & Fwd";
                    type = 2; stepcounter = 888;
                    break;

                case 77:
                case 777:
                    title = "I-Card Completed";
                    type = 2; stepcounter = 777;
                    break;

                case 99:   // Request from Task Board
                case 999:  // Request from Dashboard
                    title = "Appl rejected by Approver, Verifier";
                    type = 2; stepcounter = 999;
                    break;
            }

            // Assign resolved values to ViewBag for the view to consume
            ViewBag.Id = retint;
            ViewBag.Title = title;
            ViewBag.Type = type;
            ViewBag.StepCounter = stepcounter;
            ViewBag.jcoor = jcoor;

            // Placeholder await to satisfy async signature
            await Task.CompletedTask;

            // Return Index view
            return View();
        }

        /// <summary>
        /// Handles POST requests to retrieve I-Card index data based on user input and step count.
        /// The method determines the <c>applyForId</c> value depending on the request data,
        /// invokes the business layer to fetch records, and returns the result as JSON.
        /// </summary>
        /// <param name="dTORecord">
        /// A <see cref="DTODataTablesRequestFor_BasicDetails_Index"/> object received from the request body,
        /// containing filtering and step count information.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> returning JSON with I-Card index data on success,
        /// or a BadRequest response with an error message if an exception occurs.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetAllIndexData([FromBody] DTODataTablesRequestFor_BasicDetails_Index dTORecord)
        {
            // Retrieve current userId from claims and assign it into the DTO
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            dTORecord.UserId = userId;

            try
            {
                // Case 1: If stepcount = 0, treat as default case
                if (dTORecord.stepcount == 0)
                {
                    dTORecord.applyForId = 0;
                    var allrecord = await basicDetailBL.GetALLForIcardSttaus(dTORecord);
                    return Json(allrecord);
                }
                // Case 2: If stepcount > 0 but JCOOR is null/empty
                else if (string.IsNullOrEmpty(dTORecord.JCOOR))
                {
                    dTORecord.applyForId = 1;
                    var allrecord = await basicDetailBL.GetALLForIcardSttaus(dTORecord);
                    return Json(allrecord);
                }
                // Case 3: Otherwise, JCOOR is present → set applyForId = 2
                else
                {
                    dTORecord.applyForId = 2;
                    var allrecord = await basicDetailBL.GetALLForIcardSttaus(dTORecord);
                    return Json(allrecord);
                }
            }
            catch (Exception ex)
            {
                // Log exception with event id 1001 and return 400 Bad Request
                _logger.LogError(1001, ex, "Home->GetAllIndexData");
                return BadRequest(new { message = "Internal Server Error" });
            }
        }

        /// <summary>
        /// Displays the Approval view for IO (I-Card Officer) based on the provided Id.
        /// Validates the user, decodes the Base64 Id, and sets up ViewBag values
        /// (Title, Id, Type, StepCounter, Export flags, etc.) depending on workflow status.
        /// </summary>
        /// <param name="Id">
        /// A Base64 encoded string representing the current step/status of the I-Card workflow.
        /// </param>
        /// <param name="jcoor">
        /// An optional string parameter (coordinate or reference), used to set conditional flags in the view.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders the ApprovalForIO view with pre-populated ViewBag data,
        /// or redirects to the ContactUs page if input or user validation fails.
        /// </returns>
        public async Task<ActionResult> ApprovalForIO(string Id, string jcoor)
        {
            // Fetch current role from session and store in ViewBag
            string role = GetSessionValue();
            ViewBag.Role = role;

            // Extract userId from claims and validate it
            var userIdStr = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                TempData["error"] = "Invalid User.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            // Retrieve user and their claims for display or further logic
            var user = await userManager.FindByIdAsync(userIdStr);
            var userClaims = await userManager.GetClaimsAsync(user);
            ViewBag.UserClaims = userClaims;

            // Initialize notification object with current userId
            var noti = new MTrnNotification
            {
                ReciverAspNetUsersId = userId,
                DisplayId = 0
            };

            // Validate Id: must be Base64 encoded and non-empty
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            int retint;             // decoded integer representing workflow state
            int type = 0;           // type indicator (Pending, Approved, Rejected, etc.)
            int stepCounter = 0;    // tracks workflow step position

            try
            {
                // Decode Base64 Id into integer and assign as stepCounter
                var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(Id));
                retint = Convert.ToInt32(decodedString);
                stepCounter = retint;
            }
            catch (Exception ex)
            {
                // Log error and redirect on decoding failure
                _logger.LogError(ex, "Invalid Base64 Id: {Id}", Id);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            // Determine Title, Type, StepCounter, and Export flags based on decoded Id
            switch (retint)
            {
                case 1:
                    ViewBag.Title = "List of Register I-Card";
                    ViewBag.Id = 0;
                    break;

                case 2:
                    ViewBag.Title = "I-Card For Approval";
                    ViewBag.Id = 1;
                    type = 2;
                    noti.DisplayId = 2;
                    break;

                case 22:
                    ViewBag.Title = "Rejectd I-Card";
                    ViewBag.Id = 0;
                    type = 1;
                    stepCounter = 7;
                    break;

                case 222:
                    ViewBag.Title = "Approved I-Card";
                    ViewBag.Id = 0;
                    type = 3;
                    stepCounter = 3;
                    break;

                case 3:
                    ViewBag.Title = "I-Card For Approval";
                    type = 2;
                    ViewBag.Id = 1;
                    stepCounter = 3;
                    break;

                case 33:
                    ViewBag.Title = "Rejectd I-Card";
                    ViewBag.Id = 0;
                    type = 1;
                    stepCounter = 8;
                    break;

                case 333:
                    ViewBag.Title = "Approved I-Card";
                    ViewBag.Id = 0;
                    type = 3;
                    stepCounter = 4;
                    break;

                case 11:
                    ViewBag.Title = "Internal Forward I-Card";
                    ViewBag.Id = 0;
                    type = 3;
                    stepCounter = 11;
                    break;

                case 4:
                    ViewBag.Title = "I-Card For Export Data";
                    type = 2;
                    ViewBag.Id = 1;
                    ViewBag.dataexport = 4;
                    break;

                case 44:
                    ViewBag.Title = "Rejectd I-Card";
                    ViewBag.Id = 0;
                    type = 1;
                    stepCounter = 9;
                    break;

                case 444:
                    ViewBag.Title = "Exported I-Card";
                    ViewBag.Id = 0;
                    type = 3;
                    stepCounter = 5;
                    break;

                case 5:
                    ViewBag.Title = "Export Data";
                    type = 2;
                    ViewBag.Id = 1;
                    ViewBag.dataexport = 5;
                    break;

                case 55:
                    ViewBag.Title = "Rejectd I-Card";
                    ViewBag.Id = 0;
                    type = 1;
                    stepCounter = 10;
                    break;

                case 555:
                    ViewBag.Title = "Approved I-Card";
                    ViewBag.Id = 0;
                    type = 3;
                    stepCounter = 6;
                    break;

                case 6:
                    ViewBag.Title = "Exported Data";
                    type = 6;
                    ViewBag.Id = 1;
                    ViewBag.dataexport = 6;
                    break;

                default:
                    ViewBag.Title = "I-Card View";
                    ViewBag.Id = 0;
                    break;
            }

            // Assign computed values to ViewBag for the view
            ViewBag.Type = type;
            ViewBag.StepCounter = stepCounter;
            ViewBag.jcoor = string.IsNullOrEmpty(jcoor) ? 1 : 0;

            // Render the view with ViewBag context
            return View();
        }


        /// <summary>
        /// Handles POST requests to retrieve approval data for IO (I-Card Officer).
        /// Determines which dataset to fetch based on the JCOOR flag in the request,
        /// and returns the corresponding records as JSON.
        /// </summary>
        /// <param name="dTORecord">
        /// A <see cref="DTODataTablesRequestFor_BasicDetails_Index"/> object received from the request body,
        /// containing filtering information, user context, and JCOOR flag.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> returning JSON with approval data for IO,
        /// or a BadRequest response with an error message if an exception occurs.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetAllApprovalForIOData([FromBody] DTODataTablesRequestFor_BasicDetails_Index dTORecord)
        {
            // Extract userId from claims and assign it into the DTO
            int userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            dTORecord.UserId = userId;

            try
            {
                // If JCOOR == "1" → applyForId = 1 (fetch one type of dataset)
                if (dTORecord.JCOOR == "1")
                {
                    dTORecord.applyForId = 1;
                    var allrecord = await basicDetailBL.GetALLBasicDetail(dTORecord);
                    return Json(allrecord);
                }
                // Otherwise → applyForId = 2 (fetch alternative dataset)
                else
                {
                    dTORecord.applyForId = 2;
                    var allrecord = await basicDetailBL.GetALLBasicDetail(dTORecord);
                    return Json(allrecord);
                }
            }
            catch (Exception ex)
            {
                // Log the error with event id 1001 and return 400 Bad Request
                _logger.LogError(1001, ex, "Home->GetAllApprovalForIOData");
                return BadRequest(new { message = "Internal Server Error" });
            }
        }


        /// <summary>
        /// Handles GET requests to view details of a specific record.
        /// The Id parameter is decrypted and validated, then used to fetch
        /// the corresponding basic detail record from the business layer.
        /// </summary>
        /// <param name="Id">
        /// A protected (encrypted) string representing the request identifier.
        /// This is decrypted and converted into an integer to fetch details.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> rendering the detail view if the record exists,
        /// the "BasicDetailNotFound" view if no record is found, or redirects to 
        /// "ContactUs" with an error if the Id is invalid or tampered.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> View(string Id)
        {
            // Retrieve current logged-in user's identifier (string form)
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            string decryptedId = string.Empty; // will hold decrypted string value
            int decryptedIntId = 0;            // will hold decrypted integer value

            try
            {
                // Attempt to decrypt the provided Id using Unprotect method
                decryptedId = protector.Unprotect(Id);

                // Convert decrypted string into integer for DB lookup
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                // Log cryptographic exceptions (tampered or invalid protected Ids)
                _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);

                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                // Log generic errors (e.g., non-numeric Id after decryption)
                _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);

                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            // Fetch basic detail record by decrypted integer Id
            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(decryptedIntId);

            if (basicDetailCrtAndUpdVM != null)
            {
                // Mask Aadhaar number: keep only last 4 digits visible
                basicDetailCrtAndUpdVM.AadhaarNo = basicDetailCrtAndUpdVM.AadhaarNo
                    .Substring((basicDetailCrtAndUpdVM.AadhaarNo.Length - 4), 4);

                // Return detail view with populated ViewModel
                return View(basicDetailCrtAndUpdVM);
            }
            else
            {
                // Record not found → set status code and show "not found" view
                Response.StatusCode = 404;
                return View("BasicDetailNotFound", decryptedId.ToString());
            }
        }


        /// <summary>
        /// Handles GET requests for viewing inaccurate data or observations in I-Card requests.
        /// Decodes and validates the Base64 encoded Id, checks configuration conditions, and retrieves
        /// pending/observation data for the logged-in user.
        /// </summary>
        /// <param name="Id">
        /// A Base64 encoded string representing the type identifier (1 = Incorrect Data, 2 = Observation Raised).
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders a view with the list of inaccurate data or observations
        /// if validation passes, or redirects to ContactUs with an error message if input or configuration is invalid.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> InaccurateData(string Id)
        {
            // Extract current user identifier from claims
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate Id: must not be null/empty and must be a valid Base64 string
            if (string.IsNullOrEmpty(Id) || !service.IsValidBase64(Id))
            {
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            try
            {
                // Decode the Base64 Id into plain string
                var base64EncodedBytes = Convert.FromBase64String(Id);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

                // Convert decoded string to integer typeId (1 or 2 expected)
                int typeId = Convert.ToInt32(decodedString);

                if (typeId == 1 || typeId == 2)
                {
                    // Retrieve hardcoded ArmedId from configuration
                    short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]);
                    // If not set, fallback could be hardcoded (commented-out sample code)

                    // Load application forward condition settings from configuration
                    DTOApplFwdConditionRequest? dTOApplFwdCondition =
                        _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>()
                        ?? new DTOApplFwdConditionRequest
                        {
                            MPRSO = new MPRSO(),
                            MP6F = new MP6F(),
                            MP6A = new MP6A()
                        };

                    // Validate configuration values: must not be empty/zero
                    if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name)
                        || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0
                        || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name)
                        || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix)
                        || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name)
                        || dTOApplFwdCondition.MP6A.RankOrderby == 0
                        || ArmedIdForORO == 0)
                    {
                        // If validation fails, show error and redirect
                        TempData["error"] = "Invalid Input.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                    else
                    {
                        // Fetch inaccurate/observation records from business layer
                        var allrecord = await Task.Run(() =>
                            basicDetailTempBL.GetALLBasicDetailTemp(
                                Convert.ToInt32(userId), typeId, dTOApplFwdCondition, ArmedIdForORO));

                        // Set dynamic title depending on typeId
                        ViewBag.Title = typeId == 1
                            ? "Requests pending due to Incorrect Details/Data"
                            : "List of Observation Raised";

                        // Return view with retrieved records
                        return View(allrecord);
                    }
                }

                // If typeId is not valid, return error
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (FormatException ex)
            {
                // Handle Base64 decoding or int parsing errors
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {Id}", Id);
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Handles GET requests to view details of inaccurate data entries.
        /// Decrypts and validates the provided Id, fetches records from the business layer,
        /// and renders the view with the retrieved details if found.
        /// </summary>
        /// <param name="Id">
        /// A protected (encrypted) string representing the BasicDetail identifier.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> rendering the inaccurate data detail view if found,
        /// or redirects to ContactUs with an error message if input is invalid, tampered, or not found.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> InaccurateDataView(string Id)
        {
            // Validate Id: must not be null or empty
            if (string.IsNullOrEmpty(Id))
            {
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

            // Retrieve current logged-in user's Id from claims (as string) and convert to integer
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userIntId = Convert.ToInt32(userId); // Assumes claim value is a valid integer

            string decryptedId = string.Empty;  // will hold decrypted string Id
            int decryptedIntId = 0;             // will hold decrypted integer Id

            try
            {
                // Attempt to decrypt the protected Id
                decryptedId = protector.Unprotect(Id);

                // Validate decrypted Id: must be a valid integer
                if (!int.TryParse(decryptedId, out decryptedIntId))
                {
                    _logger.LogWarning("Decrypted Id is not a valid integer: {DecryptedId}, UserId: {UserId}", decryptedId, userId);
                    TempData["error"] = "Invalid or tampered request.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }

                // Fetch records from business layer using userId and decryptedId
                DTOBasicDetailTempRequest? dTOBasicDetail =
                    await basicDetailTempBL.GetALLBasicDetailTempByBasicDetailId(userIntId, decryptedIntId);

                // If record found, render the view with retrieved details
                if (dTOBasicDetail != null)
                {
                    return View(dTOBasicDetail);
                }
                else
                {
                    // If no record found, show error and redirect
                    TempData["error"] = "Id not found.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                // Handle cryptographic errors (tampered/invalid encrypted Id)
                _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                TempData["error"] = "Invalid or tampered request.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);
                TempData["error"] = ex.Message;
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Handles GET requests to retrieve and display all available I-Card request types.
        /// Fetches the list of request types from the business layer and renders them in the view.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> rendering the view populated with a list of I-Card types.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> RequestType()
        {
            // Retrieve all I-Card types asynchronously from the business layer
            var allrecord = await Task.Run(() => basicDetailBL.GetAllICardType());

            // Render the view with the retrieved list of I-Card types
            return View(allrecord);
        }


        #endregion

        #region Registration/BasicDetail/GetApiData/GetUserData

        /// <summary>
        /// Handles GET requests for the Registration view.
        /// Currently returns the Registration view without additional processing.
        /// </summary>
        /// <param name="Id">
        /// An optional encrypted identifier (not currently used, old code for decryption and setup is commented out).
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> rendering the Registration view.
        /// </returns>
        [HttpGet]
        public IActionResult Registration(string Id)
        {
            #region Old Code
            // Previous implementation included:
            // - Fetching logged-in userId from claims
            // - Decrypting the provided Id using Unprotect
            // - Populating dropdown options for BloodGroup, ArmedType, and Registration
            // - Building a DTORegistrationRequest with decrypted TypeId
            // - Returning the populated view with DTO model
            //
            // This logic is currently commented out and not in use.
            #endregion End Old Code

            // Render the Registration view (empty for now)
            return View();
        }


        /// <summary>
        /// Handles POST requests for Registration submission.
        /// Validates input model, performs Army Number consistency checks, 
        /// processes registration details, and either redirects to the BasicDetail view 
        /// or stores data in the temporary table based on the request type.
        /// </summary>
        /// <param name="model">
        /// A <see cref="DTORegistrationRequest"/> object containing user registration input 
        /// such as ServiceNo, OldServiceNo, personal details, and type of request.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> which either redirects to another view (BasicDetail/InaccurateData) 
        /// on success or re-renders the Registration view with errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(DTORegistrationRequest model)
        {
            try
            {
                // Extract userId from claims and assign as UpdatedBy
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.Updatedby = Convert.ToInt32(userId);

                // Validate ModelState
                if (ModelState.IsValid)
                {
                    // Case 1: SubmitType == 1 (new application flow)
                    if (model.SubmitType == 1)
                    {
                        // Special logic for TypeId == 4 (change of Army No)
                        if (model.TypeId == 4)
                        {
                            string? OldServiceNo = model.OldServiceNo;
                            string? NewServiceNo = model.ServiceNo;

                            // Validate Army number lengths
                            if ((OldServiceNo != null && (OldServiceNo.Length > 7 && OldServiceNo.Length < 10)) &&
                                (NewServiceNo != null && (NewServiceNo.Length > 7 && NewServiceNo.Length < 10)))
                            {
                                // Disallow same Old and New Army numbers
                                if (NewServiceNo == OldServiceNo)
                                {
                                    TempData["error"] = "Old Army No and New Army No not same.";
                                    goto end;
                                }
                                else
                                {
                                    // Check if Army numbers exist in DB
                                    bool OldArmyNoFound = await basicDetailBL.CheckArmyNO(OldServiceNo);
                                    bool NewArmyNoFound = await basicDetailBL.CheckArmyNO(NewServiceNo);

                                    // Extract prefixes (IC, SL, SS, etc.)
                                    string OldFirstTwo = service.CheckFirstTwoChars(OldServiceNo);
                                    string NewFirstTwo = service.CheckFirstTwoChars(NewServiceNo);

                                    // Validate Army No conditions and rank-based rules
                                    if (OldArmyNoFound == false)
                                    {
                                        TempData["error"] = "Old Army No not found.";
                                        goto end;
                                    }
                                    else if (NewArmyNoFound == true)
                                    {
                                        TempData["error"] = "New Army No is alredy used.";
                                        goto end;
                                    }
                                    else if (OldFirstTwo.IsNullOrEmpty())
                                    {
                                        // Handle OR rank cases
                                        if (NewFirstTwo.IsNullOrEmpty())
                                        {
                                            TempData["error"] = "Both Old and New Army No is OR rank.";
                                            goto end;
                                        }
                                        else if (model.ApplyForId == 2 && (NewFirstTwo == "IC" || NewFirstTwo == "SL" || NewFirstTwo == "WC" || NewFirstTwo == "SS" || NewFirstTwo == "TA"))
                                        {
                                            TempData["error"] = "Please Select Offrs tab.";
                                            goto end;
                                        }
                                        else if (model.ApplyForId == 1 && NewFirstTwo == "JC")
                                        {
                                            TempData["error"] = "Please Select JCOs/OR tab.";
                                            goto end;
                                        }
                                    }
                                    else if (!OldFirstTwo.IsNullOrEmpty())
                                    {
                                        // Validation rules for IC, SL, SS, WC, TA, JC
                                        if (OldFirstTwo == "IC" && NewFirstTwo.IsNullOrEmpty())
                                        {
                                            TempData["error"] = "Permanent Commissioned Officers are not downgraded.";
                                            goto end;
                                        }
                                        else if (OldFirstTwo == "IC" && NewFirstTwo == "IC")
                                        {
                                            TempData["error"] = "Both Old and New Army No is permanent commissioned officers.";
                                            goto end;
                                        }
                                        else if (OldFirstTwo == "IC" && (NewFirstTwo == "SS" || NewFirstTwo == "SL" || NewFirstTwo == "WC" || NewFirstTwo == "TA" || NewFirstTwo == "JC"))
                                        {
                                            TempData["error"] = "Permanent Commissioned Officers are not downgraded.";
                                            goto end;
                                        }
                                        else if ((OldFirstTwo == "SL" || OldFirstTwo == "TA") &&
                                                 (NewFirstTwo == "IC" || NewFirstTwo == "SS" || NewFirstTwo == "SL" || NewFirstTwo == "WC" || NewFirstTwo == "TA" || NewFirstTwo == "JC"))
                                        {
                                            TempData["error"] = "SL / TA are not changed Army No.";
                                            goto end;
                                        }
                                        else if ((OldFirstTwo == "SS" || OldFirstTwo == "WC") && model.ApplyForId == 2 && !NewFirstTwo.IsNullOrEmpty() && NewFirstTwo == "IC")
                                        {
                                            TempData["error"] = "Please Select Offrs tab.";
                                            goto end;
                                        }
                                        else if (OldFirstTwo == "JC" && model.ApplyForId == 2 && !NewFirstTwo.IsNullOrEmpty() && (NewFirstTwo == "SS" || NewFirstTwo == "SL" || NewFirstTwo == "WC" || NewFirstTwo == "TA"))
                                        {
                                            TempData["error"] = "Please Select  Offrs tab.";
                                            goto end;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Validation errors for missing/invalid Army No
                                if (OldServiceNo == null)
                                {
                                    ModelState.AddModelError("OldServiceNo", "Old Service No required.");
                                    goto end;
                                }
                                if (OldServiceNo.Length < 8 || OldServiceNo.Length > 9)
                                {
                                    ModelState.AddModelError("OldServiceNo", "Minimum eight and Maximum nine length of Army No.");
                                    goto end;
                                }
                                if (NewServiceNo == null)
                                {
                                    ModelState.AddModelError("ServiceNo", "New Service No required.");
                                    goto end;
                                }
                                if (NewServiceNo.Length < 8 || NewServiceNo.Length > 9)
                                {
                                    ModelState.AddModelError("ServiceNo", "Minimum eight and Maximum nine length of Army No.");
                                    goto end;
                                }
                            }
                        }

                        // Store Registration model temporarily and redirect to BasicDetail with protected Id=0
                        TempData["Registration"] = JsonConvert.SerializeObject(model);
                        return RedirectToAction("BasicDetail", "BasicDetail", new { Id = protector.Protect("0") });
                    }
                    // Case 2: SubmitType != 1 (temporary save flow)
                    else
                    {
                        BasicDetailTemp basicDetailTemp = new BasicDetailTemp
                        {
                            FName = model.FName,
                            LName = model.LName,
                            NameAsPerRecord = model.NameAsPerRecord,
                            ServiceNo = model.ServiceNo,
                            DOB = model.DOB,
                            DateOfCommissioning = model.DateOfCommissioning,
                            State = model.State,
                            District = model.District,
                            PS = model.PS,
                            PO = model.PO,
                            Tehsil = model.Tehsil,
                            Village = model.Village,
                            PinCode = model.PinCode,
                            Observations = model.Observations,
                            Updatedby = model.Updatedby,
                            RemarksIds = model.RemarksIds,
                            ApplyForId = model.ApplyForId,
                            RegistrationId = model.RegistrationId,
                            TypeId = model.TypeId,
                            RankId = model.RankId,
                            ArmedId = model.ArmedId,
                            UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
                        };

                        // Check if a temporary record already exists → Update; else → Add
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
                    // ModelState invalid → extract first error message
                    var error = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                    TempData["error"] = error[0][0].ErrorMessage;
                }
            }
            // Exception handling for DB/validation related errors
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
            // If errors occurred, re-render the Registration view with the model and error messages
            return View(model);
        }


        /// <summary>
        /// Handles GET requests for creating or editing Basic Detail records.
        /// Decrypts and validates the provided Id, retrieves data from TempData or the business layer,
        /// and returns the appropriate view (new form, edit form, or not found view).
        /// </summary>
        /// <param name="Id">
        /// A protected (encrypted) string identifier representing the BasicDetail record.
        /// If null or decrypted to "0", a new record creation flow is initiated.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders:
        /// - A creation view with pre-populated registration data if Id is null or "0" and TempData exists.
        /// - A populated edit view if a record exists for the decrypted Id.
        /// - Redirects to Registration if no TempData or invalid Id is provided.
        /// - A NotFound view if no record is found in the database.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> BasicDetail(string? Id)
        {
            // Get the current logged-in user's identifier from claims
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string decryptedId = string.Empty;
            int decryptedIntId = 0;

            // If Id is provided, attempt to decrypt and validate it
            if (Id != null)
            {
                try
                {
                    // Decrypt the Id using Unprotect method
                    decryptedId = protector.Unprotect(Id);

                    // Validate that the decrypted Id is an integer
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
                    // Handle cryptographic errors (tampered or invalid encrypted Id)
                    _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                    TempData["error"] = "Invalid or tampered request.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
                catch (Exception ex)
                {
                    // Handle unexpected exceptions during Id processing
                    _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);
                    TempData["error"] = ex.Message;
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }

            // Case 1: New record creation (Id is null or decryptedId == "0")
            if (Id == null || decryptedId == "0")
            {
                DTORegistrationRequest? model = new DTORegistrationRequest();

                // If registration data exists in TempData, pre-populate the form
                if (TempData["Registration"] != null)
                {
                    model = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());

                    // If SubmitType == 1, build a new BasicDetail creation ViewModel
                    if (model.SubmitType == 1)
                    {
                        ViewBag.OptionsUnitId = 0;
                        BasicDetailCrtAndUpdVM dTOBasicDetailCrtRequest = new BasicDetailCrtAndUpdVM
                        {
                            PreviousBasicDetailId = null,
                            FName = model.FName,
                            LName = model.LName,
                            NameAsPerRecord = model.NameAsPerRecord,
                            ServiceNo = model.ServiceNo,
                            OldServiceNo = model.OldServiceNo,
                            DOB = model.DOB,
                            DateOfCommissioning = model.DateOfCommissioning,
                            IdenMark1 = model.IdenMark1,
                            IdenMark2 = model.IdenMark2,
                            AadhaarNo = Convert.ToInt64(model.AadhaarNo).ToString("D12"),
                            ApplyForId = model.ApplyForId,
                            RegistrationId = model.RegistrationId,
                            TypeId = model.TypeId,
                            State = model.State,
                            District = model.District,
                            PS = model.PS,
                            PO = model.PO,
                            Tehsil = model.Tehsil,
                            Village = model.Village,
                            PinCode = Convert.ToInt32(model.PinCode),
                            PermanentAddress = "Village - " + model.Village + ", Post Office-" + model.PO + ", Tehsil- " + model.Tehsil +
                                                ", District- " + model.District + ", State- " + model.State +
                                                ", Pin Code- " + (model.PinCode == 0 ? "" : model.PinCode)
                        };

                        ViewBag.OptionsRankId = model.RankId;
                        ViewBag.OptionsArmedId = model.ArmedId;

                        // Render creation view with pre-populated details
                        return await Task.FromResult(View(dTOBasicDetailCrtRequest));
                    }
                    else
                    {
                        // Redirect to Registration if SubmitType != 1
                        return RedirectToAction("Registration");
                    }
                }
                else
                {
                    // No TempData → redirect to Registration
                    return RedirectToAction("Registration");
                }
            }
            // Case 2: Edit existing record (Id provided and valid)
            else
            {
                BasicDetailCrtAndUpdVM? basicDetailUpdVM = await basicDetailBL.GetBesicDetailForEditById(decryptedIntId);

                if (basicDetailUpdVM != null)
                {
                    // Populate dropdown options and permanent address
                    ViewBag.OptionsRankId = basicDetailUpdVM.RankId;
                    ViewBag.OptionsUnitId = basicDetailUpdVM.UnitId;
                    ViewBag.OptionsArmedId = basicDetailUpdVM.ArmedId;
                    ViewBag.OptionsRegimentalId = basicDetailUpdVM.RegimentalId;
                    ViewBag.OptionsBloodGroupId = basicDetailUpdVM.BloodGroupId;

                    basicDetailUpdVM.PermanentAddress = "Village - " + basicDetailUpdVM.Village + ", Post Office-" + basicDetailUpdVM.PO +
                                                        ", Tehsil- " + basicDetailUpdVM.Tehsil + ", District- " + basicDetailUpdVM.District +
                                                        ", State- " + basicDetailUpdVM.State +
                                                        ", Pin Code- " + (basicDetailUpdVM.PinCode == 0 ? "" : basicDetailUpdVM.PinCode);

                    // Load existing photo and signature if files exist
                    string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
                    string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", basicDetailUpdVM.PhotoImagePath);
                    string sourcePathSignature = Path.Combine(sourceFolderPhotoPhy, "Signature", basicDetailUpdVM.SignatureImagePath);

                    if (System.IO.File.Exists(sourcePathPhoto))
                    {
                        basicDetailUpdVM.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                        basicDetailUpdVM.ExistingPhotoImagePath = basicDetailUpdVM.PhotoImagePath;
                    }

                    if (System.IO.File.Exists(sourcePathSignature))
                    {
                        basicDetailUpdVM.ExistingSignatureInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
                        basicDetailUpdVM.ExistingSignatureImagePath = basicDetailUpdVM.SignatureImagePath;
                    }

                    // Store encrypted Id for further use
                    basicDetailUpdVM.EncryptedId = Id;

                    // If TempData exists, override some fields with submitted values
                    if (TempData["Registration"] != null)
                    {
                        var modelex = JsonConvert.DeserializeObject<DTORegistrationRequest>(TempData["Registration"].ToString());
                        basicDetailUpdVM.FName = modelex.FName;
                        basicDetailUpdVM.LName = modelex.LName;
                        basicDetailUpdVM.ServiceNo = modelex.ServiceNo;
                        basicDetailUpdVM.OldServiceNo = modelex.OldServiceNo;
                        basicDetailUpdVM.DOB = modelex.DOB;
                        basicDetailUpdVM.DateOfCommissioning = modelex.DateOfCommissioning;
                        ViewBag.OptionsRankId = modelex.RankId;
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
                        basicDetailUpdVM.PermanentAddress = "Village - " + modelex.Village + ", Post Office-" + modelex.PO +
                                                            ", Tehsil- " + modelex.Tehsil + ", District- " + modelex.District +
                                                            ", State- " + modelex.State +
                                                            ", Pin Code- " + (modelex.PinCode == 0 ? "" : modelex.PinCode);
                    }

                    // Render edit view with populated details
                    return View(basicDetailUpdVM);
                }
                else
                {
                    // Record not found → return 404 and NotFound view
                    Response.StatusCode = 404;
                    return View("BasicDetailNotFound", decryptedId.ToString());
                }
            }
        }

        /// <summary>
        /// Handles POST requests for creating or updating Basic Detail records.
        /// - Validates Aadhaar, Unit, Regimental inputs
        /// - Handles Photo and Signature uploads (with encryption and replacement logic)
        /// - Maps <see cref="BasicDetailCrtAndUpdVM"/> to entity objects
        /// - Saves details along with related entities (Address, Upload, IdentityInfo, ICardRequest, StepCounter)
        /// - Supports both Update (BasicDetailId > 0) and Create flows
        /// </summary>
        /// <param name="model">
        /// The form input data mapped to <see cref="BasicDetailCrtAndUpdVM"/>.
        /// </param>
        /// <returns>
        /// Returns redirect to Index on success, otherwise re-renders the view with errors.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> BasicDetail(BasicDetailCrtAndUpdVM model)
        {
            try
            {
                // Fetch current logged-in user ID from claims
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve application forward condition settings from configuration
                DTOApplFwdConditionRequest? dTOApplFwdCondition = _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>() ?? new DTOApplFwdConditionRequest
                {
                    MPRSO = new MPRSO(),
                    MP6F = new MP6F(),
                    MP6A = new MP6A()
                };

                // Validate that essential configuration values are present before proceeding
                if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                            string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) ||
                            dTOApplFwdCondition.MP6A.RankOrderby == 0)
                {
                    return Json(KeyConstants.InternalServerError);
                }

                // Case 1: Update existing BasicDetail (when BasicDetailId > 0)
                if (model.BasicDetailId > 0)
                {
                    // Populate dropdown options for the view
                    ViewBag.OptionsRankId = model.RankId;
                    ViewBag.OptionsUnitId = model.UnitId;
                    ViewBag.OptionsArmedId = model.ArmedId;
                    ViewBag.OptionsRegimentalId = model.RegimentalId;
                    ViewBag.OptionsBloodGroupId = model.BloodGroupId;

                    // Basic validations
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
                    if (string.IsNullOrEmpty(model.AadhaarNo) || model.AadhaarNo.Length != 12 || !model.AadhaarNo.All(char.IsDigit) || model.AadhaarNo == "000000000000" || model.AadhaarNo[0] == '0')
                    {
                        ModelState.AddModelError("AadhaarNo", "Aadhaar number must be exactly 12 digits.");
                        goto end;
                    }

                    // If ModelState is valid, proceed with update logic
                    if (ModelState.IsValid)
                    {
                        // Map ViewModel to BasicDetail entity
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
                        newBasicDetail.DateOfIssue = null;
                        newBasicDetail.Updatedby = Convert.ToInt32(userId);
                        newBasicDetail.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                        // Create related entities (Upload, Address, IdentityInfo)
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

                        // --- Handle Photo upload: encrypt new file, delete old one if exists ---
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
                                if (model.ExistingPhotoImagePath != null)
                                {
                                    string ExitImagePath = Path.Combine(sourceFolderPhotoPhy_Old, "Photo", model.ExistingPhotoImagePath);
                                    if (System.IO.File.Exists(ExitImagePath))
                                    {
                                        System.IO.File.Delete(ExitImagePath);
                                    }
                                }

                                // Encrypt and replace
                                await imageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                } 

                                mTrnUpload.PhotoImagePath = GetCreateMyFolder() + "/" + FileName + ".enc";
                            }
                        }
                        else
                        {
                            mTrnUpload.PhotoImagePath = model.ExistingPhotoImagePath;
                        }

                        
                        // --- Handle Signature upload: encrypt new file, delete old one if exists ---
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
                                
                                // Encrypt and replace
                                await imageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
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

                        // Validate record office mapping for request
                        MTrnICardRequest? mTrnICardRequest = await iTrnICardRequestBL.GetRequestByBasicDetailId(model.BasicDetailId);
                        if (mTrnICardRequest != null)
                        {
                            byte? RecordOfficeId = await basicDetailBL.GetRecordOfficeId(model.ApplyForId, model.ServiceNo, model.ArmedId, model.RankId, dTOApplFwdCondition);

                            if (RecordOfficeId != null)
                            {
                                mTrnICardRequest.RecordOfficeId = (byte)RecordOfficeId;
                            }
                            else
                            {
                                ModelState.AddModelError("", "Armed not mapped in Record Office / ORO .");
                                goto end;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid Request Id.");
                            goto end;
                        }

                        // Save via BL
                        DTOBasicDetailsSaveResponse ret1 = await basicDetailBL.SaveBasicDetailsWithAll(newBasicDetail, mTrnAddress, mTrnUpload, mTrnIdentityInfo, mTrnICardRequest, null);
                        BasicDetail basicDetail = await basicDetailBL.Get(model.BasicDetailId);
                        if (ret1.Result == true)
                        {
                            #region This code commented by Yogendra
                            //bool resultforisprocess = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
                            //if (!resultforisprocess)
                            //{
                            //    MTrnICardRequest mTrnICardRequest = new MTrnICardRequest();
                            //    mTrnICardRequest.BasicDetailId = basicDetail.BasicDetailId;
                            //    mTrnICardRequest.StatusId = 1;
                            //    mTrnICardRequest.TypeId = model.TypeId;
                            //    string tracid = model.DOB.Day.ToString("D2") + "" + model.DOB.Month.ToString("D2") + "" + model.DOB.Year + "" + Convert.ToInt32(model.AadhaarNo.Substring(model.AadhaarNo.Length - 3)).ToString("D4");
                            //    mTrnICardRequest.TrackingId = Convert.ToInt64(tracid);
                            //    mTrnICardRequest.RegistrationId = model.RegistrationId;
                            //    mTrnICardRequest.TrnDomainMappingId = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").TrnDomainMappingId;
                            //    mTrnICardRequest.UpdatedOn = DateTime.Now;
                            //    mTrnICardRequest.Updatedby = Convert.ToInt32(userId); //SessionHeplers.GetObject<string>(HttpContext.Session, "ArmyNo");
                            //    mTrnICardRequest = await iTrnICardRequestBL.AddWithReturn(mTrnICardRequest);
                            //    if (mTrnICardRequest.RequestId > 0)
                            //    {
                            //        MStepCounter mStepCounter = new MStepCounter();
                            //        mStepCounter.StepId = Convert.ToByte(1);
                            //        mStepCounter.RequestId = mTrnICardRequest.RequestId;
                            //        mStepCounter.UpdatedOn = DateTime.Now;
                            //        mStepCounter.Updatedby = Convert.ToInt32(userId);
                            //        mStepCounter.ApplyForId = newBasicDetail.ApplyForId;
                            //        await iStepCounterBL.Add(mStepCounter);
                            //    }
                            //}
                            #endregion



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
                        // Collect model validation errors and pass via TempData 
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
                else // Case 2: Create new BasicDetail (when BasicDetailId == 0)
                {
                    // Similar flow: validate, map VM to entity, handle file uploads (mandatory), create request & step counter, save via BL
                    model.Updatedby = Convert.ToInt32(userId);
                    model.StatusLevel = 0;

                    if (ModelState.IsValid)
                    {
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
                        newBasicDetail.DateOfIssue = null;

                        if (model.TypeId == 4)
                        {
                            if (model.OldServiceNo != null)
                            {
                                int? BasicDetailId = await basicDetailBL.MaxBasicDetailId(model.OldServiceNo);
                                if (BasicDetailId != null)
                                {
                                    BasicDetail basicDetail = await basicDetailBL.Get((int)BasicDetailId);
                                    newBasicDetail.PreviousBasicDetailId = basicDetail.BasicDetailId;
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid Old Service No.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Old Service No required.");
                                goto end;
                            }
                        }
                        else
                        {
                            int? BasicDetailId = await basicDetailBL.MaxBasicDetailId(model.ServiceNo);
                            if (BasicDetailId != null)
                            {
                                newBasicDetail.PreviousBasicDetailId = BasicDetailId;
                            }
                            else
                            {
                                newBasicDetail.PreviousBasicDetailId = 0;
                            }
                        }

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
                        if (string.IsNullOrEmpty(model.AadhaarNo) || model.AadhaarNo.Length != 12 || !model.AadhaarNo.All(char.IsDigit) || model.AadhaarNo == "000000000000" || model.AadhaarNo[0] == '0')
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
                                await imageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
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
                                await imageEncryptAndDecrypt.EncryptImageFile(path, destinationPath);
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



                        byte? RecordOfficeId = await basicDetailBL.GetRecordOfficeId(model.ApplyForId, model.ServiceNo, model.ArmedId, model.RankId, dTOApplFwdCondition);

                        if (RecordOfficeId != null)
                        {
                            mTrnICardRequest.RecordOfficeId = (byte)RecordOfficeId;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Armed not mapped in Record Office / ORO .");
                            goto end;
                        }

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
                // Catch unexpected errors and log
                _logger.LogError(1006, ex, "Exception");
                ModelState.AddModelError("", ex.Message);
                goto end;
            }

        end:
            // If we reach here, something failed; repopulate dropdowns and return view with model to show errors
            return View(model);

        }

        /// <summary>
        /// Calls an external API endpoint to fetch data by ICNumber.
        /// </summary>
        /// <param name="ICNumber">
        /// Identifier used to fetch data from the API.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="DTOApiDataResponse"/> object with the API response data.
        /// </returns>
        public async Task<DTOApiDataResponse> GetApiData(string ICNumber)
        {
            using (var client = new HttpClient())
            {
                // API base address (local test URL used here, can be swapped with production URL)
                // client.BaseAddress = new Uri("https://api.postalpincode.in/");
                client.BaseAddress = new Uri("https://localhost:7002/api/Fetch/GetData/");

                // Perform GET request by appending ICNumber to the base URL
                using (HttpResponseMessage response = await client.GetAsync(ICNumber))
                {
                    // Read response content synchronously as string
                    var responseContent = response.Content.ReadAsStringAsync().Result;

                    // Throw exception if status code is not successful (ensures 2xx)
                    response.EnsureSuccessStatusCode();

                    // Deserialize JSON response into DTOApiDataResponse
                    DTOApiDataResponse? responseData = JsonConvert.DeserializeObject<DTOApiDataResponse>(responseContent);

                    return responseData;
                }
            }
        }


        /// <summary>
        /// Makes an HTTP POST request to fetch user data from an external API using ICNumber.
        /// </summary>
        /// <param name="ICNumber">
        /// The identifier (ICNumber) used to request user data from the API.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the deserialized API response data if successful,
        /// otherwise it throws an exception when the API call fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetUserData(string ICNumber)
        {
            using (var client = new HttpClient())
            {
                // API base address (currently set to localhost; can be changed to production URL)
                // client.BaseAddress = new Uri("https://api.postalpincode.in/");
                client.BaseAddress = new Uri("https://localhost:7002/api/Fetch/Get/");

                // Perform GET request by appending ICNumber to the base URL
                using (HttpResponseMessage response = await client.GetAsync(ICNumber))
                {
                    // Read response content synchronously as string
                    var responseContent = response.Content.ReadAsStringAsync().Result;

                    // Ensure status code indicates success (throws otherwise)
                    response.EnsureSuccessStatusCode();

                    // Deserialize response content into a dynamic object (no type specified)
                    var responseData = JsonConvert.DeserializeObject(responseContent);

                    // Return successful result with API response
                    return Ok(responseData);
                }
            }
        }


        #endregion

        #region DecryptZipFile/DecryptZipFileData

        /// <summary>
        /// Decrypts a Base64-encoded string representing a ZIP file identifier (jcoor).
        /// Validates input, decodes from Base64, and returns the view with decoded value.  
        /// Redirects to "ContactUs" with error if invalid.
        /// </summary>
        /// <param name="jcoor">
        /// Base64-encoded string representing a ZIP file identifier.
        /// </param>
        /// <returns>
        /// A <see cref="Task{ActionResult}"/> that renders the view with decoded data,
        /// or redirects to "ContactUs" if input is invalid or an exception occurs.
        /// </returns>
        [HttpGet]
        public Task<ActionResult> DecryptZipFile(string jcoor)
        {
            // Validate that jcoor is not null/empty and is a valid Base64 string
            if (string.IsNullOrEmpty(jcoor) || !service.IsValidBase64(jcoor))
            {
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
            try
            {
                // Decode Base64 string
                var base64EncodedBytes = Convert.FromBase64String(jcoor);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

                // Pass decoded string to ViewBag for use in the View
                ViewBag.jcoor = decodedString;

                // Return view with decoded data
                return Task.FromResult<ActionResult>(View());
            }
            catch (FormatException ex)
            {
                // Log specific error if input string is invalid Base64
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {jcoor}", jcoor);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
            catch (Exception ex)
            {
                // Log generic errors
                _logger.LogError(1001, ex, "BasicDetailsController=>InaccurateData.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
            }
        }


        /// <summary>
        /// Handles the decryption and extraction of files from a ZIP file, given a private key.
        /// </summary>
        /// <param name="model">
        /// The request model containing the uploaded ZIP file and other decryption parameters.
        /// </param>
        /// <returns>
        /// A JSON response containing the filename of the decrypted ZIP file if successful,
        /// otherwise returns a model error or internal server error as a response.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DecryptZipFileData(DTODecryptZipFileRequest model)
        {
            try
            {
                // Retrieve the encryption key from the database
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    model.PrivateKey = keyRecord.PrivateKey; // Assign the private key from the database record
                }
                else
                {
                    throw new InvalidOperationException("Encryption key record not found."); // Throw error if key record is missing
                }

                // Validate the model before proceeding
                if (ModelState.IsValid)
                {
                    // Define the folder for saving the uploaded file
                    string sourceFolderPhotoPhy = Convert.ToString(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell", "Temp"));

                    // Ensure the directory exists, create it if not
                    if (!Directory.Exists(sourceFolderPhotoPhy))
                        Directory.CreateDirectory(sourceFolderPhotoPhy);

                    // Generate a unique file name for the uploaded file
                    string TempFileName = Guid.NewGuid().ToString();
                    string FileName = service.ProcessUploadedFile(model.ZipFile, sourceFolderPhotoPhy, TempFileName);

                    // Define the full path where the file will be saved
                    string destinationzipfilename = Path.GetFileName(model.ZipFile.FileName);
                    string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                    // Validate that the uploaded file is a valid ZIP file
                    bool result = service.IsValidZipHeader(path);

                    if (!result)
                    {
                        // Add error if file format is not correct and delete the invalid file
                        ModelState.AddModelError("ZipFile", "File format not correct");
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path); // Clean up the file if it's invalid
                        }
                        return Json(KeyConstants.InternalServerError); // Return an error response
                    }

                    // Decrypt and unzip the file using the provided private key
                    ZipDecrypt.DecryptAndUnzip(path, sourceFolderPhotoPhy, sourceFolderPhotoPhy, destinationzipfilename, model.PrivateKey);

                    // Return the filename of the decrypted ZIP file as a JSON response
                    return Json(model.ZipFile.FileName);
                }
                else
                {
                    // Return validation errors as a JSON response
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected errors and return an internal server error response
                _logger.LogError(1001, ex, "BasicDetail->DecryptZipFile");
                return Json(KeyConstants.InternalServerError);
            }
            //end:
            //    return View(model); // No longer used, since returning JSON response
        }


        #endregion

        #region CSVFileUpload/UploadCsv/GetHeaderMap/UploadChipAndSerial
        /// <summary>
        /// Handles the GET request to upload a CSV file, validates and decodes a Base64-encoded string (jcoor),
        /// and then passes the decoded value to the view.
        /// </summary>
        /// <param name="jcoor">
        /// Base64-encoded string containing the identifier for CSV file upload.
        /// </param>
        /// <returns>
        /// Returns the view with the decoded string (jcoor) if valid, 
        /// or redirects to the "ContactUs" page with an error message if invalid or an exception occurs.
        /// </returns>
        [HttpGet]
        public Task<ActionResult> CSVFileUpload(string jcoor)
        {
            // Validate the Base64 string (jcoor)
            if (string.IsNullOrEmpty(jcoor) || !service.IsValidBase64(jcoor))
            {
                TempData["error"] = "Invalid Input."; // Set error message in TempData
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home")); // Redirect to "ContactUs" page
            }

            try
            {
                // Decode the Base64 string (jcoor) into the original string
                var base64EncodedBytes = Convert.FromBase64String(jcoor);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

                // Pass the decoded string to the view via ViewBag
                ViewBag.jcoor = decodedString;

                return Task.FromResult<ActionResult>(View()); // Return the view with decoded string
            }
            catch (FormatException ex)
            {
                // Handle FormatException if the Base64 string is not properly formatted
                _logger.LogError(1001, ex, message: "Invalid Base64 string for Id: {jcoor}", jcoor);
                TempData["error"] = "Invalid Input."; // Set error message in TempData
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home")); // Redirect to "ContactUs" page
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                _logger.LogError(1001, ex, "BasicDetailsController=>CSVFileUpload.");
                TempData["error"] = "Invalid Input."; // Set error message in TempData
                TempData.Keep("error");
                return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home")); // Redirect to "ContactUs" page
            }
        }

        /// <summary>
        /// Handles the uploading of a CSV file, processes the contents, and validates each record.
        /// </summary>
        /// <param name="model">
        /// The request model containing the uploaded CSV file.
        /// </param>
        /// <returns>
        /// A JSON response with the processed records. If any errors occur, returns an appropriate error message.
        /// </returns>
        [HttpPost]
        public IActionResult UploadCsv(DTOCSVFileRequest model)
        {
            // Check if the CSV file is provided and not empty
            if (model.CSVFile == null || model.CSVFile.Length == 0)
            {
                // Return bad request if the file is not uploaded or is empty
                return BadRequest(new { message = "File is not uploaded or is empty." });
            }

            var records = new List<object>(); // List to store the processed records

            try
            {
                // Read the uploaded file stream
                using (var stream = new StreamReader(model.CSVFile.OpenReadStream()))
                {
                    // Read the header line of the CSV file
                    string? headerLine = stream.ReadLine();
                    if (headerLine == null)
                    {
                        // Return bad request if the file is empty or missing headers
                        return BadRequest(new { message = "File is empty or missing headers." });
                    }

                    // Split header line into columns
                    var headers = headerLine.Split(',');
                    var headerMap = GetHeaderMap(headers); // Map header columns to known fields

                    // If header mapping is invalid, return an error message with expected columns
                    if (headerMap == null)
                    {
                        return BadRequest(new
                        {
                            message = $"Invalid column names. Expected: {string.Join(", ", _expectedColumns)}"
                        });
                    }

                    // Process each line in the CSV file
                    string? line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        var values = line.Split(',');

                        // Create a record object for each line
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

                        // Validate the record (e.g., check if RequestId is valid, ChipNo length, etc.)
                        if (record.RequestId == -1 || record.ChipNo.Length != 12 || !long.TryParse(record.ChipNo, out _))
                        {
                            // If validation fails, mark the record as invalid and add it to the list
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
                            // If validation passes, add the valid record to the list
                            records.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and return a 500 status code if an exception occurs during file processing
                return StatusCode(500, new { message = $"An error occurred while processing the file: {ex.Message}" });
            }

            // Return the processed records in a successful response
            return Ok(records);
        }


        /// <summary>
        /// Maps the header columns of a CSV file to their respective indices.
        /// </summary>
        /// <param name="headers">
        /// An array of header strings from the CSV file.
        /// </param>
        /// <returns>
        /// A dictionary mapping the header column names (case-insensitive) to their indices in the CSV file,
        /// or null if any of the expected columns are missing.
        /// </returns>
        private Dictionary<string, int>? GetHeaderMap(string[] headers)
        {
            // Create a case-insensitive dictionary to store header names and their indices
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Populate the dictionary with header names and their respective index positions
            for (int i = 0; i < headers.Length; i++)
            {
                headerMap[headers[i]] = i;
            }

            // Check if all expected columns are present in the header map
            if (!_expectedColumns.All(expected => headerMap.ContainsKey(expected)))
            {
                // Return null if any expected columns are missing
                return null;
            }

            // Return the populated header map if all expected columns are present
            return headerMap;
        }


        /// <summary>
        /// Handles the POST request to upload Chip and Serial data and processes it.
        /// Validates input, calls the business logic layer to upload the data, and returns a response.
        /// </summary>
        /// <param name="data">
        /// A list of DTO objects containing Chip and Serial data to be uploaded.
        /// </param>
        /// <returns>
        /// A JSON response indicating the result of the upload operation, including success or failure message.
        /// If the input is invalid or empty, returns an appropriate error message.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> UploadChipAndSerial([FromBody] List<DTOUploadChipAndSerialRequest> data)
        {
            // Validate that data contains at least one record
            if (data == null || data.Count == 0)
            {
                // Return a bad request if no records are provided
                return BadRequest(new { message = "No records received. Please select at least one record to process." });
            }

            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();

            // Check if the model state is valid before proceeding
            if (ModelState.IsValid)
            {
                // Call the business logic layer to upload the Chip and Serial data
                response = await basicDetailBL.UploadChipAndSerial(data);

                // Return the response indicating success or failure of the upload
                if (response.Result == true)
                {
                    return Json(response); // Return success response as JSON
                }
                else
                {
                    return Json(response); // Return failure response as JSON
                }
            }
            else
            {
                // Handle the case where the model state is invalid
                response.Result = false;

                // Collect all error messages from the model state
                response.Message = ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToString();

                // Return failure response with validation error messages
                return Json(response);
            }
        }



        /// <summary>
        /// Handles the POST request to retrieve the history of uploaded CSV files for data tables.
        /// Calls the business logic layer to fetch the data and returns it as a JSON response.
        /// In case of an error, returns an empty data table with appropriate error logging.
        /// </summary>
        /// <param name="dTO">
        /// The request model containing parameters for filtering and pagination (DataTables request).
        /// </param>
        /// <returns>
        /// A JSON response containing the CSV file upload history data or an empty data table in case of failure.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> GetCSVFileUploadsHistory([FromForm] DTODataTablesRequest dTO)
        {
            try
            {
                // Fetch the data for the CSV file upload history using the business logic layer
                return Json(await _iCSVImportBL.GetDataTableResponse(dTO));
            }
            catch (Exception ex)
            {
                // In case of an error, create an empty data table response and log the error
                List<CSVImport> dTOClaimsStoreResponses = new List<CSVImport>();
                var responseData = new DTODataTablesResponse<CSVImport>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOClaimsStoreResponses
                };

                // Log the error details with the exception message
                _logger.LogError(1001, ex, "BasicDetail->GetCSVFileUploadsHistory");

                // Return the empty data table response as JSON
                return Json(responseData);
            }
        }


        #endregion

        #region SaveInternalFwd/IcardFwd/IcardRejecte/UpdateStepCounter/SaveICardRequestHold/DataExport/DataDigitalXmlSign/GenerateLastRecordXml/MergeXmlDocuments/GenerateJsonResponse

        /// <summary>
        /// Handles the saving of internal forward data, validates the request, and processes it.
        /// The method updates the necessary details and returns the success or failure response in JSON format.
        /// </summary>
        /// <param name="data">
        /// The DTO containing the internal forward request data to be saved.
        /// </param>
        /// <returns>
        /// A JSON response indicating the result of the save operation. 
        /// Returns `true` if the save is successful, `false` if it fails, or `null` if no result is obtained.
        /// </returns>
        [Authorize(Policy = "InternalWkDistrPolicy")]
        public async Task<IActionResult> SaveInternalFwd(DTOSaveInternalFwdRequest data)
        {
            try
            {
                // Retrieve session data for user ID and unit ID
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                // Set values for the data object using session data and user information
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsComplete = false;
                data.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId = Convert.ToByte(data.TypeId);

                // Check if the model state is valid
                if (ModelState.IsValid)
                {
                    // Call the business logic layer to save the internal forward data
                    bool? result = (bool)await iTrnFwnBL.SaveInternalFwd(data);

                    // Return appropriate JSON response based on the result
                    if (result != null)
                    {
                        if (result == true)
                        {
                            return Json(true); // Success
                        }
                        else
                        {
                            return Json(false); // Failure
                        }
                    }
                    else
                    {
                        return Json(null); // Null result case
                    }

                }
                else
                {
                    // Return validation errors as a JSON response if the model state is invalid
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex)
            {
                // Log the exception and return a BadRequest response if an error occurs
                _logger.LogError(1001, ex, "BasicDetails=>SaveInternalFwd");
                return BadRequest();
            }
        }


        /// <summary>
        /// Handles the forwarding of ICard data, updates the request details, and adds a new forward record.
        /// If the update is successful, returns the new forward record. Otherwise, returns a bad request response.
        /// </summary>
        /// <param name="data">
        /// The MTrnFwd data object containing the forward request details.
        /// </param>
        /// <returns>
        /// A JSON response indicating success with the new forward record, or a bad request response in case of failure.
        /// </returns>
        public async Task<IActionResult> IcardFwd(MTrnFwd data)
        {
            try
            {
                // Retrieve session data for user ID and unit ID
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                // Set values for the forward data object using session data and user information
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.UpdatedOn = DateTime.Now;
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId = Convert.ToByte(data.TypeId);

                // Check if all records by RequestId can be updated
                if (await iTrnFwnBL.UpdateAllBYRequestId(data.RequestId))
                {
                    // If successful, reset TrnFwdId and add the new forward record
                    data.TrnFwdId = 0;
                    data = await iTrnFwnBL.AddWithReturn(data);

                    // Return the new forward record as a successful response
                    return Ok(data);
                }
                else
                {
                    // Return bad request if the update failed
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions and return a bad request response
                _logger.LogError(1001, ex, "BasicDetails=>IcardFwd.");
                return BadRequest();
            }
        }


        /// <summary>
        /// Handles the rejection of an ICard forward request. Updates the forward details, performs domain mapping, 
        /// processes XML digital sign data, and returns a success or failure response.
        /// </summary>
        /// <param name="data">
        /// The MTrnFwd data object containing the forward request details to be rejected.
        /// </param>
        /// <returns>
        /// A JSON response indicating success with the updated forward record or a bad request response in case of failure.
        /// </returns>
        public async Task<IActionResult> IcardRejecte(MTrnFwd data)
        {
            try
            {
                // Create response object
                DTOBasicDetailsSaveResponse response = new DTOBasicDetailsSaveResponse();

                // Retrieve session data for user ID and unit ID
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                // Set values for the forward data object using session data and user information
                data.FromUserId = sessiondata.UserId;
                data.UnitId = sessiondata.UnitId;
                data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.UpdatedOn = DateTime.Now;
                data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                data.IsActive = true;
                data.TypeId = Convert.ToByte(1); // Set the type to 1 for rejection

                // Retrieve domain mapping using the request ID
                TrnDomainMapping Domain = new TrnDomainMapping();
                Domain = await iDomainMapBL.GetByRequestId(data.RequestId);

                if (Domain != null)
                {
                    // Set the recipient user ID and AspNetUsersId for the rejection
                    data.ToAspNetUsersId = Domain.AspNetUsersId;
                    data.ToUserId = Domain.UserId.GetValueOrDefault();

                    // Update all records by request ID
                    if (await iTrnFwnBL.UpdateAllBYRequestId(data.RequestId))
                    {
                        // Add the rejection record
                        await iTrnFwnBL.Add(data);

                        // Process digital sign XML files for the rejection
                        int[] d = new int[1];
                        d[0] = data.RequestId;
                        var dataret = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(d);

                        if (dataret != null)
                        {
                            dataret.XmlFiles = ""; // Clear the XML files after processing
                        }

                        // Save the processed XML digital sign
                        await _iTrnLoginLogBL.XmlFileDigitalSign(dataret);

                        // Return the updated rejection data as JSON
                        return Ok(data);
                    }
                    else
                    {
                        // Return a bad request if update fails
                        return BadRequest();
                    }
                }
                else
                {
                    // Return a bad request if domain mapping is not found
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions and return a bad request response
                _logger.LogError(1001, ex, "BasicDetails=>IcardRejecte.");
                return BadRequest();
            }
        }


        /// <summary>
        /// Updates the step counter information based on the provided MStepCounter object.
        /// Performs necessary checks, such as verifying domain mapping, session data, and unit mapping.
        /// Returns a response indicating success or failure along with a relevant message.
        /// </summary>
        /// <param name="mStepCounter">
        /// The MStepCounter object containing the details to be updated.
        /// </param>
        /// <returns>
        /// A JSON response indicating the result of the update operation. If successful, 
        /// it returns `Result = true`, otherwise `Result = false` with an appropriate message.
        /// </returns>
        public async Task<IActionResult> UpdateStepCounter(MStepCounter mStepCounter)
        {
            DTOBasicDetailsSaveResponse response = new DTOBasicDetailsSaveResponse();
            try
            {
                // If the Flag is 'R', perform additional checks
                if (mStepCounter.Flag == "R")
                {
                    // Retrieve domain mapping using the request ID
                    TrnDomainMapping Domain = new TrnDomainMapping();
                    Domain = await iDomainMapBL.GetByRequestId(mStepCounter.RequestId);

                    // If the UserId from the domain mapping is 0, return an error response
                    if (Domain?.UserId.GetValueOrDefault() == 0)
                    {
                        response.Message = "Profile is not mapped with domain Id!";
                        response.Result = false;
                        return Ok(response);
                    }
                }

                // Retrieve session data for unit ID
                DtoSession sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                // Get unit details based on the session's unit ID
                DTOMapUnitResponse dTOMapUnitResponse = await mapUnitBL.GetALLByUnitMapId(sessiondata.UnitId);

                // Update the step counter with the current date, user ID, and unit name
                mStepCounter.UpdatedOn = DateTime.Now;
                mStepCounter.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                mStepCounter.UnitName = dTOMapUnitResponse.UnitName;

                // Call the service to update the step counter
                await iStepCounterBL.UpdateStepCounter(mStepCounter);
                response.Result = true;
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                _logger.LogError(1001, ex, "BasicDetails=>IcardFwd.");
                response.Message = "Internal Server Error!";
            }

            // Return the response with the operation result
            return Ok(response);
        }


        /// <summary>
        /// Handles the saving of an ICard hold request. It validates the model state, checks if a record with the given request ID already exists, 
        /// and either updates or adds the record accordingly. The method returns a JSON response indicating the result of the operation.
        /// </summary>
        /// <param name="dTO">
        /// The MTrnICardHold object containing the data for the ICard hold request to be saved.
        /// </param>
        /// <returns>
        /// A JSON response indicating the outcome of the operation. Possible responses include:
        /// - KeyConstants.Save: If the data was successfully saved.
        /// - KeyConstants.Update: If the data was successfully updated.
        /// - KeyConstants.Exists: If a record with the same request ID already exists.
        /// - ModelState error messages if the model is invalid.
        /// </returns>
        [Authorize(Policy = "FlagICardApplPolicy")]
        public async Task<IActionResult> SaveICardRequestHold(MTrnICardHold dTO)
        {
            try
            {
                // Retrieve session data for the current user
                DtoSession? sessiondata = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                if (sessiondata != null)
                {
                    dTO.UserId = sessiondata.UserId;  // Set the UserId from session data
                }

                // Set additional metadata for the ICard hold request
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                // Check if the model state is valid
                if (ModelState.IsValid)
                {
                    // Check if a record with the given request ID already exists
                    if (!await _iICardHoldBL.GetByRequestId(dTO))
                    {
                        // If the ICard hold record has an ID, update it, else add a new record
                        if (dTO.ICardHoldId > 0)
                        {
                            await _iICardHoldBL.Update(dTO);  // Update existing record
                            return Json(KeyConstants.Save);   // Return success response
                        }
                        else
                        {
                            await _iICardHoldBL.Add(dTO);    // Add new record
                            return Json(KeyConstants.Update);  // Return success response
                        }
                    }
                    else
                    {
                        // If the record with the same request ID already exists, return an "exists" response
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    // If model state is invalid, return the validation errors as a JSON response
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error response
                _logger.LogError(1001, ex, "BasicDetail->SaveICardRequestHold");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Exports data based on the provided request and generates corresponding CSV and JSON files.
        /// Handles encryption of images/signatures and optionally encrypts and zips the final export folder.
        /// </summary>
        /// <param name="Data">DTODataExportRequest object containing export parameters such as DataExportType, public/private keys, etc.</param>
        /// <returns>Returns a JSON response containing the last folder name of exported data, or an internal server error key in case of failure.</returns>
        [Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<IActionResult> DataExport(DTODataExportRequest Data)
        {
            try
            {
                // Retrieve the encryption key record from the database
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    Data.publicKey = keyRecord.PublicKey;
                    Data.privateKey = keyRecord.PrivateKey;
                }
                else
                {
                    // Throw exception if encryption keys are not found
                    throw new InvalidOperationException("Encryption key record not found.");
                }

                // Get application forward condition configuration from appsettings
                DTOApplFwdConditionRequest? dTOApplFwdCondition = _configuration.GetSection("ApplFwdCondition").Get<DTOApplFwdConditionRequest>() ?? new DTOApplFwdConditionRequest
                {
                    MPRSO = new MPRSO(),
                    MP6F = new MP6F(),
                    MP6A = new MP6A()
                };

                // Validate critical fields in configuration
                if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                    dTOApplFwdCondition.MPRSO.RecordOfficeId == 0 || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) ||
                    string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) || dTOApplFwdCondition.MP6F.RecordOfficeId == 0 ||
                    string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6A.Name) || dTOApplFwdCondition.MP6A.RecordOfficeId == 0 || dTOApplFwdCondition.MP6A.RankOrderby == 0)
                {
                    // Return internal server error if configuration is invalid
                    return Json(KeyConstants.InternalServerError);
                }

                // Fetch basic details for requested data export
                List<DTODataExportsResponse> retdata = await basicDetailBL.GetBesicdetailsByRequestId(Data, dTOApplFwdCondition);
                if (retdata.Count() > 0)
                {
                    // Retrieve session object
                    DtoSession dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                    // Create folder for export with unique random name
                    string sourceFolderPhotoPhy = Convert.ToString(ForCreateFolderrandom(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell"), dtoSession.DoaminId));
                    string lastFolderName = new DirectoryInfo(sourceFolderPhotoPhy).Name;

                    // Initialize variables for processing record office-wise export
                    int recoff = 0;
                    List<DTODataExportsResponse> lst = new List<DTODataExportsResponse>(); // Temp list for JSON/CSV per record office
                    List<DTODataExportsResponse> csvlst = new List<DTODataExportsResponse>(); // List for combined CSV
                    string recofffolder = "";
                    string recoffphotos = "";
                    string recoffsing = "";
                    int count = 0;
                    string arryRequestId = "";

                    // Process each record for export
                    foreach (var data in retdata)
                    {
                        count++;

                        // If moving to a new RecordOffice, finalize previous folder
                        if (recoff != data.RecordOfficeId)
                        {
                            if (recoff != 0)
                            {
                                // Serialize and save JSON for previous record office
                                var jsonString = JsonConvert.SerializeObject(lst);
                                var jsonde = JsonConvert.DeserializeObject(jsonString);
                                System.IO.File.WriteAllText(recofffolder + "/Data.json", jsonString);

                                // Generate CSV for previous record office
                                CsvService csvService = new CsvService();
                                string csvData = csvService.GenerateCsv(lst);
                                System.IO.File.WriteAllText(recofffolder + "/Data.csv", csvData);
                            }

                            // Clear temp list for new record office
                            lst.Clear();

                            // Create new folders for photos and signatures for the new record office
                            recofffolder = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice));
                            recoffphotos = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice + "/Photos/"));
                            recoffsing = Convert.ToString(CreateFolder(sourceFolderPhotoPhy + "/" + data.RecordOffice + "/Signature"));
                        }

                        // Determine file extensions after removing ".enc" suffix
                        string temp = data.PhotoImagePath.Replace(".enc", string.Empty);
                        string[] parts = temp.Split('.');
                        string extenstionImage = parts[parts.Length - 1];

                        temp = data.SignatureImagePath.Replace(".enc", string.Empty);
                        parts = temp.Split('.');
                        string extenstionSign = parts[parts.Length - 1];

                        // Decrypt photo and signature images to respective folders
                        await imageEncryptAndDecrypt.DecryptImageFile(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Photo", data.PhotoImagePath), recoffphotos + "/" + data.ServiceNo + "." + extenstionImage);
                        await imageEncryptAndDecrypt.DecryptImageFile(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Signature", data.SignatureImagePath), recoffsing + "/" + data.ServiceNo + "." + extenstionSign);

                        // Add record to temp lists
                        lst.Add(data);
                        csvlst.Add(data);

                        // Update current RecordOfficeId
                        recoff = data.RecordOfficeId;

                        // If last record, finalize JSON and CSV for the last record office
                        if (count == retdata.Count())
                        {
                            var jsonString = JsonConvert.SerializeObject(lst);
                            var jsonde = JsonConvert.DeserializeObject(jsonString);
                            System.IO.File.WriteAllText(recofffolder + "/Data.json", jsonString);

                            CsvService csvService = new CsvService();
                            string csvData = csvService.GenerateCsv(lst);
                            System.IO.File.WriteAllText(recofffolder + "/Data.csv", csvData);
                        }

                        // Build comma-separated request IDs
                        if (count == 1)
                            arryRequestId = data.ApplId + "";
                        else
                            arryRequestId = arryRequestId + "," + data.ApplId;
                    }

                    // Generate combined CSV for all records
                    if (count != 0 && count == retdata.Count())
                    {
                        CsvService csvService = new CsvService();
                        string csvData = csvService.GenerateCsv(csvlst);
                        System.IO.File.WriteAllText(sourceFolderPhotoPhy + "/" + lastFolderName + ".csv", csvData);
                    }

                    // Optionally encrypt and zip the export folder based on DataExportType
                    if (Data.DataExportType == 1)
                    {
                        string sourceFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell", "Temp");

                        // Ensure directory exists
                        if (!Directory.Exists(sourceFolder))
                        {
                            Directory.CreateDirectory(sourceFolder);
                        }

                        string tempZipFilePath = Convert.ToString(Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "ExportAFSACCell", "Temp"));

                        // Encrypt and zip folder using public key
                        ZipEncrypt.EncryptAndZip(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip", tempZipFilePath, Data.publicKey);
                    }
                    else
                    {
                        // Create zip without encryption
                        CreateZipFromFolder(sourceFolderPhotoPhy, sourceFolderPhotoPhy + ".zip");
                    }

                    // Log export in database
                    var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    DTODataExported dTODataExported = new DTODataExported();
                    dTODataExported.AspNetUsersId = userId;
                    dTODataExported.UserId = Convert.ToInt32(dtoSession.UserId);
                    dTODataExported.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    dTODataExported.CreatedBy = dtoSession.RankName + " " + dtoSession.Name + " (" + dtoSession.ICNO + ")";
                    dTODataExported.CreatedOn = DateTime.Now;
                    dTODataExported.RequestId = arryRequestId;
                    await _iTrnLoginLogBL.AddDataExport(dTODataExported);

                    // Return last folder name to caller
                    return Json(lastFolderName);
                }
                else
                {
                    // Return internal server error if no data found
                    return Json(KeyConstants.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetails=>DataExport.");
                return Json(KeyConstants.InternalServerError);
            }
        }




        /// <summary>
        /// Handles the digital XML signing process for a set of data export requests.
        /// </summary>
        /// <param name="Data">DTO containing the list of record IDs to be digitally signed.</param>
        /// <returns>
        /// Returns a JSON object with merged XML files for digital signing if data exists,
        /// otherwise generates a JSON response indicating the absence of XML data.
        /// </returns>
        public async Task<IActionResult> DataDigitalXmlSign(DTODataExportRequest Data)
        {
            try
            {
                // Step 1: Initialize the return object which will hold the ID and merged XML files
                DTOXmlFilesFwdLogRequest ret = new DTOXmlFilesFwdLogRequest();

                // Step 2: Fetch existing XML data from the database based on provided IDs
                // The BL (Business Layer) returns an object containing XML files if they exist
                var xmldata = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(Data.Ids);

                // Step 3: Check if any XML data was retrieved
                if (xmldata != null && !string.IsNullOrEmpty(xmldata.XmlFiles))
                {
                    // Step 3a: Assign the database record ID to the return object
                    ret.Id = xmldata.Id;

                    // Step 3b: Generate XML for the last record in the provided IDs list
                    // This is likely the most recent data that needs to be added to existing XML
                    string xml = await GenerateLastRecordXml(Data.Ids[0]);

                    // Step 3c: Merge the existing XML from the database with the newly generated XML
                    // Ensures that the final XML contains all historical and latest records
                    ret.XmlFiles = MergeXmlDocuments(xmldata.XmlFiles, xml);

                    // Step 3d: Return the merged XML as a JSON response to the frontend
                    return Json(ret);
                }
                else
                {
                    // Step 4: If no XML data exists, generate a JSON response appropriately
                    // This may include a message like "No data found" or an empty XML structure
                    return await GenerateJsonResponse(xmldata, Data);
                }
            }
            catch (Exception ex)
            {
                // Step 5a: Log the exception using a unique code (1001) for easier identification
                // This helps in tracing which method the error occurred in during debugging
                _logger.LogError(1001, ex, "BasicDetails=>DataDigitalXmlSign.");

                // Step 5b: Redirect the user to a generic error page to prevent crashing the app
                return RedirectToAction("Error", "Error");
            }
        }


        /// <summary>
        /// Generates an XML string representation of the last forwarded record for a given ID.
        /// </summary>
        /// <param name="id">The ID of the record to fetch and serialize.</param>
        /// <returns>
        /// Returns a string containing the XML representation of the last record, suitable for digital signing.
        /// </returns>
        private async Task<string> GenerateLastRecordXml(int id)
        {
            // Step 1: Fetch the last record from the business layer (ICard forwarding context)
            // This typically retrieves the latest record associated with the provided ID
            var lastRec = await basicDetailBL.ICardFwdLastRec(id);

            // Step 2: Initialize an XmlSerializer to convert the DTO object into XML
            // The type parameter is DTOFwdLastRecForDigitalSign which defines the XML schema
            XmlSerializer serializer = new XmlSerializer(typeof(DTOFwdLastRecForDigitalSign));

            // Step 3: Use a StringWriter to write the XML content into a string
            using (StringWriter writer = new StringWriter())
            {
                // Step 3a: Serialize the last record object into XML format
                serializer.Serialize(writer, lastRec);

                // Step 3b: Return the serialized XML string to the caller
                return writer.ToString();
            }
        }

        /// <summary>
        /// Merges two XML documents into a single XML structure suitable for digital signing.
        /// </summary>
        /// <param name="xmlData">The existing XML data as a string.</param>
        /// <param name="lastRecordXml">The XML string of the last record to be merged.</param>
        /// <returns>
        /// Returns a string containing the merged XML, with a root element <c>RecForDigitalSign</c>.
        /// </returns>
        private string MergeXmlDocuments(string xmlData, string lastRecordXml)
        {
            // Step 1: Load the first XML document (existing XML data)
            XmlDocument xmlDoc1 = new XmlDocument();
            xmlDoc1.LoadXml(xmlData);

            // Step 2: Load the second XML document (last record XML)
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(lastRecordXml);

            // Step 3: Create a new XML document that will hold the merged content
            XmlDocument xmlDoc3 = new XmlDocument();

            // Step 3a: Create the root element "RecForDigitalSign"
            XmlElement rootElement = xmlDoc3.CreateElement("RecForDigitalSign");
            xmlDoc3.AppendChild(rootElement);

            // Step 4: Import all child nodes from the last record XML into the new document
            foreach (XmlNode node in xmlDoc2.DocumentElement.ChildNodes)
            {
                XmlNode importedNode = xmlDoc3.ImportNode(node, true); // Deep copy
                rootElement.AppendChild(importedNode);
            }

            // Step 5: Import the existing XML document's root element into the new document
            XmlNode importedRoot = xmlDoc3.ImportNode(xmlDoc1.DocumentElement, true); // Deep copy
            rootElement.AppendChild(importedRoot);

            // Step 6: Return the merged XML as a string
            return xmlDoc3.OuterXml;
        }

        /// <summary>
        /// Generates a JSON response when XML signing data is unavailable or empty.
        /// </summary>
        /// <param name="xmldata">The original XML log data object, can be null.</param>
        /// <param name="Data">The data export request containing the list of IDs to process.</param>
        /// <returns>
        /// Returns a <see cref="JsonResult"/> containing either a structured update object
        /// with ID and JSON data or a plain JSON object when xmldata is null.
        /// </returns>
        private async Task<IActionResult> GenerateJsonResponse(DTOXmlFilesFwdLogRequest xmldata, DTODataExportRequest Data)
        {
            // Step 1: Fetch the digital XML sign data as a fallback when original XML is missing
            var retData = await basicDetailBL.GetDataDigitalXmlSign(Data);

            // Step 2: Serialize the retrieved data to JSON string
            var jsonString = JsonConvert.SerializeObject(retData);

            // Step 3: Deserialize JSON string back to object for proper JSON formatting in response
            var jsonResponse = JsonConvert.DeserializeObject(jsonString);

            // Step 4: If original XML data exists, wrap it into DTOXmlFilesForUpdate object
            if (xmldata != null)
            {
                DTOXmlFilesForUpdate updateResponse = new DTOXmlFilesForUpdate
                {
                    Id = xmldata.Id,
                    jsonfile = jsonResponse
                };

                // Return the structured JSON response
                return Json(updateResponse);
            }

            // Step 5: If xmldata is null, return plain JSON response
            return Json(jsonResponse);
        }

        #endregion

        #region FaultyCard

        /// <summary>
        /// Retrieves remarks data for the given comma-separated list of remark IDs.
        /// </summary>
        /// <param name="RemarksIds">Comma-separated string of remark IDs (e.g., "1,2,3").</param>
        /// <returns>
        /// Returns a <see cref="JsonResult"/> containing the remarks data fetched from the database.
        /// </returns>
        public async Task<IActionResult> GetRemarksData(string RemarksIds)
        {
            // Step 1: Split the comma-separated string into a string array
            string[] strArray = RemarksIds.Split(',');

            // Step 2: Convert the string array to an integer array
            int[] intArray = Array.ConvertAll(strArray, int.Parse);

            // Step 3: Fetch remarks data from the business layer and return as JSON
            return Json(await faultyCardBL.GetRemarksData(intArray));
        }

        /// <summary>
        /// Displays the Faulty Card view and sets the user's claim for "ICard Export Data".
        /// </summary>
        /// <returns>
        /// Returns a <see cref="ViewResult"/> for the Faulty Card page with a ViewBag property indicating claim status.
        /// </returns>
        public async Task<ViewResult> FaultyCardAsync()
        {
            // Step 1: Get the current user's ID from the claims
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Step 2: Retrieve the user object from UserManager
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

            bool Claim = false;

            // Step 3: Get all claims for the current user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Step 4: Check if user has the "ICard Export Data" claim
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }

            // Step 5: Pass claim status to the view using ViewBag
            ViewBag.Claim = Claim;

            // Step 6: Return the Faulty Card view
            return View();
        }

        /// <summary>
        /// Retrieves all faulty card records for the DataTables frontend, taking into account the user's unit and claims.
        /// </summary>
        /// <param name="dTO">The DataTables request DTO containing paging, search, and filter parameters.</param>
        /// <returns>
        /// Returns a JSON response with faulty card records, including total and filtered counts for DataTables.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetAllFaulty(DTODataTablesRequestForFaultyCard dTO)
        {
            // Step 1: Get current user's ID from claims
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            int MapUnitId = 0;
            DtoSession? dtoSession = new DtoSession();

            // Step 2: Retrieve session data if available
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            // Step 3: Determine mapped unit ID from session
            MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;

            // Step 4: Retrieve user object from UserManager
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

            bool Claim = false;

            // Step 5: Get all claims for the current user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Step 6: Check if user has the "ICard Export Data" claim
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }

            try
            {
                // Step 7: Set claim and unit mapping info in DTO for business layer
                dTO.Claim = Claim;
                dTO.UnitMapId = dtoSession != null ? dtoSession.UnitId : 0;

                // Step 8: Fetch faulty card data from business layer and return as JSON
                return Json(await faultyCardBL.GetAllFaulty(dTO));
            }
            catch (Exception ex)
            {
                // Step 9: Prepare empty response in case of error to satisfy DataTables format
                List<DTOFaultyCardListResponse> dTOUserRegnResponses = new List<DTOFaultyCardListResponse>();
                var responseData = new DTODataTablesResponse<DTOFaultyCardListResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };

                // Step 10: Log the exception for debugging
                _logger.LogError(1001, ex, "Master->GetAllFaulty");

                // Step 11: Return empty JSON response
                return Json(responseData);
            }
        }

        /// <summary>
        /// Retrieves detailed information for a specific faulty card record.
        /// </summary>
        /// <param name="TrnFaultyCardId">The ID of the faulty card transaction to fetch details for.</param>
        /// <returns>
        /// Returns a JSON response containing the faulty card details.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetTrnFaultyCardDetail(int TrnFaultyCardId)
        {
            // Step 1: Call business layer to fetch the details for the given faulty card ID
            var detail = await faultyCardBL.GetTrnFaultyCardDetail(TrnFaultyCardId);

            // Step 2: Return the result as JSON
            return Json(detail);
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
                data.PhotoImagePath = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
            }
            if (System.IO.File.Exists(sourcePathSignature))
            {
                data.SignatureImagePath = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
            }
            return PartialView("_BasicDetail_ParitalView", data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<IActionResult> SaveFaultyCard([FromBody] DTOFaultyCardRequest dTO)
        {
            MTrnFwd? mTrnFwd = new MTrnFwd();
            DtoSession? dtoSession = new DtoSession();
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;
            //Reject Case
            if (dTO.Choice == 3)
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
                    bool Claim = false;
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
                    // UserManager service GetClaimsAsync method gets all the current claims of the user
                    var UserClaims = await userManager.GetClaimsAsync(user);
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        Claim = true;
                    }
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
                            if (trnFaultyCard != null)
                            {
                                dTO.TrnFwdId = trnFaultyCard.TrnFwdId ?? 0;
                            }

                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd, Claim);
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
                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd, Claim);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFaultyCardRequest([FromBody] DTOFaultyCardRequest dTO)
        {
            MTrnFwd? mTrnFwd = new MTrnFwd();
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;

            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                bool Claim = false;
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
                // UserManager service GetClaimsAsync method gets all the current claims of the user
                var UserClaims = await userManager.GetClaimsAsync(user);
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                {
                    Claim = true;
                }

                dTO.IsActive = true;
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
                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd, Claim);
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
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
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
        public IActionResult DownloadCsv(string fileName, string fileStoreName)
        {
            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var mimeType = "text/csv";
                return PhysicalFile(filePath, mimeType, $"E-ISAC_{fileStoreName}ExportData.csv");
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
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }
                model.IsActive = true;
                model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
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

        #region LostCard
        public async Task<ViewResult> LostCardAsync()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllLost(DTODataTablesRequest dTO)
        {
            return Json(await _lostCardBL.GetAllLost(dTO));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LostDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");
                var records = await _lostCardBL.GetDetailsByRequestIds(req);
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTOLostCardExportResponse>(true, CsvClassMapTypeEnum.HotlistExport));
                    try
                    {
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->LostDataExport");
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

        public async Task<ActionResult> LostCardRequestAsync()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLostCardRequest([FromForm] DTOLostCardAddRequest model)
        {
            DTOCommonSaveResponse dTOResponse = new DTOCommonSaveResponse();
            try
            {
                var trnLostCard = new TrnLostCard();
                if (ModelState.IsValid)
                {
                    #region Upload Supporting Document
                    string fileName = string.Empty;
                    if (model.File != null)
                    {
                        fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                        var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "LostCardSupportingDoc");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.File.CopyToAsync(stream);
                        }
                    }
                    #endregion Upload Supporting Document

                    DtoSession? dtoSession = new DtoSession();
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                    {
                        dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    }
                    trnLostCard.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
                    trnLostCard.RequestId = model.RequestId;
                    trnLostCard.Remark = model.Remark;
                    trnLostCard.LostOn = model.LostOn;
                    trnLostCard.IsFIRLogged = model.IsFIRLogged;
                    trnLostCard.SignedXML = model.SignedXML ?? string.Empty;
                    trnLostCard.SupportDocName = string.IsNullOrEmpty(fileName) ? "" : fileName;
                    trnLostCard.IsActive = true;
                    trnLostCard.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    trnLostCard.UpdatedOn = DateTime.Now;
                    bool checkduplicate = await _lostCardBL.FindAnyRequestId(model.RequestId);
                    if (checkduplicate)
                    {
                        dTOResponse.Result = false;
                        dTOResponse.Message = "The lost request already exists!";
                    }
                    else
                    {
                        var result = await _lostCardBL.AddWithReturn(trnLostCard);
                        await HotlistLostCard(trnLostCard);
                        dTOResponse.Result = true;
                        dTOResponse.Message = "Record created!";
                        dTOResponse.CurrentTime = result.UpdatedOn.GetValueOrDefault();
                        dTOResponse.Id = result.LostCardId.ToString();
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
                        dTOResponse.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOResponse.Result = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->SaveHotlistCardRequest");
                dTOResponse.Result = false;
                dTOResponse.Message = "Internal Server Error!";
            }

            return Json(dTOResponse);
        }

        private async Task HotlistLostCard(TrnLostCard lostCard)
        {
            try
            {
                var cardStatus = await basicDetailBL.CheckCardStatus(lostCard.RequestId);
                if (cardStatus == 1)
                {
                    await basicDetailBL.UpdateCardStatus(lostCard.RequestId, 3);
                }
                else
                {
                    var isHotlistExists = await _hotlistCardBL.FindRequestId(lostCard.RequestId);
                    if (!isHotlistExists)
                    {
                        TrnHotlistCard trnHotlistCard = new TrnHotlistCard()
                        {
                            RequestId = lostCard.RequestId,
                            RemarksIds = "65",
                            Remark = lostCard.Remark,
                            IsActive = lostCard.IsActive,
                            Updatedby = lostCard.Updatedby,
                            UpdatedbyUserId = lostCard.UpdatedbyUserId,
                            UpdatedOn = lostCard.UpdatedOn
                        };

                        await _hotlistCardBL.Add(trnHotlistCard);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->HotlistLostCard");
            }
        }
        #endregion HotlistCard

        #region DistributeCard
        public async Task<ViewResult> DistributeCardAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllDistribute(DTODataTablesRequest dTO)
        {
            return Json(await _distributeCardBL.GetAllDistribute(dTO));
        }

        [HttpPost]
        public async Task<IActionResult> DistributeDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");
                var records = await _distributeCardBL.GetDetailsByRequestIds(req);
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTODistributeCardExportResponse>(true, CsvClassMapTypeEnum.DistributeCard));
                    try
                    {
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->DistributeDataExport");
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
                _logger.LogError(1001, ex, "BasicDetail->DistributeDataExport");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }
        ReturnSt:
            return Json(dTOFaulty);
        }

        public async Task<ActionResult> DistributeCardRequestAsync()
        {
            return View();
        }

        public async Task<IActionResult> SaveDistributeCardRequest(TrnDistributeCard model)
        {
            DTOCommonSaveResponse dTOResponse = new DTOCommonSaveResponse();
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }
                model.IsActive = true;
                model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
                model.UpdatedOn = DateTime.Now;
                model.DistributedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    var checkCardBeforeDist = await basicDetailBL.CheckBeforeDistribution(model.RequestId);
                    if (checkCardBeforeDist.Result)
                    {
                        bool checkduplicate = await _distributeCardBL.FindRequestId(model.RequestId);
                        if (checkduplicate)
                        {
                            dTOResponse.Result = false;
                            dTOResponse.Message = "The distribute request already exists!";
                        }
                        else
                        {
                            ICardHistoryResponseAll? cardHistoryResponses = await basicDetailBL.ICardHistory(model.RequestId);
                            dTOResponse = await _distributeCardBL.SaveDistributeCard(model, cardHistoryResponses);
                        }
                    }
                    else
                    {
                        dTOResponse.Message = $"Please create a {checkCardBeforeDist.Message} entry for previous card!";
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
                        dTOResponse.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOResponse.Result = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->SaveDistributeCardRequest");
                dTOResponse.Result = false;
                dTOResponse.Message = "Internal Server Error!";
            }

            return Json(dTOResponse);
        }
        #endregion DistributeCard

      #region GetSessionValue/GetData/SearchAllServiceNo/GetBasicDetailByRequestId/GetRequestHistory/GetRegimentalListByArmedId/GetROListByArmedId/GetRemarks/CreateCSV/GetICardPrintPreviewByRequestId/GetBDetailByRequestId/GetTopArmyNoFromICardRequest/ICardRequestHold/GetAllICardRequestHold

        public async Task<IActionResult> CheckArmyNO(string ArmyNo)
        {
            try
            {
                if (!ArmyNo.IsNullOrEmpty())
                {
                    return Json(await basicDetailBL.CheckArmyNO(ArmyNo));
                }
                else
                {
                    return Json(false);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->CheckArmyNO");
                return Json(false);
            }
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

        [HttpPost]
        public async Task<IActionResult> GetData(string ICNumber, byte lCardType)
        {
            #region Old Code
            //DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            //if (ICNumber != null)
            //{
            //    BasicDetail? basicDetail = await basicDetailBL.FindServiceNo(ICNumber);
            //    if (basicDetail != null)
            //    {
            //        bool result = await iTrnICardRequestBL.GetRequestPending(basicDetail.BasicDetailId);
            //        if (result)
            //        {

            //            dTOApiDataResponse.Status = false;
            //            dTOApiDataResponse.Message = "Your I-Card is under process. Please wait.";
            //            return Ok(dTOApiDataResponse);
            //        }
            //        else
            //        {
            //            if (lCardType == 1)
            //            {
            //                dTOApiDataResponse.Message = "You didn't Select First time Smart card";
            //                dTOApiDataResponse.Status = false;
            //            }
            //            else
            //            {
            //                dTOApiDataResponse.Status = true;
            //            }

            //            return Ok(dTOApiDataResponse);
            //        }
            //    }
            //    else
            //    {
            //        if (lCardType == 1 || lCardType == 4)
            //        {
            //            dTOApiDataResponse.Status = true;
            //        }
            //        else
            //        {
            //            dTOApiDataResponse.Message = "Please Select First time Smart card";
            //            dTOApiDataResponse.Status = false;
            //        }
            //        return Ok(dTOApiDataResponse);
            //    }
            //}
            //else
            //{
            //    dTOApiDataResponse.Status = false;
            //    dTOApiDataResponse.Message = "Service no required.";
            //    return Ok(dTOApiDataResponse);
            //}
            #endregion
            DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            if (ICNumber != null)
            {
                int? BasicDetailId = await basicDetailBL.MaxBasicDetailId(ICNumber);
                if (BasicDetailId != null)
                {
                    bool result = await iTrnICardRequestBL.GetRequestPending((int)BasicDetailId);
                    if (result)
                    {
                        dTOApiDataResponse.Status = false;
                        dTOApiDataResponse.Message = "Your I-Card is under process. Please wait.";
                        return Ok(dTOApiDataResponse);
                    }
                    else
                    {
                        if (lCardType == 1)
                        {
                            dTOApiDataResponse.Message = "You didn't Select First time Smart card";
                            dTOApiDataResponse.Status = false;
                        }
                        else if (lCardType == 5)
                        {
                            bool check = await _lostCardBL.CheckServiceNoRequestInLost(ICNumber);
                            if (check)
                            {
                                dTOApiDataResponse.Status = true;
                            }
                            else
                            {
                                dTOApiDataResponse.Message = "First, report the loss and then place an I-Card request.";
                                dTOApiDataResponse.Status = false;
                            }
                        }
                        else
                        {
                            dTOApiDataResponse.Status = true;
                        }

                        return Ok(dTOApiDataResponse);
                    }
                }
                else
                {
                    if (lCardType == 1 || lCardType == 4)
                    {
                        dTOApiDataResponse.Status = true;
                    }
                    else
                    {
                        dTOApiDataResponse.Message = "Please Select First time Smart card";
                        dTOApiDataResponse.Status = false;
                    }
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

                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }
                dto.MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;

                if (dto.TypeId == KeyConstants.FaultyCardRequest)
                {
                    var user = await userManager.FindByIdAsync(dto.AspNetUsersId.ToString());

                    // UserManager service GetClaimsAsync method gets all the current claims of the user
                    var UserClaims = await userManager.GetClaimsAsync(user);
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dto.Claim = 1;
                    }
                    else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        dto.Claim = 2;
                    }
                    else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                    {
                        dto.Claim = 3;
                    }
                    else
                    {
                        dto.Claim = 0;
                    }
                }
                else
                {
                    dto.Claim = 0;
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
                                item.Image = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
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
                basicDetailCrtAndUpdVM.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);

                string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", basicDetailCrtAndUpdVM.SignatureImagePath);
                basicDetailCrtAndUpdVM.ExistingSignatureInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
                return Json(basicDetailCrtAndUpdVM);
            }
            else
            {
                return Json(null);
            }
        }

        public async Task<IActionResult> GetRequestHistory(int RequestId)
        {
            ICardHistoryResponseAll? cardHistoryResponses = new ICardHistoryResponseAll();
            var cardStatus = await basicDetailBL.CheckCardStatus(RequestId);
            if (cardStatus.GetValueOrDefault() == 1)
            {
                cardHistoryResponses = await basicDetailBL.ICardHistory(RequestId);
            }
            else if (cardStatus.GetValueOrDefault() == 2)
            {
                cardHistoryResponses = await basicDetailBL.ICardHistoryCompleted(RequestId);
            }
            return Json(cardHistoryResponses);
        }

        public async Task<IActionResult> GetCardMovementHistory(int RequestId)
        {
            return Json(await basicDetailBL.GetCardMovementHistory(RequestId));
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
                basicDetailCrtAndUpdVM.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);

                string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", basicDetailCrtAndUpdVM.SignatureImagePath);
                basicDetailCrtAndUpdVM.ExistingSignatureInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);

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
        public async Task<IActionResult> GetAllICardRequestHold(DTODataTablesRequest dTO)
        {
            try
            {
                return Json(await basicDetailBL.GetAllICardRequestHold(dTO));
            }
            catch (Exception ex)
            {
                List<DTOICardRequestHoldResponse> dTODispatchCardLists = new List<DTOICardRequestHoldResponse>();
                var responseData = new DTODataTablesResponse<DTOICardRequestHoldResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTODispatchCardLists
                };
                _logger.LogError(1001, ex, "BasicDetail->GetAllICardRequestHold");
                return Json(responseData);
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
        public static DirectoryInfo ForCreateFolderrandom(string baseFolder, string DoaminId)
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
                           Path.Combine(dayName + "" + monthName + "" + yearName + "_" + hh + "" + mm + "" + ss + "_" + DoaminId));

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

        //[Authorize(Policy = "ViewFlaggedICardApplPolicy")]
        //[HttpPost]
        //public async Task<IActionResult> GetAllICardDistribution()
        //{
        //    try
        //    {
        //        return Json(await basicDetailBL.GetAllICardRequestHold());
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(1001, ex, "BasicDetail->GetAllICardRequestHold");
        //        return Json(KeyConstants.InternalServerError);
        //    }
        //}
        #endregion Card Distribution

        #region ICard Printing
        [HttpPost]
        public async Task<IActionResult> ICardPrintUploadCsv(DTOCSVFileRequest model)
        {
            var response = new DTOCsvUploadValResponse();
            //if (model.CSVFile == null || model.CSVFile.Length == 0)
            //{
            //    response.Message = "File is not uploaded or is empty.";
            //}

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                            .SelectMany(x => x.Value!.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();
                if (errors.Any())
                {
                    response.Message = string.Join("; ", errors); // Concatenate all error messages
                }
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

        #region DestructionCard
        public async Task<ViewResult> DestructionCardAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllDestruction(DTODataTablesRequest dTO)
        {
            return Json(await _destructionCardBL.GetAllDestruction(dTO));
        }

        [HttpPost]
        public async Task<IActionResult> DestructionDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");
                var records = await _destructionCardBL.GetDetailsByRequestIds(req);
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTODestructionCardExportResponse>(true, CsvClassMapTypeEnum.HotlistExport));
                    try
                    {
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->DestructionDataExport");
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
                _logger.LogError(1001, ex, "BasicDetail->DestructionDataExport");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }
        ReturnSt:
            return Json(dTOFaulty);
        }
        public async Task<ActionResult> DestructionCardRequestAsync()
        {
            return View();
        }

        public async Task<IActionResult> SaveDestructionCardRequest(TrnDestructionCard model)
        {
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }
                model.IsActive = true;
                model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
                model.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    bool checkduplicate = await _destructionCardBL.FindAnyRequestId(model.RequestId);
                    if (checkduplicate)
                    {
                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "The destruction request already exists!";
                    }
                    else
                    {
                        var result = await _destructionCardBL.AddWithReturn(model);
                        dTOFaulty.Result = true;
                        dTOFaulty.Message = "Record created!";
                        dTOFaulty.CurrentTime = result.UpdatedOn.GetValueOrDefault();
                        dTOFaulty.Id = result.DestructedCardId.ToString();
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
                _logger.LogError(1001, ex, "BasicDetail->SaveDestructionCardRequest");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

            return Json(dTOFaulty);
        }
        #endregion DestructionCard

        #region Dispatch
        public async Task<IActionResult> GetUserIdWithName(int AspNetUsersId)
        {
            DTOGenericResponse<DTODispatchToResponse?> response = new DTOGenericResponse<DTODispatchToResponse?>();

            response = await basicDetailBL.GetUserIdWithName(AspNetUsersId);
            return Ok(response);
        }

        public async Task<IActionResult> GetDispatchToData(byte CategeryId, byte RecordRegimentId)
        {
            DTOGenericResponse<DTODispatchToResponse?> response = new DTOGenericResponse<DTODispatchToResponse?>();

            response = await basicDetailBL.GetDispatchToData(CategeryId, RecordRegimentId);
            return Ok(response);
        }

        public async Task<IActionResult> GetddlRecordRegiment(int ToUnitId)
        {
            DtoSession? dtoSession = new DtoSession();
            DTOGenericResponse<DTOOROWithRegimentAndUnitResponse> response = new DTOGenericResponse<DTOOROWithRegimentAndUnitResponse>();
            DTOOROWithRegimentAndUnitResponse ret = new DTOOROWithRegimentAndUnitResponse();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            if (dtoSession != null)
            {
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
                byte ClaimValue;

                // UserManager service GetClaimsAsync method gets all the current claims of the user
                var UserClaims = await userManager.GetClaimsAsync(user);
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                {
                    ClaimValue = 2;
                    response = await basicDetailBL.GetddlRecordRegiment(ClaimValue, dtoSession.TrnDomainMappingId, dtoSession.UnitId, ToUnitId);
                    return Ok(response);
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                {
                    ClaimValue = 3;
                    response = await basicDetailBL.GetddlRecordRegiment(ClaimValue, dtoSession.TrnDomainMappingId, dtoSession.UnitId, ToUnitId);
                    return Ok(response);
                }
                else
                {
                    response.Result = false;
                    response.Message = "An error occurred while fetching data.";
                    response.Value = ret;
                    return Ok(response);
                }
            }
            else
            {
                response.Result = false;
                response.Message = "An error occurred while fetching data.";
                response.Value = ret;
                return Ok(response);
            }
        }

        public async Task<ActionResult> DispatchOut()
        {
            DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 = SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");
            if (dTOTempSession1 != null)
            {
                ViewBag.SearchField = dTOTempSession1.SearchField;
                ViewBag.SearchText = dTOTempSession1.SearchText;

                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                // UserManager service GetClaimsAsync method gets all the current claims of the user
                var UserClaims = await userManager.GetClaimsAsync(user);
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                {
                    ViewBag.ClaimValue = 1;
                    return View();
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                {
                    ViewBag.ClaimValue = 2;
                    return View();
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                {
                    ViewBag.ClaimValue = 3;
                    return View();
                }
                else
                {
                    TempData["error"] = "Invalid User.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            else
            {
                TempData["error"] = "Invalid Session.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<ActionResult> DispatchOut([FromForm] DTODispatchOutRequest dTO)
        {
            #region Old Code
            //DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 = SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");
            //if (dTOTempSession1 != null)
            //{
            //    DtoSession? dtoSession = new DtoSession();
            //    DTOGenericResponse<DTOCardDispatchCheckResponse> response = new DTOGenericResponse<DTOCardDispatchCheckResponse>();
            //    DTOCardDispatchCheckResponse ret = new DTOCardDispatchCheckResponse();
            //    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            //    {
            //        dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            //    }

            //    if (dtoSession != null)
            //    {
            //        int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            //        var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
            //        byte ClaimValue = 0;

            //        dTO.OutDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            //        dTO.FromAspNetUsersId = AspNetUsersId;
            //        dTO.FromUserId = dtoSession.UserId;
            //        dTO.FromUnitId = dtoSession.UnitId;
            //        dTO.IsActive = true;
            //        dTO.IsComplete = false;
            //        dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            //        dTO.Updatedby = AspNetUsersId;

            //        if (ModelState.IsValid)
            //        {
            //            // UserManager service GetClaimsAsync method gets all the current claims of the user
            //            var UserClaims = await userManager.GetClaimsAsync(user);
            //            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            //            {
            //                dTO.Step = 1;
            //                ClaimValue = 1;
            //            }
            //            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
            //            {
            //                dTO.Step = 2;
            //                ClaimValue = 2;
            //            }
            //            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
            //            {
            //                dTO.Step = 2;
            //                ClaimValue = 3;
            //            }
            //            else
            //            {
            //                response.Result = false;
            //                response.Message = "Unauthorized User.";
            //                response.Value = ret;
            //                return Ok(response);
            //            }



            //            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            //            try
            //            {
            //                var records = new List<DTOCardDispatchCheckRequest>();
            //                using (var reader = new StreamReader(dTO.CSVFile.OpenReadStream()))
            //                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            //                {
            //                    csv.Context.RegisterClassMap(new CsvClassMap<DTOCardDispatchCheckRequest>(true, CsvClassMapTypeEnum.DispatchCard));
            //                    try
            //                    {
            //                        records = csv.GetRecords<DTOCardDispatchCheckRequest>().ToList();
            //                    }
            //                    catch (Exception ee)
            //                    {
            //                        _logger.LogError(1001, ee, "BasicDetail->DispatchOut");
            //                        response.Result = false;
            //                        response.Message = "Internal Server Error!";
            //                        goto Returnstm;
            //                    }
            //                }

            //                #region Upload File Without Remarks
            //                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CardDispatchCSVs", "CSVWithoutRemarks");
            //                if (!Directory.Exists(uploadsFolder))
            //                {
            //                    Directory.CreateDirectory(uploadsFolder);
            //                }
            //                var filePath = Path.Combine(uploadsFolder, fileName);

            //                using (var stream = new FileStream(filePath, FileMode.Create))
            //                {
            //                    await dTO.CSVFile.CopyToAsync(stream);
            //                }
            //                #endregion Upload User File

            //                var validateResult = await basicDetailBL.ValidateCardDispatchData(records, ClaimValue, dTO);

            //                ret.TotalRecords = validateResult.Count();
            //                ret.ValidRecords = validateResult.Where(x => x.IsValid).Count();
            //                ret.SheetInValidRecords = validateResult.Where(x => x.Status == "SheetInValid").Count();
            //                ret.DbInValidRecords = validateResult.Where(x => x.Status == "DbInvalid").Count();

            //                response.Result = true;
            //                response.Value = ret;

            //                #region Upload File With Remarks
            //                uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CardDispatchCSVs", "CSVWithRemarks");
            //                if (!Directory.Exists(uploadsFolder))
            //                {
            //                    Directory.CreateDirectory(uploadsFolder);
            //                }
            //                filePath = Path.Combine(uploadsFolder, fileName);

            //                using (var stream = new FileStream(filePath, FileMode.Create))
            //                {
            //                    await dTO.CSVFile.CopyToAsync(stream);
            //                }
            //                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            //                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //                {
            //                    csv.Context.RegisterClassMap(new CsvClassMap<DTOCardDispatchCheckRequest>(false));
            //                    try
            //                    {
            //                        csv.WriteRecords(validateResult);
            //                    }
            //                    catch (Exception ee)
            //                    {
            //                        _logger.LogError(1001, ee, "BasicDetail->DispatchOut");
            //                        response.Result = false;
            //                        response.Message = "Internal Server Error!";
            //                        response.Value = ret;
            //                        goto Returnstm;
            //                    }
            //                }
            //                #endregion Upload User File
            //                dTO.UploadFilePath = fileName;
            //                DTODispatchOutRequestWithoutIFormFile dTODispatch = new DTODispatchOutRequestWithoutIFormFile
            //                {
            //                    DispatchCardId = dTO.DispatchCardId,
            //                    Step = dTO.Step,
            //                    ApplyForId = dTO.ApplyForId,
            //                    RegId = dTO.RegId,
            //                    RecordOfficeId = dTO.RecordOfficeId,
            //                    OutDate = dTO.OutDate,
            //                    ReceiptDate = dTO.ReceiptDate,
            //                    DispatchDate = dTO.DispatchDate,
            //                    DispatchModeId = dTO.DispatchModeId,
            //                    RefOfDispatch = dTO.RefOfDispatch,
            //                    NameOfCourierIncharge = dTO.NameOfCourierIncharge,
            //                    UploadFilePath = dTO.UploadFilePath,
            //                    FromRemark = dTO.FromRemark,
            //                    ToRemark = dTO.ToRemark,
            //                    FromUnitId = dTO.FromUnitId,
            //                    ToUnitId = dTO.ToUnitId,
            //                    ToUserId = dTO.ToUserId,
            //                    FromUserId = dTO.FromUserId,
            //                    FromAspNetUsersId = dTO.FromAspNetUsersId,
            //                    ToAspNetUsersId = dTO.ToAspNetUsersId,
            //                    IsComplete = dTO.IsComplete,
            //                    IsActive = dTO.IsActive,
            //                    Updatedby = dTO.Updatedby,
            //                    UpdatedOn = dTO.UpdatedOn
            //                };

            //                SessionHeplers.SetObject(HttpContext.Session, "DestructionCardData", dTODispatch);
            //                SessionHeplers.SetObject(HttpContext.Session, "ValidDispatchCardRecordsUpload", validateResult.Where(v => v.IsValid == true).ToList());
            //                ret.FileName = fileName;
            //            }
            //            catch (Exception ex)
            //            {
            //                _logger.LogError(1001, ex, "BasicDetail->DispatchOut");
            //                response.Message = "Internal Server Error!";
            //            }
            //        Returnstm:
            //            return Json(response);

            //        }
            //        else
            //        {
            //            //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
            //            var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
            //            .SelectMany(x => x.Value!.Errors)
            //            .Select(e => e.ErrorMessage)
            //            .ToList();
            //            if (errors.Any())
            //            {
            //                response.Message = string.Join("; ", errors); // Concatenate all error messages
            //            }
            //            response.Result = false;
            //            return Json(response);
            //        }
            //    }
            //    else
            //    {
            //        response.Result = false;
            //        response.Message = "An error occurred while fetching data.";
            //        response.Value = ret;
            //        return Ok(response);
            //    }
            //}
            //else
            //{
            //    TempData["error"] = "Invalid Session / Session Timeout.";
            //    TempData.Keep("error");
            //    return RedirectToAction("ContactUs", "Home");
            //}
            #endregion
            DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 = SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");
            if (dTOTempSession1 != null)
            {
                DtoSession? dtoSession = new DtoSession();
                DTOGenericResponse<DTOCardDispatchCheckResponse> response = new DTOGenericResponse<DTOCardDispatchCheckResponse>();
                DTOCardDispatchCheckResponse ret = new DTOCardDispatchCheckResponse();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }

                if (dtoSession != null)
                {
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
                    byte ClaimValue = 0;

                    dTO.OutDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    dTO.FromAspNetUsersId = AspNetUsersId;
                    dTO.FromUserId = dtoSession.UserId;
                    dTO.FromUnitId = dtoSession.UnitId;
                    dTO.IsActive = true;
                    dTO.IsComplete = false;
                    dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    dTO.Updatedby = AspNetUsersId;

                    if (ModelState.IsValid)
                    {
                        // UserManager service GetClaimsAsync method gets all the current claims of the user
                        var UserClaims = await userManager.GetClaimsAsync(user);
                        if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                        {
                            dTO.Step = 1;
                            ClaimValue = 1;
                        }
                        else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                        {
                            dTO.Step = 2;
                            ClaimValue = 2;
                        }
                        else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                        {
                            dTO.Step = 2;
                            ClaimValue = 3;
                        }
                        else
                        {
                            response.Result = false;
                            response.Message = "Unauthorized User.";
                            response.Value = ret;
                            return Ok(response);
                        }



                        string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
                        try
                        {
                            var records = new List<DTOCardDispatchCheckRequest>();

                            var validateResult = await basicDetailBL.ValidateCardDispatchData(dTOTempSession1.RequestIds, ClaimValue, dTO);

                            ret.TotalRecords = validateResult.Count();
                            ret.ValidRecords = validateResult.Where(x => x.IsValid).Count();
                            ret.DbInValidRecords = validateResult.Where(x => x.Status == "DbInvalid").Count();

                            response.Result = true;
                            response.Value = ret;

                            if (ret.ValidRecords > 0)
                            {
                                #region Upload File With Remarks
                                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CardDispatchCSVs", "CSVWithRemarks");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                                {
                                    csv.Context.RegisterClassMap(new CsvClassMap<DTOCardDispatchCheckRequest>(false));
                                    try
                                    {
                                        csv.WriteRecords(validateResult);
                                    }
                                    catch (Exception ee)
                                    {
                                        _logger.LogError(1001, ee, "BasicDetail->DispatchOut");
                                        response.Result = false;
                                        response.Message = "Internal Server Error!";
                                        response.Value = ret;
                                        goto Returnstm;
                                    }
                                }
                                #endregion Upload User File
                                dTO.UploadFilePath = fileName;
                                ret.FileName= fileName;
                                DTOGenericResponse<string> response1 = new DTOGenericResponse<string>();
                                var ValidRecords = validateResult.Where(x => x.IsValid).ToList();
                                response1 = await basicDetailBL.CardDispatchCSVUpload(ValidRecords, dTO);
                                if (response1.Result == true)
                                {
                                    ret.LotNo = Convert.ToInt32(response1.Message);
                                    response.Result = response1.Result;
                                    response.Message = response1.Message;
                                    response.Value = ret;
                                    HttpContext.Session.Remove("DispatchLot");
                                    return Ok(response);
                                }
                                else
                                {
                                    response.Result = false;
                                    response.Message = response1.Message;
                                    response.Value = ret;
                                    return Ok(response);
                                }
                            }
                            else
                            {
                                response.Result = false;
                                response.Message = "There are no valid records!";
                                response.Value = ret;
                                return Ok(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(1001, ex, "BasicDetail->DispatchOut");
                            response.Message = "Internal Server Error!";
                        }
                    Returnstm:
                        return Json(response);

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
                            response.Message = string.Join("; ", errors); // Concatenate all error messages
                        }
                        response.Result = false;
                        return Json(response);
                    }
                }
                else
                {
                    response.Result = false;
                    response.Message = "Session Timeout.";
                    response.Value = ret;
                    return Ok(response);
                }
            }
            else
            {
                TempData["error"] = "Invalid Session / Session Timeout.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        public async Task<IActionResult> DispatchCard()
        {
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                ViewBag.ClaimValue = 1;
                return View();
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
            {
                ViewBag.ClaimValue = 2;
                return View();
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
            {
                ViewBag.ClaimValue = 3;
                return View();
            }
            else
            {
                ViewBag.ClaimValue = 0;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetAllDispatchCard(DTODataTablesRequestForCardDispatch dTO)
        {
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }

                if (dtoSession != null)
                {
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                    // UserManager service GetClaimsAsync method gets all the current claims of the user
                    var UserClaims = await userManager.GetClaimsAsync(user);
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dTO.ClaimValue = 1;
                        dTO.UnitId = dtoSession.UnitId;
                        dTO.TDMId = dtoSession.TrnDomainMappingId;
                        return Json(await basicDetailBL.GetAllDispatchCard(dTO));
                    }
                    else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        dTO.ClaimValue = 2;
                        dTO.UnitId = dtoSession.UnitId;
                        dTO.TDMId = dtoSession.TrnDomainMappingId;
                        return Json(await basicDetailBL.GetAllDispatchCard(dTO));
                    }
                    else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                    {
                        dTO.ClaimValue = 3;
                        dTO.UnitId = dtoSession.UnitId;
                        dTO.TDMId = dtoSession.TrnDomainMappingId;
                        return Json(await basicDetailBL.GetAllDispatchCard(dTO));
                    }
                    else
                    {
                        dTO.ClaimValue = 0;
                        dTO.UnitId = dtoSession.UnitId;
                        dTO.TDMId = dtoSession.TrnDomainMappingId;
                        return Json(await basicDetailBL.GetAllDispatchCard(dTO));
                    }
                }
                else
                {
                    List<DTODispatchCardListResponse> dTODispatchCardLists = new List<DTODispatchCardListResponse>();
                    var responseData = new DTODataTablesResponse<DTODispatchCardListResponse>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTODispatchCardLists
                    };
                    return Json(responseData);
                }
            }
            catch (Exception ex)
            {
                List<DTODispatchCardListResponse> dTODispatchCardLists = new List<DTODispatchCardListResponse>();
                var responseData = new DTODataTablesResponse<DTODispatchCardListResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTODispatchCardLists
                };
                _logger.LogError(1001, ex, "BasicDetail->GetAllDispatchCard");
                return Json(responseData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDispatchCardDataForDialog(DTODataTablesRequestForCardDispatchDialog dTO)
        {
            try
            {
                return Json(await basicDetailBL.GetDispatchCardDataForDialog(dTO));
            }
            catch (Exception ex)
            {
                List<DTOCardDispatchDialogResponse> dTOCards = new List<DTOCardDispatchDialogResponse>();
                var responseData = new DTODataTablesResponse<DTOCardDispatchDialogResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOCards
                };
                _logger.LogError(1001, ex, "BasicDetail->GetDispatchCardDataForDialog");
                return Json(responseData);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DispatchCardIn([FromForm] DTODispatchInRequest dTO)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();

            dTO.ReceiptDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            dTO.IsComplete = true;
            if (ModelState.IsValid)
            {
                try
                {
                    TrnDispatchCard? trnDispatchCard = await dispatchCardBL.Get(dTO.DispatchCardId);
                    if (trnDispatchCard != null)
                    {
                        if (trnDispatchCard.IsComplete == true && trnDispatchCard.ReceiptDate != null)
                        {
                            response.Result = false;
                            response.Message = "Action has already been taken by you.";
                            response.Value = string.Empty;
                        }
                        else
                        {
                            byte StepId = 0;
                            if (trnDispatchCard.Step == 1)
                            {
                                StepId = 12;
                            }
                            else if (trnDispatchCard.Step == 2)
                            {
                                StepId = 14;
                            }
                            List<DTODispatchCardInRequest> dTODispatchCards = new List<DTODispatchCardInRequest>();
                            dTODispatchCards.AddRange(await dispatchCardMappingBL.GetRequestIds(trnDispatchCard.DispatchCardId));
                            response = await basicDetailBL.DispatchCardIn(dTODispatchCards, StepId, dTO.DispatchCardId, dTO.ToRemark);
                            response.Value = string.Empty;
                        }
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "Invalid Id";
                        response.Value = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, "BasicDetail->DispatchCardIn");
                    response.Message = "Internal Server Error!";
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
                    response.Message = string.Join("; ", errors); // Concatenate all error messages
                }
                response.Result = false;
                response.Value = string.Empty;
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetDispatchCardStatusListForDialog([FromBody] DTODataTablesRequestForCardStatusList dTO)
        {
            try
            {
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
                // UserManager service GetClaimsAsync method gets all the current claims of the user
                var UserClaims = await userManager.GetClaimsAsync(user);
                byte ClaimValue;
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                {
                    ClaimValue = 1;
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                {
                    ClaimValue = 2;
                }
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                {
                    ClaimValue = 3;
                }
                else
                {
                    ClaimValue = 0;
                }
                return Json(await basicDetailBL.GetDispatchCardStatusListForDialog(dTO, ClaimValue));
            }
            catch (Exception ex)
            {
                List<DTODispatchCardStatusResponse> dTOCards = new List<DTODispatchCardStatusResponse>();
                var responseData = new DTODataTablesForDispatchCardStatusListResponse<DTODispatchCardStatusResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOCards,
                    selectedIds = null
                };
                _logger.LogError(1001, ex, "BasicDetail->GetDispatchCardDataForDialog");
                return Json(responseData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportCsvFileForDispatchCard([FromBody] DTORequestIdForCSVRequest requestIdsWrapper)
        {
            List<DTODispatchCardForCSVResponse> dTOs = new List<DTODispatchCardForCSVResponse>();
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            int[]? RequestIds = requestIdsWrapper.RequestIds;

            if (RequestIds == null || RequestIds.Length == 0)
            {
                response.Result = false;
                response.Message = "No request IDs provided.";
                response.Value = string.Empty;
                return Json(response);
            }

            string fileName = $"CSVForDispatch_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "DispatchExports", "Temp");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, fileName);
            try
            {
                dTOs = await basicDetailBL.ExportCsvFileForDispatchCard(RequestIds);
                
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap(new CsvClassMap<DTODispatchCardForCSVResponse>(true, CsvClassMapTypeEnum.CSVExport));
                    await csv.WriteRecordsAsync(dTOs);
                }
                response.Result = true;
                response.Message = "Ok";
                response.Value = Path.GetFileName(fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->ExportCsvFileForDispatchCard");
                response.Result = false;
                response.Message = "Internal Server Error!";
                response.Value =string.Empty;
            }
            return Json(response);  
        }
        [HttpPost]
        public IActionResult BeforeProceedToDispatchCheck([FromBody] DTOBeforeProceedToDispatchCheckRequest dTO)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            try
            {
                if(ModelState.IsValid)
                {
                    DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 = SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");
                    if (dTOTempSession1 != null)
                    {
                        HttpContext.Session.Remove("DispatchLot");
                    }
                    SessionHeplers.SetObject(HttpContext.Session, "DispatchLot", dTO);
                    response.Result = true;
                    response.Message = "Ok";
                    response.Value = string.Empty;
                }
                else
                {
                    response.Result = false;
                    response.Message = "No request IDs provided.";
                    response.Value = string.Empty;
                    return Json(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->BeforeProceedToDispatchCheck");
                response.Result = false;
                response.Message = "Internal Server Error!";
                response.Value = string.Empty;
            }
            return Json(response);
        }
        [HttpPost]
        public async Task<IActionResult> ExportCsvForDispatch(DTOExportDispatch Data )
        {
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var UserClaims = await userManager.GetClaimsAsync(user);
            byte ClaimValue;
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                ClaimValue = 1;
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
            {
                ClaimValue = 2;
            }
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
            {
                ClaimValue = 3;
            }
            else
            {
                ClaimValue = 0;
            }
            string fileName = $"UploadForDispatch_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            var ret = await basicDetailBL.GetDispatchCardStatusListForExport(ClaimValue, Data);
            var csv = new StringBuilder();

            // Header
            csv.AppendLine("Name,ServiceNo,ChipNo,ApplId");

            // Data rows
            foreach (var item in ret.data)
            {
                csv.AppendLine($"{item.RankName +" "+ item.NameAsPerRecord},\"{item.ServiceNo}\",\"{item.ChipNo}\",\"{item.RequestId}\"");
            }

            // Convert to byte array
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
           
           // var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Dispatchexports");
            var folderPath = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Dispatchexports");
            // Create folder if not exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);

            // Write the bytes to file
            System.IO.File.WriteAllBytes(fullPath, bytes);
            return Json(fileName);

            
            
        }
        #endregion

        #region Set Session and Get Session
        /// <summary>
        /// This method is responsible for setting the session data with the provided DTO.
        /// It receives a `DTOEncypteDecryptedColumnRequest` object, and if the data is successfully set, it returns `true`. 
        /// Otherwise, it returns `false` if there is any error.
        /// </summary>
        /// <param name="Data">The `DTOEncypteDecryptedColumnRequest` object containing the data to be set in the session.</param>
        /// <returns>
        /// Returns `true` if the session data was successfully set, otherwise `false` in case of an error.
        /// </returns>

        [HttpPost]
        public ActionResult DataSendForSetSession([FromBody] DTOEncypteDecryptedColumnRequest Data)
        {
            try
            {
                SessionHeplers.SetObject(HttpContext.Session, "DataSet", Data);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->DataSendForSetSession");
                return Json(false);
            }
        }
        /// <summary>
        /// This method retrieves session data from the current HTTP session. It attempts to fetch a `DTOEncypteDecryptedColumnRequest` 
        /// object from the session with the key "DataSet". If the data is found, it returns the data along with a success message. 
        /// If the session data is not found or an error occurs, it returns a failure message and an empty `DTOEncypteDecryptedColumnRequest` object.
        /// </summary>
        /// <returns>
        /// Returns a `DTOGenericResponse<DTOEncypteDecryptedColumnRequest>` containing the result of the session fetch. 
        /// - `Result` will be `true` if the session data was found and successfully fetched.
        /// - `Message` provides the status message, either success or error details.
        /// - `Value` contains the retrieved session data or an empty object if not found.
        /// </returns>
        [HttpPost]
        public ActionResult DataRecForGetSession()
        {
            var response = new DTOGenericResponse<DTOEncypteDecryptedColumnRequest>();
            try
            {
                var dTOEncypte = SessionHeplers.GetObject<DTOEncypteDecryptedColumnRequest>(HttpContext.Session, "DataSet");
                if (dTOEncypte != null)
                {
                    response.Result = true;
                    response.Message = "Fetch Value";
                    response.Value = dTOEncypte;
                }
                else
                {
                    response.Result = false;
                    response.Message = "Session not found.";
                    response.Value = new DTOEncypteDecryptedColumnRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->DataRecForGetSession");
                response.Result = false;
                response.Message = "Session not found.";
                response.Value = new DTOEncypteDecryptedColumnRequest();
            }
            return Json(response);
        }
        #endregion
    }
}