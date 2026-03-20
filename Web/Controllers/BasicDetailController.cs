using AutoMapper;
using BusinessLogicsLayer;
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
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.HotlistCard;
using BusinessLogicsLayer.LostCard;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.OROMapp;
using BusinessLogicsLayer.RecordOffice;
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
using System.Data;
using System.Diagnostics.Metrics;
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
        private readonly IOROMappingBL oROMappingBL;// For ORO Mapping
        private readonly IRegimentalBL regimentalBL;// For Regimental Database
        private readonly IRecordOfficeBL recordOfficeBL; // For Record Office
        public const string SessionKeySalt = "_Salt";

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
           , ITrnLoginLogBL iTrnLoginLogBL, IICardHoldBL iICardHoldBL, IcsvImportBl iCSVImportBL, IFaultyCardBL _faultyCardBL, IHotlistCardBL hotlistCardBL, ILostCardBL lostCardBL, IDistributeCardBL distributeCardBL,
           IDestructionCardBL destructionCardBL, IDispatchCardBL dispatchCardBL, IDispatchCardMappingBL dispatchCardMappingBL, IImageEncryptAndDecrypt imageEncryptAndDecrypt, IEncryptionSettingBL encryptionSettingBL,
           IOROMappingBL oROMappingBL, IRegimentalBL regimentalBL, IRecordOfficeBL recordOfficeBL)
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
            this.imageEncryptAndDecrypt = imageEncryptAndDecrypt;
            this.encryptionSettingBL = encryptionSettingBL;
            this.oROMappingBL = oROMappingBL;
            this.regimentalBL = regimentalBL;
            this.recordOfficeBL = recordOfficeBL;
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
        [HttpGet]
        public async Task<ActionResult> Index(string Id, string jcoor)
        {
            string role = SessionHelper.GetRoleFromSession(HttpContext);
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
                    stepcounter = 1;
                    break;

                case 11: // Request from Task Board → maps to Dashboard (1)
                    stepcounter = 1;
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

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
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
        [HttpGet]
        public async Task<ActionResult> ApprovalForIO(string Id, string jcoor)
        {
            // Fetch current role from session and store in ViewBag
            string role = SessionHelper.GetRoleFromSession(HttpContext);
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

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
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
            string role = SessionHelper.GetRoleFromSession(HttpContext);

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
                    short ArmedIdForORO = Convert.ToInt16(Environment.GetEnvironmentVariable("HardCodeId__ArmedIdForORO"));

                    // If not set, fallback could be hardcoded (commented-out sample code)

                    // Load application forward condition settings from configuration

                    DTOApplFwdConditionRequest dTOApplFwdCondition;

                    // Retrieve the encryption key record from the database
                    var keyRecord = await encryptionSettingBL.Get(1);
                    if (keyRecord != null)
                    {
                        if (!string.IsNullOrWhiteSpace(keyRecord.ApplFwdCondition))
                        {
                            dTOApplFwdCondition = !string.IsNullOrWhiteSpace(keyRecord.ApplFwdCondition)
                                ? JsonConvert.DeserializeObject<DTOApplFwdConditionRequest>(keyRecord.ApplFwdCondition) ?? new DTOApplFwdConditionRequest()
                                : new DTOApplFwdConditionRequest();
                        }
                        else
                        {
                            dTOApplFwdCondition = new DTOApplFwdConditionRequest();
                        }
                    }
                    else
                    {
                        // Throw exception if encryption keys are not found
                        throw new InvalidOperationException("Encryption key record not found.");
                    }

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


                        if (role == "user")
                        {
                            return View(allrecord);
                        }
                        else
                        {
                            TempData["error"] = "Switch to user role.";
                            TempData.Keep("error");
                            return RedirectToAction("ContactUs", "Home");
                        }
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
            string role = SessionHelper.GetRoleFromSession(HttpContext);
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
                    if (role == "user")
                    {
                        return View(dTOBasicDetail);
                    }
                    else
                    {
                        TempData["error"] = "Switch to user role.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
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


            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                // Render the view with the retrieved list of I-Card types
                return View(allrecord);
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
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
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                string? dd = HttpContext.Session.GetString(SessionKeySalt); // Get Salt from Session
                if (dd != null)
                {
                    ViewBag.hdns = dd;
                    return View();
                }
                else
                {
                    TempData["error"] = "Session expired. Please try again.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }


            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
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
        public async Task<IActionResult> Registration(string EncryptedData, DTORegistrationRequest model)
        {
            
         try
            {
                
                // Extract userId from claims and assign as UpdatedBy
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.Updatedby = Convert.ToInt32(userId);

                string? dd = HttpContext.Session.GetString(SessionKeySalt); // Get Salt from Session
                if (dd != null)
                {
                    ViewBag.hdns = dd;
                }
                else
                {
                    TempData["error"] = "Session expired. Please try again.";
                    TempData.Keep("error");
                    goto end;
                }
                model =await AESEncrytDecry.DecryptAESWithDTO<DTORegistrationRequest>(EncryptedData,SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);

                if (model == null)
                {
                    TempData["error"] = "Invalid data.";
                    goto end;
                }
                
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
                                    else if (string.IsNullOrEmpty(OldFirstTwo))
                                    {
                                        // Handle OR rank cases
                                        if (string.IsNullOrEmpty(NewFirstTwo))
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
                                    else if (!string.IsNullOrEmpty(OldFirstTwo))
                                    {
                                        // Validation rules for IC, SL, SS, WC, TA, JC
                                        if (OldFirstTwo == "IC" && string.IsNullOrEmpty(NewFirstTwo))
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
                                        else if ((OldFirstTwo == "SS" || OldFirstTwo == "WC") && model.ApplyForId == 2 && !string.IsNullOrEmpty(NewFirstTwo) && NewFirstTwo == "IC")
                                        {
                                            TempData["error"] = "Please Select Offrs tab.";
                                            goto end;
                                        }
                                        else if (OldFirstTwo == "JC" && model.ApplyForId == 2 && !string.IsNullOrEmpty(NewFirstTwo) && (NewFirstTwo == "SS" || NewFirstTwo == "SL" || NewFirstTwo == "WC" || NewFirstTwo == "TA"))
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

                        if (dd != null)
                        {
                            HttpContext.Session.Remove(SessionKeySalt);
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

            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            string? dd = HttpContext.Session.GetString(SessionKeySalt); // Get Salt from Session

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
                if (dd != null)
                {
                    ViewBag.hdns = dd;
                }
                else
                {
                    TempData["error"] = "Session expired. Please try again.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
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

                            AadhaarNo = string.IsNullOrWhiteSpace(model.AadhaarNo)
        ? ""
        : Convert.ToInt64(model.AadhaarNo).ToString("D12"),

                            ApplyForId = model.ApplyForId,
                            RegistrationId = model.RegistrationId,
                            TypeId = model.TypeId,

                            State = model.State,
                            District = model.District,
                            PS = model.PS,
                            PO = model.PO,
                            Tehsil = model.Tehsil,
                            Village = model.Village,

                            PinCode = model.PinCode ?? 0,

                            PermanentAddress =
        $"Village - {model.Village}, Post Office - {model.PO}, Tehsil - {model.Tehsil}, District - {model.District}, State - {model.State}, Pin Code - {(model.PinCode ?? 0)}"
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
                int CurrentAspNetUsersId = Convert.ToInt32(userId);
                DTOPreventBasicDetailEditResponse? dTOPreventBasicDetail = await basicDetailBL.GetPreventBasicDetailEdit(decryptedIntId);
                if (dTOPreventBasicDetail != null)
                {
                    if (dTOPreventBasicDetail.IsLock == false && dTOPreventBasicDetail.StatusId == 1 && dTOPreventBasicDetail.AspNetUsersId == CurrentAspNetUsersId)
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

                            if (role == "user")
                            {
                                // Render edit view with populated details
                                return View(basicDetailUpdVM);
                            }
                            else
                            {
                                TempData["error"] = "Switch to user role.";
                                TempData.Keep("error");
                                return RedirectToAction("ContactUs", "Home");
                            }

                        }
                        else
                        {
                            // Record not found → return 404 and NotFound view
                            Response.StatusCode = 404;
                            return View("BasicDetailNotFound", decryptedId.ToString());
                        }
                    }
                    else
                    {
                        if (dTOPreventBasicDetail.IsLock == true)
                        {
                            TempData["error"] = "Editing is not allowed at this time.";
                            TempData.Keep("error");
                        }
                        else if (dTOPreventBasicDetail.StatusId != 1)
                        {
                            TempData["error"] = "Application is not running.";
                            TempData.Keep("error");
                        }
                        else
                        {
                            TempData["error"] = "You are not authorized to edit this details.";
                            TempData.Keep("error");
                        }
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                else
                {
                    TempData["error"] = "Invalid Input.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
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
        public async Task<IActionResult> BasicDetail(string EncryptedData, BasicDetailCrtAndUpdVM model)
        {
            try
            {
                string? dd = HttpContext.Session.GetString(SessionKeySalt); // Get Salt from Session

                // Fetch current logged-in user ID from claims
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve application forward condition settings from configuration
                DTOApplFwdConditionRequest dTOApplFwdCondition;

                // Retrieve the encryption key record from the database
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    if (!string.IsNullOrWhiteSpace(keyRecord.ApplFwdCondition))
                    {
                        dTOApplFwdCondition = !string.IsNullOrWhiteSpace(keyRecord.ApplFwdCondition)
                            ? JsonConvert.DeserializeObject<DTOApplFwdConditionRequest>(keyRecord.ApplFwdCondition) ?? new DTOApplFwdConditionRequest()
                            : new DTOApplFwdConditionRequest();
                    }
                    else
                    {
                        dTOApplFwdCondition = new DTOApplFwdConditionRequest();
                    }
                }
                else
                {
                    // Throw exception if encryption keys are not found
                    throw new InvalidOperationException("Encryption key record not found.");
                }

                // Validate that essential configuration values are present before proceeding
                if (string.IsNullOrWhiteSpace(dTOApplFwdCondition.MPRSO.Name) || dTOApplFwdCondition.MPRSO.ArmedAbbreviation.Count == 0 ||
                            string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.Name) || string.IsNullOrWhiteSpace(dTOApplFwdCondition.MP6F.ArmyNoPrefix) ||
                            dTOApplFwdCondition.MP6A.RankOrderby == 0)
                {
                    return Json(KeyConstants.InternalServerError);
                }
                 var  modeldec = await AESEncrytDecry.DecryptAESWithDTO<BasicDetailCrtAndUpdVM>(EncryptedData, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);

                if(modeldec==null)
                {
                    ModelState.AddModelError("", "Invalid Data.");
                    goto end;
                }
                else
                {
                    IFormFile photoes = model.Photo_;
                    IFormFile Signture = model.Signature_;
                    model = modeldec;
                    model.Photo_ = photoes;
                    model.Signature_ = Signture;
                }
                // Case 1: Update existing BasicDetail (when BasicDetailId > 0)
                if (model.BasicDetailId > 0)
                {
                    int CurrentAspNetUsersId = Convert.ToInt32(userId);
                    DTOPreventBasicDetailEditResponse? dTOPreventBasicDetail = await basicDetailBL.GetPreventBasicDetailEdit(model.BasicDetailId);
                    if (dTOPreventBasicDetail != null)
                    {
                        if (dTOPreventBasicDetail.IsLock == false && dTOPreventBasicDetail.StatusId == 1 && dTOPreventBasicDetail.AspNetUsersId == CurrentAspNetUsersId)
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
                                newBasicDetail.IsLock= false;
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
                                    if (dd != null)
                                    {
                                        HttpContext.Session.Remove(SessionKeySalt);
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
                        else
                        {
                            if (dTOPreventBasicDetail.IsLock == true)
                            {
                                TempData["error"] = "Editing is not allowed at this time.";
                                TempData.Keep("error");
                            }
                            else if (dTOPreventBasicDetail.StatusId != 1)
                            {
                                TempData["error"] = "Application is not running.";
                                TempData.Keep("error");
                            }
                            else
                            {
                                TempData["error"] = "You are not authorized to edit this details.";
                                TempData.Keep("error");
                            }
                            return RedirectToAction("ContactUs", "Home");
                        }
                    }
                    else
                    {
                        TempData["error"] = "Invalid Input.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                else // Case 2: Create new BasicDetail (when BasicDetailId == 0)
                {
                    if (dd != null)
                    {
                        ViewBag.hdns = dd;
                    }
                    else
                    {
                        TempData["error"] = "Session expired. Please try again.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                    // Similar flow: validate, map VM to entity, handle file uploads (mandatory), create request & step counter, save via BL
                    model.Updatedby = Convert.ToInt32(userId);
                    model.StatusLevel = 0;

                    if (ModelState.IsValid)
                    {
                        BasicDetail newBasicDetail = _mapper.Map<BasicDetailCrtAndUpdVM, BasicDetail>(model);
                        newBasicDetail.IsLock= false;
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

                            if (dd != null)
                            {
                                HttpContext.Session.Remove(SessionKeySalt);
                            }

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
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

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

                if (role == "user")
                {
                    // Return view with decoded data
                    return Task.FromResult<ActionResult>(View());
                }
                else
                {
                    TempData["error"] = "Switch to user role.";
                    TempData.Keep("error");
                    return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home"));
                }


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

        #region UploadCsv/GetHeaderMap/GetCSVFileUploadsHistory

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
        [Authorize(Roles = "admin2")]
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
        [Authorize(Roles = "admin2")]
        public async Task<IActionResult> IcardRejecte(MTrnFwd data)
        {
            // Initialize the generic response object
            DTOGenericResponse<DTOApplicationRejecteResponse?> response = new DTOGenericResponse<DTOApplicationRejecteResponse?>();
            DTOApplicationRejecteResponse dTOApplication = new DTOApplicationRejecteResponse();
            try
            {
                DtoSession? dtoSession = new DtoSession();

                //Retrieve session data if available
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                    // Set values for the forward data object using session data and user information
                    data.FromUserId = dtoSession.UserId;
                    data.UnitId = dtoSession.UnitId;
                    data.FromAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    data.UpdatedOn = DateTime.Now;
                    data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    data.IsActive = true;
                    data.TypeId = Convert.ToByte(1); // Set the type to 1 for rejection
                    data.FwdStatusId = 3; // Set the status to 3 for rejection
                    data.IsComplete = false;

                    // Retrieve domain mapping using the request ID
                    TrnDomainMapping? Domain = new TrnDomainMapping();
                    Domain = await iDomainMapBL.GetByRequestId(data.RequestId);

                    if (Domain != null && Domain.UserId != null)
                    {
                        dTOApplication.ToAspNetUsersId = Domain.AspNetUsersId;

                        // Set the recipient user ID and AspNetUsersId for the rejection
                        data.ToAspNetUsersId = Domain.AspNetUsersId;
                        data.ToUserId = Domain.UserId.GetValueOrDefault();

                        bool result = await iTrnFwnBL.AddTrnFwdWithIsCompleteUpdate(data);

                        if (result)
                        {
                            // Process digital sign XML files for the rejection
                            int[] d = new int[1];
                            d[0] = data.RequestId;
                            var dataret = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(d);

                            if (dataret != null)
                            {
                                dataret.XmlFiles = ""; // Clear the XML files after processing

                                // Save the processed XML digital sign
                                await _iTrnLoginLogBL.XmlFileDigitalSign(dataret);
                            }

                            response.Result = true;
                            response.Message = "Reject Application successfully.";
                            response.Value = dTOApplication;
                            return Json(response);
                        }
                        else
                        {
                            response.Result = false;
                            response.Message = "Reject Application failed.";
                            response.Value = dTOApplication;
                            return Json(response);
                        }

                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "At this time, the application was not rejected because Profile is not mapped with domain Id."; 
                        response.Value = dTOApplication;
                        return Json(response);
                    }
                }
                else
                {
                    // Session has expired or not available, redirect to ContactUs with error
                    TempData["error"] = "Invalid Session.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions and return a bad request response
                _logger.LogError(1001, ex, "BasicDetails=>IcardRejecte.");
                response.Result = false;
                response.Message = "failed";
                response.Value = dTOApplication;
                return Json(response);
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
        [Authorize(Roles = "admin2")]
        public async Task<IActionResult> UpdateStepCounter(MStepCounter mStepCounter)
        {
            DTOBasicDetailsSaveResponse response = new DTOBasicDetailsSaveResponse();
            try
            {
                // If the Flag is 'R', perform additional checks
                if (mStepCounter.Flag == "R")
                {
                    // Retrieve domain mapping using the request ID
                    TrnDomainMapping? Domain = new TrnDomainMapping();
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
        
        [HttpPost]
        public async Task<IActionResult> ActionOnRequest(string  request)
        {
            DTOActionOnRequest dTOActionOn = await AESEncrytDecry.DecryptAESWithDTO<DTOActionOnRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            
            DTOGenericResponse<DTOActionOnRequestResponse?> response = new DTOGenericResponse<DTOActionOnRequestResponse?>();
            DTOActionOnRequestResponse dTOAction = new DTOActionOnRequestResponse();
            try
            {
                if(ModelState.IsValid)
                {
                    MTrnICardRequest? mTrnICard = await iTrnICardRequestBL.Get(dTOActionOn.RequestId);

                    if (mTrnICard != null && mTrnICard.StatusId == 1)
                    {
                        // Retrieve session data for unit ID
                        DtoSession dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                        int CurrentAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        if (dTOActionOn.Flag == "R")
                        {

                            DTORequestRejectDetailResponse? rejectDetailResponse = await iTrnFwnBL.RequestRejectDetail(dTOActionOn.RequestId);

                            if (rejectDetailResponse != null)
                            {

                                dTOAction.AspNetUsersId = rejectDetailResponse.ToAspNetUsersId;
                                dTOAction.BeforeAction_StepId = rejectDetailResponse.StepId;
                                dTOAction.ApplyForId = rejectDetailResponse.ApplyForId;

                                if (rejectDetailResponse.StepId == 1)
                                {
                                    response.Message = "At this stage, the application has not been rejected.";
                                    response.Value = dTOAction;
                                    response.Result = false;
                                    return Ok(response);
                                }
                                else if (rejectDetailResponse.StepId == 2 || rejectDetailResponse.StepId == 3 || rejectDetailResponse.StepId == 4)
                                {
                                    if ((rejectDetailResponse.FromAspNetUsersId != CurrentAspNetUsersId) || (rejectDetailResponse.FromUserId != dtoSession.UserId))
                                    {
                                        response.Message = "You are not authorized to reject this request.";
                                        response.Value = dTOAction;
                                        response.Result = false;
                                        return Ok(response);
                                    }

                                    if (rejectDetailResponse.ToUserId == 0)
                                    {
                                        response.Message = "Profile is not mapped with domain Id!";
                                        response.Value = dTOAction;
                                        response.Result = false;
                                        return Ok(response);
                                    }
                                }
                                else
                                {
                                    //Invalid Input
                                    response.Message = "Invalid Input";
                                    response.Value = dTOAction;
                                    response.Result = false;
                                    return Ok(response);
                                }

                                if (rejectDetailResponse.StepId == 2)
                                    dTOActionOn.StepId = 7;
                                else if (rejectDetailResponse.StepId == 3)
                                    dTOActionOn.StepId = 8;
                                else if (rejectDetailResponse.StepId == 4)
                                    dTOActionOn.StepId = 9;

                                dTOAction.AfterAction_StepId = dTOActionOn.StepId;

                                // Set values for the forward data object using session data and user information
                                dTOActionOn.FromUserId = dtoSession.UserId;
                                dTOActionOn.FromAspNetUsersId = CurrentAspNetUsersId;
                                dTOActionOn.ToUserId = rejectDetailResponse.ToUserId;
                                dTOActionOn.ToAspNetUsersId = rejectDetailResponse.ToAspNetUsersId;
                                dTOActionOn.UnitId = dtoSession.UnitId;
                                dTOActionOn.UpdatedOn = DateTime.Now;
                                dTOActionOn.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                dTOActionOn.IsActive = true;
                                dTOActionOn.TypeId = Convert.ToByte(1); // Set the type to 1 for rejection
                                dTOActionOn.FwdStatusId = 3; // Set the status to 3 for rejection
                                dTOActionOn.IsComplete = false;
                                dTOActionOn.IsLock= false;

                                await iTrnFwnBL.ActionOnRequest(dTOActionOn, rejectDetailResponse.StepId);

                                response.Message = "Reject Application successfully.";
                                response.Value = dTOAction;
                                response.Result = true;
                                return Ok(response);
                            }
                            else
                            {
                                //Invalid Input
                                response.Message = "Invalid Input";
                                response.Value = dTOAction;
                                response.Result = false;
                                return Ok(response);
                            }

                        }
                        else if (dTOActionOn.Flag == "A")
                        {
                            DTORequestFwdDetailResponse? dTORequestFwdDetail = await iTrnFwnBL.RequestFwdDetail(dTOActionOn.RequestId);
                            if (dTORequestFwdDetail != null)
                            {
                                dTOAction.BeforeAction_StepId = dTORequestFwdDetail.StepId;
                                dTOAction.ApplyForId = dTORequestFwdDetail.ApplyForId;

                                if (dTORequestFwdDetail.StepId ==1 || dTORequestFwdDetail.StepId == 2 || dTORequestFwdDetail.StepId == 3 || dTORequestFwdDetail.StepId == 7 || dTORequestFwdDetail.StepId == 8 || dTORequestFwdDetail.StepId == 9)
                                {
                                    if ((dTORequestFwdDetail.FromAspNetUsersId != CurrentAspNetUsersId) || (dTORequestFwdDetail.FromUserId != dtoSession.UserId))
                                    {
                                        response.Message = "You are not authorized to forward this request.";
                                        response.Value = dTOAction;
                                        response.Result = false;
                                        return Ok(response);
                                    }
                                    if (dTORequestFwdDetail.StepId == 1 || dTORequestFwdDetail.StepId == 7 || dTORequestFwdDetail.StepId == 8 || dTORequestFwdDetail.StepId == 9)
                                    {
                                        if ((dTOActionOn.ToAspNetUsersId == CurrentAspNetUsersId) || (dTOActionOn.ToUserId == dtoSession.UserId))
                                        {
                                            response.Message = "Source and destination DID/Profile are not the same.";
                                            response.Value = dTOAction;
                                            response.Result = false;
                                            return Ok(response);
                                        }
                                        dTORequestFwdDetail.ToAspNetUsersId = dTOActionOn.ToAspNetUsersId;
                                        dTORequestFwdDetail.ToUserId = dTOActionOn.ToUserId;

                                        dTOAction.AspNetUsersId = dTOActionOn.ToAspNetUsersId;
                                    }
                                    else if (dTORequestFwdDetail.StepId == 2 || dTORequestFwdDetail.StepId == 3)
                                    {
                                        if ((dTORequestFwdDetail.ToAspNetUsersId != dTOActionOn.ToAspNetUsersId) || (dTORequestFwdDetail.ToUserId != dTOActionOn.ToUserId))
                                        {
                                            response.Message = "Destination DID/Profile are not correct.";
                                            response.Value = dTOAction;
                                            response.Result = false;
                                            return Ok(response);
                                        }
                                        dTOAction.AspNetUsersId = dTORequestFwdDetail.ToAspNetUsersId;
                                    }
                                }
                                else
                                {
                                    //Invalid Input
                                    response.Message = "Invalid Input";
                                    response.Value = dTOAction;
                                    response.Result = false;
                                    return Ok(response);
                                }


                                if (dTORequestFwdDetail.ToUserId == 0)
                                {
                                    response.Message = "Profile is not mapped with domain Id!";
                                    response.Value = dTOAction;
                                    response.Result = false;
                                    return Ok(response);
                                }
                                else
                                {
                                    if (dTORequestFwdDetail.StepId == 1 || dTORequestFwdDetail.StepId == 7 || dTORequestFwdDetail.StepId == 8 || dTORequestFwdDetail.StepId == 9 || dTORequestFwdDetail.StepId == 10)
                                    {
                                        dTOActionOn.StepId = 2;
                                    }
                                    else
                                    {
                                        dTOActionOn.StepId = (byte)(dTORequestFwdDetail.StepId + 1);
                                    }

                                    dTOAction.AfterAction_StepId = dTOActionOn.StepId;

                                    // Set values for the forward data object using session data and user information
                                    dTOActionOn.FromUserId = dtoSession.UserId;
                                    dTOActionOn.FromAspNetUsersId = CurrentAspNetUsersId;
                                    dTOActionOn.ToUserId = dTORequestFwdDetail.ToUserId;
                                    dTOActionOn.ToAspNetUsersId = dTORequestFwdDetail.ToAspNetUsersId;
                                    dTOActionOn.UnitId = dtoSession.UnitId;
                                    dTOActionOn.UpdatedOn = DateTime.Now;
                                    dTOActionOn.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                                    dTOActionOn.IsActive = true;
                                    dTOActionOn.TypeId = dTOActionOn.StepId;
                                    dTOActionOn.StepId = dTOActionOn.StepId;
                                    dTOActionOn.FwdStatusId = 2; // Set the status to 2 for Approved
                                    dTOActionOn.IsComplete = false;
                                    dTOActionOn.IsLock = true;

                                    await iTrnFwnBL.ActionOnRequest(dTOActionOn, dTORequestFwdDetail.StepId);

                                    response.Message = "ok";
                                    response.Value = dTOAction;
                                    response.Result = true;
                                    return Ok(response);
                                }
                            }
                            else
                            {
                                //Invalid Input
                                response.Message = "Invalid Input";
                                response.Value = dTOAction;
                                response.Result = false;
                                return Ok(response);
                            }
                        }
                        else
                        {
                            //Invalid Input
                            response.Message = "Invalid Input";
                            response.Value = dTOAction;
                            response.Result = false;
                            return Ok(response);
                        }
                    }
                    else
                    {
                        if (mTrnICard == null)
                        {
                            //Invalid Request Id
                            response.Message = "Invalid Request Id";
                            response.Value = dTOAction;
                            response.Result = false;
                            return Ok(response);
                        }
                        else
                        {
                            if (mTrnICard.StatusId == 2)
                            {
                                //Request is completed, cannot be rejected
                                response.Message = "Request is completed, cannot be rejected";
                                response.Value = dTOAction;
                                response.Result = false;
                                return Ok(response);
                            }
                            else
                            {
                                //Request is closed, cannot be rejected
                                response.Message = "Request is closed, cannot be rejected";
                                response.Value = dTOAction;
                                response.Result = false;
                                return Ok(response);
                            }
                        }

                    }
                }
                else
                {
                    // Extract validation errors from ModelState
                    var errors = ModelState
                        .Where(x => x.Value?.Errors?.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    if (errors.Any())
                    {
                        // Concatenate all validation error messages
                        response.Message = string.Join("; ", errors);
                    }

                    response.Value = dTOAction;
                    response.Result = false;
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                _logger.LogError(1001, ex, "BasicDetails=>ActionOnRequest.");
                response.Message = "Internal Server Error!";
                response.Value = dTOAction;
                response.Result = false;
                return Ok(response);
            }
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
        [HttpPost]
        public async Task<IActionResult> DataExport(string request)
        {
            DTODataExportRequest Data = await AESEncrytDecry.DecryptAESWithDTO<DTODataExportRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            if(Data==null)
                return Json(KeyConstants.InternalServerError);
            try
            {
                DTOApplFwdConditionRequest dTOApplFwdCondition;

                // Retrieve the encryption key record from the database
                var keyRecord = await encryptionSettingBL.Get(1);
                if (keyRecord != null)
                {
                    Data.publicKey = keyRecord.PublicKey;
                    Data.privateKey = keyRecord.PrivateKey;
                    if (!string.IsNullOrWhiteSpace(keyRecord.ApplFwdCondition))
                    {
                        dTOApplFwdCondition = !string.IsNullOrWhiteSpace(keyRecord.ApplFwdCondition)
                            ? JsonConvert.DeserializeObject<DTOApplFwdConditionRequest>(keyRecord.ApplFwdCondition) ?? new DTOApplFwdConditionRequest()
                            : new DTOApplFwdConditionRequest();
                    }
                    else
                    {
                        dTOApplFwdCondition = new DTOApplFwdConditionRequest();
                    }
                }
                else
                {
                    // Throw exception if encryption keys are not found
                    throw new InvalidOperationException("Encryption key record not found.");
                }

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
        [HttpPost]
        public async Task<IActionResult> DataDigitalXmlSign(DTODataExportRequest Data)
        {
            try
            {
                if (ModelState.IsValid)
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
                else
                {
                    return Json(null);
                }

            }
            catch (Exception ex)
            {
                // Step 5a: Log the exception using a unique code (1001) for easier identification
                // This helps in tracing which method the error occurred in during debugging
                _logger.LogError(1001, ex, "BasicDetails=>DataDigitalXmlSign.");

                return Json(null);
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
        [HttpGet]
        public async Task<IActionResult> FaultyCard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

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
            
            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
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
                _logger.LogError(1001, ex, "BasicDetail->GetAllFaulty");

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

        /// <summary>
        /// Displays the Faulty Card Request page for a specific faulty card transaction.
        /// Decrypts the provided ID, validates it, and checks if the current user has the required claim.
        /// </summary>
        /// <param name="Id">The encrypted ID of the faulty card transaction (nullable).</param>
        /// <returns>
        /// Returns the Faulty Card Request view with decrypted ID and claim information.
        /// If the ID is invalid or tampered with, redirects to ContactUs page with error message.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> FaultyCardRequest(string? Id)
        {
            bool Claim = false;
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Step 1: Get the current logged-in user's Id
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

            string decryptedId = string.Empty;
            int decryptedIntId = 0;

            // Step 2: Decrypt the provided ID if it exists
            if (Id != null)
            {
                try
                {
                    decryptedId = protector.Unprotect(Id); // Decrypt the ID

                    // Validate that the decrypted ID is a valid integer
                    if (!int.TryParse(decryptedId, out decryptedIntId))
                    {
                        _logger.LogWarning(
                            "Decrypted Id is not a valid integer: {DecryptedId}, UserId: {UserId}",
                            decryptedId,
                            AspNetUsersId
                        );
                        TempData["error"] = "Invalid Request.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    // Log cryptographic errors if decryption fails
                    _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                    TempData["error"] = "Invalid or tampered request.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
                catch (Exception ex)
                {
                    // Log any other unexpected errors
                    _logger.LogError(1001, ex, "This error occurred because Id: {Id} value was changed by user.", Id);
                    TempData["error"] = ex.Message;
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }

            // Step 3: Check if the current user has the "ICard Export Data" claim
            var UserClaims = await userManager.GetClaimsAsync(user);
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                Claim = true;
            }

            // Step 4: Pass claim and decrypted ID to the view
            ViewBag.Claim = Claim;
            ViewBag.TrnFaultyCardId = decryptedIntId;

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Retrieves basic details for a partial view by RequestId.
        /// Decrypts the photo and signature images and converts them to Base64 strings for display.
        /// </summary>
        /// <param name="RequestId">The ID of the request to fetch details for.</param>
        /// <returns>
        /// Returns a PartialView "_BasicDetail_ParitalView" populated with DTOBasicDetailForParitalViewResponse.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetBasicDetailForParitalViewByRequestId(string Request)
        {
            int RequestId = await AESEncrytDecry.DecryptAESWithDTO<int>(Request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            // Step 1: Fetch basic details from business layer
            DTOBasicDetailForParitalViewResponse? data = await basicDetailBL.GetBasicDetailForParitalViewByRequestId(RequestId);
            if (data != null)
            {
                // Step 2: Construct physical paths for photo and signature images
                string sourceFolderPathPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
                string sourcePathPhoto = Path.Combine(sourceFolderPathPhy, "Photo", data.PhotoImagePath);
                string sourcePathSignature = Path.Combine(sourceFolderPathPhy, "Signature", data.SignatureImagePath);

                // Step 3: Decrypt and convert photo image to Base64 if it exists
                if (System.IO.File.Exists(sourcePathPhoto))
                {
                    data.PhotoImagePath = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                }

                // Step 4: Decrypt and convert signature image to Base64 if it exists
                if (System.IO.File.Exists(sourcePathSignature))
                {
                    data.SignatureImagePath = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
                }

                // Step 5: Return the partial view with populated data
                return PartialView("_BasicDetail_ParitalView", data);
            }
            else
            {
                return PartialView("_BasicDetail_ParitalView", null);
            }
        }

        /// <summary>
        /// Handles the saving of a faulty card request. 
        /// This method processes both new requests and updates to existing faulty card records. 
        /// It supports rejection flow, checks for user claims, validates model state, 
        /// handles duplicates, and returns a standard response object with success/failure message.
        /// </summary>
        /// <param name="dTO">DTOFaultyCardRequest object containing the faulty card data submitted from the client.</param>
        /// <returns>Returns a JSON response indicating success or failure along with relevant messages.</returns>
        [HttpPost]
        [Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<IActionResult> SaveFaultyCard([FromBody] DTOFaultyCardRequest dTO)
        {
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                // Mark DTO as active and set updated metadata
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;

                if (dTO.Choice == 1)
                {
                    ModelState.AddModelError("Choice", "Please select a valid choice.");
                }


                // Validate the incoming model
                if (ModelState.IsValid)
                {
                    // Initialize forwarding entity and response DTO
                    MTrnFwd? mTrnFwd = new MTrnFwd();
                    DtoSession? dtoSession = new DtoSession();

                    // Retrieve session token if available and extract user session information
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                    {
                        dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    }

                    // Assign the user ID from session to the DTO (or 0 if session is null)
                    dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;
                    dTO.UnitId = dtoSession != null ? dtoSession.UnitId : 0;

                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    dTO.Claim = true;
                    var dTOBeforeFaulty = await faultyCardBL.CheckBeforeFaultyCardReport(dTO);
                    
                    dTO.TrnFwdId = dTOBeforeFaulty.TrnFwdId;
                    dTO.RequestId = dTOBeforeFaulty.RequestId;
                    dTO.BasicDetailId = dTOBeforeFaulty.BasicDetailId;
                    dTO.ApplyForId = dTOBeforeFaulty.ApplyForId;

                    if (dTOBeforeFaulty.Result)
                    {
                        // Handle rejection scenario (Choice == 3)
                        if (dTO.Choice == 3)
                        {
                            mTrnFwd.RequestId = dTO.RequestId;
                            mTrnFwd.FromUserId = dTO.UserId;
                            mTrnFwd.UnitId = dTO.UnitId;
                            mTrnFwd.Remark = dTO.ToRemark;
                            mTrnFwd.FwdStatusId = Convert.ToByte(3); // Reject status
                            mTrnFwd.TypeId = Convert.ToByte(1);
                            mTrnFwd.StepId = Convert.ToByte(9);
                            mTrnFwd.IsComplete = false;
                            mTrnFwd.RemarksIds = dTO.RemarksIds != null && dTO.RemarksIds.Any() ? string.Join(",", dTO.RemarksIds) : string.Empty;
                            mTrnFwd.FromAspNetUsersId = AspNetUsersId;
                            mTrnFwd.UpdatedOn = DateTime.Now;
                            mTrnFwd.Updatedby = AspNetUsersId;
                            mTrnFwd.IsActive = true;

                            // Fetch domain mapping for the request
                            TrnDomainMapping? Domain = new TrnDomainMapping();
                            Domain = await iDomainMapBL.GetByRequestId(dTO.RequestId);

                            if (Domain != null)
                            {
                                // Check if domain is mapped to a user
                                if (Domain.UserId.GetValueOrDefault() == 0)
                                {
                                    dTOFaulty.Message = "Profile is not mapped with domain Id!";
                                    dTOFaulty.Result = false;
                                    return Ok(dTOFaulty);
                                }
                                else
                                {
                                    // Assign the target user and AspNetUser IDs for forwarding
                                    mTrnFwd.ToAspNetUsersId = Domain.AspNetUsersId;
                                    mTrnFwd.ToUserId = Convert.ToInt32(Domain.UserId);
                                }
                            }
                        }

                        // Save the new faulty card request
                        dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd);
                        return Json(dTOFaulty);
                    }
                    else
                    {
                        dTOFaulty.Result = dTOBeforeFaulty.Result;
                        dTOFaulty.Message = dTOBeforeFaulty.Message;
                        return Json(dTOFaulty);
                    }
                }
                else
                {
                    // Collect all model validation errors
                    var errors = ModelState
                                .Where(x => x.Value?.Errors?.Count > 0)
                                .SelectMany(x => x.Value!.Errors.Select(e =>
                                    $"{x.Key}: {e.ErrorMessage}"))
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
                // Handle unexpected exceptions
                dTOFaulty.Result = false;
                dTOFaulty.Message = ex.Message;
                return Json(dTOFaulty);
            }
        }



        /// <summary>
        /// Handles the saving of a new faulty card request.
        /// This method validates the incoming DTO, checks for duplicate requests,
        /// verifies user claims for "ICard Export Data", and returns a standardized JSON response.
        /// </summary>
        /// <param name="dTO">DTOFaultyCardRequest object containing the faulty card data submitted from the client.</param>
        /// <returns>JSON response indicating success or failure with relevant message.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveFaultyCardRequest([FromBody] DTOFaultyCardRequest dTO)
        {
            // Response DTO to return success/failure
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();

            try
            {
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Mark the request as active and record update metadata
                dTO.IsActive = true;
                dTO.Updatedby = AspNetUsersId;
                dTO.UpdatedOn = DateTime.Now;

                // Validate the incoming model
                if (ModelState.IsValid)
                {
                    // Initialize forwarding entity
                    MTrnFwd? mTrnFwd = new MTrnFwd();

                    // Initialize session object
                    DtoSession? dtoSession = new DtoSession();

                    // Retrieve user session token from HTTP session if available
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                    {
                        dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    }

                    // Set the user ID from session into the DTO; fallback to 0 if session is null
                    dTO.UserId = dtoSession != null ? dtoSession.UserId : 0;
                    dTO.UnitId = dtoSession != null ? dtoSession.UnitId : 0;

                    // Fetch user from UserManager
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                    // Retrieve all claims of the user
                    var UserClaims = await userManager.GetClaimsAsync(user);

                    // If the user has the required claim, set Claim flag to true
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dTO.Claim = true;
                    }
                    else
                    {
                        dTO.Claim = false;
                    }

                    // If this is an attempt to edit an existing faulty card, reject it
                    if (dTO.TrnFaultyCardId > 0)
                    {
                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "This action is not allowed for you. Please check.";
                        return Json(dTOFaulty);
                    }
                    else
                    {
                        // Check for duplicate request based on RequestId
                        var dTOBeforeFaulty = await faultyCardBL.CheckBeforeFaultyCardReport(dTO);
                        if (dTOBeforeFaulty.Result)
                        {
                            dTO.Choice = 1; // click btnSubmit
                            dTO.TrnFwdId= dTOBeforeFaulty.TrnFwdId;
                            // Save the new faulty card request
                            dTOFaulty = await faultyCardBL.SaveFaultyCard(dTO, mTrnFwd);
                            return Json(dTOFaulty);
                        }
                        else
                        {
                            dTOFaulty.Result = dTOBeforeFaulty.Result;
                            dTOFaulty.Message = dTOBeforeFaulty.Message;
                            return Json(dTOFaulty);
                        }
                    }
                }
                else
                {
                    // If model validation failed, collect all errors
                    var errors = ModelState
                                .Where(x => x.Value?.Errors?.Count > 0)
                                .SelectMany(x => x.Value!.Errors.Select(e =>
                                    $"{x.Key}: {e.ErrorMessage}"))
                                .ToList();
                    if (errors.Any())
                    {
                        dTOFaulty.Message = string.Join("; ", errors); // Combine all errors
                    }
                    dTOFaulty.Result = false;
                    return Json(dTOFaulty);
                }

            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions and return a failure response
                dTOFaulty.Result = false;
                dTOFaulty.Message = ex.Message;
                return Json(dTOFaulty);
            }
        }


        #endregion

        #region HotlistCard
        /// <summary>
        /// Returns the Hotlist Card view to the user.
        /// This method is asynchronous to support future enhancements if data fetching is required,
        /// although currently it simply renders the view.
        /// </summary>
        /// <returns>A ViewResult representing the Hotlist Card page.</returns>
        [HttpGet]
        public IActionResult HotlistCard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Retrieves all hotlisted card records based on the DataTables request parameters.
        /// This method returns data in JSON format suitable for client-side DataTables consumption.
        /// </summary>
        /// <param name="dTO">The DataTables request containing paging, filtering, and sorting information.</param>
        /// <returns>A JSON result containing the filtered, sorted, and paginated list of hotlisted cards.</returns>
        [HttpPost]
        public async Task<IActionResult> GetAllHotlist(DTODataTablesRequest dTO)
        {
            // Calls the business layer to get all hotlist records based on the DataTables request
            var hotlistData = await _hotlistCardBL.GetAllHotlist(dTO);

            // Returns the result as JSON for DataTables on the client side
            return Json(hotlistData);
        }


        /// <summary>
        /// Exports selected hotlist card data to a CSV file and returns the temporary file name.
        /// This endpoint accepts a list of request IDs and generates a CSV using CsvHelper.
        /// </summary>
        /// <param name="req">DTO containing request IDs for hotlist cards to export.</param>
        /// <returns>
        /// JSON containing a DTOCommonSaveResponse with:
        /// - Result: true if export succeeded, false otherwise.
        /// - Message: filename of the CSV if successful, or an error message.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> HotlistDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            // Response object to return status and message
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();

            try
            {
                // Create a temporary file with .csv extension
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");

                // Fetch hotlist card records by request IDs from the business layer
                var records = await _hotlistCardBL.GetDetailsByRequestIds(req);

                // Using StreamWriter and CsvWriter to write CSV
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Register a custom CSV mapping class for DTOHotlistCardExportResponse
                    csv.Context.RegisterClassMap(
                        new CsvClassMap<DTOHotlistCardExportResponse>(
                            true,
                            CsvClassMapTypeEnum.HotlistExport
                        )
                    );

                    try
                    {
                        // Write all records to the CSV asynchronously
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        // Log any exceptions during CSV writing
                        _logger.LogError(1001, ee, "BasicDetail->HotlistDataExport");

                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "Internal Server Error!";
                        goto ReturnSt; // Jump to return statement
                    }
                }

                // Export successful
                dTOFaulty.Result = true;
                dTOFaulty.Message = Path.GetFileName(tempFileName); // Return filename
            }
            catch (Exception ex)
            {
                // Log any exceptions during file creation or data fetching
                _logger.LogError(1001, ex, "BasicDetail->HotlistDataExport");

                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

        ReturnSt:
            // Return JSON response
            return Json(dTOFaulty);
        }


        /// <summary>
        /// Downloads a CSV file from the server's temporary folder.
        /// This endpoint takes the temporary file name and a custom file store name
        /// and returns the CSV file as a download to the client.
        /// </summary>
        /// <param name="fileName">Temporary CSV file name stored on the server.</param>
        /// <param name="fileStoreName">Custom name to use for the downloaded file.</param>
        /// <returns>
        /// Returns a PhysicalFileResult if the file exists, otherwise returns NotFound or BadRequest.
        /// </returns>
        [HttpGet]
        public IActionResult DownloadCsv(string fileName, string fileStoreName)
        {
            try
            {
                // Build the full path to the temporary file
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                // Check if file exists; if not, return 404
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                // Define MIME type for CSV
                var mimeType = "text/csv";

                // Return the file to the client with a custom download filename
                return PhysicalFile(
                    filePath,
                    mimeType,
                    $"E-ISAC_{fileStoreName}ExportData.csv"
                );
            }
            catch (Exception ex)
            {
                // Log any unexpected errors
                _logger.LogError(1001, ex, "BasicDetail->DownloadCsv");

                // Return 400 Bad Request on exception
                return BadRequest();
            }
        }


        /// <summary>
        /// Serves the Hotlist Card Request view page.
        /// This action is responsible for returning the view where users can submit or view hotlist card requests.
        /// </summary>
        /// <returns>Returns a ViewResult rendering the Hotlist Card Request page.</returns>
        [HttpGet]
        public ActionResult HotListCardRequest()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Handles saving a new hotlist card request.  
        /// Validates the input model, checks for duplicate request IDs, and saves the record if valid.  
        /// Returns a JSON response with the operation result.
        /// </summary>
        /// <param name="model">The hotlist card request model submitted by the user.</param>
        /// <returns>A JSON object containing the result, message, and relevant metadata.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveHotlistCardRequest(DTOTrnHotListCardRequest dTOTrnHot)
        {
            var dTOFaulty = new DTOGenericResponse<DTOCommonResponse>();
            try
            {
                // Check model validation state
                if (ModelState.IsValid)
                {
                    var checkCardBeforeHotList = await _hotlistCardBL.CheckBeforeHotListCardReport(dTOTrnHot.RequestId);
                    if (checkCardBeforeHotList.Result)
                    {
                        // Retrieve current session data if available
                        DtoSession? dtoSession = new DtoSession();
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                        {
                            dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                        }

                        var model = new TrnHotlistCard();
                        model.RequestId = dTOTrnHot.RequestId;
                        model.RemarksIds = dTOTrnHot.RemarksIds != null && dTOTrnHot.RemarksIds.Any() ? string.Join(",", dTOTrnHot.RemarksIds) : string.Empty;
                        model.Remark = dTOTrnHot.Remark;
                        model.IsActive = true; // Mark record as active
                        model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)); // Current ASP.NET user ID
                        model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0; // Session user ID if available
                        model.UpdatedOn = DateTime.Now; // Timestamp of update

                        // Save the record and return success response
                        var result = await _hotlistCardBL.AddWithReturn(model);
                        dTOFaulty.Result = true;
                        dTOFaulty.Message = "Record created!";
                        dTOFaulty.Value.CurrentTime = result.UpdatedOn.GetValueOrDefault(); // Return saved record timestamp
                        dTOFaulty.Value.Id = result.HotlistCardId.ToString(); // Return saved record ID
                    }
                    else
                    {
                        // Duplicate found, return failure response
                        dTOFaulty.Result = checkCardBeforeHotList.Result;
                        dTOFaulty.Message = checkCardBeforeHotList.Message;
                    }
                }
                else
                {
                    // If ModelState is invalid, extract all validation errors

                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e =>
                            $"{x.Key}: {e.ErrorMessage}"))
                        .ToList();

                    if (errors.Any())
                    {
                        // Concatenate all error messages into one string
                        dTOFaulty.Message = string.Join("; ", errors);
                    }

                    dTOFaulty.Result = false; // Validation failed
                }
            }
            catch (Exception ex)
            {
                // Log unexpected errors and return a generic failure message
                _logger.LogError(1001, ex, "BasicDetail->SaveHotlistCardRequest");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

            // Return final result as JSON
            return Json(dTOFaulty);
        }

        #endregion HotlistCard

        #region LostCard
        /// <summary>
        /// Returns the Lost Card view page.
        /// This is typically the page where users can view or submit lost card requests.
        /// </summary>
        /// <returns>The LostCard view.</returns>
        [HttpGet]
        public IActionResult LostCard()
        {
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Fetches all lost card records based on DataTables request parameters.
        /// This endpoint is called via AJAX to populate the lost card DataTable.
        /// </summary>
        /// <param name="dTO">DataTables request object containing paging, sorting, and filtering info.</param>
        /// <returns>JSON result containing the list of lost card records.</returns>
        [HttpPost]
        public async Task<IActionResult> GetAllLost(DTODataTablesRequestForCommanCheckAll dTO)
        {
            // If an exception occurs, return an empty response to avoid breaking the UI
            List<DTOLostCardGetResponse> dTOLosts = new List<DTOLostCardGetResponse>();
            var responseData = new DTODataTablesWithSelectedIdsResponse<DTOLostCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                selectedIds = null,
                data = dTOLosts
            };
            try
            {
                if (ModelState.IsValid)
                {
                    // Call the business layer to get all lost card records and return as JSON
                    return Json(await _lostCardBL.GetAllLost(dTO));
                }
                else
                {
                    return Json(responseData);
                }

            }
            catch (Exception ex)
            {
                // Log the exception for debugging and tracking
                _logger.LogError(1001, ex, "BasicDetail->GetAllLost");

                // Return JSON with empty data
                return Json(responseData);
            }
        }


        /// <summary>
        /// Exports lost card data based on the provided request IDs to a CSV file.
        /// The CSV file is created in the temporary folder and the file name is returned in the JSON response.
        /// </summary>
        /// <param name="req">Request object containing the IDs of lost card records to export.</param>
        /// <returns>JSON response containing the status and temporary CSV file name.</returns>
        [HttpPost]
        public async Task<IActionResult> LostDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            // Response object to store result and message
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                // Generate temporary CSV file name
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");

                // Fetch records from business layer based on request IDs
                var records = await _lostCardBL.GetDetailsByRequestIds(req);

                // Write records to CSV using CsvHelper
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Register mapping for DTO to CSV columns
                    csv.Context.RegisterClassMap(new CsvClassMap<DTOLostCardExportResponse>(true, CsvClassMapTypeEnum.HotlistExport));

                    try
                    {
                        // Write records asynchronously to the CSV file
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        // Log error if writing fails
                        _logger.LogError(1001, ee, "BasicDetail->LostDataExport");

                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "Internal Server Error!";
                        goto ReturnSt;
                    }
                }

                // If CSV creation succeeds
                dTOFaulty.Result = true;
                dTOFaulty.Message = Path.GetFileName(tempFileName);
            }
            catch (Exception ex)
            {
                // Log unexpected errors
                _logger.LogError(1001, ex, "BasicDetail->LostDataExport");

                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

        ReturnSt:
            // Return the JSON response with result and file name
            return Json(dTOFaulty);
        }


        /// <summary>
        /// Returns the Lost Card Request view for the user to submit new requests.
        /// </summary>
        /// <returns>The Lost Card Request view.</returns>
        [HttpGet]
        public ActionResult LostCardRequest()
        {
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Handles the creation of a lost card request.
        /// </summary>
        /// <param name="model">The lost card request data transfer object containing user inputs and optional supporting file.</param>
        /// <returns>A JSON response indicating success, failure, or validation errors.</returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates the model state.
        /// 2. Uploads the supporting document if provided.
        /// 3. Maps the DTO to the entity model <see cref="TrnLostCard"/>.
        /// 4. Checks for duplicate requests in the database.
        /// 5. Saves the record and triggers any related post-save logic.
        /// 6. Returns a <see cref="DTOCommonSaveResponse"/> with the result, message, and metadata.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> SaveLostCardRequest([FromForm] DTOLostCardAddRequest model)
        {
            var dTOResponse = new DTOGenericResponse<DTOCommonResponse?>();
            try
            {
                int TDMId;
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(model.SignedXML))
                    {
                        Span<byte> buffer = new Span<byte>(new byte[model.SignedXML.Length]);

                        if (!Convert.TryFromBase64String(model.SignedXML, buffer, out int bytesWritten))
                        {
                            dTOResponse.Result = false;
                            dTOResponse.Message = "Invalid format.";
                            return Json(dTOResponse);
                        }

                        string xmlString = Encoding.UTF8.GetString(buffer.Slice(0, bytesWritten));
                        model.SignedXML = xmlString;
                    }
                    // Retrieve user session
                    DtoSession? dtoSession = new DtoSession();
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                    {
                        dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    }
                    TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
                    var checkCardBeforeLost = await _lostCardBL.CheckBeforeLostReport(model.RequestId, TDMId);

                    if (checkCardBeforeLost.Result)
                    {
                        #region Upload Supporting Document
                        string fileName = string.Empty;
                        if (model.File != null)
                        {
                            /// Generate unique filename using timestamp
                            fileName = $"{DateTime.Now:yyyyMMddHHmmss}.pdf";
                            var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "LostCardSupportingDoc");

                            // Ensure folder exists
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            var filePath = Path.Combine(uploadsFolder, fileName);

                            // Save uploaded file to server
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }
                        }
                        #endregion

                        model.IsActive = true;
                        model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        model.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
                        model.SupportDocName = string.IsNullOrEmpty(fileName) ? "" : fileName;
                        model.SignedXML = model.SignedXML ?? string.Empty;
                        model.StatusId = checkCardBeforeLost.StatusId;
                        model.BasicDetailId = checkCardBeforeLost.BasicDetailId;
                        model.AppointmentName = checkCardBeforeLost.AppointmentName;
                        model.HotlistCardId = checkCardBeforeLost.HotlistCardId;

                        // Save entity and trigger related business logic
                        dTOResponse = await _lostCardBL.SaveLostCardRequest(model);
                    }
                    else
                    {
                        dTOResponse.Result = false;
                        dTOResponse.Message = checkCardBeforeLost.Message;
                    }
                }
                else
                {
                    // Collect and return model validation errors
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                                            .SelectMany(x => x.Value!.Errors.Select(e =>
                                                $"{x.Key}: {e.ErrorMessage}"))
                                            .ToList();
                    if (errors.Any())
                    {
                        dTOResponse.Message = string.Join("; ", errors);
                    }
                    dTOResponse.Result = false;
                }

            }
            catch (Exception ex)
            {
                // Log exception and return generic error message
                _logger.LogError(1001, ex, "BasicDetail->SaveLostCardRequest");
                dTOResponse.Result = false;
                dTOResponse.Message = "Internal Server Error!";
            }

            return Json(dTOResponse);
        }

        #endregion LostCard

        #region DistributeCard
        /// <summary>
        /// Returns the view for the card distribution page.
        /// </summary>
        /// <returns>A <see cref="ViewResult"/> representing the Distribute Card view.</returns>
        [HttpGet]
        public IActionResult DistributeCard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Retrieves all distributed card records based on the given DataTables request parameters.
        /// </summary>
        /// <param name="dTO">The DataTables request object containing paging, sorting, and filter information.</param>
        /// <returns>A JSON result containing the list of distributed cards.</returns>
        [HttpPost]
        public async Task<IActionResult> GetAllDistribute(DTODataTablesRequestForCommanCheckAll dTO)
        {
            // If an exception occurs, return an empty response to avoid breaking the UI
            List<DTODistributeCardGetResponse> dTODistributes = new List<DTODistributeCardGetResponse>();
            var responseData = new DTODataTablesWithSelectedIdsResponse<DTODistributeCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                selectedIds = null,
                data = dTODistributes
            };
            try
            {
                if (ModelState.IsValid)
                {
                    DtoSession? dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    dTO.UnitMapId = dtoSession != null ? dtoSession.UnitId : 0;
                    // Call business layer to retrieve dispatch card data for dialog
                    return Json(await _distributeCardBL.GetAllDistribute(dTO));
                }
                else
                {
                    return Json(responseData);
                }

            }
            catch (Exception ex)
            {
                // Log the exception for debugging and tracking
                _logger.LogError(1001, ex, "BasicDetail->GetAllDistribute");

                // Return JSON with empty data
                return Json(responseData);
            }
        }


        /// <summary>
        /// Exports distributed card data to a CSV file based on the provided request IDs.
        /// </summary>
        /// <param name="req">The export request object containing the IDs of records to export.</param>
        /// <returns>A JSON result indicating success or failure, with the temporary CSV file name on success.</returns>
        [HttpPost]
        public async Task<IActionResult> DistributeDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                // Generate a temporary CSV file path
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");

                // Fetch the records to export
                var records = await _distributeCardBL.GetDetailsByRequestIds(req);

                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Register CSV mapping
                    csv.Context.RegisterClassMap(new CsvClassMap<DTODistributeCardExportResponse>(true, CsvClassMapTypeEnum.DistributeCard));

                    try
                    {
                        // Write records to CSV
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


        /// <summary>
        /// Returns the view for creating a new Distribute Card request.
        /// </summary>
        /// <returns>The DistributeCardRequest view.</returns>
        [HttpGet]
        public ActionResult DistributeCardRequest()
        {
            return View();
        }


        /// <summary>
        /// Saves a new Distribute Card request.
        /// Handles session info, duplicate check, and card history validation before saving.
        /// </summary>
        /// <param name="model">The TrnDistributeCard model containing the request details.</param>
        /// <returns>
        /// A JSON response indicating the result of the operation and any relevant messages.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SaveDistributeCardRequest(TrnDistributeCard model)
        {
            DTOCommonSaveResponse dTOResponse = new DTOCommonSaveResponse();
            try
            {
                int UnitId;
                // Retrieve user session information
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                // Set audit and status fields
                model.IsActive = true;
                model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
                model.UpdatedOn = DateTime.Now;
                model.DistributedOn = DateTime.Now;
                UnitId = dtoSession != null ? dtoSession.UnitId : 0;

                // Validate model state
                if (ModelState.IsValid)
                {
                    // Check whether the card can be distributed based on previous card status
                    var checkCardBeforeDist = await basicDetailBL.CheckBeforeDistribution(model.RequestId, UnitId);
                    if (checkCardBeforeDist.Result)
                    {
                        // Fetch card history to record distribution details
                        ICardHistoryResponseAll? cardHistoryResponses = await basicDetailBL.ICardHistory(model.RequestId);
                        // Save the distribution record and get the response
                        dTOResponse = await _distributeCardBL.SaveDistributeCard(model, cardHistoryResponses);
                    }
                    else
                    {
                        // Inform user if previous card entry is missing
                        dTOResponse.Message = checkCardBeforeDist.Message;
                    }
                }
                else
                {
                    // Gather all model validation errors
                    var errors = ModelState
                                .Where(x => x.Value?.Errors?.Count > 0)
                                .SelectMany(x => x.Value!.Errors.Select(e =>
                                    $"{x.Key}: {e.ErrorMessage}"))
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
                // Log and return internal server error
                _logger.LogError(1001, ex, "BasicDetail->SaveDistributeCardRequest");
                dTOResponse.Result = false;
                dTOResponse.Message = "Internal Server Error!";
            }

            return Json(dTOResponse);
        }

        #endregion DistributeCard

        #region GetData/SearchAllServiceNo/GetBasicDetailByRequestId/GetRequestHistory/GetRegimentalListByArmedId/GetROListByArmedId/GetRemarks/CreateCSV/GetICardPrintPreviewByRequestId/GetBDetailByRequestId/GetTopArmyNoFromICardRequest/ICardRequestHold/GetAllICardRequestHold

        /// <summary>
        /// Checks whether the provided Army Number exists or is valid.
        /// </summary>
        /// <param name="ArmyNo">The Army Number to validate.</param>
        /// <returns>
        /// A JSON response with `true` if the Army Number is valid, otherwise `false`.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CheckArmyNO(string ArmyNo)
        {
            try
            {
                // Check if the input Army Number is not null or empty
                if (!string.IsNullOrEmpty(ArmyNo))
                {
                    // Call business layer to check validity and return the result as JSON
                    return Json(await basicDetailBL.CheckArmyNO(ArmyNo));
                }
                else
                {
                    // Return false if the Army Number is empty
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                _logger.LogError(1001, ex, "BasicDetail->CheckArmyNO");
                // Return false in case of an error
                return Json(false);
            }
        }

        /// <summary>
        /// Retrieves I-Card request status based on the provided IC number and card type.
        /// </summary>
        /// <param name="ICNumber">The service number of the user.</param>
        /// <param name="lCardType">The type of card requested (1 = First time, 5 = Lost card, 4 = Reissue, etc.).</param>
        /// <returns>Returns a JSON response indicating whether the I-Card request is pending or can be placed.</returns>
        [HttpPost]
        public async Task<IActionResult> GetData(string ICNumber, byte lCardType)
        {
           
            // Create a new DTO object to hold the response data
            DTOApiDataResponse dTOApiDataResponse = new DTOApiDataResponse();
            ICNumber = AESEncrytDecry.DecryptAES(ICNumber, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);  //decrypt password
            if (ICNumber == null)
            {
                // Loss not reported yet
                dTOApiDataResponse.Message = "invalid ArmyNo.";
                dTOApiDataResponse.Status = false; // Return error message for invalid ArmyNo
                return Ok(dTOApiDataResponse);
            }
            // Check if the ICNumber is provided
            if (ICNumber != null)
            {
                // Get the maximum BasicDetailId for the given ICNumber
                int? BasicDetailId = await basicDetailBL.MaxBasicDetailId(ICNumber);

                // If a BasicDetailId exists
                if (BasicDetailId != null)
                {
                    // Check if there is already a pending I-Card request
                    bool result = await iTrnICardRequestBL.GetRequestPending((int)BasicDetailId);
                    if (result)
                    {
                        // I-Card request is still under process
                        dTOApiDataResponse.Status = false;
                        dTOApiDataResponse.Message = "Your I-Card is under process. Please wait.";
                        return Ok(dTOApiDataResponse);
                    }
                    else
                    {
                        // Handle first-time smart card request
                        if (lCardType == 1)
                        {
                            dTOApiDataResponse.Message = "You didn't Select First time Smart card";
                            dTOApiDataResponse.Status = false;
                        }
                        // Handle lost card scenario
                        else if (lCardType == 5)
                        {
                            bool check = await _lostCardBL.CheckServiceNoRequestInLost(ICNumber);
                            if (check)
                            {
                                // Loss reported, can proceed with I-Card request
                                dTOApiDataResponse.Status = true;
                            }
                            else
                            {
                                // Loss not reported yet
                                dTOApiDataResponse.Message = "First, report the loss and then place an I-Card request.";
                                dTOApiDataResponse.Status = false;
                            }
                        }
                        else
                        {
                            // Other card types can proceed
                            dTOApiDataResponse.Status = true;
                        }

                        return Ok(dTOApiDataResponse);
                    }
                }
                else
                {
                    // No BasicDetailId found
                    if (lCardType == 1 || lCardType == 4)
                    {
                        // First-time or reissue card request allowed
                        dTOApiDataResponse.Status = true;
                    }
                    else
                    {
                        // Other card types require first-time smart card selection
                        dTOApiDataResponse.Message = "Please Select First time Smart card";
                        dTOApiDataResponse.Status = false;
                    }
                    return Ok(dTOApiDataResponse);
                }
            }
            else
            {
                // ICNumber not provided
                dTOApiDataResponse.Status = false;
                dTOApiDataResponse.Message = "Service no required.";
                return Ok(dTOApiDataResponse);
            }
        }


        /// <summary>
        /// Searches all service numbers based on the provided criteria in DTOSearchArmyNoRequest.
        /// </summary>
        /// <param name="dto">The search criteria including TypeId and optional filters.</param>
        /// <returns>Returns an Ok response with the search results including encrypted images or a BadRequest if invalid.</returns>
        [HttpPost]
        public async Task<IActionResult> SearchAllServiceNo([FromForm] DTOSearchArmyNoRequest dto)
        {
            try
            {
                // Set the current logged-in user's ID
                dto.AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dto.MapUnitId = 0;

                // Retrieve session data
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    // Deserialize session token to DtoSession object
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }
                dto.MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;

                // Determine claim value based on TypeId and user claims
                if (dto.TypeId == KeyConstants.FaultyCardRequest)
                {
                    var user = await userManager.FindByIdAsync(dto.AspNetUsersId.ToString());

                    // Get all claims of the current user
                    var UserClaims = await userManager.GetClaimsAsync(user);

                    // Determine claim type based on user's claims
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

                // If the model state is valid, proceed with search
                if (ModelState.IsValid)
                {
                    var Ret = await basicDetailBL.SearchAllServiceNo(dto);
                    if (Ret != null)
                    {
                        // Encrypt each user's photo and convert it to Base64 for transmission
                        foreach (var item in Ret)
                        {
                            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");
                            string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", item.Image);

                            if (System.IO.File.Exists(sourcePathPhoto))
                            {
                                item.Image = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                            }
                        }

                        // Return the search results
                        return Ok(Ret);
                    }
                }

                // Return bad request if model state invalid or no results
                return BadRequest();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and maintenance
                _logger.LogError(1001, ex, "BasicDetailController=>SearchAllServiceNo.");
                return BadRequest();
            }
        }

        /// <summary>
        /// Retrieves the basic detail of a user based on the provided RequestId,
        /// including decrypted photo and signature in Base64 format.
        /// </summary>
        /// <param name="RequestId">The request identifier used to fetch the user's basic details.</param>
        /// <returns>Returns a JSON object with the user's details including encrypted images, or null if not found.</returns>
        [HttpPost]
        public async Task<IActionResult> GetBasicDetailByRequestId(int RequestId)
        {
            // Fetch the basic detail data for the given request ID
            BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(RequestId);

            if (basicDetailCrtAndUpdVM != null)
            {
                // Set the physical path to the storage folder for images
                string sourceFolderPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

                // Construct full path for the photo and decrypt it to Base64
                string sourcePathPhoto = Path.Combine(sourceFolderPhy, "Photo", basicDetailCrtAndUpdVM.PhotoImagePath);
                basicDetailCrtAndUpdVM.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);

                // Construct full path for the signature and decrypt it to Base64
                string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", basicDetailCrtAndUpdVM.SignatureImagePath);
                basicDetailCrtAndUpdVM.ExistingSignatureInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);

                // Return the JSON object with decrypted images
                return Json(basicDetailCrtAndUpdVM);
            }
            else
            {
                // Return null if no basic detail found for the request ID
                return Json(null);
            }
        }


        /// <summary>
        /// Retrieves the I-Card request history for a given RequestId,
        /// including pending or completed card history based on the card status.
        /// </summary>
        /// <param name="RequestId">The request identifier for which the history is fetched.</param>
        /// <returns>Returns a JSON object containing the I-Card history details.</returns>
        [HttpPost]
        public async Task<IActionResult> GetRequestHistory(string Request)
        {
            int RequestId=await AESEncrytDecry.DecryptAESWithDTO<int>(Request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            // Initialize the response object for card history
            ICardHistoryResponseAll? cardHistoryResponses = new ICardHistoryResponseAll();

            // Check the current status of the card for the given request
            var cardStatus = await basicDetailBL.CheckCardStatus(RequestId);

            if (cardStatus.GetValueOrDefault() == 1 || cardStatus.GetValueOrDefault() == 3)
            {
                // If card is pending, fetch pending card history
                cardHistoryResponses = await basicDetailBL.ICardHistory(RequestId);
            }
            else if (cardStatus.GetValueOrDefault() == 2)
            {
                // If card is completed, fetch completed card history
                cardHistoryResponses = await basicDetailBL.ICardHistoryCompleted(RequestId);
            }

            // Return the card history as JSON
            return Json(cardHistoryResponses);
        }


        /// <summary>
        /// Retrieves the movement history of an I-Card for a specific request.
        /// </summary>
        /// <param name="RequestId">The request identifier for which the card movement history is fetched.</param>
        /// <returns>Returns a JSON object containing the card movement history details.</returns>
        [HttpPost]
        public async Task<IActionResult> GetCardMovementHistory(string Request)
        {
            int RequestId = await AESEncrytDecry.DecryptAESWithDTO<int>(Request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            // Fetch the card movement history from the business layer and return as JSON
            return Json(await basicDetailBL.GetCardMovementHistory(RequestId));
        }


        /// <summary>
        /// Retrieves the list of regimental units based on the given armed forces ID.
        /// </summary>
        /// <param name="ArmedId">The ID of the armed force for which regimental units are fetched.</param>
        /// <returns>Returns a JSON object containing the list of regimentals.</returns>
        [HttpPost]
        public async Task<JsonResult> GetRegimentalListByArmedId(byte ArmedId)
        {
            // Call the service layer to get the list of regimental units for the given ArmedId
            var regimentals = await service.GetRegimentalListByArmedId(ArmedId);

            // Return the result as JSON to the client
            return Json(regimentals);
        }


        /// <summary>
        /// Retrieves the list of Record Offices (RO) based on the given armed forces ID.
        /// </summary>
        /// <param name="ArmedId">The ID of the armed force for which record offices are fetched.</param>
        /// <returns>Returns an HTTP 200 response with the list of record offices in JSON format, or null if none found.</returns>
        [HttpPost]
        public async Task<IActionResult> GetROListByArmedId(byte ArmedId)
        {
            // Call the business layer to fetch record office list for the given ArmedId
            List<MRecordOffice>? mRecordOffices = await basicDetailBL.GetROListByArmedId(ArmedId);

            // Check if any record offices were retrieved
            if (mRecordOffices != null)
            {
                // Return the list as HTTP 200 OK with JSON
                return Ok(mRecordOffices);
            }
            else
            {
                // Return null if no record offices found
                return Ok(null);
            }
        }


        /// <summary>
        /// Retrieves remarks based on the type ID provided in the request.
        /// </summary>
        /// <param name="Data">The DTO containing the TypeId for which remarks are to be fetched.</param>
        /// <returns>Returns a JSON result containing the list of remarks.</returns>
        [HttpPost]
        public async Task<IActionResult> GetRemarks(DTORemarksRequest Data)
        {
            // Call the master business layer to get remarks by TypeId
            return Json(await _IMasterBL.GetRemarksByTypeId(Data));
        }


        /// <summary>
        /// Creates a CSV file based on the provided export request model.
        /// Determines whether to use RequestId or TrnFwdId based on user claims.
        /// Saves the CSV to the server and returns the temporary file name as JSON.
        /// </summary>
        /// <param name="model">DTO containing parameters for CSV export.</param>
        /// <returns>Returns JSON containing the temporary CSV file name or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCSV(string request)
        {
            DTOCSVExportRequest model = await AESEncrytDecry.DecryptAESWithDTO<DTOCSVExportRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            if(model==null)
                return Json(KeyConstants.InternalServerError);
            try
            {
                // Generate CSV string from the business layer
                string? csvData = await basicDetailBL.GetCSVString(model);
                if (csvData != null)
                {
                    // Generate a temporary file name using a GUID
                    string TempFileName = Guid.NewGuid().ToString();

                    // Define the folder path for saving CSV files
                    string sourceFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CSVFile");

                    // Check if directory exists; if not, create it
                    if (!Directory.Exists(sourceFolder))
                    {
                        Directory.CreateDirectory(sourceFolder);
                    }

                    // Write the CSV data to a file in the folder
                    System.IO.File.WriteAllText(sourceFolder + "/" + TempFileName + ".csv", csvData);

                    // Return the temporary file name as JSON
                    return Json(TempFileName);
                }
                else
                {
                    // If CSV data generation failed, return an internal server error constant
                    return Json(KeyConstants.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions and return an internal server error
                _logger.LogError(1001, ex, "BasicDetails=>CreateCSV.");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves the I-Card print preview details for a given RequestId.
        /// Decrypts the associated photo and signature images to Base64 strings for frontend display.
        /// </summary>
        /// <param name="RequestId">The RequestId of the I-Card request.</param>
        /// <returns>Returns JSON containing the BasicDetailCrtAndUpdVM with decrypted images, or null if not found.</returns>
        [HttpPost]
        public async Task<IActionResult> GetICardPrintPreviewByRequestId(string Request)
        {
            int RequestId = await AESEncrytDecry.DecryptAESWithDTO<int>(Request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            // Initialize the generic response object
            DTOGenericResponse<BasicDetailCrtAndUpdVM?> response = new DTOGenericResponse<BasicDetailCrtAndUpdVM?>();
            try
            {
                // Retrieve the basic detail record for the given RequestId
                BasicDetailCrtAndUpdVM? basicDetailCrtAndUpdVM = await basicDetailBL.GetBasicDetailByRequestId(RequestId);

                if (basicDetailCrtAndUpdVM != null)
                {
                    // Define the root physical folder where images are stored
                    string sourceFolderPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

                    // Build the full path for the photo image
                    string sourcePathPhoto = Path.Combine(sourceFolderPhy, "Photo", basicDetailCrtAndUpdVM.PhotoImagePath);

                    // Decrypt the photo image and assign it to the VM
                    basicDetailCrtAndUpdVM.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);

                    // Build the full path for the signature image
                    string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", basicDetailCrtAndUpdVM.SignatureImagePath);

                    // Decrypt the signature image and assign it to the VM
                    basicDetailCrtAndUpdVM.ExistingSignatureInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);

                    // Return the VM as JSON
                    response.Result = true;
                    response.Message ="Success";
                    response.Value= basicDetailCrtAndUpdVM;
                    return Json(response);
                }
                else
                {
                    // Return null if no record was found for the given RequestId
                    response.Result = false;
                    response.Message = "RequestId not found.";
                    response.Value = null;
                    return Json(response);
                }
            }
            catch (FileNotFoundException ex)
            {
                // Log any exception with an error code and context
                _logger.LogError(1001, ex, "BasicDetail->GetICardPrintPreviewByRequestId");
                response.Result = false;
                response.Message = "Photo and Signature not found.";
                response.Value = null;
                return Json(response);
            }
            catch (Exception ex)
            {
                // Log any exception with an error code and context
                _logger.LogError(1001, ex, "BasicDetail->GetICardPrintPreviewByRequestId");
                response.Result = false;
                response.Message = "Internal Error.";
                response.Value = null;
                return Json(response);
            }

        }


        /// <summary>
        /// Retrieves the basic detail record for a given RequestId.
        /// Secured with the "FlagICardApplPolicy" authorization policy.
        /// </summary>
        /// <param name="RequestId">The RequestId of the I-Card request.</param>
        /// <returns>Returns JSON containing the basic detail record, or internal server error on exception.</returns>
        [Authorize(Policy = "FlagICardApplPolicy")]
        [HttpPost]
        public async Task<IActionResult> GetBDetailByRequestId(int RequestId)
        {
            try
            {
                // Call business layer to get the basic detail by RequestId and return as JSON
                return Json(await basicDetailBL.GetBDetailByRequestId(RequestId));
            }
            catch (Exception ex)
            {
                // Log any exception with an error code and context
                _logger.LogError(1001, ex, "BasicDetail->GetBDetailByRequestId");

                // Return a generic internal server error key as JSON
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves the top (latest) Army Number entry from I-Card requests for the given ArmyNo.
        /// Secured with the "FlagICardApplPolicy" authorization policy.
        /// </summary>
        /// <param name="ArmyNo">The Army Number to search for in I-Card requests.</param>
        /// <returns>Returns JSON containing the top Army Number entry, or internal server error on exception.</returns>
        [Authorize(Policy = "FlagICardApplPolicy")]
        [HttpPost]
        public async Task<IActionResult> GetTopArmyNoFromICardRequest(string ArmyNo)
        {
            try
            {
                // Call business layer to get the latest ArmyNo from I-Card requests and return as JSON
                return Json(await basicDetailBL.GetTopArmyNoFromICardRequest(ArmyNo));
            }
            catch (Exception ex)
            {
                // Log any exception with an error code and context
                _logger.LogError(1001, ex, "BasicDetail->GetTopArmyNoFromICardRequest");

                // Return a generic internal server error key as JSON
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Displays the I-Card requests currently on hold for the logged-in user.
        /// Secured with the "ViewFlaggedICardApplPolicy" authorization policy.
        /// </summary>
        /// <returns>Returns the ICardRequestHold view populated with the user's claims.</returns>
        [Authorize(Policy = "ViewFlaggedICardApplPolicy")]
        [HttpGet]
        public async Task<IActionResult> ICardRequestHold()
        {
            // Retrieve the currently logged-in user's ID from claims
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the user object using the UserManager service
            var user = await userManager.FindByIdAsync(userId);

            // Retrieve all claims associated with this user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Store the claims in ViewBag to make them available to the view
            ViewBag.UserClaims = UserClaims;

            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Retrieves all I-Card requests that are currently on hold, based on the DataTables request parameters.
        /// Secured with the "ViewFlaggedICardApplPolicy" authorization policy.
        /// </summary>
        /// <param name="dTO">The DataTables request object containing paging, sorting, and filtering parameters.</param>
        /// <returns>Returns a JSON result containing the list of I-Card requests on hold, formatted for DataTables.</returns>
        [Authorize(Policy = "ViewFlaggedICardApplPolicy")]
        [HttpPost]
        public async Task<IActionResult> GetAllICardRequestHold(DTODataTablesRequest dTO)
        {
            try
            {
                // Call the business layer to get all I-Card requests on hold based on the DataTables request
                return Json(await basicDetailBL.GetAllICardRequestHold(dTO));
            }
            catch (Exception ex)
            {
                // In case of an exception, create an empty response for DataTables
                List<DTOICardRequestHoldResponse> dTODispatchCardLists = new List<DTOICardRequestHoldResponse>();
                var responseData = new DTODataTablesResponse<DTOICardRequestHoldResponse>
                {
                    draw = 0,                 // Draw counter for DataTables
                    recordsTotal = 0,         // Total records count
                    recordsFiltered = 0,      // Filtered records count
                    data = dTODispatchCardLists // Empty data list
                };

                // Log the exception with an error code and method context
                _logger.LogError(1001, ex, "BasicDetail->GetAllICardRequestHold");

                // Return the empty response as JSON
                return Json(responseData);
            }
        }


        #endregion

        #region CreateFolder:-GetCreateMyFolder/GetCreateMyFolder/ForCreateFolderrandom/CreateFolder/CreateZipFromFolder
        /// <summary>
        /// Creates a folder hierarchy based on the current date inside the specified base folder.
        /// Format: baseFolder/yyyy/MMMM/dd
        /// </summary>
        /// <param name="baseFolder">The root folder in which the date-based folder structure will be created.</param>
        /// <returns>Returns a <see cref="DirectoryInfo"/> object representing the created folder.</returns>
        public static DirectoryInfo GetCreateMyFolder(string baseFolder)
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MMMM");
            var dayName = now.ToString("dd");

            // Build the full folder path: baseFolder/yyyy/MMMM/dd
            var folder = Path.Combine(baseFolder, Path.Combine(yearName, Path.Combine(monthName, dayName)));

            // Create the folder (if it already exists, returns the existing DirectoryInfo)
            return Directory.CreateDirectory(folder);
        }

        /// <summary>
        /// Creates a folder hierarchy based on the current date in the current working directory.
        /// Format: yyyy/MMMM/dd
        /// </summary>
        /// <returns>Returns a <see cref="DirectoryInfo"/> object representing the created folder.</returns>
        public static DirectoryInfo GetCreateMyFolder()
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MMMM");
            var dayName = now.ToString("dd");

            // Build the folder path: yyyy/MMMM/dd
            var folder = Path.Combine(yearName, Path.Combine(monthName, dayName));

            // Create the folder
            return Directory.CreateDirectory(folder);
        }

        /// <summary>
        /// Creates a uniquely named folder based on the current date, time, and domain ID.
        /// Format: baseFolder/ddMMyyyy_HHmmss_DoaminId
        /// </summary>
        /// <param name="baseFolder">The root folder in which the folder will be created.</param>
        /// <param name="DoaminId">A domain identifier to append to the folder name.</param>
        /// <returns>Returns a <see cref="DirectoryInfo"/> object representing the created folder.</returns>
        public static DirectoryInfo ForCreateFolderrandom(string baseFolder, string DoaminId)
        {
            var now = DateTime.Now;
            var yearName = now.ToString("yyyy");
            var monthName = now.ToString("MM");
            var dayName = now.ToString("dd");
            var hh = now.ToString("HH");
            var mm = now.ToString("mm");
            var ss = now.ToString("ss");

            // Build folder name: ddMMyyyy_HHmmss_DoaminId
            var folder = Path.Combine(baseFolder, $"{dayName}{monthName}{yearName}_{hh}{mm}{ss}_{DoaminId}");

            // Create the folder
            return Directory.CreateDirectory(folder);
        }

        /// <summary>
        /// Creates a folder at the specified path.
        /// </summary>
        /// <param name="baseFolder">The full path of the folder to create.</param>
        /// <returns>Returns a <see cref="DirectoryInfo"/> object representing the created folder.</returns>
        public static DirectoryInfo CreateFolder(string baseFolder)
        {
            return Directory.CreateDirectory(baseFolder);
        }

        /// <summary>
        /// Creates a ZIP archive from the specified folder.
        /// </summary>
        /// <param name="sourceFolder">The folder whose contents will be zipped.</param>
        /// <param name="zipFilePath">The full path (including filename) of the ZIP archive to create.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when the source folder does not exist.</exception>
        public void CreateZipFromFolder(string sourceFolder, string zipFilePath)
        {
            if (Directory.Exists(sourceFolder))
            {
                // Create a ZIP archive from the folder with fastest compression
                // includeBaseDirectory = true to include the root folder inside the ZIP
                ZipFile.CreateFromDirectory(sourceFolder, zipFilePath, CompressionLevel.Fastest, true);
            }
            else
            {
                throw new DirectoryNotFoundException($"Source folder not found: {sourceFolder}");
            }
        }

        #endregion

        #region CSVFileUpload/ICardPrintUploadCsv/ICardPrintValidRecordsUpload

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
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

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

                if (role == "user")
                {
                    return Task.FromResult<ActionResult>(View()); // Return the view with decoded string
                }
                else
                {
                    TempData["error"] = "Switch to user role.";
                    TempData.Keep("error");
                    return Task.FromResult<ActionResult>(RedirectToAction("ContactUs", "Home")); // Redirect to "ContactUs" page
                }
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
        /// Uploads a CSV file containing I-Card printing requests, validates the data,
        /// saves the file both with and without remarks, and stores import metadata in the database.
        /// </summary>
        /// <param name="model">DTO containing the uploaded CSV file.</param>
        /// <returns>
        /// Returns a JSON response containing validation results, total records, valid records,
        /// invalid records, and the saved file name.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> ICardPrintUploadCsv(DTOCSVFileRequest model)
        {
            // Initialize response object
            var response = new DTOCsvUploadValResponse();

            // Check model validation
            if (!ModelState.IsValid)
            {
                // Collect all model validation errors
                var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                            .SelectMany(x => x.Value!.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();
                if (errors.Any())
                {
                    response.Message = string.Join("; ", errors); // Concatenate all error messages
                }
                goto Returnstm; // Skip the rest if model is invalid
            }

            // Generate a unique filename using current timestamp
            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.csv";

            try
            {
                var records = new List<DTOCardPriningRequest>();

                // Read CSV file using CsvHelper
                using (var reader = new StreamReader(model.CSVFile.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    // Register class map to map CSV columns to DTO
                    csv.Context.RegisterClassMap(new CsvClassMap<DTOCardPriningRequest>(true));

                    try
                    {
                        // Parse CSV records into DTO list
                        records = csv.GetRecords<DTOCardPriningRequest>().ToList();
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError(1001, ee, "BasicDetail->ICardPrintUploadCsv");
                        response.Result = false;
                        response.Message = "Internal Server Error!";
                        goto Returnstm; // Exit on parsing error
                    }
                }

                #region Upload File Without Remarks
                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "CardPrinitngCSVs", "CSVWithoutRemarks");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Create directory if it does not exist
                }
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the original CSV file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CSVFile.CopyToAsync(stream);
                }
                #endregion Upload File Without Remarks

                // Validate the CSV records
                var validateResult = await basicDetailBL.ValidateCardPrinitng(records);

                // Update response with validation statistics
                response.Result = true;
                response.TotalRecords = validateResult.Count();
                response.ValidRecords = validateResult.Count(x => x.IsValid);
                response.SheetInValidRecords = validateResult.Count(x => x.Status == "SheetInValid");
                response.DbInValidRecords = validateResult.Count(x => x.Status == "DbInvalid");

                #region Upload File With Remarks
                uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "CardPrinitngCSVs", "CSVWithRemarks");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Create directory if missing
                }
                filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file again for CSV with remarks
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CSVFile.CopyToAsync(stream);
                }

                // Write the validated records into the CSV with remarks
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
                        goto Returnstm; // Exit on write error
                    }
                }
                #endregion Upload File With Remarks

                #region Insert record metadata into DB
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

                // Store the import ID in session for further reference
                SessionHeplers.SetObject(HttpContext.Session, "CsvImportId", csvImportInsert.Id);
                #endregion Insert record

                // Store valid records in session for immediate usage
                SessionHeplers.SetObject(HttpContext.Session, "ValidRecordsCardUpload", validateResult.Where(v => v.IsValid).ToList());

                // Return the generated file name
                response.FileName = fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetail->ICardDistibutionUploadCsv");
                response.Message = "Internal Server Error!";
            }

        Returnstm:
            // Return the final JSON response
            return Json(response);
        }


        /// <summary>
        /// Uploads the valid I-Card printing records stored in session to the database.
        /// Updates the CSV import record to mark it as processed in the DB.
        /// </summary>
        /// <returns>
        /// Returns a JSON response containing the upload result and messages.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> ICardPrintValidRecordsUpload()
        {
            // Initialize response object
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();

            try
            {
                // Retrieve valid records from session
                var records = SessionHeplers.GetObject<List<DTOCardPriningRequest>>(HttpContext.Session, "ValidRecordsCardUpload");

                if (records?.Count() > 0)
                {
                    // Upload valid records using business logic layer
                    response = await basicDetailBL.CardPrinitngCSVUpload(records);
                }
                else
                {
                    // No valid records found in session
                    response.Message = "There are no valid records!";
                }

                // Retrieve CSV import ID from session
                var csvImportId = SessionHeplers.GetObject<int>(HttpContext.Session, "CsvImportId");

                // Fetch CSV import details from DB
                var getCsvDetById = await _iCSVImportBL.Get(csvImportId);

                // Mark the CSV import as processed
                getCsvDetById.DBUpdated = true;

                // Update CSV import record in DB
                await _iCSVImportBL.Update(getCsvDetById);
            }
            catch (Exception ee)
            {
                // Log any errors that occur during upload
                _logger.LogError(1001, ee, "BasicDetail->ICardPrintValidRecordsUpload");

                // Set response message to indicate error
                response.Message = "Internal Server Error!";
            }

            // Return JSON response to client
            return Json(response);
        }

        #endregion ICard Printing

        #region DestructionCard
        /// <summary>
        /// Displays the Destruction Card view.
        /// </summary>
        /// <returns>
        /// Returns the Destruction Card view to the client.
        /// </returns>
        [HttpGet]
        public IActionResult DestructionCard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Retrieves all destruction card records based on the provided DataTables request parameters.
        /// </summary>
        /// <param name="dTO">The DataTables request object containing paging, sorting, and filtering info.</param>
        /// <returns>
        /// Returns a JSON result containing the destruction card records.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetAllDestruction(DTODataTablesRequestForCommanCheckAll dTO)
        {
            // If an exception occurs, return an empty response to avoid breaking the UI
            List<DTODestructionCardGetResponse> dTODestructions = new List<DTODestructionCardGetResponse>();
            var responseData = new DTODataTablesWithSelectedIdsResponse<DTODestructionCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                selectedIds = null,
                data = dTODestructions
            };
            try
            {
                if (ModelState.IsValid)
                {
                    // Call the business layer to fetch all destruction card records and return as JSON
                    return Json(await _destructionCardBL.GetAllDestruction(dTO));
                }
                else
                {
                    return Json(responseData);
                }

            }
            catch (Exception ex)
            {
                // Log the exception for debugging and tracking
                _logger.LogError(1001, ex, "BasicDetail->GetAllDestruction");

                // Return JSON with empty data
                return Json(responseData);
            }
        }


        /// <summary>
        /// Exports destruction card data for the given request IDs into a CSV file.
        /// </summary>
        /// <param name="req">DTO containing the request IDs to export.</param>
        /// <returns>
        /// Returns a JSON object with the result status and the CSV file name if successful.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DestructionDataExport([FromBody] DTOHotlistCardsExportRequest req)
        {
            // Response object to return status and message
            DTOCommonSaveResponse dTOFaulty = new DTOCommonSaveResponse();
            try
            {
                // Create a temporary CSV file path
                var tempFileName = Path.GetTempFileName().Replace(".tmp", ".csv");

                // Fetch destruction card records based on the provided request IDs
                var records = await _destructionCardBL.GetDetailsByRequestIds(req);

                // Write the records to CSV file
                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Register class map for proper CSV column mapping
                    csv.Context.RegisterClassMap(new CsvClassMap<DTODestructionCardExportResponse>(true, CsvClassMapTypeEnum.HotlistExport));
                    try
                    {
                        // Write all records asynchronously to CSV
                        await csv.WriteRecordsAsync(records);
                    }
                    catch (Exception ee)
                    {
                        // Log error and set response in case of CSV writing failure
                        _logger.LogError(1001, ee, "BasicDetail->DestructionDataExport");
                        dTOFaulty.Result = false;
                        dTOFaulty.Message = "Internal Server Error!";
                        goto ReturnSt;
                    }
                }

                // Set success response with generated file name
                dTOFaulty.Result = true;
                dTOFaulty.Message = Path.GetFileName(tempFileName);
            }
            catch (Exception ex)
            {
                // Log any general exception and set response
                _logger.LogError(1001, ex, "BasicDetail->DestructionDataExport");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

        ReturnSt:
            // Return the response as JSON
            return Json(dTOFaulty);
        }

        /// <summary>
        /// Returns the view for destruction card requests.
        /// </summary>
        /// <returns>ViewResult containing the Destruction Card Request page.</returns>
        [HttpGet]
        public ActionResult DestructionCardRequest()
        {
            // Simply return the view associated with destruction card requests
            return View();
        }


        /// <summary>
        /// Saves a destruction card request. Validates the model, checks for duplicates,
        /// and inserts the record if valid. Returns the operation result as JSON.
        /// </summary>
        /// <param name="model">The destruction card model to save.</param>
        /// <returns>JSON containing the result, messages, record ID, and timestamp.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveDestructionCardRequest(DTOTrnDestructionCardSaveRequest dTOTrnDestruction)
        {
            // Initialize the response object for front-end
            var dTOFaulty = new DTOGenericResponse<DTOCommonResponse>();

            try
            {
                // Check if the model passed server-side validation
                if (ModelState.IsValid)
                {
                    var checkCardBeforeDistruction = await _destructionCardBL.CheckBeforeDestructionCardReport(dTOTrnDestruction.RequestId);
                    if (checkCardBeforeDistruction.Result)
                    {
                        // Initialize session DTO
                        DtoSession? dtoSession = new DtoSession();

                        // Retrieve session token if available
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                        {
                            dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                        }
                        var model = new TrnDestructionCard();
                        model.RequestId = dTOTrnDestruction.RequestId;
                        model.RemarksIds = dTOTrnDestruction.RemarksIds != null && dTOTrnDestruction.RemarksIds.Any() ? string.Join(",", dTOTrnDestruction.RemarksIds) : string.Empty;
                        model.Remark = dTOTrnDestruction.Remark;
                        model.DestructedOn = dTOTrnDestruction.DestructedOn;
                        model.IsActive = true;
                        model.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        model.UpdatedbyUserId = dtoSession != null ? dtoSession.UserId : 0;
                        model.UpdatedOn = DateTime.Now;

                        // Add the new destruction card and return success response
                        var result = await _destructionCardBL.AddWithReturn(model);
                        dTOFaulty.Result = true;
                        dTOFaulty.Message = "Record created!";
                        dTOFaulty.Value.CurrentTime = result.UpdatedOn.GetValueOrDefault();
                        dTOFaulty.Value.Id = result.DestructedCardId.ToString();
                    }
                    else
                    {
                        // Duplicate found, return failure response
                        dTOFaulty.Result = checkCardBeforeDistruction.Result;
                        dTOFaulty.Message = checkCardBeforeDistruction.Message;
                    }
                }
                else
                {
                    // Collect and return all model validation errors
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e =>
                            $"{x.Key}: {e.ErrorMessage}"))
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
                // Log exception and return generic error response
                _logger.LogError(1001, ex, "BasicDetail->SaveDestructionCardRequest");
                dTOFaulty.Result = false;
                dTOFaulty.Message = "Internal Server Error!";
            }

            // Return JSON response to the client
            return Json(dTOFaulty);
        }

        #endregion DestructionCard

        #region Dispatch
        /// <summary>
        /// Retrieves the user details including ID and name for a given ASP.NET Users ID.
        /// </summary>
        /// <param name="AspNetUsersId">The ID of the ASP.NET user.</param>
        /// <returns>JSON containing a generic response with user details.</returns>
        [HttpPost]
        public async Task<IActionResult> GetUserIdWithName(int AspNetUsersId)
        {
            // Initialize the generic response object
            DTOGenericResponse<DTODispatchToResponse?> response = new DTOGenericResponse<DTODispatchToResponse?>();

            // Call the business layer to get user details by ID
            response = await basicDetailBL.GetUserIdWithName(AspNetUsersId);

            // Return the result as HTTP 200 OK with JSON payload
            return Ok(response);
        }


        /// <summary>
        /// Retrieves dispatch-to data based on category and regiment ID.
        /// </summary>
        /// <param name="CategeryId">The category ID for the dispatch.</param>
        /// <param name="RecordRegimentId">The regiment ID for the dispatch record.</param>
        /// <returns>JSON containing a generic response with dispatch-to details.</returns>
        [HttpPost]
        [Authorize(Policy = "ICardExportDataPolicy")]
        public async Task<IActionResult> GetDispatchToData(string CategeryIds, string RecordRegimentIds)
        {
            DTOGenericResponse<DTODispatchToResponse?> response = new DTOGenericResponse<DTODispatchToResponse?>();
            byte CategeryId= await AESEncrytDecry.DecryptAESWithDTO<byte>(CategeryIds, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            byte RecordRegimentId = await AESEncrytDecry.DecryptAESWithDTO<byte>(RecordRegimentIds, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            if (CategeryId == 0 || RecordRegimentId == 0)
                return BadRequest(response);
            // Initialize the generic response object
            

            // Call the business layer to get dispatch-to details
            response = await basicDetailBL.GetDispatchToData(CategeryId, RecordRegimentId);

            // Return the result as HTTP 200 OK with JSON payload
            return Ok(response);
        }


        /// <summary>
        /// Retrieves a dropdown list of record regiments for a given ToUnitId based on user claims and session.
        /// </summary>
        /// <param name="ToUnitId">The target unit ID for which regiments are fetched.</param>
        /// <returns>JSON containing a generic response with regiment and unit details.</returns>
        [HttpPost]
        [Authorize(Policy = "ddlRecordRegimentPolicy")]
        public async Task<IActionResult> GetddlRecordRegiment(int ToUnitId)
        {
            // Initialize the session object
            DtoSession? dtoSession = new DtoSession();

            // Initialize the generic response object
            DTOGenericResponse<DTOOROWithRegimentAndUnitResponse> response = new DTOGenericResponse<DTOOROWithRegimentAndUnitResponse>();

            // Initialize a default return object
            DTOOROWithRegimentAndUnitResponse ret = new DTOOROWithRegimentAndUnitResponse();

            // Check if session token exists and fetch it
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            }

            if (dtoSession != null)
            {
                // Get current logged-in user's ID
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch user object using UserManager
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                byte ClaimValue;

                // Get all claims associated with the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                // Check if user has both "Dispatch Card" and "Appl Approver" claims
                if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                {
                    ClaimValue = 2;

                    // Call business layer to fetch record regiments
                    response = await basicDetailBL.GetddlRecordRegiment(ClaimValue, dtoSession.TrnDomainMappingId, dtoSession.UnitId, ToUnitId);
                    return Ok(response);
                }
                // Check if user has only "Dispatch Card" claim
                else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                {
                    ClaimValue = 3;

                    // Call business layer to fetch record regiments
                    response = await basicDetailBL.GetddlRecordRegiment(ClaimValue, dtoSession.TrnDomainMappingId, dtoSession.UnitId, ToUnitId);
                    return Ok(response);
                }
                else
                {
                    // User does not have required claims; return error response
                    response.Result = false;
                    response.Message = "An error occurred while fetching data.";
                    response.Value = ret;
                    return Ok(response);
                }
            }
            else
            {
                // Session token not found; return error response
                response.Result = false;
                response.Message = "An error occurred while fetching data.";
                response.Value = ret;
                return Ok(response);
            }
        }


        /// <summary>
        /// Loads the Dispatch Out page based on session data and user claims.
        /// </summary>
        /// <returns>View with appropriate ClaimValue or redirects to error page if session/user invalid.</returns>
        [Authorize(Policy = "ICardDispatchPolicy")]
        [HttpGet]
        public async Task<ActionResult> DispatchOut()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Retrieve session object containing previous search/filter parameters
            DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 =
                SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");

            if (dTOTempSession1 != null)
            {
                // Pass search/filter parameters to the view
                ViewBag.SearchField = dTOTempSession1.SearchField;
                ViewBag.SearchText = dTOTempSession1.SearchText;

                // Get currently logged-in user's ID
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                // Get all claims associated with the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                // Check user claims and set appropriate ClaimValue for the view
                if (UserClaims.Count > 0)
                {
                    if (UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        ViewBag.ClaimValue = 1; // User can export ICard data
                    }
                    else if (UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        ViewBag.ClaimValue = 2; // User is Dispatch Card + Application Approver
                    }
                    else if (UserClaims.Any(i => i.Value == "Dispatch Card"))
                    {
                        ViewBag.ClaimValue = 3; // User is Dispatch Card only
                    }

                    if (role == "user")
                    {
                        return View();
                    }
                    else
                    {
                        TempData["error"] = "Switch to user role.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                }
                else
                {
                    // User does not have required claims, redirect to ContactUs with error
                    TempData["error"] = "Invalid User.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            else
            {
                // Session has expired or not available, redirect to ContactUs with error
                TempData["error"] = "Invalid Session.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }


        /// <summary>
        /// Handles the dispatch of cards based on user role and session data.
        /// This method validates the dispatch request, generates CSV with remarks,
        /// uploads valid records, and returns summary information.
        /// </summary>
        /// <param name="dTO">The DTO containing dispatch request data from the form.</param>
        /// <returns>
        /// JSON result containing dispatch summary, file information, or error messages.
        /// </returns>
        [HttpPost]
        [Authorize(Policy = "ICardDispatchPolicy")]
        public async Task<ActionResult> DispatchOut(string request)
        {
            DTOGenericResponse<DTOCardDispatchCheckResponse> response = new DTOGenericResponse<DTOCardDispatchCheckResponse>();
            DTODispatchOutRequest dTO = await AESEncrytDecry.DecryptAESWithDTO<DTODispatchOutRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            // Retrieve temporary session object for DispatchLot
            if(dTO==null)
            {
                // Unauthorized user
                response.Result = false;
                response.Message = "Invalid Data.";
                response.Value = null;
                return Ok(response);
            }
            TryValidateModel(dTO);
            DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 = SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");
            bool Valid = false;
            if (dTOTempSession1 != null)
            {
                DtoSession? dtoSession = new DtoSession();
               
                DTOCardDispatchCheckResponse ret = new DTOCardDispatchCheckResponse();

                // Get the main user session token if available
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                if (dtoSession != null)
                {
                    // Get the current user's ID from claims
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());
                    byte ClaimValue = 0;

                    // Set timestamps and metadata for the dispatch
                    dTO.OutDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    dTO.FromAspNetUsersId = AspNetUsersId;
                    dTO.FromUserId = dtoSession.UserId;
                    dTO.FromUnitId = dtoSession.UnitId;
                    dTO.FromTDMId = dtoSession.TrnDomainMappingId;
                    dTO.IsActive = true;
                    dTO.IsComplete = false;
                    dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    dTO.Updatedby = AspNetUsersId;

                    if (ModelState.IsValid)
                    {
                        // Get all claims of the current user
                        var UserClaims = await userManager.GetClaimsAsync(user);

                        // Determine claim type and set corresponding step value
                        if (UserClaims.Count > 0)
                        {
                            if (UserClaims.Any(i => i.Value == "ICard Export Data"))
                            {
                                dTO.RecordOfficeId = dTO.ApplyForId == 1 ? dTO.RecordOfficeId : null;
                                dTO.RegId = dTO.ApplyForId == 2 ? dTO.RegId : null;
                                dTO.Step = 1;
                                ClaimValue = 1;
                            }
                            else if (UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                            {
                                dTO.ApplyForId = 1;
                                dTO.Step = 2;
                                dTO.RegId=null;
                                ClaimValue = 2;
                            }
                            else if (UserClaims.Any(i => i.Value == "Dispatch Card"))
                            {
                                dTO.ApplyForId = 2;
                                dTO.Step = 2;
                                dTO.RecordOfficeId = null;
                                ClaimValue = 3;
                            }

                        }
                        else
                        {
                            // Unauthorized user
                            response.Result = false;
                            response.Message = "Unauthorized User.";
                            response.Value = ret;
                            return Ok(response);
                        }

                        //Check data before proceeding to dispatch and get validation results

                        if (ClaimValue == 1)
                        {
                            DTOGenericResponse<DTODispatchToResponse?> dTOGeneric = new DTOGenericResponse<DTODispatchToResponse?>();

                            if (dTO.ApplyForId == 1) //Officer
                            {
                                if (dTO.RecordOfficeId == null)
                                {
                                    response.Result = false;
                                    response.Message = "The Record Office field is required when dispatch lot.";
                                    response.Value = ret;
                                    return Ok(response);
                                }
                                else
                                {
                                    dTOGeneric = await basicDetailBL.GetDispatchToData(dTO.ApplyForId, Convert.ToInt32(dTO.RecordOfficeId));
                                    if (dTOGeneric.Result == true && dTOGeneric.Value != null)
                                    {
                                        if (dTOGeneric.Value.UnitId == dTO.ToUnitId && dTOGeneric.Value.AspNetUsersId == dTO.ToAspNetUsersId && dTOGeneric.Value.UserId == dTO.ToUserId)
                                        {
                                            Valid = true;
                                        }
                                        else
                                        {
                                            Valid = false;
                                        }
                                    }
                                    else
                                    {
                                        Valid = false;
                                    }

                                    if (Valid == false)
                                    {
                                        response.Result = false;
                                        response.Message = "Invalid Officr Record Office Id.";
                                        response.Value = ret;
                                        return Ok(response);
                                    }
                                }
                            }
                            else // JCO/ORO
                            {
                                if (dTO.RegId == null)
                                {
                                    response.Result = false;
                                    response.Message = "The Regiment field is required when dispatch lot.";
                                    response.Value = ret;
                                    return Ok(response);
                                }
                                else
                                {
                                    dTOGeneric = await basicDetailBL.GetDispatchToData(dTO.ApplyForId, Convert.ToInt32(dTO.RegId));
                                    if (dTOGeneric.Result == true && dTOGeneric.Value != null)
                                    {
                                        if (dTOGeneric.Value.UnitId == dTO.ToUnitId && dTOGeneric.Value.AspNetUsersId == dTO.ToAspNetUsersId && dTOGeneric.Value.UserId == dTO.ToUserId)
                                        {
                                            Valid = true;
                                        }
                                        else
                                        {
                                            Valid = false;
                                        }
                                    }
                                    else
                                    {
                                        Valid = false;
                                    }

                                    if (Valid == false)
                                    {
                                        response.Result = false;
                                        response.Message = "Invalid Regiment Id.";
                                        response.Value = ret;
                                        return Ok(response);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dTO.ToUnitId == 0)
                            {
                                response.Result = false;
                                response.Message = "The Unit Id is required when you dispatch lot.";
                                response.Value = ret;
                                return Ok(response);
                            }
                            else
                            {
                                // Verifiy Sender
                                if (ClaimValue == 2) 
                                {
                                    Valid = await oROMappingBL.ValidateTDMIdInOROMapping(dTO.FromTDMId);
                                }
                                else if (ClaimValue == 3)
                                {
                                    Valid = await regimentalBL.ValidateUnitIdInRegimental(dTO.FromUnitId);
                                }

                                if (Valid == false)
                                {
                                    response.Result = false;
                                    response.Message = "Unauthorized User.";
                                    response.Value = ret;
                                    return Ok(response);
                                }

                                //Validate User Id based of ToUnitId
                                Valid = false;
                                List<DTOGetMappedForRecordResponse>? dTOGets =await recordOfficeBL.GetDDMappedForRecord(dTO.ToUnitId);
                                if (dTOGets != null && dTOGets.Count > 0)
                                {
                                    foreach (var item in dTOGets)
                                    {
                                        if (item.AspNetUsersId == dTO.ToAspNetUsersId && item.UserId == dTO.ToUserId)
                                        {
                                            Valid = true;
                                            break;
                                        }
                                    }
                                }

                                if (Valid == false)
                                {
                                    response.Result = false;
                                    response.Message = "Invalid User.";
                                    response.Value = ret;
                                    return Ok(response);
                                }
                            }
                        }

                        // Generate a timestamped CSV file name
                        string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
                        try
                        {
                            var records = new List<DTOCardDispatchCheckRequest>();

                            // Validate dispatch data based on claim value and session data
                            var validateResult = await basicDetailBL.ValidateCardDispatchData(dTOTempSession1.RequestIds, ClaimValue, dTO);

                            // Set summary statistics
                            ret.TotalRecords = validateResult.Count();
                            ret.ValidRecords = validateResult.Where(x => x.IsValid).Count();
                            ret.DbInValidRecords = validateResult.Where(x => x.Status == "DbInvalid").Count();

                            response.Result = true;
                            response.Value = ret;

                            if (ret.ValidRecords > 0)
                            {
                                #region Upload File With Remarks
                                // Define folder to save CSV files with remarks
                                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "CardDispatchCSVs", "CSVWithRemarks");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                // Write validated records to CSV
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
                                ret.FileName = fileName;

                                DTOGenericResponse<string> response1 = new DTOGenericResponse<string>();
                                var ValidRecords = validateResult.Where(x => x.IsValid).ToList();

                                // Upload valid records to database
                                response1 = await basicDetailBL.CardDispatchCSVUpload(ValidRecords, dTO);
                                if (response1.Result == true)
                                {
                                    ret.LotNo = Convert.ToInt32(response1.Message);
                                    response.Result = response1.Result;
                                    response.Message = response1.Message;
                                    response.Value = ret;

                                    // Remove temporary session
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
                                // No valid records found
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
                        var errors = ModelState
                                    .Where(x => x.Value?.Errors?.Count > 0)
                                    .SelectMany(x => x.Value!.Errors.Select(e =>
                                        $"{x.Key}: {e.ErrorMessage}"))
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
                    // Session expired or token missing
                    response.Result = false;
                    response.Message = "Session Timeout.";
                    response.Value = ret;
                    return Ok(response);
                }
            }
            else
            {
                // DispatchLot session is invalid
                TempData["error"] = "Invalid Session / Session Timeout.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Determines the dispatch card view based on the current user's claims.
        /// Sets a ClaimValue in ViewBag according to the user's permissions.
        /// </summary>
        /// <returns>
        /// Returns the DispatchCard view with the appropriate ClaimValue.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> DispatchCard()
        {
            // Retrieve the user's role from the session
            string role = SessionHelper.GetRoleFromSession(HttpContext);


            // Get the current logged-in user's ID from claims
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Fetch the user object from UserManager using the retrieved ID
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

            // Retrieve all claims associated with the current user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Check if the user has the "ICard Export Data" claim
            if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
            {
                // User has export rights; set ClaimValue = 1
                ViewBag.ClaimValue = 1;
            }
            // Check if user has both "Dispatch Card" and "Appl Approver" claims
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
            {
                // User is a dispatch card approver; set ClaimValue = 2
                ViewBag.ClaimValue = 2;
            }
            // Check if user has only "Dispatch Card" claim
            else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
            {
                // User can dispatch cards but not approve; set ClaimValue = 3
                ViewBag.ClaimValue = 3;
            }
            else
            {
                // User has no relevant claims; set ClaimValue = 0
                ViewBag.ClaimValue = 0;
            }
            if (role == "user")
            {
                return View();
            }
            else
            {
                TempData["error"] = "Switch to user role.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }

        /// <summary>
        /// Retrieves all dispatch cards for the current user based on their claims and session data.
        /// Populates the ClaimValue, UnitId, and TrnDomainMappingId before querying the business layer.
        /// </summary>
        /// <param name="dTO">The DataTables request containing filters, paging, and sorting parameters.</param>
        /// <returns>
        /// Returns a JSON response containing the dispatch card list, total records, and filtered records.
        /// If the session is invalid or an exception occurs, returns an empty list response.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetAllDispatchCard(DTODataTablesRequestForCardDispatch dTO)
        {
            try
            {
                // Initialize a DTO for session data
                DtoSession? dtoSession = new DtoSession();

                // Check if session token exists and retrieve session object
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                // If session is valid, proceed
                if (dtoSession != null)
                {
                    // Get current logged-in user ID from claims
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    // Fetch user details from UserManager
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                    // Retrieve all claims associated with the user
                    var UserClaims = await userManager.GetClaimsAsync(user);

                    // Determine ClaimValue based on user's claims
                    if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        dTO.ClaimValue = 1; // User has export permissions
                    }
                    else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        dTO.ClaimValue = 2; // User can approve dispatch cards
                    }
                    else if (UserClaims.Count > 0 && UserClaims.Any(i => i.Value == "Dispatch Card"))
                    {
                        dTO.ClaimValue = 3; // User can dispatch cards only
                    }
                    else
                    {
                        dTO.ClaimValue = 0; // No relevant claims
                    }

                    // Populate session-related properties
                    dTO.UnitId = dtoSession.UnitId;
                    dTO.TDMId = dtoSession.TrnDomainMappingId;

                    // Call business layer to get dispatch card data and return JSON response
                    return Json(await basicDetailBL.GetAllDispatchCard(dTO));
                }
                else
                {
                    // Session is invalid or expired: return empty data response
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
                // Log any exceptions and return empty response to avoid breaking front-end
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


        /// <summary>
        /// Retrieves dispatch card data specifically for a dialog (modal/pop-up) based on the DataTables request.
        /// Calls the business layer method to fetch paginated, filtered, and sorted dispatch card data.
        /// </summary>
        /// <param name="dTO">The DataTables request object containing pagination, search, and filter criteria.</param>
        /// <returns>
        /// Returns a JSON response containing the dispatch card data for dialog. 
        /// If an exception occurs, returns an empty list with draw, recordsTotal, and recordsFiltered set to 0.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetDispatchCardDataForDialog(string request)
        {
            DTODataTablesRequestForCardDispatchDialog dTO= await AESEncrytDecry.DecryptAESWithDTO<DTODataTablesRequestForCardDispatchDialog>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            List <DTOCardDispatchDialogResponse> dTOCards = new List<DTOCardDispatchDialogResponse>();
            // If an exception occurs, return an empty response to avoid breaking the UI
            var responseData = new DTODataTablesWithSelectedIdsResponse<DTOCardDispatchDialogResponse>
            {
                draw = 0,               // DataTables draw counter (0 since error)
                recordsTotal = 0,       // Total records (0 since error)
                recordsFiltered = 0,    // Filtered records (0 since error)
                selectedIds = null,     // No selected IDs
                data = dTOCards         // Empty list of data
            };
            if (dTO == null)
                return BadRequest();
            try
            {
                TryValidateModel(dTO);
                if (ModelState.IsValid)
                {
                    // Get the current logged-in user's ASP.NET Identity Id
                    int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    // Fetch the user object from UserManager
                    var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                    // Retrieve all claims associated with the user
                    var UserClaims = await userManager.GetClaimsAsync(user);

                    DtoSession? dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    dTO.UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                    dTO.TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;

                    dTO.StepId = 2;
                    dTO.ClaimValue = 0; // User has no relevant claims
                    
                    if (UserClaims.Count > 0)
                    {
                        if (UserClaims.Any(i => i.Value == "ICard Export Data"))
                        {
                            dTO.StepId = 1;
                            dTO.ClaimValue = 1; // User can export ICard data
                        }
                        else if (UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                        {
                            dTO.ClaimValue = 2; // User is Dispatch Card and Application Approver
                            var recordRegiment = await basicDetailBL.GetRecordRegimentId(dTO.ClaimValue, dTO.TDMId, dTO.UnitId);
                            dTO.RecordOfficeId = recordRegiment?.Id ?? 0;
                        }
                        else if (UserClaims.Any(i => i.Value == "Dispatch Card"))
                        {
                            dTO.ClaimValue = 3; // User is only Dispatch Card role
                            var recordRegiment = await basicDetailBL.GetRecordRegimentId(dTO.ClaimValue, dTO.TDMId, dTO.UnitId);
                            dTO.RegId = recordRegiment?.Id ?? 0;
                        }
                    }
                    // Call business layer to retrieve dispatch card data for dialog
                    return Json(await basicDetailBL.GetDispatchCardDataForDialog(dTO));
                }
                else
                {
                    return Json(responseData);
                }

            }
            catch (Exception ex)
            {
                // Log the exception for debugging and tracking
                _logger.LogError(1001, ex, "BasicDetail->GetDispatchCardDataForDialog");
                // Return JSON with empty data
                return Json(responseData);
            }
        }


        /// <summary>
        /// Handles the "Dispatch Card In" action for a given dispatch card.
        /// Sets the receipt date, marks the card as complete, and processes the dispatch card step.
        /// Returns a <see cref="DTOGenericResponse{string}"/> with the result of the operation.
        /// </summary>
        /// <param name="dTO">The DTO containing Dispatch Card Id, remarks, and other metadata from the form.</param>
        /// <returns>An <see cref="ActionResult"/> containing a JSON response with success/failure and messages.</returns>
        [HttpPost]
        public async Task<ActionResult> DispatchCardIn([FromForm] DTODispatchInRequest dTO)
        {
            // Initialize the response object
            DTORecordRegimentIdResponse? dTORecordRegimentId = new DTORecordRegimentIdResponse();
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();

            // Set the receipt date to current India Standard Time
            dTO.ReceiptDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            // Mark the dispatch as complete
            dTO.IsComplete = true;

            // Validate the incoming model
            if (ModelState.IsValid)
            {
                try
                {
                    // Get current user and session data
                    var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = await userManager.FindByIdAsync(userId.ToString());

                    if (user == null)
                    {
                        response.Message = "User not found";
                        response.Result = false;
                        return Ok(response);
                    }

                    // Retrieve all claims associated with the user
                    var UserClaims = await userManager.GetClaimsAsync(user);
                    DtoSession? dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                    // Set session data
                    dTO.UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                    dTO.TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;

                    dTO.StepId = 2;
                    dTO.ClaimValue = 0;

                    if (UserClaims.Count > 0)
                    {
                        if (UserClaims.Any(i => i.Value == "ICard Export Data"))
                        {
                            dTO.StepId = 1;
                            dTO.ClaimValue = 1; // User can export ICard data
                        }
                        else if (UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                        {
                            dTO.ClaimValue = 2; // User is Dispatch Card and Application Approver
                            dTO.StepId = 1;
                            var recordRegiment = await basicDetailBL.GetRecordRegimentId(dTO.ClaimValue, dTO.TDMId, dTO.UnitId);
                            dTO.RecordOfficeId = recordRegiment?.Id ?? 0;
                        }
                        else if (UserClaims.Any(i => i.Value == "Dispatch Card"))
                        {
                            dTO.ClaimValue = 3; // User is only Dispatch Card role
                            dTO.StepId = 1;
                            var recordRegiment = await basicDetailBL.GetRecordRegimentId(dTO.ClaimValue, dTO.TDMId, dTO.UnitId);
                            dTO.RegId = recordRegiment?.Id ?? 0;
                        }
                    }

                    // Fetch the dispatch card from the database using the provided DispatchCardId
                    var dispatchCard = await dispatchCardBL.Get(dTO.DispatchCardId);

                    if (dispatchCard == null)
                    {
                        response.Message = "Invalid Id";
                        response.Result = false;
                        response.Value = string.Empty;
                        return Ok(response);
                    }

                    // Check if the action has already been performed
                    if (dispatchCard.IsComplete == true && dispatchCard.ReceiptDate != null)
                    {
                        response.Result = false;
                        response.Message = "Action has already been taken by you.";
                        response.Value = string.Empty;
                        return Ok(response);
                    }
                    
                    bool shouldProcess = false;

                    if (dTO.ClaimValue == 1)
                    {
                        response.Result = false;
                        response.Message = "You are not authorized to take Action.";
                        response.Value = string.Empty;
                        return Ok(response);
                    }
                    else if (dTO.ClaimValue == 2 && dispatchCard.Step == dTO.StepId && dispatchCard.RecordOfficeId == dTO.RecordOfficeId)
                    {
                        shouldProcess = true;
                    }
                    else if (dTO.ClaimValue == 3 && dispatchCard.Step == dTO.StepId && dispatchCard.RegId == dTO.RegId)
                    {
                        shouldProcess = true;
                    }
                    else if (dTO.ClaimValue == 0 && dispatchCard.Step == dTO.StepId && dispatchCard.ToUnitId == dTO.UnitId)
                    {
                        shouldProcess = true;
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "You are not authorized to take Action.";
                        response.Value = string.Empty;
                        return Ok(response);
                    }

                    if (shouldProcess)
                    {
                        // Determine the step ID based on the current step of the dispatch card
                        byte StepId = dispatchCard.Step == 1 ? (byte)12 : dispatchCard.Step == 2 ? (byte)14 : (byte)0;
                        // Get related requests
                        var dispatchRequests = (await dispatchCardMappingBL.GetRequestIds(dispatchCard.DispatchCardId)).ToList();

                        // Process the dispatch card using the business logic layer
                        response = await basicDetailBL.DispatchCardIn(
                            dispatchRequests,
                            StepId,
                            dTO.DispatchCardId,
                            dTO.ToRemark
                        );

                        // Clear the Value field in the response
                        response.Value = string.Empty;
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "You are not authorized to take Action.";
                        response.Value = string.Empty;
                        return Ok(response);
                    }

                }
                catch (Exception ex)
                {
                    // Log any unexpected exceptions
                    _logger.LogError(1001, ex, "BasicDetail->DispatchCardIn");
                    response.Message = "Internal Server Error!";
                    response.Result = false;
                    response.Value = string.Empty;
                }
            }
            else
            {
                // Extract validation errors from ModelState
                var errors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                if (errors.Any())
                {
                    // Concatenate all validation error messages
                    response.Message = string.Join("; ", errors);
                }

                response.Result = false;
                response.Value = string.Empty;
            }

            // Return the response as an HTTP 200 OK with JSON content
            return Ok(response);
        }


        /// <summary>
        /// Retrieves the dispatch card status list for the dialog based on the current user's claims.
        /// Determines the claim value of the user and fetches the corresponding dispatch card statuses.
        /// </summary>
        /// <param name="dTO">The data tables request object containing paging, filtering, and sorting parameters.</param>
        /// <returns>An <see cref="IActionResult"/> containing a JSON response with the dispatch card status list.</returns>
        [HttpPost]
        public async Task<IActionResult> GetDispatchCardStatusListForDialog([FromBody] DTODataTablesRequestForCardStatusList dTO)
        {
            try
            {
                // Get the current logged-in user's ASP.NET Identity Id
                int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch the user object from UserManager
                var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

                // Retrieve all claims associated with the user
                var UserClaims = await userManager.GetClaimsAsync(user);

                // Determine the claim value based on user's permissions
                byte ClaimValue;
                DtoSession? dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                dTO.TDMId= dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
                dTO.UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                ClaimValue = 0; // User has no relevant claims
                if (UserClaims.Count > 0)
                {
                    if (UserClaims.Any(i => i.Value == "ICard Export Data"))
                    {
                        ClaimValue = 1; // User can export ICard data
                    }
                    else if (UserClaims.Any(i => i.Value == "Dispatch Card") && UserClaims.Any(i => i.Value == "Appl Approver"))
                    {
                        ClaimValue = 2; // User is Dispatch Card and Application Approver
                    }
                    else if (UserClaims.Any(i => i.Value == "Dispatch Card"))
                    {
                        ClaimValue = 3; // User is only Dispatch Card role
                    }
                }

                // Call the business layer to get the dispatch card status list based on the claim value
                return Json(await basicDetailBL.GetDispatchCardStatusListForDialog(dTO, ClaimValue));
            }
            catch (Exception ex)
            {
                // Initialize an empty list of responses in case of an exception
                List<DTODispatchCardStatusResponse> dTOCards = new List<DTODispatchCardStatusResponse>();

                // Create a response object with zeroed metadata
                var responseData = new DTODataTablesWithSelectedIdsResponse<DTODispatchCardStatusResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOCards,
                    selectedIds = null
                };

                // Log the exception with a specific event id for easier tracing
                _logger.LogError(1001, ex, "BasicDetail->GetDispatchCardDataForDialog");

                // Return the empty response as JSON
                return Json(responseData);
            }
        }


        /// <summary>
        /// Exports the selected dispatch card data as a CSV file.
        /// Creates a temporary CSV file in the server and returns the file name.
        /// </summary>
        /// <param name="requestIdsWrapper">An object containing the list of request IDs to export.</param>
        /// <returns>An <see cref="IActionResult"/> containing a JSON response with the CSV file name or error message.</returns>
        [HttpPost]
        public async Task<IActionResult> ExportCsvFileForDispatchCard([FromBody] DTORequestIdForCSVRequest requestIdsWrapper)
        {
            // Initialize the list of DTOs that will hold CSV data
            List<DTODispatchCardForCSVResponse> dTOs = new List<DTODispatchCardForCSVResponse>();

            // Initialize a generic response object to return status and messages
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();

            // Extract the array of request IDs from the wrapper
            int[]? RequestIds = requestIdsWrapper.RequestIds;

            // Check if request IDs are provided
            if (RequestIds == null || RequestIds.Length == 0)
            {
                response.Result = false;
                response.Message = "No request IDs provided.";
                response.Value = string.Empty;
                return Json(response); // Return early if no IDs
            }

            // Generate a unique CSV file name based on current timestamp
            string fileName = $"CSVData_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";

            // Define the folder path where the CSV will be temporarily stored
            var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "DispatchExports", "Temp");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Combine folder path and file name to get full file path
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                // Fetch the dispatch card data for the given request IDs from the business layer
                dTOs = await basicDetailBL.ExportCsvFileForDispatchCard(RequestIds);

                // Write data to CSV using CsvHelper
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Register the class map for CSV export configuration
                    csv.Context.RegisterClassMap(new CsvClassMap<DTODispatchCardForCSVResponse>(true, CsvClassMapTypeEnum.CSVExport));

                    // Write all records asynchronously to the CSV
                    await csv.WriteRecordsAsync(dTOs);
                }

                // Update response object to indicate success
                response.Result = true;
                response.Message = "Ok";
                response.Value = Path.GetFileName(fileName); // Return only file name
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during processing
                _logger.LogError(1001, ex, "BasicDetail->ExportCsvFileForDispatchCard");

                // Set response as failed
                response.Result = false;
                response.Message = "Internal Server Error!";
                response.Value = string.Empty;
            }

            // Return the JSON response with result status, message, and CSV file name (if successful)
            return Json(response);
        }

        /// <summary>
        /// Stores or updates the DispatchLot session object before proceeding with dispatch operations.
        /// Ensures that any previous session data is removed before setting the new data.
        /// </summary>
        /// <param name="dTO">The request object containing dispatch check data.</param>
        /// <returns>An <see cref="IActionResult"/> containing a JSON response indicating success or failure.</returns>
        [HttpPost]
        [Authorize(Policy = "ICardDispatchPolicy")]
        public async Task<IActionResult> BeforeProceedToDispatchCheck(string request)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            //[FromBody] DTOBeforeProceedToDispatchCheckRequest dTO
            DTOBeforeProceedToDispatchCheckRequest dTO = await AESEncrytDecry.DecryptAESWithDTO<DTOBeforeProceedToDispatchCheckRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
           if(dTO==null)
            {
                // Model is invalid, set response accordingly
                response.Result = false;
                response.Message = "Invalid Data.";
                response.Value = string.Empty;
                return Json(response); // Return early if model validation fails
            }
            // Initialize the generic response object
            
            try
            {
                // Check if the incoming model is valid
                if (ModelState.IsValid)
                {
                    // Retrieve any existing DispatchLot session object
                    DTOBeforeProceedToDispatchCheckRequest? dTOTempSession1 =
                        SessionHeplers.GetObject<DTOBeforeProceedToDispatchCheckRequest>(HttpContext.Session, "DispatchLot");

                    // Remove the existing session object if it exists
                    if (dTOTempSession1 != null)
                    {
                        HttpContext.Session.Remove("DispatchLot");
                    }

                    // Store the new DispatchLot object in session
                    SessionHeplers.SetObject(HttpContext.Session, "DispatchLot", dTO);

                    // Update response to indicate success
                    response.Result = true;
                    response.Message = "Ok";
                    response.Value = string.Empty;
                }
                else
                {
                    // Model is invalid, set response accordingly
                    response.Result = false;
                    response.Message = "No request IDs provided.";
                    response.Value = string.Empty;
                    return Json(response); // Return early if model validation fails
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during session handling
                _logger.LogError(1001, ex, "BasicDetail->BeforeProceedToDispatchCheck");

                // Update response to indicate failure
                response.Result = false;
                response.Message = "Internal Server Error!";
                response.Value = string.Empty;
            }

            // Return JSON response to the client
            return Json(response);
        }

        /// <summary>
        /// Exports the Dispatch Card data to a CSV file for download.
        /// The CSV file is created under the "WriteReadData/Dispatchexports" folder in wwwroot.
        /// </summary>
        /// <param name="Data">The request object containing filters and parameters for export.</param>
        /// <returns>An <see cref="IActionResult"/> containing the file name of the generated CSV.</returns>
        [HttpPost]
        public async Task<IActionResult> ExportCsvForDispatch(DTOExportDispatch Data)
        {
            // Get the current user's ID from the authentication claims
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve the user object from the UserManager service
            var user = await userManager.FindByIdAsync(AspNetUsersId.ToString());

            // Get all claims associated with the current user
            var UserClaims = await userManager.GetClaimsAsync(user);

            // Determine the claim value based on user permissions
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

            // Create a unique file name for the CSV
            string fileName = $"UploadForDispatch_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";

            // Fetch the data to export from the business layer
            var ret = await basicDetailBL.GetDispatchCardStatusListForExport(ClaimValue, Data);

            // Initialize a StringBuilder to construct the CSV content
            var csv = new StringBuilder();

            // Add CSV header row
            csv.AppendLine("Name,ServiceNo,ChipNo,ApplId");

            // Add CSV data rows
            foreach (var item in ret.data)
            {
                // Combine rank and name, and wrap other fields in quotes
                csv.AppendLine($"{item.RankName + " " + item.NameAsPerRecord},\"{item.ServiceNo}\",\"{item.ChipNo}\",\"{item.RequestId}\"");
            }

            // Convert the CSV string into a byte array for writing to disk
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            // Define the folder path to save the CSV
            var folderPath = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Dispatchexports");

            // Create the folder if it does not exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Combine folder path and file name to get full file path
            var fullPath = Path.Combine(folderPath, fileName);

            // Write the CSV byte array to the file
            System.IO.File.WriteAllBytes(fullPath, bytes);

            // Return the generated file name as JSON
            return Json(fileName);
        }

        #endregion

        #region Set Session and Get Session
        /// <summary>
        /// Sets the session data with the provided DTO object.
        /// This method receives a `DTOEncypteDecryptedColumnRequest` object and stores it in the session under the key "DataSet".
        /// </summary>
        /// <param name="Data">The `DTOEncypteDecryptedColumnRequest` object containing data to be stored in session.</param>
        /// <returns>
        /// Returns `true` if the session data was successfully set, otherwise returns `false` in case of any exception.
        /// </returns>
        [HttpPost]
        public ActionResult DataSendForSetSession([FromBody] DTOEncypteDecryptedColumnRequest Data)
        {
            try
            {
                // Store the provided DTO object in the session with key "DataSet"
                SessionHeplers.SetObject(HttpContext.Session, "DataSet", Data);

                // Return true to indicate the session data was successfully set
                return Json(true);
            }
            catch (Exception ex)
            {
                // Log the exception with a unique event ID for troubleshooting
                _logger.LogError(1001, ex, "BasicDetail->DataSendForSetSession");

                // Return false to indicate failure in setting session data
                return Json(false);
            }
        }

        /// <summary>
        /// Retrieves session data from the current HTTP session.
        /// This method attempts to fetch a `DTOEncypteDecryptedColumnRequest` object stored in the session under the key "DataSet".
        /// </summary>
        /// <returns>
        /// Returns a `DTOGenericResponse<DTOEncypteDecryptedColumnRequest>` containing:
        /// - `Result`: true if session data was successfully found, false otherwise
        /// - `Message`: provides a success or failure message
        /// - `Value`: the retrieved session data if available; otherwise, an empty `DTOEncypteDecryptedColumnRequest` object
        /// </returns>
        [HttpPost]
        public ActionResult DataRecForGetSession()
        {
            // Initialize response object
            var response = new DTOGenericResponse<DTOEncypteDecryptedColumnRequest>();

            try
            {
                // Attempt to retrieve the DTO object from session
                var dTOEncypte = SessionHeplers.GetObject<DTOEncypteDecryptedColumnRequest>(HttpContext.Session, "DataSet");

                if (dTOEncypte != null)
                {
                    // Session data found, return success
                    response.Result = true;
                    response.Message = "Fetch Value";
                    response.Value = dTOEncypte;
                }
                else
                {
                    // Session data not found, return failure with empty DTO
                    response.Result = false;
                    response.Message = "Session not found.";
                    response.Value = new DTOEncypteDecryptedColumnRequest();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions for debugging
                _logger.LogError(1001, ex, "BasicDetail->DataRecForGetSession");

                // Return failure response with empty DTO
                response.Result = false;
                response.Message = "Session not found.";
                response.Value = new DTOEncypteDecryptedColumnRequest();
            }

            // Return JSON response
            return Json(response);
        }

        #endregion
    }
}