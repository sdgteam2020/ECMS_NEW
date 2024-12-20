using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAccountCountResponse
    {
        public int User { get; set; } = 0;
        public int ActiveUser { get; set; } = 0;
        public int InActiveUser { get; set; } = 0;
        public int MappedUser { get; set; } = 0;
        public int UnMappedUser { get; set; } = 0;
        public int VerifiedUser { get; set; } = 0;
        public int NotVerifiedUser { get; set; } = 0;
        public int IO { get; set; } = 0;
        public int CO { get; set; } = 0;
        public int RO { get; set; } = 0;
        public int ORO { get; set; } = 0;
    }
}
