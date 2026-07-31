using Azure;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Globalization;
using System.Text.RegularExpressions;
using iTextImage = iText.Layout.Element.Image;
using Path = System.IO.Path;
using Div_iText = iText.Layout.Element.Div;


namespace BusinessLogicsLayer.Posting
{
    public class ApplCloseBL : GenericRepositoryDL<TrnApplClose>, IApplCloseBL
    {
        private readonly IApplCloseDB _iApplCloseDB;
         private static readonly Color PrimaryBlue = new DeviceRgb(13, 110, 253);
        private static readonly Color SuccessGreen = new DeviceRgb(25, 135, 84);
        private static readonly Color DangerRed = new DeviceRgb(220, 53, 69);
        private static readonly Color WarningYellow = new DeviceRgb(255, 193, 7);
        private static readonly Color InfoBlue = new DeviceRgb(13, 202, 240);
        private static readonly Color LightBorder = new DeviceRgb(222, 226, 230);
        private static readonly Color LightBackground = new DeviceRgb(248, 249, 250);
        private static readonly Color MutedText = new DeviceRgb(73, 80, 87);
        private readonly PdfFont _boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        public ApplCloseBL(ApplicationDbContext context, IApplCloseDB iApplCloseDB) : base(context)
        {
            _iApplCloseDB = iApplCloseDB;           
        }
        public async Task<DTOApplicationCloseResponse> RequestIdExists(DTOApplicationCloseRequest DTo)
        {
          return  await _iApplCloseDB.RequestIdExists(DTo);   
        }
        public async Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data, ICardHistoryResponseAll? cardHistoryResponses)
        {         
            return await _iApplCloseDB.ApplCloseWithUpdateStatus(Data, cardHistoryResponses);
        }
        public Task<DTOGenericResponse<byte[]>> GenerateClosedHistoryPDF(ICardHistoryResponseAll responseAll, string ipAddress)
        {
            var response = new DTOGenericResponse<byte[]>
            {
                Result = false,
                Message = string.Empty,
                Value = Array.Empty<byte>()
            };
            if (responseAll?.BasicDetail is null)
            {
                response.Message = "Applicant details are not available.";
                return Task.FromResult(response);
            }

            byte[] fileBytes = GeneratePdfDocument(responseAll, ipAddress);
            if (fileBytes.Length == 0)
            {
                response.Message = "PDF could not be generated.";
                return Task.FromResult(response);
            }

            response.Result = true;
            response.Message = "PDF generated successfully.";
            response.Value = fileBytes;

            return Task.FromResult(response);
        }
        private byte[] GeneratePdfDocument(ICardHistoryResponseAll responseAll, string ipAddress)
        {
            using var memoryStream = new MemoryStream();

            iTextImage? photoImage = TryCreateImage(
               responseAll.BasicDetail.PhotoInBase64,
               maxWidth: 95,
               maxHeight: 115);

            iTextImage? signatureImage = TryCreateImage(
                responseAll.BasicDetail.SignatureInBase64,
                maxWidth: 105,
                maxHeight: 45);

            DateTime generatedAt = DateTime.Now;

            var writerProperties = new WriterProperties()
                .SetCompressionLevel(CompressionConstants.BEST_COMPRESSION)
                .UseSmartMode();

            using (PdfWriter writer = new PdfWriter(memoryStream))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf))
            {
                pdf.SetDefaultPageSize(PageSize.A4);
                document.SetMargins(topMargin: 52, rightMargin: 34, bottomMargin: 52, leftMargin: 34);
                document.SetFontSize(10);

                string serviceNumber = ValueOrEmpty(responseAll.BasicDetail.ServiceNo);
                string documentTitle = string.IsNullOrWhiteSpace(serviceNumber)
                    ? "Closed I-Card History"
                    : $"Closed I-Card History - {serviceNumber}";

                pdf.GetDocumentInfo()
                    .SetTitle(documentTitle)
                    .SetSubject("Closed  I-Card application and movement history")
                    .SetCreator("I-Card Management System");

                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderFooterHandler(_boldFont));
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new DiagonalWatermarkHandler(ipAddress, generatedAt, _boldFont));

                // Personal details
                AddDocumentHeader_Personal(document);
                AddPersonalDetailsTable(document, responseAll.BasicDetail, photoImage, signatureImage);

                // Application forward history
                AddDocumentHeader_Fwd(document);
                AddFwdDetailsTable(document, responseAll);

                // Card movement history
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                AddDocumentHeader_CardMovement(document);
                AddCardMovementHistory(document, responseAll);
            }

            return memoryStream.ToArray();
        }
        private void AddDocumentHeader_Personal(Document document)
        {
            AddSectionHeader(document, "Personal Information", "Applicant details and identification record");
        }
        private void AddSectionHeader(Document document, string title, string subtitle)
        {
            var titleTable = new Table(1)
                .UseAllAvailableWidth()
                .SetMarginTop(3)
                .SetMarginBottom(5);

            titleTable.AddCell(
                new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(new DeviceRgb(33, 37, 41))
                    .SetPadding(9)
                    .Add(
                        new Paragraph(title)
                            .SetFont(_boldFont)
                            .SetFontSize(14)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetMargin(0)));

            document.Add(titleTable);

            document.Add(
                new Paragraph(subtitle)
                    .SetFont(_boldFont)
                    .SetFontSize(9.5f)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(SuccessGreen)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(5)
                    .SetMarginTop(0)
                    .SetMarginBottom(10));
        }
        private static iTextImage? TryCreateImage(string? base64Image, float maxWidth, float maxHeight)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
            {
                return null;
            }

            try
            {
                string base64Data = base64Image.Trim();
                int commaIndex = base64Data.IndexOf(',');

                if (commaIndex >= 0)
                {
                    base64Data = base64Data[(commaIndex + 1)..];
                }

                base64Data = Regex.Replace(base64Data, @"\s+", string.Empty);

                byte[] imageBytes = Convert.FromBase64String(base64Data);
                if (imageBytes.Length == 0)
                {
                    return null;
                }

                ImageData imageData = ImageDataFactory.Create(imageBytes);
                var image = new iTextImage(imageData);
                image.ScaleToFit(maxWidth, maxHeight);
                image.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                return image;
            }
            catch (Exception)
            {
                // Invalid/missing image data should not stop PDF generation.
                return null;
            }
        }
        private sealed class HeaderFooterHandler : AbstractPdfDocumentEventHandler
        {
            private readonly PdfFont _font;

            public HeaderFooterHandler(PdfFont font)
            {
                _font = font;
            }
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
            {
                var documentEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDocument = documentEvent.GetDocument();
                PdfPage page = documentEvent.GetPage();
                Rectangle pageSize = page.GetPageSize();

                float width = pageSize.GetWidth();
                float height = pageSize.GetHeight();
                int pageNumber = pdfDocument.GetPageNumber(page);

                var canvas = new PdfCanvas(
                    page.NewContentStreamAfter(),
                    page.GetResources(),
                    pdfDocument);

                canvas.SaveState();

                canvas.SetStrokeColor(LightBorder)
                    .SetLineWidth(0.5f)
                    .MoveTo(34, height - 42)
                    .LineTo(width - 34, height - 42)
                    .MoveTo(34, 42)
                    .LineTo(width - 34, 42)
                    .Stroke();

                DrawCenteredText(
                    canvas,
                    _font,
                    "CONFIDENTIAL",
                    10,
                    width,
                    height - 29,
                    DangerRed);

                DrawCenteredText(
                    canvas,
                    _font,
                    $"CONFIDENTIAL  |  Page {pageNumber}",
                    8,
                    width,
                    27,
                    MutedText);

                canvas.RestoreState();
                canvas.Release();
            }
            private static void DrawCenteredText(
                PdfCanvas canvas,
                PdfFont font,
                string text,
                float fontSize,
                float pageWidth,
                float y,
                Color color)
            {
                float textWidth = font.GetWidth(text, fontSize);
                float x = (pageWidth - textWidth) / 2;

                canvas.BeginText()
                    .SetFontAndSize(font, fontSize)
                    .SetFillColor(color)
                    .MoveText(x, y)
                    .ShowText(text)
                    .EndText();
            }
        }
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
        private sealed class DiagonalWatermarkHandler : AbstractPdfDocumentEventHandler
        {
            private readonly string _ipAddress;
            private readonly DateTime _generatedAt;
            private readonly PdfFont _font;

            public DiagonalWatermarkHandler(string ipAddress, DateTime generatedAt, PdfFont font)
            {
                _ipAddress = ValueOrEmpty(ipAddress);
                _generatedAt = generatedAt;
                _font = font;
            }

            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
            {
                var documentEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDocument = documentEvent.GetDocument();
                PdfPage page = documentEvent.GetPage();
                Rectangle pageSize = page.GetPageSize();

                string watermarkText = string.IsNullOrWhiteSpace(_ipAddress)
                    ? $"Generated {_generatedAt:dd-MM-yyyy HH:mm:ss}"
                    : $"{_ipAddress}  |  {_generatedAt:dd-MM-yyyy HH:mm:ss}";

                float centerX = pageSize.GetWidth() / 2;
                float centerY = pageSize.GetHeight() / 2;
                float fontSize = Math.Clamp(
                    pageSize.GetWidth() / Math.Max(watermarkText.Length * 0.55f, 1),
                    18,
                    30);

                const float radians = (float)(Math.PI / 4);
                float cos = (float)Math.Cos(radians);
                float sin = (float)Math.Sin(radians);

                var canvas = new PdfCanvas(
                    page.NewContentStreamAfter(),
                    page.GetResources(),
                    pdfDocument);

                canvas.SaveState();
                canvas.SetExtGState(new PdfExtGState().SetFillOpacity(0.08f));
                canvas.SetFillColor(DangerRed);

                canvas.BeginText()
                    .SetFontAndSize(_font, fontSize)
                    .SetTextMatrix(cos, sin, -sin, cos, centerX, centerY)
                    .MoveText(-_font.GetWidth(watermarkText, fontSize) / 2, 0)
                    .ShowText(watermarkText)
                    .EndText();

                canvas.RestoreState();
                canvas.Release();
            }
        }
        private void AddPersonalDetailsTable(Document document, DTOBasicDetailForCompleteClosed detail, iTextImage? photoImage, iTextImage? signatureImage)
        {
            var outerTable = new Table(UnitValue.CreatePercentArray([76, 24]))
                .UseAllAvailableWidth()
                .SetMarginBottom(14);

            var detailsTable = new Table(UnitValue.CreatePercentArray([31, 69]))
                .UseAllAvailableWidth();

            AddDetailRow(detailsTable, "Name", JoinNonEmpty(detail.RankName, detail.FName, detail.LName));
            AddDetailRow(detailsTable, "Rank", detail.RankName);
            AddDetailRow(detailsTable, "Arm / Service", detail.ArmedName);
            AddDetailRow(detailsTable, "Army No", detail.ServiceNo);
            AddDetailRow(detailsTable, "Identification Mark", detail.IdenMark1);
            AddDetailRow(detailsTable, "Date of Birth", FormatDateValue(detail.DOB));
            AddDetailRow(detailsTable, "Height (cm)", detail.Height.ToString(CultureInfo.InvariantCulture));
            AddDetailRow(detailsTable, "Aadhaar No", MaskAadhaar(detail.AadhaarNo));
            AddDetailRow(detailsTable, "Blood Group", detail.BloodGroup);
            AddDetailRow(detailsTable, "Place of Issue", detail.PlaceOfIssue);
            AddDetailRow(detailsTable, "Date of Issue", FormatNullableDate(detail.DateOfIssue));
            AddDetailRow(detailsTable, "Issuing Authority", detail.IssuingAuthorityName);
            AddDetailRow(
                detailsTable,
                "Commissioning / Enrollment",
                FormatDateValue(detail.DateOfCommissioning));
            AddDetailRow(detailsTable, "Permanent Address", BuildAddress(detail));

            outerTable.AddCell(
                new Cell()
                    .SetBorder(new SolidBorder(LightBorder, 0.8f))
                    .SetPadding(0)
                    .SetVerticalAlignment(VerticalAlignment.TOP)
                    .Add(detailsTable));

            var mediaTable = new Table(1)
                .UseAllAvailableWidth();

            AddImagePanel(mediaTable, "Photograph", photoImage, 125);
            AddImagePanel(mediaTable, "Signature", signatureImage, 65);

            outerTable.AddCell(
                new Cell()
                    .SetBorder(new SolidBorder(LightBorder, 0.8f))
                    .SetPadding(6)
                    .SetVerticalAlignment(VerticalAlignment.TOP)
                    .Add(mediaTable));

            document.Add(outerTable);
        }
        private void AddDetailRow(Table table, string label, string? value)
        {
            table.AddCell(
                new Cell()
                    .SetBackgroundColor(LightBackground)
                    .SetBorder(new SolidBorder(LightBorder, 0.5f))
                    .SetPadding(5)
                    .Add(
                        new Paragraph(label)
                            .SetFont(_boldFont)
                            .SetFontSize(8.5f)
                            .SetMargin(0)));

            table.AddCell(
                new Cell()
                    .SetBorder(new SolidBorder(LightBorder, 0.5f))
                    .SetPadding(5)
                    .Add(
                        new Paragraph(DisplayValue(value))
                            .SetFontSize(8.5f)
                            .SetMargin(0)));
        }

        private void AddImagePanel(Table table, string label, iTextImage? image, float minimumHeight)
        {
            table.AddCell(
                new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(LightBackground)
                    .SetPadding(4)
                    .Add(
                        new Paragraph(label)
                            .SetFont(_boldFont)
                            .SetFontSize(8.5f)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMargin(0)));

            var imageCell = new Cell()
                .SetBorder(Border.NO_BORDER)
                .SetMinHeight(minimumHeight)
                .SetPadding(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

            if (image is null)
            {
                imageCell.Add(
                    new Paragraph("Not available")
                        .SetFontColor(MutedText)
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
            }
            else
            {
                imageCell.Add(image);
            }

            table.AddCell(imageCell);
        }

        private static string BuildAddress(DTOBasicDetailForCompleteClosed detail)
        {
            var parts = new[]
            {
                PrefixValue("Village", detail.Village),
                PrefixValue("Post Office", detail.PO),
                PrefixValue("Tehsil", detail.Tehsil),
                PrefixValue("District", detail.District),
                PrefixValue("State", detail.State),
                PrefixValue("PIN", detail.PinCode)
            };

            return string.Join(", ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string PrefixValue(string label, object? value)
        {
            string text = Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim() ?? string.Empty;
            return string.IsNullOrWhiteSpace(text) ? string.Empty : $"{label}: {text}";
        }

        private static string FormatDateValue(DateTime value)
        {
            return value == DateTime.MinValue
                ? string.Empty
                : value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }

        private static string FormatNullableDate(DateTime? value)
        {
            return !value.HasValue || value.Value == DateTime.MinValue
                ? string.Empty
                : value.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }

        private static string MaskAadhaar(string? aadhaarNumber)
        {
            if (string.IsNullOrWhiteSpace(aadhaarNumber))
            {
                return string.Empty;
            }

            string digits = Regex.Replace(aadhaarNumber, @"\D", string.Empty);
            if (digits.Length <= 4)
            {
                return digits;
            }

            return new string('X', digits.Length - 4) + digits[^4..];
        }

        private static string JoinNonEmpty(params string?[] values)
        {
            return string.Join(
                " ",
                values
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x!.Trim()));
        }
        private void AddFwdDetailsTable(Document document, ICardHistoryResponseAll request)
        {
            IReadOnlyList<ICardHistoryResponse> history = request.ICardHistory ?? [];
            IReadOnlyList<ICardHistoryPostingOutResponse> postingOut = request.PostingOut ?? [];
            IReadOnlyList<ICardHistoryFaultyCardResponse> faultyCards = request.FaultyCard ?? [];

            if (history.Count == 0)
            {
                AddEmptyState(document, "No I-Card application history is available.");
                return;
            }
            /*
             * TrnFwdId is int?.
             *
             * int.MinValue is used internally for null TrnFwdId values.
             * It prevents nullable-key conversion errors and avoids exceptions.
             */
            const int NullForwardId = int.MinValue;

            ILookup<int, ICardHistoryPostingOutResponse> postingOutByForwardId = postingOut.ToLookup(item => item.TrnFwdId ?? NullForwardId);

            Dictionary<int, ICardHistoryFaultyCardResponse> faultyCardByForwardId = faultyCards.GroupBy(item => item.TrnFwdId ?? NullForwardId)
                                                                                                .ToDictionary(
                                                                                                    group => group.Key,
                                                                                                    group => group.First());
            for (var index = 0; index < history.Count; index++)
            {
                ICardHistoryResponse item = history[index];
                bool isFirstItem = index == 0;
                bool isLastItem = index == history.Count - 1;

                int currentForwardId = item.TrnFwdId ?? NullForwardId;

                (string statusText, Color statusColor) = GetStatus(item);

                /*
                 * TrnFwdId = 0 posting-out records are displayed
                 * only before the first forward movement.
                 */
                if (isFirstItem)
                {
                    IEnumerable<ICardHistoryPostingOutResponse> postingOutBeforeFirstForward = postingOutByForwardId[0];

                    AddInitialSubmission(document, item, postingOutBeforeFirstForward);
                }
                /*
                 * Posting-out records after a movement:
                 * - TrnFwdId must not be null
                 * - TrnFwdId must not be 0
                 * - TrnFwdId must match the current movement
                 */
                IEnumerable<ICardHistoryPostingOutResponse> postingOutAfterCurrentForward = currentForwardId != NullForwardId && currentForwardId != 0
                                                                                            ? postingOutByForwardId[currentForwardId]
                                                                                            : Enumerable.Empty<ICardHistoryPostingOutResponse>();

                faultyCardByForwardId.TryGetValue(currentForwardId, out ICardHistoryFaultyCardResponse? faultyCard);

                AddMovement(
                    document,
                    item,
                    statusText,
                    statusColor,
                    postingOutAfterCurrentForward,
                    faultyCard,
                    isLastItem ? request.CloseCard : null);
            }
        }
        private void AddDocumentHeader_Fwd(Document document)
        {
            AddSectionHeader(
                document,
                "I-Card Application History",
                "Step-by-step application forwarding history");
        }

        private void AddInitialSubmission(Document document, ICardHistoryResponse item, IEnumerable<ICardHistoryPostingOutResponse> postingOutBeforeForward)
        {
            var content = CreateContentContainer();

            content.Add(
                new Paragraph()
                    .SetMargin(0)
                    .Add(
                        new Text("I-Card Submitted By - ")
                            .SetFont(_boldFont))
                    .Add(
                        FormatPerson(
                            item.FromDomain,
                            item.FromRank,
                            item.FromProfile)));

            foreach (ICardHistoryPostingOutResponse posting in postingOutBeforeForward)
            {
                AddPostingOut(content, posting);
            }

            AddTimelineRow(document, FormatDate(item.UpdatedOn), SuccessGreen, content);
        }

        private void AddMovement(Document document, ICardHistoryResponse item, string statusText, Color statusColor, IEnumerable<ICardHistoryPostingOutResponse> postingOutAfterForward
                                    , ICardHistoryFaultyCardResponse? faultyCard, ICardApplCloseCardResponse? closeCard)
        {
            var content = CreateContentContainer();

            content.Add(
                new Paragraph(
                        FormatPerson(
                            item.FromDomain,
                            item.FromRank,
                            item.FromProfile))
                    .SetFont(_boldFont)
                    .SetMarginTop(0)
                    .SetMarginBottom(4));

            if (!string.IsNullOrWhiteSpace(statusText))
            {
                Color foregroundColor =
                    statusText.StartsWith(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase)
                        ? ColorConstants.BLACK
                        : ColorConstants.WHITE;

                content.Add(
                    CreateBadge(
                        statusText,
                        statusColor,
                        foregroundColor));
            }

            content.Add(
                new Paragraph("Remark")
                    .SetFont(_boldFont)
                    .SetMarginTop(6)
                    .SetMarginBottom(1));

            content.Add(
                new Paragraph(ValueOrEmpty(item.Remark))
                    .SetMarginTop(0)
                    .SetMarginBottom(2));

            AddHashSeparatedRemarks(
                content,
                item.Remarks2,
                ColorConstants.BLACK);

            AddDownArrow(content);

            if (item.IsComplete == 0)
            {
                content.Add(
                    CreateBadge(
                        "Pending from",
                        WarningYellow,
                        ColorConstants.BLACK));
            }

            content.Add(
                new Paragraph(
                        FormatPerson(
                            item.ToDomain,
                            item.ToRank,
                            item.ToProfile))
                    .SetFont(_boldFont)
                    .SetMarginTop(4)
                    .SetMarginBottom(2));

            foreach (ICardHistoryPostingOutResponse posting
                     in postingOutAfterForward)
            {
                AddPostingOut(content, posting);
            }

            if (faultyCard is not null)
            {
                AddFaultyCard(content, faultyCard);
            }

            AddTimelineRow(
                document,
                FormatDate(item.UpdatedOn),
                statusColor,
                content);
        }

        private Div_iText CreateContentContainer()
        {
            return new Div_iText()
                .SetBackgroundColor(LightBackground)
                .SetBorder(new SolidBorder(LightBorder, 0.8f))
                .SetPadding(8);
        }

        private void AddTimelineRow(Document document, string dateText, Color dateColor, Div_iText content)
        {
            var timeline = new Table(UnitValue.CreatePercentArray([23, 2, 75]))
                .UseAllAvailableWidth()
                .SetMarginBottom(9);

            timeline.AddCell(
                new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetPaddingRight(6)
                    .SetVerticalAlignment(VerticalAlignment.TOP)
                    .Add(
                        new Paragraph(DisplayValue(dateText))
                            .SetFont(_boldFont)
                            .SetFontSize(7.5f)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetBackgroundColor(dateColor)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(5)
                            .SetMargin(0)));

            timeline.AddCell(
                new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(PrimaryBlue)
                    .SetPadding(0));

            timeline.AddCell(
                new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetPaddingLeft(7)
                    .SetPaddingTop(0)
                    .SetPaddingBottom(0)
                    .SetVerticalAlignment(VerticalAlignment.TOP)
                    .Add(content));

            document.Add(timeline);
        }

        private void AddPostingOut(Div_iText content, ICardHistoryPostingOutResponse posting)
        {
            AddDownArrow(content);

            string reason = string.IsNullOrWhiteSpace(posting.Reason)
                ? "Posting Out"
                : posting.Reason.Trim();

            content.Add(
                new Paragraph(reason)
                    .SetFont(_boldFont)
                    .SetFontColor(DangerRed)
                    .SetMarginTop(2)
                    .SetMarginBottom(3));

            AddLabelAndValue(content, "From Unit", posting.FromUnit, InfoBlue);
            AddLabelAndValue(content, "To Unit", posting.UnitName, InfoBlue);
        }

        private void AddFaultyCard(Div_iText content, ICardHistoryFaultyCardResponse faultyCard)
        {
            AddDownArrow(content);

            content.Add(
                new Paragraph("Faulty Card")
                    .SetFont(_boldFont)
                    .SetFontColor(DangerRed)
                    .SetUnderline()
                    .SetMarginTop(2)
                    .SetMarginBottom(2));

            content.Add(
                new Paragraph("Reason")
                    .SetFont(_boldFont)
                    .SetFontColor(DangerRed)
                    .SetMarginTop(0)
                    .SetMarginBottom(1));

            AddHashSeparatedRemarks(content, faultyCard.RemarksNameList, DangerRed);

            content.Add(
                new Paragraph()
                    .SetMarginTop(3)
                    .SetMarginBottom(0)
                    .Add(new Text("By :- ").SetFont(_boldFont))
                    .Add(ValueOrEmpty(faultyCard.FaultyStage)));
        }

        private void AddLabelAndValue(Div_iText content, string label, string? value, Color labelColor)
        {
            content.Add(
                new Paragraph(label)
                    .SetFont(_boldFont)
                    .SetFontColor(labelColor)
                    .SetMarginTop(2)
                    .SetMarginBottom(0));

            content.Add(
                new Paragraph(ValueOrEmpty(value))
                    .SetMarginTop(0)
                    .SetMarginBottom(2));
        }

        private void AddHashSeparatedRemarks(Div_iText content, string? value, Color textColor)
        {
            foreach (string remark in SplitHashValues(value))
            {
                content.Add(
                    new Paragraph($"- {remark}")
                        .SetFontColor(textColor)
                        .SetMarginLeft(10)
                        .SetMarginTop(0)
                        .SetMarginBottom(1));
            }
        }

        private void AddDownArrow(Div_iText content)
        {
            content.Add(
                new Paragraph("|")
                    .SetFont(_boldFont)
                    .SetFontColor(PrimaryBlue)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(3)
                    .SetMarginBottom(-3));

            content.Add(
                new Paragraph("v")
                    .SetFont(_boldFont)
                    .SetFontColor(PrimaryBlue)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(0)
                    .SetMarginBottom(2));
        }

        private Paragraph CreateBadge(string text, Color background, Color foreground)
        {
            return new Paragraph(text)
                .SetFont(_boldFont)
                .SetFontSize(8)
                .SetFontColor(foreground)
                .SetBackgroundColor(background)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(4)
                .SetMarginTop(2)
                .SetMarginBottom(3);
        }

        private static (string StatusText, Color StatusColor) GetStatus(ICardHistoryResponse item)
        {
            string status = item.Status?.Trim() ?? string.Empty;

            if (item.IsComplete == 0 &&
                status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                return ("Pending And Sent To", SuccessGreen);
            }

            if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                return ("Approved And Sent To", SuccessGreen);
            }

            if (status.Equals("Reject", StringComparison.OrdinalIgnoreCase))
            {
                return ("Reject And Sent To", DangerRed);
            }

            if (status.Equals("Internal Forward", StringComparison.OrdinalIgnoreCase))
            {
                return ("Internal Forward And Sent To", SuccessGreen);
            }

            return (string.Empty, SuccessGreen);
        }

        private static IEnumerable<string> SplitHashValues(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return [];
            }

            return value
                .Split('#', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        private static string FormatPerson(string? domain, string? rank, string? profile)
        {
            string domainText = ValueOrEmpty(domain);
            string nameText = string.Join(
                " ",
                new[] { rank, profile }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x!.Trim()));

            if (string.IsNullOrWhiteSpace(nameText))
            {
                return domainText;
            }

            return string.IsNullOrWhiteSpace(domainText)
                ? $"({nameText})"
                : $"{domainText} ({nameText})";
        }

        private static string FormatDate(string? dateValue)
        {
            if (string.IsNullOrWhiteSpace(dateValue))
                return string.Empty;

            string[] supportedFormats =
            {
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-dd HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "dd-MM-yyyy HH:mm:ss"
            };

            if (DateTime.TryParseExact(
                    dateValue,
                    supportedFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out DateTime parsedDate))
            {
                return parsedDate.ToString(
                    "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }

            // Handles other valid date formats returned by JSON.
            if (DateTime.TryParse(
                    dateValue,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out parsedDate))
            {
                return parsedDate.ToString(
                    "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }

            // Return the original value when parsing fails.
            return dateValue;
        }

        private static string ValueOrEmpty(string? value)
        {
            return value?.Trim() ?? string.Empty;
        }
        private static string DisplayValue(string? value)
        {
            string text = ValueOrEmpty(value);
            return string.IsNullOrWhiteSpace(text) ? "-" : text;
        }
        private void AddEmptyState(Document document, string message)
        {
            Div_iText container = CreateContentContainer();
            container.SetMarginTop(4);
            container.SetMarginBottom(10);

            container.Add(
                new Paragraph(message)
                    .SetFont(_boldFont)
                    .SetFontColor(MutedText)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMargin(0));

            document.Add(container);
        }
        private void AddDocumentHeader_CardMovement(Document document)
        {
            AddSectionHeader(
                document,
                "I-Card Movement History",
                "Step-by-step card status and movement history");
        }
        private void AddCardMovementHistory(Document document, ICardHistoryResponseAll history)
        {
            if (history?.CardMovement == null)
            {
                AddNoCardMovementMessage(document);
                return;
            }

            var cardMovements = history.CardMovement.ToList();

            if (cardMovements.Count == 0)
            {
                AddNoCardMovementMessage(document);
                return;
            }

            for (int index = 0; index < cardMovements.Count; index++)
            {
                var item = cardMovements[index];

                bool isLastItem = index == cardMovements.Count - 1;

                string stepName = string.IsNullOrWhiteSpace(item.StepName) ? string.Empty : item.StepName.Trim();

                bool isLostOrHotlist =
                    stepName.Equals(
                        "I-Card Lost",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    stepName.Equals(
                        "I-Card Hotlist",
                        StringComparison.OrdinalIgnoreCase);

                Color badgeColor = isLostOrHotlist ? DangerRed : SuccessGreen;

                Div_iText content = CreateContentContainer();

                content.Add(
                    CreateBadge(
                        stepName,
                        badgeColor,
                        ColorConstants.WHITE));

                content.Add(
                    new Paragraph()
                        .SetMarginTop(5)
                        .SetMarginBottom(4)
                        .Add(new Text("Reported by: ").SetFont(_boldFont))
                        .Add(DisplayValue(item.ReportedBy)));

                content.Add(
                    new Paragraph("Remark")
                        .SetFont(_boldFont)
                        .SetMarginTop(4)
                        .SetMarginBottom(1));

                content.Add(
                    new Paragraph(DisplayValue(item.Remark))
                        .SetMarginTop(0)
                        .SetMarginBottom(2));

                if (!isLastItem)
                {
                    AddDownArrow(content);
                }

                AddTimelineRow(
                    document,
                    FormatDate(Convert.ToString(item.ReportedOn, CultureInfo.InvariantCulture)),
                    badgeColor,
                    content);
            }
        }
        private void AddNoCardMovementMessage(Document document)
        {
            AddEmptyState(document, "No I-Card movement history is available.");
        }

    }
}
