using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataAccessLayer
{
    public class UserDB : GenericRepositoryDL<UserM>,IUserDB
    {
        public UserDB(ApplicationDbContext context) : base(context)
        {

        }
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

        /// <summary>
        /// Retrieves a list of roles for user selection.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="SelectListItem"/> objects representing the available roles.
        /// The list includes a default "-- Select --" option and a "User" role.
        /// </returns>
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