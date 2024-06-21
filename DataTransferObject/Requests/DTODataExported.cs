using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODataExported
    {
        public int Id { get; set; }
        public int AspNetUsersId { get; set; }
        public int UserId { get; set; }
        public string IP { get; set; }
        public string CreatedBy { get; set; }
        public string RequestId { get; set; }
        public DateTime CreatedOn { get; set;}
    }
}
