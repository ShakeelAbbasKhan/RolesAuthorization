namespace RolesAuthorization.Claims
{
    public interface IPermissionService
    {
        Task<HashSet<string>> GetPermissionsAsync(string memberId);
    }
}
