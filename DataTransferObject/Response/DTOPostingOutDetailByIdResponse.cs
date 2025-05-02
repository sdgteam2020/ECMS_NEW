using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOPostingOutDetailByIdResponse
    {
        public string Reason { get; set; }
        public DateTime SOSDate { get; set; }
        public string Authority { get; set; }
        public string ToUnitName { get; set; }
        public string TOArmyNO { get; set; }
        public string ToRankName { get; set; }
        public string FromName { get; set; }
        public string TODomainId { get; set; }
        public string ToApptName { get; set; }
    }
}
