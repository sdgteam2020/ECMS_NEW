using DataTransferObject.Domain;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IUserProfileDB : IGenericRepositoryDL<MUserProfile>
    {
        public Task<DTOProfileIdCheckInFKTableResponse> ProfileIdCheckInFKTable(int UserId);
        public Task<DTOProfileManageDeleteResponse> DeleteProfile(MUserProfile mUserProfile);
        public Task<DTOGenericResponse<string>> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO);
        public Task<bool?> FindByArmyNo(string ArmyNo);
        public Task<bool?> FindByArmyNoWithUserId(string ArmyNo, int UserId);
        public Task<DTOProfileResponse?> GetProfileByUserId(int UserId);
        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo,int UserId);
        public Task<MUserProfile> GetByIsWithoutTokenApply(int UserId);
        public Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo);
        public Task<DTOUserProfileResponse?> GetByArmyNo(string ArmyNo,int UserId);
        public Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId, string Name, int TypeId,int IsRO, int IsORO, int DomainMapId);
        public Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int IsRO,int IsORO, int IsAfsacCell, int BasicDetailsId, int DomainMapId);
        public Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo,int AspNetUsersId);
        public  Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo);
        public Task<List<DTOAllRelatedDataByArmyNoResponse>?> GetTopByArmyNo(string ArmyNo);
        public Task<DTOCheckedBeforeUpdateProfileResponse> CheckedBeforeUpdateProfile(DTOUpdateProfileWithMappingRequest dTO);
        public Task<DTOGenericResponse<DTOTokenStatusResponse?>> GetTokenStatus(int AspNetUsersId);
    }
}
