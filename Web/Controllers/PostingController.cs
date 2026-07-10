using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.Posting;
using BusinessLogicsLayer.Service;
using DataAccessLayer;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Web.Healpers;
using Web.Healpers.BaseInterfaces;
using Web.WebHelpers;

namespace Web.Controllers
{
    /// <summary>
    /// This controller manages posting operations including posting in, posting out, and application closure.
    /// </summary>
    [Authorize] 
    public class PostingController : Controller
    {
        private readonly IPostingBL _iPostingBL;// Interface for posting business logic
        private readonly IApplCloseBL _iApplCloseBL;// Interface for application closure business logic
        private readonly ITrnICardRequestBL _iTrnICardRequestBL;// Interface for ICard request business logic
        private readonly IService service;// Interface for general services
        private readonly ILogger<PostingController> _logger;// Logger for logging information and errors
        private readonly IWebHostEnvironment hostingEnvironment;// Hosting environment for accessing web root path
        private readonly IDataProtector _protector;// Data protector for securing sensitive data
        private readonly IImageEncryptAndDecrypt imageEncryptAndDecrypt;// Interface for image encryption and decryption
        private readonly IBasicDetailBL basicDetailBL;// For Basic Detail
        public PostingController(IPostingBL postingBL, IApplCloseBL iApplCloseBL, ITrnICardRequestBL trnICardRequestBL, IService service, ILogger<PostingController> logger, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings, IImageEncryptAndDecrypt imageEncryptAndDecrypt, IBasicDetailBL basicDetailBL)
        {
            _iPostingBL = postingBL;
            _iApplCloseBL = iApplCloseBL;
            _iTrnICardRequestBL = trnICardRequestBL;
            this.service = service;
            _logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            _protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            this.imageEncryptAndDecrypt = imageEncryptAndDecrypt;
            this.basicDetailBL = basicDetailBL;
        }

        /// <summary>
        /// Handles the posting in process.
        /// Returns the PostingIn view.
        /// Optionally accepts an encrypted identifier (EncId) for additional context.
        /// </summary>
        /// <param name="EncId">
        /// Optional encrypted identifier that can be used to fetch or filter data in the view.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> that renders the PostingIn view.
        /// </returns>
        [HttpGet]
        public IActionResult PostingIn(string? EncId)
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
        /// Retrieves and returns the posting information for a given Army Number (ArmyNo).
        /// It fetches relevant data and decrypts the associated image for display purposes.
        /// </summary>
        /// <param name="ArmyNo">
        /// The Army Number used to retrieve the relevant posting data.
        /// </param>
        /// <returns>
        /// Returns a <see cref="JsonResult"/> containing the posting data, including a decrypted photo image.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetPostingIn(string ArmyNo)
        {
            // Fetch posting information based on Army Number (ArmyNo)
            DTOPostingInResponse data = await _iPostingBL.GetArmyDataForPostingOut(ArmyNo);

            // Define the folder path for photos and signatures
            string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

            // Define the full path for the photo image based on the stored file path
            string sourcePathPhoto = Path.Combine(sourceFolderPhotoPhy, "Photo", data.PhotoImagePath);

            // Check if the photo file exists
            if (System.IO.File.Exists(sourcePathPhoto))
            {
                // If the file exists, decrypt the image and assign it to the data's PhotoImagePath property
                data.PhotoImagePath = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
            }

            // Return the data as a JSON response, including the decrypted photo image
            return Json(data);
        }


        /// <summary>
        /// This method handles the retrieval of posting out records based on the provided base64-encoded `Type` and `PostingType`.
        /// It validates the input, decodes the base64 strings, and sets the corresponding session values for `PostingType` and `Type`.
        /// If any input is invalid, the user is redirected to the ContactUs page with an error message.
        /// </summary>
        /// <param name="Type">A base64 encoded string representing the type.</param>
        /// <param name="PostingType">A base64 encoded string representing the posting type.</param>
        /// <returns>
        /// Returns a view with the decoded values for `Type` and `PostingType` as ViewBag properties.
        /// </returns>
        [HttpGet]
        public IActionResult GetPostingOutWithType(string Type, string PostingType)
        {
            // Check if Type or PostingType are null, empty or invalid base64-encoded
            if (string.IsNullOrEmpty(Type) || !service.IsValidBase64(Type) || string.IsNullOrEmpty(PostingType) || !service.IsValidBase64(PostingType))
            {
                TempData["error"] = "Invalid Input.";  // Store error message in TempData
                TempData.Keep("error");  // Retain the error message across redirects
                return RedirectToAction("ContactUs", "Home");  // Redirect to the ContactUs page if input is invalid
            }

            try
            {
                // Decode the base64-encoded Type and PostingType strings
                var base64EncodedBytes = Convert.FromBase64String(Type);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);  // Decode the Type string
                var PostingTy = Encoding.UTF8.GetString(Convert.FromBase64String(PostingType));  // Decode the PostingType string

                // Convert the decoded Type string to an integer
                int t = Convert.ToInt32(decodedString);

                // Store the decoded values in the session for future use
                SessionHeplers.SetObject(HttpContext.Session, "PostingType", PostingTy);
                SessionHeplers.SetObject(HttpContext.Session, "Type", t);

                // Pass the decoded values to the view using ViewBag
                ViewBag.Type = t;
                ViewBag.PostingType = PostingTy;

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
            catch (Exception ex)
            {
                // Log any exceptions that occur during decoding and session handling
                _logger.LogError(1001, ex, "PostingController=>GetPostingOutWithType.");
                TempData["error"] = "Invalid Input.";  // Store error message in TempData
                TempData.Keep("error");  // Retain the error message across redirects
                return RedirectToAction("ContactUs", "Home");  // Redirect to the ContactUs page if an exception occurs
            }
        }

        /// <summary>
        /// This method retrieves all posting out records based on the provided `DTODataTablesRequest` and session data for `PostingType` and `Type`.
        /// It fetches the posting out data using the `GetPostingOutWithType` method from the business logic layer and returns the result in JSON format.
        /// </summary>
        /// <param name="dTO">The data table request object containing parameters for filtering and pagination.</param>
        /// <returns>
        /// Returns a JSON response with the posting out records matching the provided filters and session data.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> GetAllPostingOutWithType(DTODataTablesRequest dTO)
        {
            // Initialize an empty list of DTOPostingOutDetilsResponse to store the results
            List<DTOPostingOutDetilsResponse> dTOPostingOutDetilsResponses = new List<DTOPostingOutDetilsResponse>();
            var responseData = new DTODataTablesResponse<DTOPostingOutDetilsResponse>
            {
                draw = dTO.Draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOPostingOutDetilsResponses  // Set the initial empty list of data
            };

            try
            {
                // Retrieve session information
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }

                // Get unit ID and other session-based values
                int MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;
                string PostingType = SessionHeplers.GetObject<string>(HttpContext.Session, "PostingType");
                int Type = SessionHeplers.GetObject<int>(HttpContext.Session, "Type");

                // Get the user ID from the current logged-in user
                int userid = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fetch the posting out details using the business logic layer
                responseData = await _iPostingBL.GetPostingOutWithType(dTO, userid, MapUnitId, Type, PostingType);
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the execution of the method
                _logger.LogError(1001, ex, "Posting->GetAllPostingOutWithType");
            }

            // Return the response data as a JSON result
            return Json(responseData);
        }



        /// <summary>
        /// This method is responsible for saving or updating posting out records.
        /// It checks if the posting out record exists and updates it, or adds a new one.
        /// </summary>
        /// <param name="dTO">The `TrnPostingOut` object containing the data to be saved or updated.</param>
        /// <returns>
        /// Returns a JSON result indicating success or failure of the operation.
        /// - Returns `KeyConstants.Update` if the record is updated successfully.
        /// - Returns `KeyConstants.Save` if a new record is created successfully.
        /// - Returns `KeyConstants.IncorrectData` if the data is incorrect.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SavePoasingOut(string request)
        {
            DTOPostingOutRequest dTO = await AESEncrytDecry.DecryptAESWithDTO<DTOPostingOutRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
           
            DTOGenericResponse<DTOBeforePostingOutCheckedInputDataResponse?> response = new DTOGenericResponse<DTOBeforePostingOutCheckedInputDataResponse?>();
            DTOBeforePostingOutCheckedInputDataResponse closeResponse = new DTOBeforePostingOutCheckedInputDataResponse();
            if (dTO == null)
            {
                response.Message = "Internal Server Error.";
                response.Value = closeResponse;
                response.Result = false;
                return Ok(response);
            }
            try
            {
                
                TrnPostingOut trnPostingOut = new TrnPostingOut();
                int CurrentAspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));  // Get the current user ID
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    trnPostingOut.FromUserID = dtoSession != null ? dtoSession.UserId : 0;
                    trnPostingOut.FromUnitID = dtoSession != null ? dtoSession.UnitId : 0;
                }
                trnPostingOut.Id = 0;
                trnPostingOut.RequestId= dTO.RequestId;
                trnPostingOut.TrnFwdId= dTO.TrnFwdId;
                trnPostingOut.SOSDate= dTO.SOSDate;
                trnPostingOut.ReasonId= dTO.ReasonId;
                trnPostingOut.Authority= dTO.Authority;
                trnPostingOut.ToUnitID= dTO.ToUnitID;
                trnPostingOut.ToAspNetUsersId = dTO.ToAspNetUsersId;
                trnPostingOut.ToUserID = dTO.ToUserID;
                trnPostingOut.IsActive = true;
                trnPostingOut.FromAspNetUsersId = CurrentAspNetUsersId;
                trnPostingOut.Updatedby = CurrentAspNetUsersId;
                trnPostingOut.UpdatedOn = DateTime.Now;  // Set the updated timestamp

                // Check if the model is valid
                ModelState.Clear();
                if (TryValidateModel(dTO))
                {
                    closeResponse = await _iPostingBL.BeforePostingOutCheckedInputData(trnPostingOut);
                    if (closeResponse.Result == true)
                    {
                        // If it's a new record, use UpdateForPosting method (this handles both add and update)
                        bool result = await _iPostingBL.UpdateForPosting(trnPostingOut);  // Attempt to add or update the record
                        if (result == true)
                        {
                            response.Message = "Posting Out successfully";
                            response.Value = closeResponse;
                            response.Result = true;
                            return Ok(response);
                        }
                        else
                        {
                            response.Message = "Internal Server Error.";
                            response.Value = closeResponse;
                            response.Result = false;
                            return Ok(response);
                        }
                    }
                    else
                    {
                        response.Value = closeResponse;
                        response.Message = closeResponse.Message;
                        response.Result = false;
                        return Ok(response);
                    }
                }
                else
                {
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

                    response.Value = closeResponse;
                    response.Result = false;
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur and return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// This method handles saving the dispatch details of a posting out record.
        /// It checks if the dispatch details already exist; if not, it saves them.
        /// </summary>
        /// <param name="dTO">The `DTODispatchDetailsSaveRequest` object containing dispatch details.</param>
        /// <returns>
        /// Returns a JSON result indicating success or failure of the operation.
        /// - Returns `Result = true` and success message if the record is saved.
        /// - Returns a message indicating if the dispatch details already exist or if there are validation errors.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SavePostingOutDispatchDetails(string Request)
        {
            // Initialize the response object for front-end
            var dTOResponse = new DTOGenericResponse<string>();

            try
            {
                var session = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                if (session == null)
                {
                    dTOResponse.Result = false;
                    dTOResponse.Message = "Session expired.";
                    dTOResponse.Value = string.Empty;
                    return Json(dTOResponse);
                }

                DTODispatchDetailsSaveRequest dTO = await AESEncrytDecry.DecryptAESWithDTO<DTODispatchDetailsSaveRequest>(Request, session.Salt);

                if (dTO == null)
                {
                    dTOResponse.Result = false;
                    dTOResponse.Message = "Invalid Data.";
                    dTOResponse.Value = string.Empty;
                    return Json(dTOResponse);
                }



                // Decrypt the encrypted ID (encId) and validate it
                var encId = _protector.Unprotect(dTO.encId);
                if (int.TryParse(encId, out int Id))
                {
                    ModelState.Clear();
                    if (TryValidateModel(dTO))
                    {
                        // Fetch the posting out details based on the Id
                        var postingOutDetails = await _iPostingBL.Get(Id);

                        if (postingOutDetails.FromUnitID != session.UnitId) 
                        {
                            dTOResponse.Result = false;
                            dTOResponse.Message = "Unit is not authorized.";
                            return Json(dTOResponse);
                        }
                        // If dispatch details already exist, return a message
                        else if (postingOutDetails.DispatchedOn.HasValue)
                        {
                            dTOResponse.Result = false;
                            dTOResponse.Message = "Dispatch details already exists!";
                            return Json(dTOResponse);
                        }
                        else
                        {
                            // If dispatch details do not exist, set them and save the record
                            postingOutDetails.DispatchedOn = dTO.DispatchedOn;
                            postingOutDetails.RefNo = dTO.RefNo;
                            postingOutDetails.DispatchUpdatedBy = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));  // Get current user ID
                            postingOutDetails.DispatchUpdatedOn = DateTime.Now;
                            await _iPostingBL.Update(postingOutDetails);  // Update the record in the database
                            dTOResponse.Result = true;
                            dTOResponse.Message = "Record Saved!";  // Success message
                            return Json(dTOResponse);
                        }
                    }
                    else
                    {
                        var errors = ModelState
                                    .Where(x => x.Value?.Errors?.Count > 0)
                                    .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                                    .ToList();

                        dTOResponse.Result = false;
                        dTOResponse.Message = errors.Any() ? string.Join("; ", errors) : "Invalid request.";
                        return Json(dTOResponse);
                    }
                }
                else
                {
                    // If the encrypted ID is invalid, log an error and return a failure message
                    _logger.LogError(1001, $"Invalid Id -: {dTO.encId}", "Posting->SavePostingOutDispatchDetails");
                    dTOResponse.Result = false;
                    dTOResponse.Message = "Invalid Id.";
                    return Json(dTOResponse);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an internal server error message
                _logger.LogError(1001, ex, "Posting->SavePostingOutDispatchDetails");
                dTOResponse.Result = false;
                dTOResponse.Message = "Internal Server Error!";
                return Json(dTOResponse);
            }
        }

        /// <summary>
        /// This method returns the application close view.
        /// </summary>
        /// <returns>
        /// Returns the view for application closure.
        /// </returns>
        [HttpGet]
        public IActionResult ApplicationClose()
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
        /// This method handles the saving of application close data.
        /// It checks whether the application close record already exists and saves or updates it accordingly.
        /// </summary>
        /// <param name="dTO">The `TrnApplClose` object containing the application close data.</param>
        /// <returns>
        /// Returns a JSON result indicating the success or failure of the operation.
        /// - `KeyConstants.Save` if the record is successfully saved.
        /// - `KeyConstants.IncorrectData` if the data is incorrect.
        /// - `KeyConstants.Exists` if the application close record already exists.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SaveApplicationClose(string request)
        {
            DTOApplicationCloseRequest dTO=await AESEncrytDecry.DecryptAESWithDTO<DTOApplicationCloseRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
            
            DTOGenericResponse<DTOApplicationCloseResponse?> response = new DTOGenericResponse<DTOApplicationCloseResponse?>();
            DTOApplicationCloseResponse closeResponse = new DTOApplicationCloseResponse();
            if (dTO == null)
            {
                response.Message = "Internal Server Error.";
                response.Value = closeResponse;
                response.Result = false;
                return Ok(response);
            }
            try
            {
               
                TrnApplClose applClose = new TrnApplClose();
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    dTO.UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                }
                
                // Validate the model before saving
                ModelState.Clear();
                if (TryValidateModel(dTO))
                {
                    applClose.Id = 0;
                    applClose.ReasonId = dTO.ReasonId;
                    applClose.Authority = dTO.Authority;
                    applClose.Remarks = dTO.Remarks;
                    applClose.RequestId = dTO.RequestId;
                    applClose.UserId = dtoSession != null ? dtoSession.UserId : 0;
                    applClose.IsActive = true;
                    applClose.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    applClose.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                    closeResponse = await _iApplCloseBL.RequestIdExists(dTO);
                    // Check if the application close record already exists
                    if (closeResponse.Result == true)
                    {
                        // Fetch card history to record distribution details
                        ICardHistoryResponseAll? cardHistoryResponses = await basicDetailBL.ICardHistory(applClose.RequestId);
                        // Save the distribution close record and get the response
                        bool reuslt = await _iApplCloseBL.ApplCloseWithUpdateStatus(applClose, cardHistoryResponses);
                        if (reuslt == true)
                        {
                            response.Message = "Appl closed successfully.";
                            response.Value = closeResponse;
                            response.Result = true;
                            return Ok(response);

                        }
                        else
                        {
                            response.Message = "Internal Server Error.";
                            response.Value = closeResponse;
                            response.Result = false;
                            return Ok(response);
                        }
                    }
                    else
                    {
                        response.Value = closeResponse;
                        response.Message = closeResponse.Message;
                        response.Result = false;
                        return Ok(response);
                    }
                }
                else
                {
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

                    response.Value = closeResponse;
                    response.Result = false;
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "PostingController=>SaveApplicationClose.");
                response.Message = "Internal Server Error.";
                response.Value = closeResponse;
                response.Result = false;
                return Ok(response);
            }
        }

        /// <summary>
        /// This method returns the list of application closed records based on provided Id and jcoor parameters.
        /// It decodes the Id from Base64 and determines the corresponding closed applications.
        /// </summary>
        /// <param name="Id">The ID of the closed application, encoded in Base64 format.</param>
        /// <param name="jcoor">The optional parameter for the type of application closure.</param>
        /// <returns>
        /// Returns a view containing the list of closed applications based on the parameters.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> AppCloseList(string jcoor)
        {
            var userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Validate the input parameters
            if (jcoor != null)
            {
                if (!service.IsValidBase64(jcoor))
                {
                    TempData["error"] = "Invalid Input.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }

            try
            {
                ViewBag.Title = "List of Closed Appl";

                if (role == "user")
                {
                    if (string.IsNullOrEmpty(jcoor))
                    {
                        ViewBag.applyFor = 1;                    }
                    else
                    {
                        ViewBag.applyFor = 2;
                    }
                    return View();
                }
                else
                {
                    TempData["error"] = "Switch to user role.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
            catch (FormatException ex)
            {
                // Handle invalid Base64 format exception and return an error message
                _logger.LogError(1001, ex, message: "Invalid Base64 string for jcoor: {jcoor} ", jcoor);
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
            catch (Exception ex)
            {
                // Log any other exceptions and return an error message
                _logger.LogError(1001, ex, "PostingController=>AppCloseList.");
                TempData["error"] = "Invalid Input.";
                TempData.Keep("error");
                return RedirectToAction("ContactUs", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetAllAppCloseList(DTODataTableRequestForAppCloseList dTORecord)
        {
            // If an exception occurs, return an empty response to avoid breaking the UI
            List<DTOAppClosedListResponse> dTOApps = new List<DTOAppClosedListResponse>();
            var responseData = new DTODataTablesResponse<DTOAppClosedListResponse>
            {
                draw = dTORecord.Draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOApps
            };
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                }
                dTORecord.UnitMapId = dtoSession != null ? dtoSession.UnitId : 0;

                if (ModelState.IsValid)
                {
                    var allrecord = await _iPostingBL.GetAppClosedList(dTORecord);
                    return Json(allrecord);
                }
                else
                {
                    return Json(responseData);
                }

            }
            catch (Exception ex)
            {
                // Log the exception for debugging and tracking
                _logger.LogError(1001, ex, "Posting->GetAllAppCloseList");

                // Return JSON with empty data
                return Json(responseData);
            }
        }

    }
}
