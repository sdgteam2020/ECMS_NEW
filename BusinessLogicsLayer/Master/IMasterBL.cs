using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IMasterBL
    {
        public Task<List<DTORemarksResponse>> GetRemarksByTypeId(DTORemarksRequest Data);
    }
}
