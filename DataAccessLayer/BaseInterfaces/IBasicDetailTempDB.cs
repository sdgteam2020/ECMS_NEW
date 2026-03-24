using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IBasicDetailTempDB:IGenericRepositoryDL<BasicDetailTemp>
    {
        public Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId,bool claim, short ArmedIdForORO, int typeId);
        public Task<DTOGenericResponse<DTOBasicDetailTempRequest?>> GetALLBasicDetailTempByBasicDetailId(int AspNetUsersId, int BasicDetailId, bool claim);
        public Task<bool> UpdateByArmyNo(string ArmyNo);
        public Task<BasicDetailTemp?> GetByArmyNo(string ArmyNo);
    }
}
