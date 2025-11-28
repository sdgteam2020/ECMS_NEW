namespace DataTransferObject.Response
{
    public class DTOGetTokenArmyNoResponse
    {
        public string ICNO { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}
