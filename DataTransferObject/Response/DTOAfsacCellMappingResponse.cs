namespace DataTransferObject.Response
{
    public class DTOAfsacCellMappingResponse
    {
        public int TotalFilteredRecords { get; set; }
        public short AfsacCellMappingId { get; set; }
        public int? TDMId { get; set; }
        public int? UnitId { get; set; }
        public string? DomainId { get; set; }
        public string? ArmyNo { get; set; }
        public string? RankAbbreviation { get; set; }
        public string? Name { get; set; }
        public string? Sus_no { get; set; }
        public string? Suffix { get; set; }
        public string? UnitName { get; set; }
    }
}
