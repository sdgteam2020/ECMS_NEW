using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MAppointment:Common
    {
        [Key]
        public short ApptId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string AppointmentName { get; set; }
        [Required(ErrorMessage = "required!")]
       
        [ForeignKey("Comd"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short FormationId { get; set; }
        public MFormation? mFormation { get; set; }
    }
}
