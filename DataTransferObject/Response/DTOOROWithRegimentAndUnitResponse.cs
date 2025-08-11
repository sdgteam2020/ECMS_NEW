using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOOROWithRegimentAndUnitResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SUSNo { get; set; }=  string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
    }
}
