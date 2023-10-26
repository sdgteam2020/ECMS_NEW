using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IUserDB : IGenericRepositoryDL<UserM>
    {
        public Task<UserM> GetByUserName(string UserName);
        public IEnumerable<SelectListItem> GetRole();
    }
}
