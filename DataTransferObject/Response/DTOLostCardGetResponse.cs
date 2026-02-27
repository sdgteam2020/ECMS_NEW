using System.Text.Json.Serialization;

namespace DataTransferObject.Response
{
    public class DTOLostCardGetResponse
    {
        public int TotalFilteredRecords { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public int RequestId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public int LostCardId { get; set; }
        public DateTime LostOn { get; set; }
        public string RemarksNameList { get; set; } = string.Empty;
        public string? Remark { get; set; }
       
        [JsonIgnore]
        public string? SupportDocName { get; set; }
        public bool IsFIRLogged { get; set; }
    }
}
