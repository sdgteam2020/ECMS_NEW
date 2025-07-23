using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.DispatchCardMapping
{
    public class DispatchCardMappingBL:GenericRepositoryDL<TrnDispatchCardMapping>,IDispatchCardMappingBL
    {
        private readonly IDispatchCardMappingDB dispatchCardMappingDB;     
        public DispatchCardMappingBL(ApplicationDbContext context, IDispatchCardMappingDB dispatchCardMappingDB) :base(context)
        {
            this.dispatchCardMappingDB = dispatchCardMappingDB;
        }
        public async Task<List<DTODispatchCardInRequest>> GetRequestIds(int DispatchCardId)
        {
            return await dispatchCardMappingDB.GetRequestIds(DispatchCardId);
        }
    }
}
