using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOUpdateROValueRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RecordOfficeId is number.")]
        public byte RecordOfficeId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "TDMId is number.")]
        public int TDMId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "Old TDMId is number.")]
        public int OldTDMId { get; set; }
        
        [RegularExpression(@"^[\w\&\.\-\; ]*$", ErrorMessage = "Only Alphabets ,Numbers and some symbol (& . -) allowed.")]
        [MaxLength(150, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string? Message { get; set; }

        public int Updatedby { get; set; }
        
        public DateTime UpdatedOn { get; set; }
    }
}
