using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.TrnLoginLog;
using DataTransferObject.Constants;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using iText.Commons.Actions;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Web.Healpers;
using Web.Healpers.BaseInterfaces;
using Web.WebHelpers;
using iTextImage = iText.Layout.Element.Image;
using Path = System.IO.Path;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for handling log-related actions and views. Authorized access is required.
    /// </summary>
    [Authorize]
    public class LogController : Controller
    {
        private readonly ITrnLoginLogBL _iTrnLoginLogBL;//Interface for login log business logic layer
        private readonly IBasicDetailBL BasicDetailBL;//Interface for basic detail business logic layer
        private readonly IWebHostEnvironment hostingEnvironment;//Interface for accessing web hosting environment details
        private readonly IHttpContextAccessor _httpContextAccessor;//Interface for accessing HTTP context details
        private readonly IImageEncryptAndDecrypt imageEncryptAndDecrypt;//Interface for image encryption and decryption operations
        private readonly ILogger<LogController> _logger;

        //constructor to initialize dependencies and configuration settings.
        public LogController(ITrnLoginLogBL iTrnLoginLogBL, IWebHostEnvironment hostingEnvironment, IBasicDetailBL BasicDetailBL, IHttpContextAccessor httpContextAccessor, IImageEncryptAndDecrypt imageEncryptAndDecrypt, ILogger<LogController> logger)
        {
            _iTrnLoginLogBL = iTrnLoginLogBL;
            this.hostingEnvironment = hostingEnvironment;
            this.BasicDetailBL = BasicDetailBL;
            _httpContextAccessor = httpContextAccessor;
            this.imageEncryptAndDecrypt = imageEncryptAndDecrypt;
            _logger = logger;
        }
        /// <summary>
        /// Action method to retrieve and display login logs based on the user's session.
        /// The method checks if the session contains a valid token and retrieves the login logs
        /// for all users belonging to the same unit ID as the current user.
        /// </summary>
        /// <returns>A view displaying login logs for the user's unit or null if the session is invalid.</returns>
        [HttpGet]
        public async Task<IActionResult> LoginLog()
        {
            string role = SessionHelper.GetRoleFromSession(HttpContext);

            // Get the referer header from the request
            string referer = HttpContext.Request.Headers["Referer"].ToString();

            // Retrieve the session data
            DtoSession dtoSession = new DtoSession();
            dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            // If the session is valid, retrieve and display the login logs for the user's unit
            if (dtoSession != null)
            {
                var data = await _iTrnLoginLogBL.GetAllUserByUnitId(dtoSession.UnitId);

                if (role == "user")
                {
                    return View(data);
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
                if (role == "user")
                {
                    // If session is invalid, return a view with no data
                    return View(null);
                }
                else
                {
                    TempData["error"] = "Switch to user role.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }
        }

        /// <summary>
        /// Action method to retrieve login logs for a specific user based on their AspNetUserId
        /// and an optional date range (From and To dates). It returns the logs as a JSON response.
        /// </summary>
        /// <param name="AspNetUsersId">The ID of the user whose login logs are being requested.</param>
        /// <param name="FmDate">The optional start date for filtering the logs.</param>
        /// <param name="ToDate">The optional end date for filtering the logs.</param>
        /// <returns>A JSON response containing the login logs or 0 in case of an error.</returns>
        [HttpPost]
        public async Task<IActionResult> LoginLogByAspNetUsersId(int AspNetUsersId, DateTime? FmDate, DateTime? ToDate)
        {
            try
            {
                int UnitId = 0;
                // Initialize the DTO session object
                DtoSession? dtoSession = new DtoSession();

                // Retrieve the session data if available
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                    // Retrieve relevant session information such as UnitId, TrnDomainMappingId, and UserId
                    UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                }

                // Retrieve the login logs based on the user ID and date range
                return Json(await _iTrnLoginLogBL.GetLoginLogByUserId(AspNetUsersId, UnitId, FmDate, ToDate));
            }
            catch (Exception ex)
            {
                // Return an error response in case of an exception
                return Json(0);
            }
        }

        /// <summary>
        /// Action method to handle the digital signing of an XML file for forwarding logs.
        /// It updates the data with the current timestamp and the ID of the logged-in user
        /// and returns the result as a JSON response.
        /// </summary>
        /// <param name="Data">The request data containing the XML file information to be digitally signed.</param>
        /// <returns>A JSON response indicating the success or failure of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> XmlFileDigitalSign(string request)
        {
            try
            {
                DTOXmlFilesFwdLogRequest Data = await AESEncrytDecry.DecryptAESWithDTO<DTOXmlFilesFwdLogRequest>(request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);
                if (Data == null)
                {
                    return Json(KeyConstants.IncorrectData); // Return error message for invalid data
                }
                ModelState.Clear();
                if (TryValidateModel(Data))
                {
                    // Decode the Base64 string
                    byte[] decodedBytes = Convert.FromBase64String(Data.XmlFiles);
                    string xmlString = Encoding.UTF8.GetString(decodedBytes);

                    Data.XmlFiles = xmlString;
                    Data.UpdatedOn = DateTime.Now;
                    Data.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    Data.IsActive = true;
                    Data.Id = Data.Id;

                    return Json(await _iTrnLoginLogBL.XmlFileDigitalSign(Data));
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Log=>XmlFileDigitalSign.");
                return Json(0);
            }
        }


        /// <summary>
        /// Action method to save a digitally signed PDF. It receives a base64-encoded PDF string and saves it to a specified directory.
        /// The PDF is saved with a filename based on the service number and request ID.
        /// </summary>
        /// <param name="RequestId">The ID of the request associated with the PDF.</param>
        /// <param name="base64">The base64-encoded string representing the PDF content to be saved.</param>
        /// <returns>A JSON response containing the filename of the saved PDF.</returns>
        [HttpPost]
        public async Task<IActionResult> DigitalpdfsignatureSave(int RequestId, string base64)
        {
            // Retrieve the basic details of the request based on the RequestId
            DTOBasicDetailByRequestIdResponse? db = await BasicDetailBL.GetBasicDetailByRequestId(RequestId);

            // Define the file path where the PDF will be saved
            var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignaturePdf\\" + db.ServiceNo + "_" + RequestId + ".pdf");

            // Convert the base64 string to a byte array representing the PDF
            byte[] pdfBytes = Convert.FromBase64String(base64);

            // Write the PDF bytes to the specified file path
            System.IO.File.WriteAllBytes(filePath1, pdfBytes);

            // Return the filename of the saved PDF in the response
            return Json(db.ServiceNo + "_" + RequestId + ".pdf");
        }

        /// <summary>
        /// Action method to create an XML file based on the provided request ID. It generates an XML file
        /// and saves it in a specific directory with a filename that includes the service number, request ID,
        /// and the current timestamp (year, month, day, hour, minute, second).
        /// </summary>
        /// <param name="RequestId">The ID of the request used to generate the XML file.</param>
        /// <returns>A JSON response containing the filename of the saved XML file, or 0 in case of an error.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateXmlAsync(string Request)
        {
            int RequestId = await AESEncrytDecry.DecryptAESWithDTO<int>(Request, SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token").Salt);

            try
            {
                // Generate date components for naming the file
                var now = DateTime.Now;
                var yearName = now.ToString("yyyy");
                var monthName = now.ToString("MMMM");
                var dayName = now.ToString("dd");
                var hh = now.ToString("hh");
                var mm = now.ToString("mm");
                var ss = now.ToString("ss");

                // Initialize the array for the request ID and fetch the XML data
                int[] d = new int[1];
                d[0] = RequestId;
                var sata = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(d);

                // Replace any ampersand symbols with the proper XML encoding
                string XmlFilesRemoveAndChar = sata.XmlFiles.Replace("&", "&amp;");

                // Parse the cleaned-up XML data into an XDocument
                XDocument document = XDocument.Parse(Convert.ToString(XmlFilesRemoveAndChar));

                // Retrieve basic details of the request for naming the XML file
                DTOBasicDetailByRequestIdResponse? db = await BasicDetailBL.GetBasicDetailByRequestId(RequestId);

                // Define the directory where the XML file will be saved
                string sourceFolder = Path.Combine(hostingEnvironment.WebRootPath, "DigitallysignatureXml");

                // Check if the directory exists, and create it if it does not
                if (!Directory.Exists(sourceFolder))
                {
                    Directory.CreateDirectory(sourceFolder);
                }

                // Generate the XML file name based on the request details and current timestamp
                string xmlname = db.ServiceNo + "_" + RequestId + "_" + yearName + "" + monthName + "" + dayName + "" + hh + "" + mm + "" + ss + ".xml";
                var filePath1 = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\DigitallysignatureXml\\" + xmlname);

                // Save the XML document to the specified file path
                document.Save(filePath1);

                // Return the filename of the saved XML file as a JSON response
                return Json(xmlname);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                // Return an error response if the directory is not found
                return Json(0);
            }
            catch (Exception ex)
            {
                // Return a generic error response for other exceptions
                return Json(0);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdfAsync(string Request)
        {
            var session = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
            if (session == null)
                return Unauthorized();

            int requestId = await AESEncrytDecry.DecryptAESWithDTO<int>(Request, session.Salt);
            if (requestId <= 0)
                return BadRequest("Invalid RequestId");

            try
            {
                string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown IP";

                var levelDictionary = new Dictionary<int, string>
                {
                    { 1, "1st Level" },
                    { 2, "2nd Level" },
                    { 3, "3rd Level" },
                    { 4, "4th Level" }
                };

                var now = DateTime.Now;
                string timestamp = $"{now:yyyy}{now:MMMM}{now:dd}{now:hh}{now:mm}{now:ss}";

                var sata = await _iTrnLoginLogBL.XmlFileDigitalSignFromData(new[] { requestId });
                if (sata == null || string.IsNullOrWhiteSpace(sata.XmlFiles))
                    return BadRequest("XML data not found.");

                string sanitizedXml = sata.XmlFiles.Replace("&", "&amp;");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sanitizedXml);


                var digitalSignList = ProcessDigitalSignatures(xmlDoc);
                DTOGenericResponse<DTOBasicDetailForParitalViewResponse> response = await BasicDetailBL.GetBasicDetailForParitalViewByRequestId(requestId);

                if (response.Result == false)
                    return NotFound("The requested information is currently unavailable. Please try again after some time.");

                var digitalSignPlusLogList = ProcessForwardingDetails(xmlDoc, levelDictionary, digitalSignList, response.Value.ServiceNo);

                byte[] pdfBytes = await GeneratePdfDocument(response.Value, digitalSignPlusLogList, ipAddress, timestamp, requestId);

                string pdfName = $"{response.Value.ServiceNo}_{requestId}_{timestamp}.pdf";

                return File(pdfBytes, "application/pdf", pdfName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PDF for RequestId: {RequestId}", requestId);
                return StatusCode(500, "Error generating PDF");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadGeneratedPdf(int requestId)
        {
            try
            {
                var session = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");
                if (session == null)
                    return Unauthorized();

                // Add your authorization check here if needed
                // Example:
                // bool allowed = await _logBL.UserCanAccessRequest(requestId, session.UserId);
                // if (!allowed) return Forbid();

                DTOBasicDetailByRequestIdResponse? db = await BasicDetailBL.GetBasicDetailByRequestId(requestId);
                if (db == null)
                    return NotFound();

                string secureFolder = Path.Combine(hostingEnvironment.ContentRootPath, "SecurePdfFiles");
                if (!Directory.Exists(secureFolder))
                    return NotFound();

                // find latest generated PDF for this request
                string searchPattern = $"*_{requestId}_*.pdf";
                string? latestFile = Directory.GetFiles(secureFolder, searchPattern)
                                              .OrderByDescending(System.IO.File.GetCreationTime)
                                              .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(latestFile) || !System.IO.File.Exists(latestFile))
                    return NotFound();

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(latestFile);
                string fileName = Path.GetFileName(latestFile);

                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading PDF for RequestId: {RequestId}", requestId);
                return StatusCode(500, "Unable to download PDF.");
            }
        }

        private List<DTOFwdLastRecForDigitalSign> ProcessDigitalSignatures(XmlDocument xmlDoc)
        {

            var digitalSignList = new List<DTOFwdLastRecForDigitalSign>();
            var certificateNodes = xmlDoc.GetElementsByTagName("X509Certificate");
            bool WithExpTo = false;
            if (WithExpTo == false)
            {
                foreach (XmlNode node in certificateNodes)
                {
                    try
                    {
                        byte[] certBytes = Convert.FromBase64String(node.InnerText);
                        using var certificate = new X509Certificate2(certBytes);

                        // Optimized subject parsing
                        var subjectDict = new Dictionary<string, string>();
                        var subjectParts = certificate.Subject.Split(',', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var part in subjectParts)
                        {
                            var kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
                            if (kv.Length == 2)
                                subjectDict[kv[0]] = kv[1];
                        }
                        digitalSignList.Add(new DTOFwdLastRecForDigitalSign
                        {
                            FromProfile = subjectDict.GetValueOrDefault("CN", ""),
                            FromArmyNo = subjectDict.GetValueOrDefault("SERIALNUMBER", "").ToUpper()
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to process digital signature");
                    }
                }
            }
            else
            {
                var CERT_SERIAL_1 = Environment.GetEnvironmentVariable("CERT_SERIAL_1") ?? string.Empty;
                var CERT_SERIAL_2 = Environment.GetEnvironmentVariable("CERT_SERIAL_2") ?? string.Empty;
                var ARMY_NO_1 = Environment.GetEnvironmentVariable("ARMY_NO_1") ?? string.Empty;
                var ARMY_NO_2 = Environment.GetEnvironmentVariable("ARMY_NO_2") ?? string.Empty;

                // Precompute Uppercase serials for comparison
                var serial1 = CERT_SERIAL_1.ToUpper();
                var serial2 = CERT_SERIAL_2.ToUpper();

                foreach (XmlNode node in certificateNodes)
                {
                    try
                    {
                        byte[] certBytes = Convert.FromBase64String(node.InnerText);
                        using var certificate = new X509Certificate2(certBytes);

                        // Optimized subject parsing
                        var subjectDict = new Dictionary<string, string>();
                        var subjectParts = certificate.Subject.Split(',', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var part in subjectParts)
                        {
                            var kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
                            if (kv.Length == 2)
                                subjectDict[kv[0]] = kv[1];
                        }

                        var serialNumber = subjectDict.GetValueOrDefault("SERIALNUMBER", "").ToUpper();
                        string armyNo = serialNumber switch
                        {
                            string s when s == serial1 => ARMY_NO_1.ToUpper(),
                            string s when s == serial2 => ARMY_NO_2.ToUpper(),
                            _ => serialNumber
                        };

                        digitalSignList.Add(new DTOFwdLastRecForDigitalSign
                        {
                            FromProfile = subjectDict.GetValueOrDefault("CN", ""),
                            FromArmyNo = armyNo
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to process digital signature");
                    }
                }
            }


            return digitalSignList;
        }
        private List<DTODigitalSignPlusLog> ProcessForwardingDetails(XmlDocument xmlDoc,Dictionary<int, string> levelDictionary, List<DTOFwdLastRecForDigitalSign> digitalSignList, string ServiceNo)
        {
            var result = new List<DTODigitalSignPlusLog>();
            XmlNodeList forwardDetails = xmlDoc.GetElementsByTagName("RecForDigitalSign");

            var orderedNodes = forwardDetails.Cast<XmlNode>()
                                .OrderBy(x => 
                                {
                                    int.TryParse(x.SelectSingleNode("StepId")?.InnerText, out int stepId);
                                    return stepId;
                                }).ToList();

            for (int i = 0; i < orderedNodes.Count; i++)
            {
                var node = orderedNodes[i];
                int stepId = Convert.ToInt32(node["StepId"]?.InnerText);

                var logEntry = new DTODigitalSignPlusLog
                {
                    Sno = i + 1,
                    FromDomain = node["FromDomain"]?.InnerText,
                    FromRank = node["FromRank"]?.InnerText,
                    FromProfile = node["FromProfile"]?.InnerText,
                    FromArmyNo = node["FromArmyNo"]?.InnerText?.ToUpperInvariant() ?? string.Empty,
                    FromDate = Convert.ToDateTime(node["FromDate"]?.InnerText),
                    LevelMessage = levelDictionary.GetValueOrDefault(i + 1, "Unknown Level")
                };

                // Find matching digital signature
                var digitalSignature = digitalSignList.FirstOrDefault(x =>x.FromArmyNo.Contains(logEntry.FromArmyNo,StringComparison.OrdinalIgnoreCase));

                if (stepId == 2 && digitalSignature == null)
                {
                    digitalSignature = digitalSignList.FirstOrDefault(x =>x.FromArmyNo.Contains(ServiceNo, StringComparison.OrdinalIgnoreCase));
                }

                if (digitalSignature != null)
                {
                    logEntry.IsLogWithSign = true;
                    logEntry.DSProfile = digitalSignature.FromProfile;
                    logEntry.DSArmyNo = digitalSignature.FromArmyNo.ToUpper();
                }

                result.Add(logEntry);
            }

            return result;
        }

        private async Task<byte[]> GeneratePdfDocument(
             DTOBasicDetailForParitalViewResponse db,
             List<DTODigitalSignPlusLog> digitalSignPlusLogList,
             string ipAddress,
             string timestamp,
             int requestId)
        {
            using var memoryStream = new MemoryStream();

            var photoImage = await GetDecryptedImage(db.PhotoImagePath, "Photo", 60);
            var signatureImage = await GetDecryptedImage(db.SignatureImagePath, "Signature", 80);

            using (PdfWriter writer = new PdfWriter(memoryStream))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf))
            {
                pdf.SetDefaultPageSize(PageSize.A4);
                document.SetMargins(36, 36, 36, 36);
                document.SetFontSize(12f);

                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderFooterHandler());
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new BottomLeftDiagonalWatermarkHandler(ipAddress));

                AddDocumentHeader(document);
                AddPersonalDetailsTable(document, db, photoImage, signatureImage);
                AddDigitalSignatureTable(document, digitalSignPlusLogList);
            }

            return memoryStream.ToArray();
        }

        private void AddDocumentHeader(Document document)
        {
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph("I-Card Process Digital Signature")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            document.Add(new Paragraph("Pers info Details")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(15));
        }

        private void AddPersonalDetailsTable(Document document, DTOBasicDetailForParitalViewResponse db, iTextImage? photoImage, iTextImage? signatureImage)
        {
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            Table table = new Table(4);

            // Add photo
            table.AddCell(new Paragraph("Photos").SetFont(boldFont));
            table.AddCell(photoImage != null ? new Cell().Add(photoImage) : new Cell().Add(new Paragraph("N/A")));

            // Add personal details
            AddTableRow(table, "NAME", $"{db.RankName} {db.FName} {db.LName}".Trim(), boldFont);
            AddTableRow(table, "Rank", db.RankName ?? string.Empty, boldFont);
            AddTableRow(table, "Arm / Service", db.ArmedName ?? string.Empty, boldFont);
            AddTableRow(table, "Army No", db.ServiceNo, boldFont);
            AddTableRow(table, "IdenMark1", db.IdenMark1, boldFont);
            AddTableRow(table, "Date of Birth", db.DOB.ToString("dd-MMM-yyyy"), boldFont);
            AddTableRow(table, "Height (Cm)", db.Height.ToString(), boldFont);
            AddTableRow(table, "AADHAAR No", Regex.Replace(db.AadhaarNo, @"\d(?=\d{4})", "X"), boldFont);
            AddTableRow(table, "BloodGroup", db.BloodGroup ?? string.Empty, boldFont);
            AddTableRow(table, "Place of Issue", db.PlaceOfIssue ?? string.Empty, boldFont);
            AddTableRow(table, "Date of Issue",
                db.DateOfIssue is DateTime dateOfIssue && dateOfIssue != DateTime.MinValue ? dateOfIssue.ToString("dd-MMM-yyyy") : "",
                boldFont);
            AddTableRow(table, "Issuing Authority", db.IssuingAuthorityName, boldFont);
            AddTableRow(table, "Date of Commissioning/ Enrollment",
                db.DateOfCommissioning.ToString("dd-MMM-yyyy"), boldFont);

            // Address row
            table.AddCell(new Paragraph("Permt Address as per Service Records").SetFont(boldFont));
            table.AddCell(new Cell(1, 3).Add(new Paragraph(
                $"Village - {db.Village}, Post Office-{db.PO}, Tehsil- {db.Tehsil}, " +
                $"District- {db.District}, State- {db.State}, Pin Code- {db.PinCode}")));

            // Signature
            table.AddCell(new Paragraph("Signature").SetFont(boldFont));
            table.AddCell(signatureImage != null ? new Cell().Add(signatureImage) : new Cell().Add(new Paragraph("N/A")));

            document.Add(table);
        }

        private void AddTableRow(Table table, string header, string value, PdfFont boldFont)
        {
            table.AddCell(new Paragraph(header).SetFont(boldFont));
            table.AddCell(new Paragraph(value));
        }

        private async Task<iTextImage?> GetDecryptedImage(string imagePath, string folder, float width)
        {
            string sourcePath = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", folder, imagePath);

            // Return null when the image file does not exist
            if (!System.IO.File.Exists(sourcePath))
            {
                return null;
            }

            string base64Image = await imageEncryptAndDecrypt.DecryptImageToBase64(sourcePath);

            if (string.IsNullOrEmpty(base64Image))
                return null;

            string base64Data = base64Image.Split(',')[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            ImageData imageData = ImageDataFactory.Create(imageBytes);

            iTextImage image = new iTextImage(imageData);
            image.SetWidth(width);

            return image;
        }

        private void AddDigitalSignatureTable(Document document, List<DTODigitalSignPlusLog> digitalSignPlusLogList)
        {
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph("Details of Digital Signature & Digital Log.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            float[] columnWidths = { 18F, 144F, 73F, 144F, 72F, 72F };
            Table tableFwd = new Table(columnWidths);
            tableFwd.SetPadding(5);
            tableFwd.SetSpacingRatio(2);

            // Add headers
            tableFwd.AddHeaderCell(new Paragraph("Ser No").SetFont(boldFont));
            tableFwd.AddHeaderCell(new Paragraph("Personal Details").SetFont(boldFont));
            tableFwd.AddHeaderCell(new Paragraph("Approvers").SetFont(boldFont));
            tableFwd.AddHeaderCell(new Paragraph("Date and Time").SetFont(boldFont));
            tableFwd.AddHeaderCell(new Paragraph("Digital Log").SetFont(boldFont));
            tableFwd.AddHeaderCell(new Paragraph("Digital Signature").SetFont(boldFont));

            int i = 1;
            foreach (var digitaldata in digitalSignPlusLogList)
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
                        tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd-MMM-yyyy HH:mm:ss") : ""));
                        tableFwd.AddCell(CreateApprovedImage());
                        tableFwd.AddCell(CreateDigitalSignImage());
                    }
                    else
                    {
                        Cell cell_1 = new Cell();
                        cell_1.Add(new Paragraph(i.ToString()));
                        cell_1.SetTextAlignment(TextAlignment.CENTER);

                        tableFwd.AddCell(cell_1);
                        tableFwd.AddCell(new Paragraph("" + digitaldata.FromRank + " " + digitaldata.FromProfile + "\n" + digitaldata.FromArmyNo));
                        tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage + " (" + digitaldata.FromDomain + " )"));
                        tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd-MMM-yyyy HH:mm:ss") : ""));
                        tableFwd.AddCell(CreateApprovedImage());
                        tableFwd.AddCell(new Paragraph("NA"));

                        i++;

                        Cell cell_2 = new Cell();
                        cell_2.Add(new Paragraph(i.ToString()));
                        cell_2.SetTextAlignment(TextAlignment.CENTER);

                        tableFwd.AddCell(cell_2);
                        tableFwd.AddCell(new Paragraph(digitaldata.DSProfile + "\n" + digitaldata.DSArmyNo));
                        tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage));
                        tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd-MMM-yyyy HH:mm:ss") : ""));
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
                    tableFwd.AddCell(new Paragraph("" + digitaldata.FromRank + " " + digitaldata.FromProfile + "\n" + digitaldata.FromArmyNo));
                    tableFwd.AddCell(new Paragraph(digitaldata.LevelMessage + " (" + digitaldata.FromDomain + " )"));
                    tableFwd.AddCell(new Paragraph(digitaldata.FromDate != null ? digitaldata.FromDate.Value.ToString("dd-MMM-yyyy HH:mm:ss") : ""));
                    tableFwd.AddCell(CreateApprovedImage());
                    tableFwd.AddCell(new Paragraph("NA"));
                }
                i++;
            }

            document.Add(tableFwd);
        }
        // Custom event handler for header and footer
        public class HeaderFooterHandler : AbstractPdfDocumentEventHandler
        {
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
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
            System.String imFileFwd = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Images\\digital log stamp.jpg");
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
            System.String imFileFwd = System.IO.Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Images\\Digital Approved Stamp.jpg");
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
        public class BottomLeftDiagonalWatermarkHandler : AbstractPdfDocumentEventHandler
        {
            private string _ipAddress;
            public BottomLeftDiagonalWatermarkHandler(string ipAddress)
            {
                _ipAddress = ipAddress;
            }
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
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
