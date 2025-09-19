namespace DataTransferObject.Response
{
    public class DTODigitalSignPlusLog
    {
        public int Sno { get; set; }
        public bool IsLogWithSign { get; set; } = false;
        public string? FromDomain { get; set; }
        public string? FromProfile { get; set; }
        public string? FromRank { get; set; }
        public string FromArmyNo { get; set; }=string.Empty;
        public DateTime? FromDate { get; set; }
        public string? LevelMessage { get; set; }

        public string? DSProfile { get; set; }
        public string? DSArmyNo { get; set; }
    }
}
