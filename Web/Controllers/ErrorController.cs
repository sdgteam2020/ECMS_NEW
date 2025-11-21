using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling error responses and logging.
    /// </summary>
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;//Logger instance for logging error details

        //constructor to initialize the logger dependency
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }


        /// <summary>
        /// Action method that handles HTTP status code errors and logs details about the error.
        /// Based on the provided status code, a specific error message is set, and relevant information is logged.
        /// </summary>
        /// <param name="statusCode">The status code representing the HTTP error.</param>
        /// <returns>A view that displays the error message corresponding to the status code or a generic NotFound view.</returns>
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            // Get the details of the error from the re-executed request
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // Check if error details exist
            if (statusCodeResult != null)
            {
                // Switch case for handling different HTTP status codes
                switch (statusCode)
                {
                    case 400:
                        // Bad Request error
                        ViewBag.ErrorMessage = "Sorry, Bad Request";
                        logger.LogWarning($"400 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 401:
                        // Unauthorized error
                        ViewBag.ErrorMessage = "Sorry, Unauthorized";
                        logger.LogWarning($"401 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 403:
                        // Forbidden error
                        ViewBag.ErrorMessage = "Sorry, Forbidden";
                        logger.LogWarning($"403 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 404:
                        // Not Found error
                        ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                        logger.LogWarning($"404 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 405:
                        // Method Not Allowed error
                        ViewBag.ErrorMessage = "Sorry, Method Not Allowed";
                        logger.LogWarning($"405 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 406:
                        // Not Acceptable error
                        ViewBag.ErrorMessage = "Sorry, Not Acceptable";
                        logger.LogWarning($"406 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 407:
                        // Proxy Authentication Required error
                        ViewBag.ErrorMessage = "Sorry, Proxy Authentication Required";
                        logger.LogWarning($"407 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 412:
                        // Precondition Failed error
                        ViewBag.ErrorMessage = "Sorry, Precondition Failed";
                        logger.LogWarning($"412 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 414:
                        // Request URI Too Long error
                        ViewBag.ErrorMessage = "Sorry, Request URI Too Long";
                        logger.LogWarning($"414 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 415:
                        // Unsupported Media Type error
                        ViewBag.ErrorMessage = "Sorry, Unsupported Media Type";
                        logger.LogWarning($"415 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 500:
                        // Internal Server Error
                        ViewBag.ErrorMessage = "Sorry, Internal Server Error";
                        logger.LogWarning($"500 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 501:
                        // Not Implemented error
                        ViewBag.ErrorMessage = "Sorry, Not Implemented";
                        logger.LogWarning($"501 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 502:
                        // Bad Gateway error
                        ViewBag.ErrorMessage = "Sorry, Bad Gateway";
                        logger.LogWarning($"502 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 503:
                        // Service Unavailable error
                        ViewBag.ErrorMessage = "Sorry, Service Unavailable";
                        logger.LogWarning($"503 error occurred. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                }

                // Return the NotFound view with the error message
                return View("NotFound");
            }

            // If no status code result is found, return NotFound
            return NotFound();
        }

        /// <summary>
        /// Action method that handles exceptions globally. It logs the exception details and displays a generic error view to the user.
        /// </summary>
        /// <returns>A view displaying a generic error message.</returns>
        [Route("Error")]
        public IActionResult Error()
        {
            // Get the exception feature from the HTTP context
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // If an exception has been caught, log the error details
            if (exceptionHandlerPathFeature != null)
            {
                // Log the exception details, including the path and exception message
                logger.LogError(1984, exceptionHandlerPathFeature.Error,
                    $"The path {exceptionHandlerPathFeature.Path} threw an exception: {exceptionHandlerPathFeature.Error}");
            }

            // Return the error view
            return View("Error");
        }

    }
}
