using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTORequestFwdDetailResponse
    {
        public byte ApplyForId { get; set; }
        public byte StepId { get; set; }
        public int FromAspNetUsersId { get; set; }
        public int FromUserId { get; set; }
        public int ToAspNetUsersId { get; set; }
        public int ToUserId { get; set; }
    }
}
