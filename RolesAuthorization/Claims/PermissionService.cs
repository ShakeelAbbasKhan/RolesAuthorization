using Microsoft.AspNetCore.Identity;
using RolesAuthorization.Data;
using System.Security.Claims;

namespace RolesAuthorization.Claims
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PermissionService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            
        }
        public async Task<HashSet<string>> GetPermissionsAsync(string memberId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(memberId.ToString());

            if (user == null)
            {
                return new HashSet<string>();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            HashSet<string> permissions = new HashSet<string>();

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    foreach (var claim in roleClaims)
                    {
                        permissions.Add(claim.Value);
                    }
                }
            }

            return permissions;
        }

    }
}
