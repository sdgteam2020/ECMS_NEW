using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FetchController : ControllerBase
    {
        String[] first = new String[] {"Brown", "Black", "White", "Orange", "Wild", "Tiger", "Snow Leopard", "Koo",
                                              "Kooapps", "Gray", "Zombie", "Gumdrop", "Candy", "Choco", "Darth", "Dark",
                                              "Goldfish on a", "Evil", "German", "Beach", "City", "Haunted", "Spooky"};
        String[] last = new String[] {"Dog", "Cat", "Dalmation", "Bird", "Koobird", "Goldfish", "Turtle", "Clyde",
                                            "Selina", "Troy", "Oscar", "Lily", "Skateboard", "Swim E Fresh", "Pip", "Leo",
                                            "Raph", "Donny", "Mikey", "Man", "Sloth", "Ferret", "Grandpa", "Voviboye"};
        string n = "";
        Random rnd = new Random();
        int x = -1;
        int y = -1;



        public List<UserData> _userDataList;
        Random rnd1 = new Random();
        public FetchController()
        {
            x = rnd.Next(0, first.Length);
            y = rnd.Next(0, last.Length);

            _userDataList = new List<UserData>()
            {
                new UserData()
                {
                    Name = first[x] + " " + last[y],
                    ServiceNo = "IC"+ Random.Shared.Next(50000, 99999)+"X",
                    AadhaarNo = rnd1.Next(1000, 9999).ToString()+" "+ rnd1.Next(1000, 9999).ToString()+" "+ rnd1.Next(1000, 9999).ToString(),
                    Ht = Random.Shared.Next(60, 84)
                },
            };
        }
        [EnableCors("CorsPolicy")]
        [HttpGet("{ICNumber}")]
        public async Task<ActionResult> Get(string ICNumber)
        {

            if (ICNumber != null)
            {
                UserData? userData = _userDataList.FirstOrDefault();
                if (userData != null)
                {
                    return Ok(userData);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }

        }
    }
}
