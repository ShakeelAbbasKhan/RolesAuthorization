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
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            
            if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            {

            var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult(policy.Build());
            }


            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    }


}
