using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IDispatchCardMappingDB
    {
        public Task<List<DTODispatchCardInRequest>> GetRequestIds(int DispatchCardId);
    }
}
