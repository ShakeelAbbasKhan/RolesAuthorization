using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RolesAuthorization.Constant;
using RolesAuthorization.Data;
using RolesAuthorization.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RolesAuthorization.Helper
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public JWTService(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //public string GenerateToken(IEnumerable<Claim> claims)
        //{
        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
        //    var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Issuer = _configuration["JWTKey:ValidIssuer"],
        //        Audience = _configuration["JWTKey:ValidAudience"],
        //        //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
        //        Expires = DateTime.UtcNow.AddMinutes(30),
        //        SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
        //        Subject = new ClaimsIdentity(claims)
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}


        public async Task<string> GenerateTokenString(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var authClaims = new List<Claim>();

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                authClaims.AddRange(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                });

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                    var role = await _roleManager.FindByNameAsync(userRole);

                    if (role != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(role);

                        var permissionClaims = roleClaims
                            .Select(claim => new Claim("Permission", claim.Value));

                        authClaims.AddRange(permissionClaims);
                    }
                }
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));

            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: _configuration["JWTKey:ValidIssuer"],
                audience: _configuration["JWTKey:ValidAudience"],
                signingCredentials: signingCred);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

    }
}
