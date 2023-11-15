using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using RolesAuthorization.Data;

namespace RolesAuthorization.Filters
{
    public class DynamicPermissionCheckFilter : IActionFilter
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public DynamicPermissionCheckFilter(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                if (!ValidatePermissions(user).Result)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        private async Task<bool> ValidatePermissions(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _roleManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var userRoleId = await _roleManager.GetRoleIdAsync(user);
            var userRoles = new List<string> { userRoleId }; 

            var roleClaims = userRoles.SelectMany(r => _roleManager.GetClaimsAsync(_roleManager.Roles.FirstOrDefault(x => x.Id == r)).Result);

            var tokenPermissions = principal.FindAll("permissions").Select(c => c.Value);

            return roleClaims.All(c => tokenPermissions.Contains(c.Value));
        }

    }
}
