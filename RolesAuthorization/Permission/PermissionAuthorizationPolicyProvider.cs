using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace RolesAuthorization.Permission
{
    //public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    //{
    //    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    //    {
    //    }
    //    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    //    {
    //        AuthorizationPolicy? policy  = await base.GetPolicyAsync(policyName);

    //        if(policy is not null) {
    //            return policy;
    //        }
    //        return new AuthorizationPolicyBuilder().AddRequirements(new PermissionRequirement(policyName))
    //                .Build();
    //    }
    //}

    public class PermissionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // There can only be one policy provider in ASP.NET Core.
            // We only handle permissions related policies, for the rest
            /// we will use the default provider.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        // Dynamically creates a policy with a requirement that contains the permission.
        // The policy name must match the permission that is needed.
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            //if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            //{
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult(policy.Build());
            //}

            // Policy is not for permissions, try the default provider.
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    }


}
