using BusinessLogicsLayer.BasicDet;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDetTemp
{
    public class BasicDetailTempBL : GenericRepositoryDL<BasicDetailTemp>, IBasicDetailTempBL
    {
        private readonly IBasicDetailTempDB _iBasicDetailTempDB;
        public BasicDetailTempBL(ApplicationDbContext context, IBasicDetailTempDB BasicDetailTemp) : base(context)
        {
            _iBasicDetailTempDB = BasicDetailTemp;
        }
        public async Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId, bool claim, short ArmedIdForORO, int typeId)
        {
            return await _iBasicDetailTempDB.GetALLBasicDetailTemp(UserId, claim, ArmedIdForORO, typeId);
        }

        public Task<DTOGenericResponse<DTOBasicDetailTempRequest?>> GetALLBasicDetailTempByBasicDetailId(int AspNetUsersId, int BasicDetailId, bool claim)
        {
            return _iBasicDetailTempDB.GetALLBasicDetailTempByBasicDetailId(AspNetUsersId, BasicDetailId, claim);
        }

        public Task<BasicDetailTemp?> GetByArmyNo(string ArmyNo)
        {
            return _iBasicDetailTempDB.GetByArmyNo(ArmyNo);
        }

        public Task<bool> UpdateByArmyNo(string ArmyNo)
        {
            return _iBasicDetailTempDB.UpdateByArmyNo(ArmyNo);
        }
    }
}
