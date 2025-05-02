using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOPostingOutDetilsResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string Rank { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public DateTime SOSDate { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string FromDomainId { get; set; } = string.Empty;
        public string FromUnitName { get; set; } = string.Empty;
        public string FromArmyNO { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string FromRankName { get; set; } = string.Empty;
        public string ToDomainId { get; set; } = string.Empty;
        public string ToUnitName { get; set; } = string.Empty;
        public string ToArmyNO { get; set; } = string.Empty;
        public DateTime DispatchedOn { get; set; }
        public string RefNo { get; set; } = string.Empty;
        public DateTime DispatchUpdatedOn { get; set; }
        public string DispatchUpdatedBy { get; set; } = string.Empty;
        public bool CanAddDispatchDetail { get; set; }
    }
}
