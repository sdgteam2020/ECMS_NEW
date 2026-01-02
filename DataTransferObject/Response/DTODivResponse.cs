namespace DataTransferObject.Response
{
    public class DTODivResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int DivId { get; set; }
        public string DivName { get; set; }=string.Empty;
        public string ComdName { get; set; } = string.Empty;
        public int ComdId { get; set; }
        public string CorpsName { get; set; } = string.Empty;
        public int CorpsId { get; set; }

        
    }
}
