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

namespace BusinessLogicsLayer.Master
{
    public interface IUserProfileBL : IGenericRepository<MUserProfile>
    {
        public Task<bool?> FindByArmyNo(string ArmyNo);
        public Task<bool?> FindByArmyNoWithUserId(string ArmyNo, int UserId);
        public Task<DTOProfileResponse?> GetProfileByUserId(int UserId);
        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo, int UserId);
        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId);
        public Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo);
        public Task<List<DTOUserProfileResponse>> GetAll(int DomainId, int UserId);
        public Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId,int UnitId,string Name,int TypeId, int IsIO, int IsCO, int IsRO, int IsORO);
        public Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int ISIO, int ISCO,int IsRO,int IsORO,int BasicDetailsId);
        public Task<List<BasicDetailVM>> GetByRequestId(int RequestId);
        public Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo, int AspNetUsersId);
        public Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo);
        public Task<List<DTOAllRelatedDataByArmyNoResponse>?> GetTopByArmyNo(string ArmyNo);
    }
}
