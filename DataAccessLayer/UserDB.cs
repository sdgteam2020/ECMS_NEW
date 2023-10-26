using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLayer
{
    public class UserDB : GenericRepositoryDL<UserM>,IUserDB
    {
        public UserDB(ApplicationDbContext context) : base(context)
        {

        }
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}
        public IEnumerable<SelectListItem> GetRole()
        {
            var roles = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="User", Value = "user" },
            };
            return new SelectList(roles, "Value", "Text");
        }
        public Task<UserM> GetByUserName(string UserName)
        {
            throw new NotImplementedException();
        }
    }
}