using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOBeforeProceedToDispatchCheckRequest
    {
        public required int[] RequestIds { get; set; }
        
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Only Alphabets,Numbers,and underscores are allowed.")]
        public string SearchField { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z0-9_ ]*$", ErrorMessage = "Only Alphabets,Numbers,and underscores are allowed.")]
        public string SearchText { get; set; }=string.Empty;
    }
}
