using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RolesAuthorization.Data;

namespace RolesAuthorization.Claims
{
    public class PermissionUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpContext _httpContext;

        public PermissionUser(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userManager = userManager;
        }

        public string GetUserId()
        {
            ApplicationUser? user = _userManager.FindByNameAsync(_httpContext.User.Identity.Name).Result;
            return user?.Id;
        }
    }
}
