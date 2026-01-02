namespace DataTransferObject.Response
{
    public class DTOArmedResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public bool FlagInf { get; set; }
        public string Inf { get; set; } = string.Empty;
        public int ArmedCatId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
