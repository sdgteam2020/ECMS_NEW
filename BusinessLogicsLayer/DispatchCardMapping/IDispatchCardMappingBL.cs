using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.DispatchCardMapping
{
    public interface IDispatchCardMappingBL:IGenericRepositoryDL<TrnDispatchCardMapping>
    {
        public Task<List<DTODispatchCardInRequest>> GetRequestIds(int DispatchCardId);
    }
}
