using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Posting;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PostingController : Controller
    {
        private readonly IPostingBL _iPostingBL;
        public PostingController(IPostingBL postingBL)
        {
            _iPostingBL = postingBL;
        }
        public IActionResult PostingIn()
        {
            return View();
        }
        public async Task<IActionResult> GetPostingIn(string ArmyNo)
        {
            var data = await _iPostingBL.GetArmyDataForPostingIn(ArmyNo);
            return Json(data);
        }
    }
}
