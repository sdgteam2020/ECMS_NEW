using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTONotificationResponse
    { 
        public int DisplayId { get; set; }
        public string Spanname { get; set; }
        public string Message { get; set; }
        public string DomainId { get; set; }
        public string RankAbbreviation { get; set; }
        public string Name { get; set; }
        public string ServiceNo { get; set; }
        public string PhotoImagePath { get; set; }
        public string TrackingId { get; set; }
        public string Url { get; set; }

    }
}
