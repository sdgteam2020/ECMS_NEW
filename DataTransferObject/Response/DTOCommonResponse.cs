using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCommonResponse
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CurrentTime { get; set; }
    }
}
