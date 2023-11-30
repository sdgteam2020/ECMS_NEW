using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Master
{ 
    public class UserProfileBL : GenericRepositoryDL<MUserProfile>, IUserProfileBL
    {
        private readonly IUserProfileDB _iUserProfileDB;

        public UserProfileBL(ApplicationDbContext context, IUserProfileDB userProfileDB) : base(context)
        {
            _iUserProfileDB = userProfileDB;   
        }

        public Task<List<DTOUserProfileResponse>> GetAll(string DomainId, int UserId)
        {
            return _iUserProfileDB.GetAll(DomainId, UserId);
        }

        public Task<DTOUserProfileResponse> GetAllByArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetAllByArmyNo(ArmyNo, UserId);
        }

        public Task<bool> GetByArmyNo(MUserProfile Data, int UserId)
        {
          return  _iUserProfileDB.GetByArmyNo(Data, UserId);
        }

        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetByArmyNo(ArmyNo, UserId);
        }

        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetByMArmyNo(ArmyNo, UserId);
        }
    }
}
