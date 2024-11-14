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
using iTextImage = iText.Layout.Element.Image;
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
using iText.Layout.Font;
using Microsoft.SqlServer.Management.Smo.Wmi;
using static System.Net.Mime.MediaTypeNames;
using Web.Healpers;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities.Net;

namespace Web.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ITrnLoginLogBL _iTrnLoginLogBL;
        private readonly IBasicDetailBL BasicDetailBL;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogController(ITrnLoginLogBL iTrnLoginLogBL, IWebHostEnvironment hostingEnvironment, IBasicDetailBL BasicDetailBL, IHttpContextAccessor httpContextAccessor)
        {
            _iTrnLoginLogBL = iTrnLoginLogBL;
            this.hostingEnvironment = hostingEnvironment;
            this.BasicDetailBL = BasicDetailBL;
            _httpContextAccessor = httpContextAccessor;
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
        public async Task<IActionResult> CreateXmlAsync(int RequestId)
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
                XDocument document = XDocument.Parse(Convert.ToString(sata.XmlFiles));

                BasicDetailCrtAndUpdVM? db = await BasicDetailBL.GetBasicDetailByRequestId(RequestId);

                string xmlname = db.ServiceNo + "_" + RequestId + "_" + yearName + "" + monthName + "" + dayName + "" + hh + "" + mm + "" + ss + ".xml";
                var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignatureXml\\"+ xmlname);

                document.Save(filePath1);

                return Json(xmlname);
            }
            catch (Exception ex) { 
                return Json(0);
            }
           
        }
        public async Task<IActionResult> CreatePdfAsync(int RequestId)
        {
            try
            {
                // Retrieve client IP address
                string ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown IP";

                Dictionary<int, LevelMessage> dictionarLevel = new Dictionary<int, LevelMessage>();
                LevelMessage levelMessage1 = new LevelMessage()
                {
                    ID = 1,
                    Name = "1st Level"
                };
                LevelMessage levelMessage2 = new LevelMessage()
                {
                    ID = 2,
                    Name = "2nd Level"  
                };
                LevelMessage levelMessage3 = new LevelMessage()
                {
                    ID = 3,
                    Name = "3rd Level"
                };
                LevelMessage levelMessage4 = new LevelMessage()
                {
                    ID = 4,
                    Name = "4th Level"
                };
                dictionarLevel.Add(levelMessage1.ID, levelMessage1);
                dictionarLevel.Add(levelMessage2.ID, levelMessage2);
                dictionarLevel.Add(levelMessage3.ID, levelMessage3);
                dictionarLevel.Add(levelMessage4.ID, levelMessage4);

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
                List<DTOFwdLastRecForDigitalSign> DigitalSignList = new List<DTOFwdLastRecForDigitalSign>();

                foreach (XmlNode node in certificateNodes)
                {
                    string base64EncodedCertificate = node.InnerText;
                    byte[] certBytes = Convert.FromBase64String(base64EncodedCertificate);
                    X509Certificate2 certificate = new X509Certificate2(certBytes);
                    certificates.Add(certificate);

                    var subdata = certificate.Subject.Split(",");
                    DTOFwdLastRecForDigitalSign obj = new DTOFwdLastRecForDigitalSign();
                    obj.FromProfile = subdata[0].Replace("CN=", "");
                    string temp, t;
                    t = subdata[1].Replace("SERIALNUMBER=", "");
                    if (t.ToLower().Trim() == "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996".ToLower())
                    {
                        obj.FromArmyNo = subdata[1].Replace("SERIALNUMBER=", "").Replace("7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996", "IC71150W");
                        temp = obj.FromArmyNo.Trim();
                        obj.FromArmyNo = temp;
                    }
                    else if (t.ToLower().Trim() == "A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448".ToLower())
                    {
                        obj.FromArmyNo = subdata[1].Replace("SERIALNUMBER=", "").Replace("A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448", "IC60056M");
                        temp = obj.FromArmyNo.Trim();
                        obj.FromArmyNo = temp;
                    }

                    DigitalSignList.Add(obj);
                }
                ///////////////End Certificate////////////////////

                List<DTOFwdLastRecForDigitalSign> lstproDetails = new List<DTOFwdLastRecForDigitalSign>();

                XmlDocument xmlDoc1 = new XmlDocument();
                xmlDoc.LoadXml(Convert.ToString(sata.XmlFiles));

                XmlNodeList fwddetails = xmlDoc.GetElementsByTagName("RecForDigitalSign");
                BasicDetailCrtAndUpdVM? db = await BasicDetailBL.GetBasicDetailByRequestId(RequestId);

                int i = 1;
                List<DTODigitalSignPlusLog> DigitalSignPlusLogList = new List<DTODigitalSignPlusLog>();

                foreach (XmlNode node in fwddetails)
                {
                    string base64EncodedCertificate1 = node.InnerXml;
                    // byte[] certBytes = Convert.FromBase64String(base64EncodedCertificate);
                    LevelMessage levelMessage = dictionarLevel[i];
                    DTODigitalSignPlusLog obj = new DTODigitalSignPlusLog();
                    obj.Sno = i;
                    obj.FromDomain = node["FromDomain"].InnerText;
                    obj.FromRank = node["FromRank"].InnerText;
                    obj.FromProfile = node["FromProfile"].InnerText;
                    obj.FromArmyNo = node["FromArmyNo"].InnerText;
                    obj.FromDate = Convert.ToDateTime(node["FromDate"].InnerText);
                    obj.LevelMessage = levelMessage.Name;

                    DTOFwdLastRecForDigitalSign? objDS = DigitalSignList.Where(x => x.FromArmyNo.Contains(obj.FromArmyNo)).FirstOrDefault();

                    int stepId = Convert.ToInt32(node["StepId"].InnerText);

                    if (stepId == 2)
                    {
                        if (objDS != null)
                        {
                            obj.IsLogWithSign = true;
                            obj.DSProfile = objDS.FromProfile;
                            obj.DSArmyNo = objDS.FromArmyNo;
                        }
                        else
                        {
                            DTOFwdLastRecForDigitalSign? dTOFwd = DigitalSignList.Where(x => x.FromArmyNo.Contains(db.ServiceNo)).FirstOrDefault();
                            if (dTOFwd != null)
                            {
                                obj.IsLogWithSign = true;
                                obj.DSProfile = dTOFwd.FromProfile;
                                obj.DSArmyNo = dTOFwd.FromArmyNo;
                            }
                        }
                    }
                    else
                    {
                        if (objDS != null)
                        {
                            obj.IsLogWithSign = true;
                            obj.DSProfile = objDS.FromProfile;
                            obj.DSArmyNo = objDS.FromArmyNo;
                        }
                    }

                    DigitalSignPlusLogList.Add(obj);
                    i++;
                }

                string pdfname = db.ServiceNo + "_" + RequestId + "_" + yearName + "" + monthName + "" + dayName + "" + hh + "" + mm + "" + ss + ".pdf";
                var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignaturePdf\\" + pdfname);
                //if (!System.IO.File.Exists(filePath1))
                //{
                PdfWriter writer = new PdfWriter(filePath1);
                PdfDocument pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(PageSize.A4);
                Document document = new Document(pdf);
                document.SetMargins(36, 36, 36, 36);
                document.SetFontSize(12f);
                
                // Add header and footer event
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderFooterHandler());

                // Add watermark to each page
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new BottomLeftDiagonalWatermarkHandler(ipAddress));

                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                Paragraph header = new Paragraph("I-Card Process" +
                    " Digital Signature").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);

                document.Add(header);

                Paragraph subheader = new Paragraph("Pers info Details").SetTextAlignment(TextAlignment.CENTER).SetFontSize(15);
                document.Add(subheader);

                Table table = new Table(4);

                String imphotoFile = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\WriteReadData\\Photo\\" + db.PhotoImagePath);
                ImageData dataphoto = ImageDataFactory.Create(imphotoFile);

                iTextImage imagedataphoto = new iTextImage(dataphoto);
                imagedataphoto.SetWidth(60);
                Cell imageCellphotos = new Cell().Add(imagedataphoto);
                //imageCellphotos.SetBorder(null);
                table.AddCell(new Paragraph("Photos").SetFont(boldFont));
                table.AddCell(imageCellphotos);

                // Add header cells to the table
                table.AddCell(new Paragraph("NAME").SetFont(boldFont));
                table.AddCell(db.RankName + " " + db.FName + (db.LName == null ? "" : " " + db.LName));
                table.AddCell(new Paragraph("Rank").SetFont(boldFont));
                table.AddCell(db.RankName);

                // Add data row with 4 columns
                table.AddCell(new Paragraph("Arm / Service").SetFont(boldFont));
                table.AddCell(db.ArmedName);
                table.AddCell(new Paragraph("Army No").SetFont(boldFont));
                table.AddCell(db.ServiceNo);
                table.AddCell(new Paragraph("IdenMark1").SetFont(boldFont));
                table.AddCell(db.IdenMark1);
                table.AddCell(new Paragraph("Date of Birth").SetFont(boldFont));
                table.AddCell(Convert.ToString(db.DOB.ToShortDateString()).Replace("-", "/"));
                table.AddCell(new Paragraph("Height (Cm)").SetFont(boldFont));
                table.AddCell(Convert.ToString(db.Height));
                table.AddCell(new Paragraph("AADHAAR No").SetFont(boldFont));
                table.AddCell(db.AadhaarNo);
                table.AddCell(new Paragraph("BloodGroup").SetFont(boldFont));
                table.AddCell(db.BloodGroup);
                table.AddCell(new Paragraph("Place of Issue").SetFont(boldFont));
                table.AddCell(db.PlaceOfIssue);
                table.AddCell(new Paragraph("Date of Issue").SetFont(boldFont));
                table.AddCell(Convert.ToString(db.DateOfIssue.ToShortDateString()).Replace("-", "/"));
                table.AddCell(new Paragraph("Issuing Authority").SetFont(boldFont));
                table.AddCell(db.IssuingAuthorityName);
                table.AddCell(new Paragraph("Date of Commissioning/ Enrollment").SetFont(boldFont));
                table.AddCell(Convert.ToString(db.DateOfCommissioning.ToShortDateString()).Replace("-", "/"));

                table.AddCell(new Paragraph("Permt Address as per Service Records").SetFont(boldFont));
                //table.AddCell(new Cell(1, 3).Add(new Paragraph("Amount")));
                table.AddCell("Village - " + db.Village + ", Post Office-" + db.PO + ", Tehsil- " + db.Tehsil + ", District- " + db.District + ", State- " + db.State + ", Pin Code- " + db.PinCode);
                table.AddCell(new Paragraph("Signature").SetFont(boldFont));
                String sigFile = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\WriteReadData\\Signature\\" + db.SignatureImagePath);
                ImageData datasig = ImageDataFactory.Create(sigFile);

                iTextImage imagedatasig = new iTextImage(datasig);
                imagedatasig.SetWidth(60);
                Cell imageCellsig = new Cell().Add(imagedatasig);
                //imageCellsig.SetBorder(null);
                table.AddCell(imageCellsig);

                document.Add(table);

                Paragraph header2 = new Paragraph("Details of Digital Signature & Digital Log.").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);

                document.Add(header2);

                // Define custom column widths
                float[] columnWidths = { 18F, 144F, 73F, 144F, 72F, 72F };

                Table tableFwd = new Table(columnWidths);
                tableFwd.SetPadding(5);
                tableFwd.SetSpacingRatio(2);
                tableFwd.AddHeaderCell(new Paragraph("Ser No").SetFont(boldFont));
                tableFwd.AddHeaderCell(new Paragraph("Personal Details").SetFont(boldFont));
                tableFwd.AddHeaderCell(new Paragraph("Approvers").SetFont(boldFont));
                tableFwd.AddHeaderCell(new Paragraph("Date and Time").SetFont(boldFont));
                tableFwd.AddHeaderCell(new Paragraph("Digital Log").SetFont(boldFont));
                tableFwd.AddHeaderCell(new Paragraph("Digital Signature").SetFont(boldFont));

                i = 1;
                if (DigitalSignPlusLogList.Count > 0)
                {
                    foreach (var digitaldata in DigitalSignPlusLogList)
                    {
                        if (digitaldata.IsLogWithSign == true)
                        {
                            if (digitaldata.FromArmyNo == digitaldata.DSArmyNo)
                            {
                                Cell cell_1 = new Cell();
                                cell_1.Add(new Paragraph(i.ToString()));
                                cell_1.SetTextAlignment(TextAlignment.CENTER);

                                tableFwd.AddCell(cell_1);
                                tableFwd.AddCell(new Paragraph(digitaldata.FromProfile + "\n" + digitaldata.FromArmyNo));
                                tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage + " (" + digitaldata.FromDomain + " )"));
                                tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""));
                                tableFwd.AddCell(CreateApprovedImage());
                                tableFwd.AddCell(CreateDigitalSignImage());
                            }
                            else
                            {
                                Cell cell_1 = new Cell();
                                cell_1.Add(new Paragraph(i.ToString()));
                                cell_1.SetTextAlignment(TextAlignment.CENTER);

                                tableFwd.AddCell(cell_1);
                                //tableFwd.AddCell(new Paragraph(digitaldata.Sno.ToString()).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                tableFwd.AddCell(new Paragraph("" + digitaldata.FromRank + " " + digitaldata.FromProfile + "\n" + digitaldata.FromArmyNo));
                                tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage + " (" + digitaldata.FromDomain + " )"));
                                tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""));
                                tableFwd.AddCell(CreateApprovedImage());
                                tableFwd.AddCell(new Paragraph("NA"));

                                i++;

                                Cell cell_2 = new Cell();
                                cell_2.Add(new Paragraph(i.ToString()));
                                cell_2.SetTextAlignment(TextAlignment.CENTER);

                                tableFwd.AddCell(cell_2);
                                //tableFwd.AddCell(new Paragraph(digitaldata.Sno.ToString()).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                                tableFwd.AddCell(new Paragraph(digitaldata.DSProfile + "\n" + digitaldata.DSArmyNo));
                                tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage));
                                tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""));
                                tableFwd.AddCell(new Paragraph("NA"));
                                tableFwd.AddCell(CreateDigitalSignImage());
                            }


                        }
                        else
                        {
                            Cell cell_1 = new Cell();
                            cell_1.Add(new Paragraph(i.ToString()));
                            cell_1.SetTextAlignment(TextAlignment.CENTER);

                            tableFwd.AddCell(cell_1);
                            //tableFwd.AddCell(new Paragraph(digitaldata.Sno.ToString()).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                            tableFwd.AddCell(new Paragraph("" + digitaldata.FromRank + " " + digitaldata.FromProfile + "\n" + digitaldata.FromArmyNo));
                            tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage + " (" + digitaldata.FromDomain + " )"));
                            tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""));
                            tableFwd.AddCell(CreateApprovedImage());
                            tableFwd.AddCell(new Paragraph("NA"));
                        }
                        i++;
                    }
                }

                document.Add(tableFwd);
                document.Close();
                return Json(pdfname);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }

        // Custom event handler for header and footer
        public class HeaderFooterHandler : IEventHandler
        {
            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();

                // Get the current page
                PdfPage page = docEvent.GetPage();
                float width = page.GetPageSize().GetWidth();
                float height = page.GetPageSize().GetHeight();

                // Create the bold font
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Header Text
                string headerText = "Confidential";
                // Footer Text
                string footerText = "Confidential";

                // Add header
                PdfCanvas canvas = new PdfCanvas(page);

                // Center header text
                float headerTextWidth = boldFont.GetWidth(headerText, 12);
                float headerTextPositionX = (width - headerTextWidth) / 2;

                canvas.BeginText()
                    .SetFontAndSize(boldFont, 12)
                    .MoveText(headerTextPositionX, height - 30)  // Center the text horizontally
                    .ShowText(headerText)
                    .EndText();

                // Center footer text
                float footerTextWidth = boldFont.GetWidth(footerText, 10);
                float footerTextPositionX = (width - footerTextWidth) / 2;

                canvas.BeginText()
                    .SetFontAndSize(boldFont, 10)
                    .MoveText(footerTextPositionX, 30)  // Center the text horizontally
                    .ShowText(footerText)
                    .EndText();
            }
        }
        public Cell CreateApprovedImage()
        {
            String imFileFwd = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Images\\digital log stamp.jpg");
            ImageData dataFwd = ImageDataFactory.Create(imFileFwd);

            iTextImage imageFwd = new iTextImage(dataFwd);
            //imageFwd.ScaleAbsolute(100, 150);
            imageFwd.SetMargins(0, 0, 0, 0);
            Cell imageCellFwd = new Cell().Add(imageFwd);
            imageCellFwd.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            imageCellFwd.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            //imageCellFwd.SetBorder(Border.NO_BORDER);
            imageFwd.SetWidth(100);
            //imageFwd.SetHeight(150);
            return imageCellFwd;
        }
        public Cell CreateDigitalSignImage()
        {
            String imFileFwd = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Images\\Digital Approved Stamp.jpg");
            ImageData dataFwd = ImageDataFactory.Create(imFileFwd);

            iTextImage imageFwd = new iTextImage(dataFwd);
            //imageFwd.ScaleAbsolute(100, 150);
            imageFwd.SetMargins(0, 0, 0, 0);
            Cell imageCellFwd = new Cell().Add(imageFwd);
            imageCellFwd.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            imageCellFwd.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            //imageCellFwd.SetBorder(Border.NO_BORDER);
            imageFwd.SetWidth(100);
            //imageFwd.SetHeight(150);
            return imageCellFwd;
        }
        // Custom event handler for diagonal watermark from bottom-left to top-right
        public class BottomLeftDiagonalWatermarkHandler : IEventHandler
        {
            private string _ipAddress;
            public BottomLeftDiagonalWatermarkHandler(string ipAddress)
            {
                _ipAddress = ipAddress;
            }
            public void HandleEvent(Event @event)
            {
                //_ipAddress = "192.168.100.10";
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfPage page = docEvent.GetPage();
                float width = page.GetPageSize().GetWidth();
                float height = page.GetPageSize().GetHeight();

                string watermarkText = $"{_ipAddress}  {DateTime.Now:dd-MM-yyyy HH:mm:ss}";

                // Create font for the watermark text
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // PdfCanvas for drawing
                PdfCanvas canvas = new PdfCanvas(page);

                // Set red color and opacity for the watermark
                canvas.SaveState();
                canvas.SetFillColor(new DeviceRgb(255, 0, 0)); // Red color

                PdfExtGState gState = new PdfExtGState().SetFillOpacity(0.2f); // 20% opacity
                canvas.SetExtGState(gState); // Apply the opacity setting

                // Set font and size (adjusted to ensure text stays within bounds)
                float fontSize = 40; // Adjust based on page size if needed
                canvas.SetFontAndSize(font, fontSize);

                // Calculate the center of the page
                float centerX = width / 2;
                float centerY = height / 2;

                // Rotation angle (45 degrees for a diagonal watermark)
                float angle = 45;
                float radians = (float)(angle * Math.PI / 180);

                // Translate, rotate, and then draw text from center point
                canvas.SaveState();
                canvas.ConcatMatrix((float)Math.Cos(radians), (float)Math.Sin(radians),
                                    (float)-Math.Sin(radians), (float)Math.Cos(radians),
                                    centerX, centerY);
                canvas.BeginText()
                      .MoveText(-font.GetWidth(watermarkText, fontSize) / 2, -fontSize / 2) // Adjusts to keep text centered
                      .ShowText(watermarkText)
                      .EndText();
                canvas.RestoreState();
            }
        }
    }
}
