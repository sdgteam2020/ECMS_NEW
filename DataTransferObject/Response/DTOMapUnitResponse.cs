using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOMapUnitResponse
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int BdeId { get; set; }
        public string BdeName { get; set; }
        public int ComdId { get; set; }
        public string ComdName { get; set; }
        public int CorpsId { get; set; }
        public string CorpsName { get; set; }
        public int DivId { get; set; }
        public string DivName { get; set; }

       

    }
}
