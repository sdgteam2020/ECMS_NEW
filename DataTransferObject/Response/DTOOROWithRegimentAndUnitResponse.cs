namespace DataTransferObject.Response
{
    public class DTOOROWithRegimentAndUnitResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SUSNo { get; set; }=  string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
    }
}
