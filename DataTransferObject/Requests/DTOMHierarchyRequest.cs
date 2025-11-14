namespace DataTransferObject.Requests
{
    public class DTOMHierarchyRequest
    {
        public int? TableId { get; set; }
        public int? UnitType { get; set; }
        public byte? ComdId { get; set; }
        public byte? CorpsId { get; set; }
        public byte? DivId { get; set; }
        public byte? BdeId { get; set; }
        public byte? FmnBranchID { get; set; }
        public byte? PsoId { get; set; }
        public byte? SubDteId { get; set; }
        public int? UnitMapId { get; set; }
    }
}
