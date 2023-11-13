using Microsoft.IdentityModel.Tokens;
using RolesAuthorization.Constant;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RolesAuthorization.Helper
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;
        //  private readonly UserManager<ApplicationUser> _userManager;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
            //  _userManager = userManager;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);

            //// Include other claims
            //var identity = new ClaimsIdentity(claims);

            //// Example: Adding permissions as claims
            //identity.AddClaim(new Claim("Permission", Permissions.Products.View));
            //identity.AddClaim(new Claim("Permission", Permissions.Products.Create));
            //identity.AddClaim(new Claim("Permission", Permissions.Products.Edit));
            //identity.AddClaim(new Claim("Permission", Permissions.Products.Delete));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
