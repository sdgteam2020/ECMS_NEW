using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCheckBeforeLostReportResponse
    {
        public bool Result { get; set; } 
        public string Message { get; set; }=string.Empty;
        public int BasicDetailId { get; set; }
        public byte StatusId { get; set; }
        public string AppointmentName { get; set; } = string.Empty;
        public int? HotlistCardId { get; set; }
    }
}
