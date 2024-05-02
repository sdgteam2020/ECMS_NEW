using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAccountCountResponse
    {
        public int User { get; set; }
        public int ActiveUser { get; set; }
        public int InActiveUser { get; set; }
        public int MappedUser { get; set; }
        public int UnMappedUser { get; set; }
        public int VerifiedUser { get; set; }
        public int NotVerifiedUser { get; set; }
        public int IO { get; set; }
        public int CO { get; set; }
        public int RO { get; set; }
        public int ORO { get; set; }
    }
}
