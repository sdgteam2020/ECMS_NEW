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
        public string ServiceNo { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string Rank { get; set; }
        public string Reason { get; set; }
        public string Authority { get; set; }
        public string SOSDate { get; set; }
        public string UpdatedOn { get; set; }
        public string FromDomainId { get; set; }
        public string FromUnitName { get; set; }
        public string FromArmyNO { get; set; }
        public string FromName { get; set; }
        public string FromRankName { get; set; }
        public string ToDomainId { get; set; }
        public string ToUnitName { get; set; }
        public string ToArmyNO { get; set; }
    }
}
