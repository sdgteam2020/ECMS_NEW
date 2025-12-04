using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOPreventBasicDetailEditResponse
    {
        public int RequestId { get; set; }
        public bool IsLock { get; set; } = false;
        public int AspNetUsersId { get; set; }
        public byte StatusId { get; set; }
    }
}
