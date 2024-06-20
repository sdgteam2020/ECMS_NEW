using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDetTemp
{ 
    public interface IBasicDetailTempBL : IGenericRepository<BasicDetailTemp>
    {
        public Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId, int TypeId);
        public Task<DTOBasicDetailTempRequest> GetALLBasicDetailTempByBasicDetailId(int UserId,int BasicDetailId);

        public Task<bool> UpdateByArmyNo(string ArmyNo);

        public Task<BasicDetailTemp> GetByArmyNo(string ArmyNo);
    }
}
