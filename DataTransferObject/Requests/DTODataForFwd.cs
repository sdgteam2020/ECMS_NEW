using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODataForFwd
    {
        public string Name { get; set; }
        public int TypeId { get; set; }
        public int StepId { get; set; }
        public int UnitId { get; set; }
        public int ISRO { get; set; }
        public int IsORO { get; set; }
        
    }
}
