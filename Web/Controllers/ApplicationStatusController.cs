using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Helpers;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Healpers.BaseInterfaces;
using Web.Validation;
using Web.WebHelpers;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling application status related actions.And view for tracking application status.
    /// </summary>
    public class ApplicationStatusController : Controller
    {
        private readonly IBasicDetailBL _basicDetailBL;//Interface for basic detail business logic layer
        private readonly IConfiguration _configuration;//Configuration interface for accessing application settings
        private readonly IWebHostEnvironment hostingEnvironment;// For Hosting Environment
        private readonly IImageEncryptAndDecrypt imageEncryptAndDecrypt;// For Image Encrypt and Decrypt
        private readonly ILogger<ApplicationStatusController> _logger;// For Logging

        //constructor to initialize dependencies and configuration settings.
        public ApplicationStatusController(IBasicDetailBL basicDetailBL, IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IImageEncryptAndDecrypt imageEncryptAndDecrypt, ILogger<ApplicationStatusController> logger)
        {
            _configuration = configuration;
            _basicDetailBL = basicDetailBL;
            this.hostingEnvironment = hostingEnvironment;
            this.imageEncryptAndDecrypt = imageEncryptAndDecrypt;
            _logger = logger;
        }

        /// <summary>
        /// Action method to display the application status view based on the provided tracking ID.
        /// It retrieves application history data and sets the appropriate flags for displaying the status.
        /// </summary>
        /// <param name="TrackingId">The tracking ID used to fetch application status details.</param>
        /// <returns>A view displaying the application status along with footer and client IP information.</returns>
        [HttpGet]
        [AllowAnonymous]
        [AnySessionRequired]
        public IActionResult AppStatus(int RequestId)
        {
            // Set the footer content fetched from the configuration file
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;

            // Get the client IP address from the connection and convert it to IPv4
            ViewBag.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            // Return the view to display the status page
            return View();
        }

        /// <summary>
        /// Retrieves the request history for a given request ID.
        /// </summary>
        /// <param name="RequestId">
        /// The unique identifier of the request whose card history is to be fetched.
        /// </param>
        /// <returns>
        /// A JSON result containing a list of <see cref="ICardHistoryResponse"/> objects 
        /// if history data is found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method performs the following operations:
        /// <list type="number">
        ///   <item>Calls the business logic layer to fetch card history data for the given request ID.</item>
        ///   <item>If data exists, it is returned as a JSON response.</item>
        ///   <item>If no data exists, a JSON null result is returned.</item>
        ///   <item>If an exception occurs, it is logged and a JSON null result is returned.</item>
        /// </list>
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while retrieving card history. 
        /// The exception is logged, and a null JSON result is returned.
        /// </exception>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetRequestHistoryByRequestId(int RequestId)
        {
            try
            {
                // Retrieve the card history for the provided tracking ID
                List<ICardHistoryResponse>? cardHistoryResponses = await _basicDetailBL.ICardHistoryByRequestId(RequestId);

                // Check if any card history data is returned
                if (cardHistoryResponses != null)
                {
                    // If data exists, return it as a JSON response
                    return Json(cardHistoryResponses);
                }
                else
                {
                    // If no data is found, return null in the JSON response
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ApplicationStatus->GetRequestHistoryByRequestId");
                // In case of an exception, return null in the JSON response
                return Json(null);
            }
        }

        /// <summary>
        /// Retrieves the basic details for a given request ID, including 
        /// photo and signature images decrypted into Base64 strings.
        /// </summary>
        /// <param name="RequestId">
        /// The unique identifier of the request for which basic details are to be fetched.
        /// </param>
        /// <returns>
        /// A JSON result containing a <see cref="BasicDetailCrtAndUpdVM"/> object with
        /// basic details, photo, and signature (in Base64 format) if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Fetches the basic details using the business logic layer.</item>
        ///   <item>Builds the physical path for the photo and signature files.</item>
        ///   <item>Decrypts the images and assigns them as Base64 strings to the view model.</item>
        ///   <item>Returns the populated object as a JSON response.</item>
        /// </list>
        /// </remarks>
        /// <exception cref="Exception">
        /// Any exception encountered during fetching, path resolution, 
        /// or decryption is logged, and a null JSON result is returned.
        /// </exception>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetBasicDetailByRequestId(int RequestId)
        {
            DTOGenericResponse<DTOGetICardPrintPreviewByRequestIdResponse> response = new DTOGenericResponse<DTOGetICardPrintPreviewByRequestIdResponse>();
            response.Result = false;
            response.Value = new DTOGetICardPrintPreviewByRequestIdResponse();

            try
            {
                // Retrieve the basic detail record for the given RequestId
                response = await _basicDetailBL.GetICardPrintPreviewByRequestId(RequestId);

                if (response.Result == true)
                {
                    response.Value.AadhaarNo = _basicDetailBL.MaskAadhaar(response.Value.AadhaarNo);
                    // Define the root physical folder where images are stored
                    string sourceFolderPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData");

                    // Build the full path for the photo image
                    string sourcePathPhoto = Path.Combine(sourceFolderPhy, "Photo", response.Value.PhotoImagePath);
                    string sourcePathSignature = Path.Combine(sourceFolderPhy, "Signature", response.Value.SignatureImagePath);

                    if (System.IO.File.Exists(sourcePathPhoto))
                    {
                        // Decrypt the photo image and assign it to the VM
                        response.Value.ExistingPhotoInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathPhoto);
                    }
                    if (System.IO.File.Exists(sourcePathSignature))
                    {
                        // Decrypt the signature image and assign it to the VM
                        response.Value.ExistingSignatureInBase64 = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePathSignature);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                // Log any exception with an error code and context
                _logger.LogError(1001, ex, "ApplicationStatus->GetBasicDetailByRequestId");
                response.Message = "Photo and Signature not found.";
            }
            catch (Exception ex)
            {
                // Log any exception with an error code and context
                _logger.LogError(1001, ex, "ApplicationStatus->GetBasicDetailByRequestId");
                response.Message = "Internal Error.";
            }
            return Json(response);

        }

    }
}
