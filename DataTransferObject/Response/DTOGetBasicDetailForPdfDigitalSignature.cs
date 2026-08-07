using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetBasicDetailForPdfDigitalSignature
    {
        public DateTime? DateOfIssue { get; set; }
        public string? PlaceOfIssue { get; set; }
    }
}
