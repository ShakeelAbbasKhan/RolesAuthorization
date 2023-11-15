using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RolesAuthorization.Claims;
using RolesAuthorization.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RolesAuthorization.Permission
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {

        private readonly PermissionUser _permissionUser;
        private readonly IPermissionService _permissionService;
        public PermissionAuthorizationHandler(PermissionUser permissionUser, IPermissionService permissionService)
        {

            _permissionUser = permissionUser;
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
           
         
            string memberId = _permissionUser.GetUserId();

            if (memberId == null)
            {
                return;
            }

            HashSet<string> permissions = await _permissionService.GetPermissionsAsync(memberId);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }


            //if (context.User == null)
            //{
            //    return;
            //}
            //var permissionss = context.User.Claims.Where(x => x.Type == "Permission" &&
            //                                                x.Value == requirement.Permission);
            //if (permissionss.Any())
            //{
            //    context.Succeed(requirement);
            //    return;
            //}
        }
    }
}
