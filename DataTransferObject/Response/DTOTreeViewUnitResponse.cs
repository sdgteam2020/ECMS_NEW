using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOTreeViewUnitResponse
    {
        public List<MComd> MComd { get; set; }
        public List<MCorps> MCorps { get; set; }  
        public List<MDiv> MDiv { get; set; }  
        public List<MBde> MBde { get; set; }   
        public List<DTOMapUnitResponse> Unit { get; set; }
    }
}
