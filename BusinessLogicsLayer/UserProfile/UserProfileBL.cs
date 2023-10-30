using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class UserProfileBL : GenericRepositoryDL<MUserProfile>, IUserProfileBL
    {
        private readonly IUserProfileDB _iUserProfileDB;

        public UserProfileBL(ApplicationDbContext context, IUserProfileDB userProfileDB) : base(context)
        {
            _iUserProfileDB = userProfileDB;   
        }

        
    }
}
