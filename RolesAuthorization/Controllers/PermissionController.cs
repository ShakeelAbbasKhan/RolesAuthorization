using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RolesAuthorization.Constant;
using RolesAuthorization.Data;
using RolesAuthorization.Helper;
using RolesAuthorization.ViewModels;
using System.Security;

namespace RolesAuthorization.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        [HttpGet("{roleId}")]
        public async Task<ActionResult<PermissionViewModel>> Index(string roleId)
        {
            var model = new PermissionViewModel();
            var allPermissions = new List<RoleClaimsViewModel>();
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            model.RoleId = roleId;
            var claims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in claims)
            {
                var roleClaim = new RoleClaimsViewModel
                {
                    Type = claim.Type,
                    Value = claim.Value,
                    Selected = true,
                    
            };

                allPermissions.Add(roleClaim);
            }

            model.RoleClaims = allPermissions;
            return Ok(model);
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaim(role, claim.Value);
            }

            return Ok(new { Message = "Permissions updated successfully." });
        }
    }

}
