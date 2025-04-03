using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOFaultyCardSaveResponse
    {
        public bool Result { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime CurrentTime { get; set; }
    }
}
