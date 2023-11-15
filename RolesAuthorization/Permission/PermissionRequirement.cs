using Microsoft.AspNetCore.Authorization;

namespace RolesAuthorization.Permission
{
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get;}

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
