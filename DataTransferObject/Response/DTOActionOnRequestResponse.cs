using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOActionOnRequestResponse
    {
        public byte ApplyForId { get; set; }
        public byte BeforeAction_StepId { get; set; }
        public byte AfterAction_StepId { get; set; }
        public int AspNetUsersId { get; set; }
    }
}
