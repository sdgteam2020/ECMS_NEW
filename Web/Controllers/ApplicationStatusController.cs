using BusinessLogicsLayer.BasicDet;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling application status related actions.And view for tracking application status.
    /// </summary>
    public class ApplicationStatusController : Controller
    {
        private readonly IBasicDetailBL _basicDetailBL;//Interface for basic detail business logic layer
        private readonly IConfiguration _configuration;//Configuration interface for accessing application settings

        //constructor to initialize dependencies and configuration settings.
        public ApplicationStatusController(IBasicDetailBL basicDetailBL, IConfiguration configuration)
        {
            _configuration = configuration;
            _basicDetailBL = basicDetailBL;
        }

        /// <summary>
        /// Action method to display the application status view based on the provided tracking ID.
        /// It retrieves application history data and sets the appropriate flags for displaying the status.
        /// </summary>
        /// <param name="TrackingId">The tracking ID used to fetch application status details.</param>
        /// <returns>A view displaying the application status along with footer and client IP information.</returns>
        public async Task<IActionResult> AppStatus(string TrackingId)
        {
            // Initialize the DTO object for storing application tracking data.
            //DTOApplicationTrack dTOApplicationTrack = new DTOApplicationTrack();

            // Try-catch block for error handling in fetching application history data
            //try
            //{
            //    // Fetch the application history based on the tracking ID
            //    dTOApplicationTrack = await _basicDetailBL.ApplicationHistory(TrackingId);

            //    // Check if the application details were successfully retrieved
            //    if (dTOApplicationTrack.dTOApplicationDetails != null)
            //    {
            //        // If data exists, set the flag to indicate data is available
            //        ViewBag.IsData = 1;
            //    }
            //    else
            //    {
            //        // If no data is found, set the flag to indicate no data
            //        ViewBag.IsData = 0;
            //    }
            //}
            //catch (Exception ex) 
            //{
            //    // In case of an exception, set the flag to indicate no data
            //    ViewBag.IsData = 0; 
            //}

            // Set the footer content fetched from the configuration file
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;

            // Get the client IP address from the connection and convert it to IPv4
            ViewBag.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            // Return the view to display the status page
            return View();
        }

        /// <summary>
        /// Action method to retrieve the request history based on the provided tracking ID.
        /// It fetches the card history associated with the given tracking ID and returns it as a JSON response.
        /// </summary>
        /// <param name="TrackingId">The tracking ID used to fetch the card history data.</param>
        /// <returns>A JSON response containing the card history or null if no data is found or an exception occurs.</returns>
        public async Task<IActionResult> GetRequestHistoryByTrackingId(string TrackingId)
        {
            try
            {
                // Retrieve the card history for the provided tracking ID
                List<ICardHistoryResponse>? cardHistoryResponses = await _basicDetailBL.ICardHistoryByTrackingId(TrackingId);

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
                // In case of an exception, return null in the JSON response
                return Json(null);
            }
        }

        public async Task<IActionResult> GetBasicDetailByRequestId(int Id)
        {
            return Json(await _basicDetailBL.GetBasicDetailByRequestId(Id));
        }
    }
}
