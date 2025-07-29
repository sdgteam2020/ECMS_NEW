using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAllOROResponse
    {
        public short OROMappingId { get; set; }
        public byte RecordOfficeId { get; set; }
        public string Name { get; set; }=string.Empty;
    }
}
