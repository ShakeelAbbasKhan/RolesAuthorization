using Microsoft.AspNetCore.Identity;
using RolesAuthorization.ViewModels;
using System.Reflection;
using System.Security.Claims;

namespace RolesAuthorization.Helper
{
    public static class ClaimsHelper
    {
        
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
