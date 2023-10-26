using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class State
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StateId { get; set; }

        [Required(ErrorMessage = "State Code is required.")]
        public string StateCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "State Name is required.")]
        public string StateName { get; set; } = string.Empty;

    }
}
