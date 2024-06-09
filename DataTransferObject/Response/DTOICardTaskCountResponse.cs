using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class DTOICardTaskCountResponse
    {
        public int ToDrafted { get; set; }
        public int ToSubmitted { get; set; }
        public int ToCompleted { get; set; }
        public int ToRejected { get; set; }
        public int _2ndLevelPending { get; set; }
        public int _2ndLevelApproved { get; set; }
        public int _2ndLevelReject { get; set; }
        public int _3rdLevelPending { get; set; }
        public int _3rdLevelApproved { get; set; }
        public int _3rdLevelReject { get; set; }
        public int _4thLevelPending { get; set; }
        public int _4thLevelApproved { get; set; }
        public int _4thLevelReject { get; set; }
        public int ExportPending { get; set; }
        public int ExportApproved { get; set; }
        public int ExportReject { get; set; }

    }
}
