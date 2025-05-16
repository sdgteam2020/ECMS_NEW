using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODispatchDetailsSaveRequest
    {
        public string encId { get; set; }

        [Required]
        [Display(Name = "Dispatched Date")]
        public DateTime? DispatchedOn { get; set; }

        [Required]
        [Display(Name = "Ref No Regd SDS")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Reference number must contain only letters and numbers.")]
        public string? RefNo { get; set; }
        public DateTime? DispatchUpdatedOn { get; set; }
        public int? DispatchUpdatedBy { get; set; }
    }
}
