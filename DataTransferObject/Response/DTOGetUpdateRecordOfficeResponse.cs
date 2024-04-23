using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetUpdateRecordOfficeResponse
    {
        public byte RecordOfficeId { get; set; }
        public string RecordOfficeName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string ArmedName { get; set; } = string.Empty;
        public string? Message { get; set; }

    }
}
