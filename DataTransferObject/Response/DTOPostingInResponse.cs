using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOPostingInResponse
    {
        public int RequestId { get; set; }
        public string? Name { get; set; }
        public string? ServiceNo { get; set; }
        public string? RankName { get; set; }
        public string? ApplyFor { get; set; }
        public string? TrackingId { get; set; }
        public string? Status { get; set; }
        public string? PhotoImagePath { get; set; }
        public string? UnitName { get; set; }
        public string? Suffix { get; set; }
        public string? Sus_no { get; set; }
        public string? Users_DomainId { get; set; }
        public string? Users_ArmyNo { get; set; }
        public string? Users_Name { get; set; }
        public string? Users_RankName { get; set; }
        public string? Users_AppointmentName { get; set; }
    }
}
