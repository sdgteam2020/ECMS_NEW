using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCardDistributionCheckRes
    {
        public DTOCardDistributionUploadEnum Result { get; set; }
        public List<DTOCardDistributionRequest> ValidRecords { get; set; }
        public List<DTOCardDistributionRequest> InValidRecords { get; set; }
    }

    public enum DTOCardDistributionUploadEnum
    { 
        InternalError = 0,
        FullyUpload = 1,
        PartiallyUpload = 2,
        Rejected = 3
    }
}
