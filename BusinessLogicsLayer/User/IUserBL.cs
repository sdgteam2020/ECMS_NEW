using DataAccessLayer;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessLogicsLayer.User
{
    public interface IUserBL : IGenericRepositoryDL<UserM>
    {
        public IEnumerable<SelectListItem> GetRole();
    }
}
