using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
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
        public Task<DTOProfileIdCheckInFKTableResponse> ProfileIdCheckInFKTable(int UserId);
        public Task<DTOProfileManageDeleteResponse> DeleteProfile(MUserProfile mUserProfile);
        public Task<bool?> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO);
        public Task<bool?> FindByArmyNo(string ArmyNo);
        public Task<bool?> FindByArmyNoWithUserId(string ArmyNo, int UserId);
        public Task<DTOProfileResponse?> GetProfileByUserId(int UserId);
        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo,int UserId);
        public Task<MUserProfile> GetByIsWithoutTokenApply(int UserId);
        public Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo);
        public Task<DTOUserProfileResponse?> GetByArmyNo(string ArmyNo,int UserId);
        public Task<List<DTOUserProfileResponse>> GetAll(int DomainId, int UserId);
        public Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId, string Name, int TypeId,int IsRO, int IsORO, int DomainMapId);
        public Task<List<BasicDetailVM>> GetByRequestId(int RequestId);
        public Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int IsRO,int IsORO, int IsAfsacCell, int BasicDetailsId, int DomainMapId, DTOApplFwdConditionRequest dTOApplFwdCondition);
        public Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo,int AspNetUsersId);
        public  Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo);
        public Task<List<DTOAllRelatedDataByArmyNoResponse>?> GetTopByArmyNo(string ArmyNo);
    }
}
