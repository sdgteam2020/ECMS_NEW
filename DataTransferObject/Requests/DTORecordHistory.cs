using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTORecordHistory : DTODataTablesRequest
    {
        public DTOMHierarchyRequest Data { get; set; }
        public int ApplyForId { get; set; }
        public int StepId { get; set; }
        public int IsApproveId { get; set; }
    }
}
