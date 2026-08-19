using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODataForFwd
    {
        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public int? AspNetUsersId { get; set; }
       
    }
}
