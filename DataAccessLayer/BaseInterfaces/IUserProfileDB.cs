using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IUserProfileDB : IGenericRepositoryDL<MUserProfile>
    {
        public Task<bool> GetByArmyNo(MUserProfile Data, int UserId);
        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo,int UserId);
        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo,int UserId);
        public Task<List<DTOUserProfileResponse>> GetAll(string DomainId, int UserId);
        public Task<DTOUserProfileResponse> GetAllByArmyNo(string ArmyNo, int UserId);
    }
}
