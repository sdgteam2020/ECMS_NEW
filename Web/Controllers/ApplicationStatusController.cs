using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Posting;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Web.Controllers
{
    public class ApplicationStatusController : Controller
    {
        private readonly IBasicDetailBL _basicDetailBL;
        private readonly IConfiguration _configuration;
        public ApplicationStatusController(IBasicDetailBL basicDetailBL, IConfiguration configuration)
        {
            _configuration = configuration;
            _basicDetailBL = basicDetailBL;
        }
        public async Task<IActionResult> AppStatus(string TrackingId)
        {
            //DTOApplicationTrack dTOApplicationTrack=new DTOApplicationTrack();
            //try
            //{
            //     dTOApplicationTrack = await _basicDetailBL.ApplicationHistory(TrackingId);
            //    if (dTOApplicationTrack.dTOApplicationDetails != null)
            //    {
            //        ViewBag.IsData = 1;

            //    }
            //    else
            //    {
            //        ViewBag.IsData = 0;

            //    }


            //}
            //catch (Exception ex) { 
            //    ViewBag.IsData = 0; 
            //}

            //return View(dTOApplicationTrack);
            string? Footer = _configuration["Footer:Test"];
            ViewBag.Footer = Footer;
            ViewBag.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return View();
        }
        public async Task<IActionResult> GetRequestHistoryByTrackingId(string TrackingId)
        {
            List<ICardHistoryResponse>? cardHistoryResponses = await _basicDetailBL.ICardHistoryByTrackingId(TrackingId);
            if (cardHistoryResponses != null) 
            {
                return Json(await _basicDetailBL.ICardHistoryByTrackingId(TrackingId));
            }
            else
            {
                return Json(null);
            }
            
        }
        public async Task<IActionResult> GetBasicDetailByRequestId(int Id)
        {
            return Json(await _basicDetailBL.GetBasicDetailByRequestId(Id));
        }
    }
}
