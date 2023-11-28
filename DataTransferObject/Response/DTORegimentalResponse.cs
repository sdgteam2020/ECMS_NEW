using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORegimentalResponse
    {
       
        public int RegId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; } 
        public int ArmedId { get; set; }
        public string ArmedName { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}
