using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
