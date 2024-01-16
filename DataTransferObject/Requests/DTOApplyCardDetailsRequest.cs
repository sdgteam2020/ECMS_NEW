using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOApplyCardDetailsRequest
    {
        public int ApplyForId { get; set; }
        public int RegistrationId { get; set; }
        public int TypeId { get; set; }
        public int UserId { get; set; }
    }
}
