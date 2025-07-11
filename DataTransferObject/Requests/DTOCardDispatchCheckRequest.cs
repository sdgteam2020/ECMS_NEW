using AutoMapper.Configuration.Annotations;

namespace DataTransferObject.Requests
{
    public class DTOCardDispatchCheckRequest
    {
        public string ChipNo { get; set; }=string.Empty;
        
        [Ignore]
        public bool IsValid { get; set; } = true;
        public string Status { get; set; } = "Valid";
        public string Remarks { get; set; } = string.Empty;
    }
}
