using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAccountResponse
    {
        public int Id { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public bool AdminFlag { get; set; } = false;
    }
}
