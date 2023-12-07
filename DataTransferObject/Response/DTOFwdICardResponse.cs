using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOFwdICardResponse
    {
        public int AspNetUsersId { get; set; }
        public int UserId { get; set; }
        public string DomainId { get; set; }
        public string ArmyNo { get; set; }
        public string Name { get; set; }
    }
}
