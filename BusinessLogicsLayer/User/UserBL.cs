using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataTransferObject.Response.User;
using DataAccessLayer;
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

        //public UserBL(IUserDB userdb)
        //{
        //    _userdb = userdb;
        //}


    }
}
