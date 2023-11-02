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
    public class UserProfileMappingBL : GenericRepositoryDL<MMappingProfile>, IUserProfileMappingBL
    {
        private readonly IUserProfileDB _iUserProfileDB;

        public UserProfileMappingBL(ApplicationDbContext context) : base(context)
        {
        }
    }
}
