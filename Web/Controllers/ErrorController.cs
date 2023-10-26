using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCodeResult != null)
            {
                switch (statusCode)
                {
                    case 400:
                        ViewBag.ErrorMessage = "Sorry, Bad Request";
                        logger.LogWarning($"400 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 401:
                        ViewBag.ErrorMessage = "Sorry, Unauthorized";
                        logger.LogWarning($"401 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 403:
                        ViewBag.ErrorMessage = "Sorry,Forbidden";
                        logger.LogWarning($"403 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 404:
                        ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                        logger.LogWarning($"404 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 405:
                        ViewBag.ErrorMessage = "Sorry, Method Not Allowed";
                        logger.LogWarning($"405 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 406:
                        ViewBag.ErrorMessage = "Sorry, Not Acceptable";
                        logger.LogWarning($"406 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 407:
                        ViewBag.ErrorMessage = "Sorry, Proxy Authentication Required";
                        logger.LogWarning($"407 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 412:
                        ViewBag.ErrorMessage = "Sorry, Precondition Failed";
                        logger.LogWarning($"412 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 414:
                        ViewBag.ErrorMessage = "Sorry, Request Uri Too Long";
                        logger.LogWarning($"414 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 415:
                        ViewBag.ErrorMessage = "Sorry, Unsupported Media Type";
                        logger.LogWarning($"415 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 500:
                        ViewBag.ErrorMessage = "Sorry, Internal Server Error";
                        logger.LogWarning($"500 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 501:
                        ViewBag.ErrorMessage = "Sorry, Not Implemented";
                        logger.LogWarning($"501 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 502:
                        ViewBag.ErrorMessage = "Sorry, Bad Gateway";
                        logger.LogWarning($"502 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                    case 503:
                        ViewBag.ErrorMessage = "Sorry, Service Unavailable";
                        logger.LogWarning($"503 error occured. Path= {statusCodeResult.OriginalPath}" +
                            $"and QueryString = {statusCodeResult.OriginalQueryString}");
                        break;
                }
                return View("NotFound");
            }
            return NotFound();
        }
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //LogError() method logs the exception under Error categoy in the log
            if (exceptionHandlerPathFeature != null)
            {
                logger.LogError(1984, exceptionHandlerPathFeature.Error, $"The path {exceptionHandlerPathFeature.Path}" + $" threw an excepation {exceptionHandlerPathFeature.Error}");
                //ViewBag.ExceptionPath = exceptionHandlerPathFeature.Path;
                //ViewBag.ExceptionMessage = exceptionHandlerPathFeature.Error.Message;
                //ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;
            }
            return View("Error");
        }
    }
}
