using BusinessLogicsLayer.TrnLoginLog;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.WebHelpers;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Net;
using System.Net.Http.Headers;
using iText.Kernel.Pdf;
using BusinessLogicsLayer.BasicDet;
using DataTransferObject.ViewModels;
using BusinessLogicsLayer.Bde;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ITrnLoginLogBL _iTrnLoginLogBL;
        private readonly IBasicDetailBL BasicDetailBL;
        private readonly IWebHostEnvironment hostingEnvironment;
        public LogController(ITrnLoginLogBL iTrnLoginLogBL, IWebHostEnvironment hostingEnvironment, IBasicDetailBL BasicDetailBL)
        {
            _iTrnLoginLogBL = iTrnLoginLogBL;
            this.hostingEnvironment = hostingEnvironment;
            this.BasicDetailBL = BasicDetailBL;
        }
        public async Task<IActionResult> LoginLog()
        {
            string referer = HttpContext.Request.Headers["Referer"].ToString();

            DtoSession dtoSession = new DtoSession();
            dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
             if (dtoSession != null )
            {
                var data = await _iTrnLoginLogBL.GetAllUserByUnitId(dtoSession.UnitId);

                return View(data);
            }
             else
            { 
                return View(null); }  
           
        }
        public async Task<IActionResult> LoginLogByAspNetUsersId(int AspNetUsersId,DateTime? FmDate,DateTime? ToDate)
        {
            try
            {
                return Json(await _iTrnLoginLogBL.GetLoginLogByUserId(AspNetUsersId, FmDate, ToDate));
            }
            catch (Exception ex)
            {
                return Json(0);
            }
            
        }
        public async Task<IActionResult> XmlFileDigitalSign(DTOXmlFilesFwdLogRequest Data)
        {

            try
            {
                Data.UpdatedOn = DateTime.Now;
                Data.Updatedby= Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                Data.IsActive = 1;
                Data.Id = 0;
                return Json(await _iTrnLoginLogBL.XmlFileDigitalSign(Data));
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
        public async Task<IActionResult> CreatePdfAsync(int RequestId)
        {
            
            try
            {
                BasicDetailCrtAndUpdVM? db = await BasicDetailBL.GetByRequestIdBesicDetails(RequestId);
                var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignaturePdf\\"+db.ServiceNo+ "_"+RequestId+".pdf");
                PdfWriter writer = new PdfWriter(filePath1);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                Paragraph header = new Paragraph("I-Card Processs Digital Signature").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);

                document.Add(header);

                Paragraph subheader = new Paragraph("Pers info Details").SetTextAlignment(TextAlignment.CENTER).SetFontSize(15);
                document.Add(subheader);

                Table table = new Table(4);

                // Add header cells to the table
                table.AddCell("NAME");
                table.AddCell(db.Name);
                table.AddCell("Rank");
                table.AddCell(db.RankName);

                // Add data row with 4 columns
                table.AddCell("Arm / Service");
                table.AddCell(db.ArmedName);
                table.AddCell("Army No");
                table.AddCell(db.ServiceNo);
                table.AddCell("IdenMark1");
                table.AddCell(db.IdenMark1);
                table.AddCell("Date of Birth");
                table.AddCell(Convert.ToString(db.DOB.ToShortDateString()).Replace("-","/"));
                table.AddCell("Height (Cm)");
                table.AddCell(Convert.ToString(db.Height));
                table.AddCell("AADHAAR No");
                table.AddCell(db.AadhaarNo);
                table.AddCell("BloodGroup");
                table.AddCell(db.BloodGroup);
                table.AddCell("Place of Issue");
                table.AddCell(db.PlaceOfIssue);
                table.AddCell("Date of Issue");
                table.AddCell(Convert.ToString(db.DateOfIssue.ToShortDateString()).Replace("-", "/"));
                table.AddCell("Issuing Authority");
                table.AddCell(Convert.ToString(db.IssuingAuthorityId));
                table.AddCell("Date of Commissioning/ Enrollment");
                table.AddCell(Convert.ToString(db.DateOfCommissioning.ToShortDateString()).Replace("-", "/"));

                table.AddCell("Permt Address as per Service Records");
                //table.AddCell(new Cell(1, 3).Add(new Paragraph("Amount")));
                table.AddCell("Village - " + db.Village + ", Post Office-" + db.PO + ", Tehsil- " + db.Tehsil + ", District- " + db.District + ", State- " + db.State + ", Pin Code- " + db.PinCode);
                table.AddCell("Approved Date");
                table.AddCell(Convert.ToString(DateTime.Now.ToShortDateString()).Replace("-", "/"));
                table.AddCell("Approved By");
                DtoSession dtoSession = new DtoSession();
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");


                table.AddCell(dtoSession.RankName+" " + dtoSession.Name+" ("+dtoSession.ICNO+")");
                // Add the table to the document
                document.Add(table);


                document.Close();
                return Json(db.ServiceNo+ "_"+RequestId+".pdf");
            }
            catch (Exception ex) { 
                return Json(0);
            }
           
        }







    
    }
}
