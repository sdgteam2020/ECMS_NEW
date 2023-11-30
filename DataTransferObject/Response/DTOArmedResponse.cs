using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOArmedResponse
    {
        public int ArmedId { get; set; }
        public string ArmedName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public bool FlagInf { get; set; }
        public int ArmedCatId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
