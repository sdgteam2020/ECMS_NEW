using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessLogicsLayer.User
{ 
    public class UserBL : GenericRepositoryDL<UserM>,IUserBL
    {
        private readonly IUserDB _userdb;

        public UserBL(ApplicationDbContext context, IUserDB userDB) : base(context)
        {
            _userdb = userDB;   
        }

        public IEnumerable<SelectListItem> GetRole()
        {
            return _userdb.GetRole();
        }
    }
}