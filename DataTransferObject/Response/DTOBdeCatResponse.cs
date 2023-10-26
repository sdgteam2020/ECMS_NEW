using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOBdeCatResponse
    {
        public int BdeCatId { get; set; }
       
        public string BdeCatName { get; set; }

        public int ComdId { get; set; }
        public string ComdName { get; set; }
        public int CorpsId { get; set; }
        public string CorpsName { get; set; }
    }
}
