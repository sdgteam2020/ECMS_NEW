using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IUserProfileBL : IGenericRepository<MUserProfile>
    {
        public Task<bool> GetByArmyNo(MUserProfile Data);
        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo);
        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo);
        public Task<List<DTOUserProfileResponse>> GetAll(string DomainId);
    }
}
