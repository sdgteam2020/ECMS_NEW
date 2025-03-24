using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCardDistributionUploadResponse
    {
        public DTOCardDistributionUploadEnum Result { get; set; }
        public int ValidRecordsCount { get; set; }
        public int InValidRecordsCount { get; set; }
    }
}
