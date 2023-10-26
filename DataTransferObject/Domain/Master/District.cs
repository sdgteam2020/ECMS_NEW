using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class District
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }
        public State State { get; set; }
    }
}
