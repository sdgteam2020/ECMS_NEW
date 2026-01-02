namespace DataTransferObject.Response
{
    public class DTOBdeResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int BdeId { get; set; }
        public string BdeName { get; set; }=string.Empty;

        public int ComdId { get; set; }
        public string ComdName { get; set; } = string.Empty;
        public int CorpsId { get; set; }
        public string CorpsName { get; set; } = string.Empty;

        public int DivId { get; set; }
        public string DivName { get; set; } = string.Empty;



    }
}
