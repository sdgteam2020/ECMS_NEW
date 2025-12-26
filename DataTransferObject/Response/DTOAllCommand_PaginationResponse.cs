namespace DataTransferObject.Response
{
    public class DTOAllCommand_PaginationResponse
    {
        public int TotalFilteredRecords { get; set; }
        public byte ComdId { get; set; }
        public string ComdName { get; set; } = string.Empty;
        public string ComdAbbreviation { get; set; } = string.Empty;
        public int Orderby { get; set; }
    }
}
