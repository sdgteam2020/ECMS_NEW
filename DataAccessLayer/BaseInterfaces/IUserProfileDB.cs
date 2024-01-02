using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using DataTransferObject.ViewModels;
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
        public Task<DTOProfileResponse?> GetProfileByUserId(int UserId);
        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo,int UserId);
        public Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo);
        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo,int UserId);
        public Task<List<DTOUserProfileResponse>> GetAll(int DomainId, int UserId);
        public Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId, string Name, int TypeId);
        public Task<List<BasicDetailVM>> GetByRequestId(int RequestId);
        public Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId);
        public Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo,int AspNetUsersId);
        public  Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo);
    }
}
