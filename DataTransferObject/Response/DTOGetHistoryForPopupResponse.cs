using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetHistoryForPopupResponse
    {
        public DTOGetHistoryCommanResponse? UnderProcess { get; set; }
        public List<DTOGetHistoryCommanResponse> CardComplete { get; set; } = new();
        public List<DTOGetHistoryCommanResponse> CardClosed { get; set; } = new();
    }
    public class DTOGetHistoryCommanResponse
    {
        public int RequestId { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
