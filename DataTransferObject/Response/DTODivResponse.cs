using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODivResponse
    {
        public int DivId { get; set; }
        public string DivName { get; set; }
        public string ComdName { get; set; }
        public int ComdId { get; set; }
        public string CorpsName { get; set; }
        public int CorpsId { get; set; }

        
    }
}
