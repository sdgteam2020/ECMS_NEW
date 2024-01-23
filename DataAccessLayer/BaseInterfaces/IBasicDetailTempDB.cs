using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IBasicDetailTempDB:IGenericRepositoryDL<BasicDetailTemp>
    {
        public Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId);
        public Task<DTOBasicDetailTempRequest> GetALLBasicDetailTempByBasicDetailId(int UserId, int BasicDetailId);
    }
}
