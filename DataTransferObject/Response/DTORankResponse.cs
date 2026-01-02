namespace DataTransferObject.Response
{
    public class DTORankResponse
    {
        public int TotalFilteredRecords { get; set; }
        public short RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public short Orderby { get; set; }
        public byte ApplyForId { get; set; }
    }
}
