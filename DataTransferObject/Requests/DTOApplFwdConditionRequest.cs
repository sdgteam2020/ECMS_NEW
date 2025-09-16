namespace DataTransferObject.Requests
{
    public class DTOApplFwdConditionRequest
    {
        public required MPRSO MPRSO { get; set; }
        public required MP6F MP6F { get; set; }
        public required MP6A MP6A { get; set; }
    }
    public class MPRSO
    {
        public short RecordOfficeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> ArmedAbbreviation { get; set; } = new();
    }
    public class MP6F
    {
        public short RecordOfficeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArmyNoPrefix { get; set; } = string.Empty;
    }
    public class MP6A
    {
        public short RecordOfficeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public short RankOrderby { get; set; }
    }
}
