namespace DataTransferObject.Response
{
    public class DTORegimentalResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int RegId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public int ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int? UnitId { get; set; }
        public string? Sus_no { get; set; }
        public string? Suffix { get; set; }
        public string? UnitName { get; set; }
        public string? UnitAbbreviation { get; set; }
    }
}
