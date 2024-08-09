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
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using iText.Layout.Borders;
using System.Xml.Serialization;
using System;

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
                Data.Id = Data.Id;

                return Json(await _iTrnLoginLogBL.XmlFileDigitalSign(Data));
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
        //public async Task<IActionResult> XmlFileDigitalSignFromData(int RequestId)
        //{
        //    return Json(await _iTrnLoginLogBL.XmlFileDigitalSignFromData(RequestId));
        //}
       // public async Task<IActionResult> CreatePDF
        public async Task<IActionResult> DigitalpdfsignatureSave(int RequestId, string base64)
        {
            BasicDetailCrtAndUpdVM? db = await BasicDetailBL.GetBasicDetailByRequestId(RequestId);
            var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignaturePdf\\" + db.ServiceNo + "_" + RequestId + ".pdf");

            byte[] pdfBytes = Convert.FromBase64String(base64);
            System.IO.File.WriteAllBytes(filePath1, pdfBytes);
            return Json(db.ServiceNo + "_" + RequestId + ".pdf");
        }
        public async Task<IActionResult> CreatePdfAsync(int RequestId)
        {
            try
            {
                var now = DateTime.Now;
                var yearName = now.ToString("yyyy");
                var monthName = now.ToString("MMMM");
                var dayName = now.ToString("dd");
                var hh = now.ToString("hh");
                var mm = now.ToString("mm");
                var ss = now.ToString("ss");

                int[] d;
                d = new int[1];
                d[0] = RequestId;
                var sata = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(d);
                ////////////certificate//////
                List<X509Certificate2> certificates = new List<X509Certificate2>();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Convert.ToString(sata.XmlFiles));

                XmlNodeList certificateNodes = xmlDoc.GetElementsByTagName("X509Certificate");

                foreach (XmlNode node in certificateNodes)
                {
                    string base64EncodedCertificate = node.InnerText;
                    byte[] certBytes = Convert.FromBase64String(base64EncodedCertificate);
                    X509Certificate2 certificate = new X509Certificate2(certBytes);
                    certificates.Add(certificate);
                }
                ///////////////End Certificate////////////////////
                ///
                List<DTOFwdLastRecForDigitalSign> lstproDetails = new List<DTOFwdLastRecForDigitalSign>();

                XmlDocument xmlDoc1 = new XmlDocument();
                xmlDoc.LoadXml(Convert.ToString(sata.XmlFiles));

                XmlNodeList fwddetails = xmlDoc.GetElementsByTagName("RecForDigitalSign");

                foreach (XmlNode node in fwddetails)
                {
                    string  base64EncodedCertificate1 = node.InnerXml;
                    // byte[] certBytes = Convert.FromBase64String(base64EncodedCertificate);
                    DTOFwdLastRecForDigitalSign Fwddetails=new DTOFwdLastRecForDigitalSign();
                    Fwddetails.FromProfile = node["FromProfile"].InnerText;
                    Fwddetails.FromDomain = node["FromDomain"].InnerText;
                    Fwddetails.FromRank = node["FromRank"].InnerText;
                    Fwddetails.FromArmyNo = node["FromArmyNo"].InnerText;
                    Fwddetails.FromDate = Convert.ToDateTime(node["FromDate"].InnerText);
                    //XmlSerializer serializer = new XmlSerializer(typeof(DTOFwdLastRecForDigitalSign));
                    //using (StringReader reader = new StringReader(base64EncodedCertificate1))
                    //{
                    //    DTOFwdLastRecForDigitalSign person = (DTOFwdLastRecForDigitalSign)serializer.Deserialize(reader);

                    //    lstproDetails.Add(person);
                    //}

                    lstproDetails.Add(Fwddetails);
                }




                BasicDetailCrtAndUpdVM? db = await BasicDetailBL.GetBasicDetailByRequestId(RequestId);
                string pdfname = db.ServiceNo + "_" + RequestId + "_" + yearName + "" + monthName + "" + dayName + "" + hh + "" + mm + "" + ss + ".pdf";
                var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignaturePdf\\"+ pdfname);
                //if (!System.IO.File.Exists(filePath1))
                //{
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
                    table.AddCell(db.RankName+" "+db.FName + (db.LName == null ? "" : " " + db.LName));
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
                    table.AddCell(Convert.ToString(db.DOB.ToShortDateString()).Replace("-", "/"));
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
                    table.AddCell(db.IssuingAuthorityName);
                    table.AddCell("Date of Commissioning/ Enrollment");
                    table.AddCell(Convert.ToString(db.DateOfCommissioning.ToShortDateString()).Replace("-", "/"));

                    table.AddCell("Permt Address as per Service Records");
                    //table.AddCell(new Cell(1, 3).Add(new Paragraph("Amount")));
                    table.AddCell("Village - " + db.Village + ", Post Office-" + db.PO + ", Tehsil- " + db.Tehsil + ", District- " + db.District + ", State- " + db.State + ", Pin Code- " + db.PinCode);
                //    table.AddCell("Approved Date");
                //    table.AddCell(Convert.ToString(DateTime.Now.ToShortDateString()).Replace("-", "/"));
                //    table.AddCell("Approved By");
                //    DtoSession dtoSession = new DtoSession();
                //    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");



                //table.AddCell(dtoSession.RankName + " " + dtoSession.Name + " (" + dtoSession.ICNO + ")");
                String imphotoFile = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\WriteReadData\\Photo\\"+db.PhotoImagePath);
                ImageData dataphoto = ImageDataFactory.Create(imphotoFile);

                Image imagedataphoto = new Image(dataphoto);
                imagedataphoto.SetWidth(60);
                Cell imageCellphotos = new Cell().Add(imagedataphoto);
                //imageCellphotos.SetBorder(null);
                table.AddCell("Photos");
                table.AddCell(imageCellphotos);
                table.AddCell("Signature");
                String sigFile = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\WriteReadData\\Signature\\" + db.SignatureImagePath);
                ImageData datasig = ImageDataFactory.Create(sigFile);

                Image imagedatasig = new Image(datasig);
                imagedatasig.SetWidth(60);
                Cell imageCellsig = new Cell().Add(imagedatasig);
                //imageCellsig.SetBorder(null);
                table.AddCell(imageCellsig);

                // Add the table to the document
                document.Add(table);

                //Paragraph digtalsign = new Paragraph("Digital Signature Details").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);

                //document.Add(digtalsign);
                //////////////////digital stump/////////////////////////////////
                Table tabled = new Table(4);
                tabled.SetMarginTop(20);
                tabled.SetProperty(Property.BORDER_BOTTOM, Border.NO_BORDER);
                tabled.SetProperty(Property.BORDER_LEFT, Border.NO_BORDER);
                tabled.SetProperty(Property.BORDER_RIGHT, Border.NO_BORDER);
                tabled.SetProperty(Property.BORDER_TOP, Border.NO_BORDER);
                tabled.SetProperty(Property.BORDER, Border.NO_BORDER);
                tabled.SetBorder(Border.NO_BORDER);
                String imFile = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Images\\digitalsignature.png");
                ImageData data = ImageDataFactory.Create(imFile);

                Image image = new Image(data);
                image.SetWidth(100);
                Cell imageCell = new Cell().Add(image);
                imageCell.SetBorder(null);
               
                //document.Add(image);
                foreach (var digitaldata in certificates)
                    {
                    var subdata = digitaldata.Subject.Split(",");
                    //  tabled.AddCell(imageCell + " "+subdata[0].Replace("CN=", "") +"\n" +subdata[1].Replace("SERIALNUMBER=", "").Replace("7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996", "IC3432432X")).SetBorder(Border.NO_BORDER);
                    // tabled.AddCell(new Cell().Add(imageCell));
                    Paragraph paragraph = new Paragraph();
                    paragraph.Add(imageCell);
                    paragraph.Add("\n"+subdata[0].Replace("CN=", "") + "\n" + subdata[1].Replace("SERIALNUMBER=", "").Replace("7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996", "IC3432432X") + "\n" +DateTime.Now).SetBorder(Border.NO_BORDER);
                    Cell cell = new Cell();
                    cell.Add(paragraph);
                    cell.SetBorder(Border.NO_BORDER);
                    tabled.AddCell(cell);
                    tabled.SetWidth(300);

                }
                document.Add(tabled);

                //////////////////End digital stump/////////////////////////////////

                //////////////////Fwd stump/////////////////////////////////
                Table tableFwd = new Table(4);
                tableFwd.SetMarginTop(20);
                tableFwd.SetProperty(Property.BORDER_BOTTOM, Border.NO_BORDER);
                tableFwd.SetProperty(Property.BORDER_LEFT, Border.NO_BORDER);
                tableFwd.SetProperty(Property.BORDER_RIGHT, Border.NO_BORDER);
                tableFwd.SetProperty(Property.BORDER_TOP, Border.NO_BORDER);
                tableFwd.SetProperty(Property.BORDER, Border.NO_BORDER);
                tableFwd.SetBorder(Border.NO_BORDER);
                String imFileFwd = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Images\\approved.jpg");
                ImageData dataFwd = ImageDataFactory.Create(imFileFwd);

                Image imageFwd = new Image(dataFwd);
                imageFwd.SetWidth(100);
                Cell imageCellFwd = new Cell().Add(imageFwd);
                imageCellFwd.SetBorder(null);

                //document.Add(image);
                foreach (var digitaldata in lstproDetails)
                {
                   
                   
                    Paragraph paragraph = new Paragraph();
                    paragraph.Add(imageCellFwd);
                    paragraph.Add("\n"+digitaldata.FromRank +" "+ digitaldata.FromProfile + "\n" + digitaldata.FromArmyNo + "\n" + digitaldata.FromDate).SetBorder(Border.NO_BORDER);
                    Cell cell = new Cell();
                    cell.Add(paragraph);
                    cell.SetBorder(Border.NO_BORDER);
                    tableFwd.AddCell(cell);
                    tableFwd.SetWidth(300);

                }
                document.Add(tableFwd);

                //////////////////End Fwd stump/////////////////////////////////


                


                document.Close();
                    return Json(pdfname);
                //}
                //else
                //{
                //    return Json(db.ServiceNo + "_" + RequestId + ".pdf");
                //}
            }
            catch (Exception ex) { 
                return Json(0);
            }
           
        }

    }
}
