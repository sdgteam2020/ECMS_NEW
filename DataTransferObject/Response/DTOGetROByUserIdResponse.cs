using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetROByUserIdResponse
    {
        public byte RecordOfficeId { get; set; }
        public int UnitId { get; set; }
        public bool IsRO { get; set; }
        public bool IsORO { get; set; }
        public int TDMId { get; set; }
    }
}
