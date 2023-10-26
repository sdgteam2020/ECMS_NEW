using BusinessLogicsLayer;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Get()
        {
            //var data = unitOfWork.Users.GetByIdAsync(11).Result;
            //var data1 = unitOfWork.Users.GetByUserName("Kapoor").Result;
            return Ok("");
        }
    }
}
