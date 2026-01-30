namespace DataTransferObject.Response
{
    public class DTOCheckUnitMappedInMapUnitResponse
    {
        public int? UnitId { get; set; }
        public bool IsVerify { get; set; }
        public int? UnitMapId { get; set; }
        public string Sus_no { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
    }
}
