using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RolesAuthorization.Data;

namespace RolesAuthorization.AuthorizeMiddleware
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class AdminRequirement : IAuthorizationRequirement
    {
    }
}
