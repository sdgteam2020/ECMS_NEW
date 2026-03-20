using DataTransferObject.Response;

namespace DataTransferObject.Requests
{
    public class DtoSession: DTORsaKeyResponse
    {
        public string ICNO { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int TrnDomainMappingId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public Boolean IsToken { get; set; }
        // DoaminId Stored in session for IAM
        public string DoaminId { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}
