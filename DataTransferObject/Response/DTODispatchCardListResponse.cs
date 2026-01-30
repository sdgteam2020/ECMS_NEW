
namespace DataTransferObject.Response
{
    public class DTODispatchCardListResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int DispatchCardId { get; set; }
        public byte Step { get; set; } // 1 For AFSAC AND 2 FOR Regiment / Record
        public byte ApplyForId { get; set; }
        public string ApplyFor { get; set; }=string.Empty;
        public string? RegimentalName { get; set; }
        public string? RecordOfficeName { get; set; }
        public DateTime OutDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime DispatchDate { get; set; }
        public string DispatchMode { get; set; } = string.Empty;
        public string RefOfDispatch { get; set; } = string.Empty;
        public string NameOfCourierIncharge { get; set; } = string.Empty;
        public string UploadFilePath { get; set; } = string.Empty;
        public string? FromRemark { get; set; }
        public string? ToRemark { get; set; }
        public string FromUnit { get; set; } = string.Empty;
        public string? FromSUSNo { get; set; }
        public string? FromSuffix { get; set; }
        public string ToUnit { get; set; } = string.Empty;
        public string? ToSUSNo { get; set; }
        public string? ToSuffix { get; set; }
        public string FromRankName { get; set; } = string.Empty;
        public string ToRankName { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public string FromServiceNo { get; set; } = string.Empty;
        public string ToServiceNo { get; set; } = string.Empty;
        public string FromDID { get; set; } = string.Empty;
        public string ToDID { get; set; } = string.Empty;
        public bool IsComplete { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime UpdatedOn { get; set; }
    }
}
