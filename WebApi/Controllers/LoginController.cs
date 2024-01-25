using DataTransferObject.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {

            _config = config;
        }
        [HttpPost]
        public IActionResult Login(DTOAPILoginRequest login)
        {
            bool isLoggedIn = false;
            if (login.ClientName == "admin" && login.ClientPW == "123")
            {
                isLoggedIn = true;
            }
            else
                isLoggedIn = false;
            if (isLoggedIn)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.ClientName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };


            //    authClaims.Add(new Claim(ClaimTypes.Role, login.Role));


                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            else
            {
                return BadRequest();
            }
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        private string GenerateToken(DTOLoginRequest user)
        {
            // var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.None);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
               // new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            var ss = new JwtSecurityTokenHandler().WriteToken(token);
            return ss;

        }

        [HttpGet]
        public ActionResult Get(string Id)
        {
           string ids= HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return Ok(ids);
        }

    }
}
