using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOActionOnRequest
    {
        //For Step Counter Update fields
        public int RequestId { get; set; }
        public byte StepId { get; set; }
        public byte ApplyForId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        //For Forwarding/Reject Action fields
        public int TrnFwdId { get; set; }
        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public int FromAspNetUsersId { get; set; }
        public int ToAspNetUsersId { get; set; }
        public int UnitId { get; set; }
        
        [StringLength(100)]
        public string? Remark { get; set; } = string.Empty;

        public byte FwdStatusId { get; set; }
        public byte TypeId { get; set; }
        public bool IsComplete { get; set; } = false;
        public string? RemarksIds { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
