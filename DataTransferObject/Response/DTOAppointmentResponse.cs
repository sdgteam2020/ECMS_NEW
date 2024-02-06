using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAppointmentResponse
    {
        public int ApptId { get; set; }
        public string AppointmentName { get; set; }=string.Empty;
        //public int FormationId { get; set; }
        //public string FormationName { get; set; }
    }
}
