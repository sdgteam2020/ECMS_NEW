namespace DataTransferObject.Response
{
    public class DTOCorpsResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int CorpsId { get; set; }
        public string CorpsName { get; set; }=string.Empty;
        public string ComdName { get; set; } = string.Empty;
        public int ComdId { get; set; }
    }
}
