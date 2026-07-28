using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCheckApplicationCloseRequestIdResponse
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int DestructedCardId { get; set; }
    }
}
