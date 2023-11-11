using Microsoft.AspNetCore.Identity;

namespace RolesAuthorization.Data
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsRevoked { get; set; }
    }
}
