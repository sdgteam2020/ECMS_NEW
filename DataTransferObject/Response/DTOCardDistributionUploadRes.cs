using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCardDistributionUploadRes
    {
        public DTOCardDistributionUploadEnum Result { get; set; }
        public int ValidDataCount { get; set; }
        public int InValidDataCount { get; set; }
        public string Message { get; set; }
    }
}
