using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOAPILoginRequest
    {
        public string? ClientKey { get; set; }
        public string? ClientIP { get; set; }
        public string? ClientURL { get; set; }
        public string? ClientPW { get; set; }

        public string ClientName { get; set; }
        //public string? email { get; set; }
        //public string? password { get; set; }
    }
}
