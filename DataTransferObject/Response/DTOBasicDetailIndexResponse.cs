using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Response
{
    public class DTOBasicDetailIndexResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int BasicDetailId { get; set; }
        public int RegistrationApplyFor { get; set; }
        public string? EncryptedId { get; set; }
        public string? EncryptedRequestId { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
        public int IsTrnFwdId { get; set; }
        public int StepCounter { get; set; }
        public int StepId { get; set; }
        public string? ICardType { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public byte ApplyForId { get; set; }
        public int RequestId { get; set; }
        public byte IsFwdStatusId { get; set; }
        public int? ApplId { get; set; }
        public string? RankName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string? RegimentalName { get; set; }
        public string? Remark { get; set; }
        public int IsPosting { get; set; }
        public bool IsLock { get; set; }

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public string? FName_1 { get; set; }

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public string? LName_1 { get; set; }

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public string? FName_2 { get; set; }

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public string? LName_2 { get; set; }
    }
}
