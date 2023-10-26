using DataTransferObject.Response.User;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessLogicsLayer.User
{
    public interface IUserBL : IGenericRepository<UserM>
    {

        //public Task<UserM> GetByUserName(string UserName);
        public IEnumerable<SelectListItem> GetRole();
    }
}
