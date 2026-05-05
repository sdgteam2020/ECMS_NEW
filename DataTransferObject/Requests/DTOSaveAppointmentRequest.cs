using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOSaveAppointmentRequest
    {
        [Range(0, short.MaxValue, ErrorMessage = "ApptId must be a positive number.")]
        public short ApptId { get; set; }

        [Required(ErrorMessage = "AppointmentName is required.")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/()\-]+(?: [A-Za-z0-9&\/()\-]+)*$", ErrorMessage = "Appointment name must contain at least one alphabet. Only A-Z, a-z, 0-9, & - / ( ) and single space allowed.")]
        [MaxLength(50, ErrorMessage = "Maximum length of Appointment Name is fifty character.")]
        public string AppointmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment abbreviation is required.")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/()\-]+(?: [A-Za-z0-9&\/()\-]+)*$", ErrorMessage = "Abbreviation name must contain at least one alphabet. Only A-Z, a-z, 0-9, & - / ( ) and single space allowed.")]
        [MaxLength(20, ErrorMessage = "Maximum length of Appointment Abbreviation is twenty character.")]
        public string AppointmentAbbreviation { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool Approved { get; set; } = false;
    }
}
