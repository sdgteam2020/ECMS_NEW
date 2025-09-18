namespace DataTransferObject.Response
{
    public class DTOUserProfileResponse
    { 
        public int MapId { get; set; }
        public int UserId { get; set; }
        public int TDMId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string? Thumbprint { get; set; }
        public int ApptId { get; set; }
        public string AppointmentName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public bool IsRO { get; set; }
        public bool IsIO { get; set; }
        public bool IsCO { get; set; }
        public bool IsORO { get; set; }
        public bool IsToken { get; set; }
        public bool IsWithTokenApply { get; set; }
        public string SusNo { get; set; } = string.Empty;
        //public int RequestId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Rank { get; set;} = string.Empty;
        public int RankId { get; set; }

        public string DomainId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string MappedDate { get; set; } = string.Empty;
        public string MappedBy { get; set; } = string.Empty;
    }
}
