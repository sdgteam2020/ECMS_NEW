namespace DataTransferObject.Response
{
    public class DTOPostingInResponse
    {
        public int RequestId { get; set; }
        public int BasicDetailId { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? ServiceNo { get; set; }
        public string? RankName { get; set; }
        public byte? ApplyForId { get; set; }
        public string? ApplyFor { get; set; }
        public byte? StatusId { get; set; }
        public string PhotoImagePath { get; set; } = string.Empty;
        public string? UnitName { get; set; }
        public string? Suffix { get; set; }
        public string? Sus_no { get; set; }
        public string? Users_DomainId { get; set; }
        public string? Users_ArmyNo { get; set; }
        public string? Users_Name { get; set; }
        public string? Users_RankName { get; set; }
        public string? Users_AppointmentName { get; set; }
        public int FromAspNetUsersId { get; set; }
        public int FromUnitID { get; set; }
        public int FromUserID { get; set; }
        public int? MaxTrnFwdId { get; set; }
    }
}
