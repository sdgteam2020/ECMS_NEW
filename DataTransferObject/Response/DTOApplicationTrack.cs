using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOApplicationTrack
    {
        public DTOApplicationDetails dTOApplicationDetails { get; set; }
        public List<DTOTrackHistory> dTOTrackHistory { get; set; }
    }
    public class DTOApplicationDetails
    {
        public string? RankName { get; set; }
        public string? Name { get; set; }
        public string? ArmyNo { get; set; }
        public string? UnitName { get; set; }
        public string? PhotoImagePath { get; set; }
        public string? FromRank { get; set; }
        public string? FromName { get; set; }
        public string? FromArmyNo { get; set; }
        public string? DomainId { get; set; }
    }
    public class DTOTrackHistory
    {      
        public int FwdStatusId { get; set;}
        public int stepId { get; set;}
        public DateTime UpdatedOn { get; set;}
        public string? Name { get; set;}
        public int IsComplete { get; set;}
        public string? Remark { get; set; }
        public string? Remark2 { get; set; }
    }
}
