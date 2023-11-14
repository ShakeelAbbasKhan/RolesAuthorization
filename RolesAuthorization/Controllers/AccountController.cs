using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RolesAuthorization.Constant;
using RolesAuthorization.Data;
using RolesAuthorization.Helper;
using RolesAuthorization.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RolesAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly JWTService _jWTService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger,JWTService jWTService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _jWTService = jWTService;
            _roleManager = roleManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            TokenViewModel _TokenViewModel = new();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && user.LastLoginDate.HasValue)
                {
                    DateTime lastLoginDate = user.LastLoginDate.Value; // Convert to DateTime
                    if (lastLoginDate.AddDays(5) < DateTime.UtcNow)
                    {
                        return Ok("Password Expires Reset the Password");
                    }
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                       new Claim(ClaimTypes.Name, user.UserName),
                       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };


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
                    _TokenViewModel.AccessToken = _jWTService.GenerateToken(authClaims);
                    //  _TokenViewModel.RefreshToken = _authService.GenerateRefreshToken();
                    _TokenViewModel.StatusCode = 1;
                    _TokenViewModel.StatusMessage = "Success";

                    var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
                    user.RefreshToken = _TokenViewModel.AccessToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);

                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    // await _userManager.UpdateAsync(user);


                    return Ok(new { _TokenViewModel });

                }

                if (result.IsLockedOut)
                {
                    var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                    return BadRequest($"Your account is locked out until {lockoutEndDate}.");
                }

                return BadRequest("Invalid login attempt");
            }

            return BadRequest(ModelState);
        }


        //[HttpPost("Login")]
        //public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user != null && user.LastLoginDate.HasValue)
        //        {
        //            DateTime lastLoginDate = user.LastLoginDate.Value; // Convert to DateTime
        //            if (lastLoginDate.AddDays(5) < DateTime.UtcNow)
        //            {
        //                return Ok("Password Expires Reset the Password");
        //            }
        //        }

        //        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
        //        if (result.Succeeded)
        //        {
        //            return Ok(new { Message = "Login successful." });
        //        }

        //        if (result.IsLockedOut)
        //        {
        //            return BadRequest(new { Message = "Your account is locked out due to 5 invalid attempts. Please reset your password." });
        //        }

        //        return BadRequest(new { Message = "Invalid login attempt." });
        //    }

        //    return BadRequest(ModelState);
        //}


        [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email,
                    LastLoginDate = DateTime.Now,
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerModel.RoleName);

                    return Ok("Registration successful");
                }

                return BadRequest(result.Errors);
            }

            return BadRequest(ModelState);
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out successfully");
        }

        //   [Authorize(Policy = "SuperUserRights")]
        [HttpGet("UserList")]
        public IActionResult UserList()
        {
            var users = _userManager.Users.Select(u => new UserListVM
            {
                Id = u.Id,
                Email = u.Email,
                Roles = _userManager.GetRolesAsync(u).Result.ToList()
            }).ToList();

            return Ok(users);
        }

    }
}
