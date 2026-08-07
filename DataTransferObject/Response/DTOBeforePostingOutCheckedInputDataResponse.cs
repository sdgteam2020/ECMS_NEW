using System.Text.Json.Serialization;

namespace DataTransferObject.Response
{
    public class DTOBeforePostingOutCheckedInputDataResponse
    {
        [JsonIgnore]
        public int BasicDetailId { get; set; }
        
        [JsonIgnore]
        public int UnitId { get; set; }

        [JsonIgnore]
        public byte StatusId { get; set; }

        [JsonIgnore]
        public int? MaxTrnFwdId { get; set; }

        [JsonIgnore]
        public int? ToAspNetUsersId { get; set; }

        [JsonIgnore]
        public int? ToUserID { get; set; }

        [JsonIgnore]
        public int TrnDomainMappingId { get; set; }

        [JsonIgnore]
        public string PlaceOfIssue { get; set; } = string.Empty;
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
