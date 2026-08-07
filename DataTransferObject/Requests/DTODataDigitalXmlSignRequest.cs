using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODataDigitalXmlSignRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public int RequestId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public int StepId { get; set; }
    }
}
