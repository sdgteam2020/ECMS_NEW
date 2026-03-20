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
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(50, ErrorMessage = "Maximum length of Appointment Name is fifty character.")]
        public string AppointmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment abbreviation is required.")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [MaxLength(20, ErrorMessage = "Maximum length of Appointment Abbreviation is twenty character.")]
        public string AppointmentAbbreviation { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool Approved { get; set; } = false;
    }
}
