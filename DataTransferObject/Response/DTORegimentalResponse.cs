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
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public int ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
