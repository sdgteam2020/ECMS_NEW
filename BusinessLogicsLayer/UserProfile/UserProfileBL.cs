using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;

namespace BusinessLogicsLayer.Master
{ 
    public class UserProfileBL : GenericRepositoryDL<MUserProfile>, IUserProfileBL
    {
        private readonly IUserProfileDB _iUserProfileDB;
        public UserProfileBL(ApplicationDbContext context, IUserProfileDB userProfileDB) : base(context)
        {
            _iUserProfileDB = userProfileDB;   
        }
        public async Task<DTOProfileIdCheckInFKTableResponse> ProfileIdCheckInFKTable(int UserId)
        {
            return await _iUserProfileDB.ProfileIdCheckInFKTable(UserId);
        }
        public async Task<DTOProfileManageDeleteResponse> DeleteProfile(MUserProfile mUserProfile)
        {
            return await _iUserProfileDB.DeleteProfile(mUserProfile);
        }
        /// <summary>
        /// Validates mapping data and updates the user profile with domain mapping.
        /// </summary>
        /// <param name="dTO">Profile update request containing mapping details.</param>
        /// <returns>
        /// A generic response indicating success or failure of the update operation.
        /// </returns>
        public async Task<DTOGenericResponse<string>> UpdateProfileWithMapping(DTOUpdateProfileWithMappingRequest dTO)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            DTOCheckedBeforeUpdateProfileResponse dTOChecked = await _iUserProfileDB.CheckedBeforeUpdateProfile(dTO);
            if(dTOChecked.TDMId !=null && dTOChecked.UserId != null && dTOChecked.ApplyForId != null && dTOChecked.ApplyForId !=2 && dTOChecked.RankAbbreviation != null)
            {
                dTO.TDMId = dTOChecked.TDMId.Value;
                dTO.UserId = dTOChecked.UserId.Value;

                response = await _iUserProfileDB.UpdateProfileWithMapping(dTO);
                response.Value = dTOChecked.RankAbbreviation;
            }
            else
            {
                if (dTOChecked.TDMId == null)
                {
                    response.Message = "DID is not mapped to the mapping table.";
                }
                else if(dTOChecked.UserId == null)
                {
                    response.Message = "Profile is not mapped to the mapping table.";
                }
                else if (dTOChecked.ApplyForId == null || dTOChecked.RankAbbreviation == null || dTOChecked.ApplyForId == 2)
                {
                    response.Message = "Invalid Rank Id.";
                }
                response.Value = "";
                response.Result = false;
            }
            return response;
        }

        public Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId, string Name, int TypeId, int RO,int ORO, int DomainMapId)
        {
            return _iUserProfileDB.GetDataForFwd(StepId, UnitId, Name,TypeId, RO, ORO, DomainMapId);
        }
        public async Task<DTOProfileResponse?> GetProfileByUserId(int UserId)
        {
            return await _iUserProfileDB.GetProfileByUserId(UserId);
        }

        public async Task<bool?> FindByArmyNoWithUserId(string ArmyNo, int UserId)
        {
          return  await _iUserProfileDB.FindByArmyNoWithUserId(ArmyNo, UserId);
        }

        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetByArmyNo(ArmyNo, UserId);
        }

        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetByMArmyNo(ArmyNo, UserId);
        }

        public async Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.GetUserProfileByArmyNo(ArmyNo);
        }
        public Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo, int AspNetUsersId)
        {
            return _iUserProfileDB.CheckArmyNoInUserProfile(ArmyNo, AspNetUsersId);
        }

        public Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int IsRO, int IsORO, int IsAfsacCell, int BasicDetailsId,int DomainMapId)
        {
            return _iUserProfileDB.GetOffrsByUnitMapId(UnitId, IsRO, IsORO, IsAfsacCell, BasicDetailsId, DomainMapId);
        }
        public async Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.GetAllRelatedDataByArmyNo(ArmyNo);
        }
        public async Task<List<DTOAllRelatedDataByArmyNoResponse>?> GetTopByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.GetTopByArmyNo(ArmyNo);
        }
        public async Task<bool?> FindByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.FindByArmyNo(ArmyNo);
        }

        public async Task<MUserProfile> GetByIsWithoutTokenApply(int UserId)
        {
            return await _iUserProfileDB.GetByIsWithoutTokenApply(UserId);
        }
        public async Task<DTOCheckedBeforeUpdateProfileResponse> CheckedBeforeUpdateProfile(DTOUpdateProfileWithMappingRequest dTO)
        {
            return await _iUserProfileDB.CheckedBeforeUpdateProfile(dTO);
        }
        public async Task<DTOGenericResponse<DTOTokenStatusResponse?>> GetTokenStatus(int AspNetUsersId)
        {
            return await _iUserProfileDB.GetTokenStatus(AspNetUsersId);
        }
    }
}
