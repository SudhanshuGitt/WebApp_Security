using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SecurtiyWeb_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "123")
            {
                // creating securtiy contextS

                // for jwt token authentication payload only contain the claims information
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, credential.UserName),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin","true"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate","2024-01-01")

                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    accees_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });

            }
            else
            {
                ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
                return Unauthorized(ModelState);
            }

        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            // Genrate JWt token
            // convert key to array
            var securityKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");

            // token is not valid before the given time
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireAt,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature)

            );

            // to comnver the jwt object to string we will use JWT security token handler

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }

    public class Credential
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
