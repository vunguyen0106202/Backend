using API.Data;
using API.Dtos;
using API.Helper.Result;
using API.Helper.SignalR;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleClaimController : Controller
    {
        private readonly DPContext _Db;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRoleClaimController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _Db = context;
            _hubContext = hubContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpGet("GetsUserRoleClaim/{idUser}")]
        public async Task<IActionResult> GetsUserRoleClaim(string idUser)
        {
            var user = await _userManager.FindByIdAsync(idUser);
            if (user == null)
                return Ok(Result<object>.Success(null, 0, "User không tồn tại."));
            var q = user.Quyen;
            var role = await _Db.Roles.Where(r=>r.Name==q).FirstOrDefaultAsync();
            var roleId = role.Id;
            var c = await _Db.RoleClaims.Where(c => c.RoleId == roleId).ToListAsync();

            var userClaimDtos = c.Select(c => new UserClaimDto
            {
                Id = c.Id,
                Type = c.ClaimType,
                Value = c.ClaimValue
            });

            return Ok(Result<IEnumerable<UserClaimDto>>.Success(userClaimDtos, 0, ""));
        }


        [HttpPost("AddUserRoleClaim")]
        public async Task<IActionResult> AddRoleClaim(string userId, [FromBody] UserClaimDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Ok(Result<object>.Success(null, 0, "User không tồn tại."));

            var existingClaims = await _Db.UserClaims.Where(c => c.UserId == userId).ToListAsync();
            var existingClaim = existingClaims.Any(c => c.ClaimType == model.Type && c.ClaimValue == model.Value);
            if (existingClaim)
                return Ok(Result<object>.Success(model, 0, "Claim đã tồn tại trong Role."));

            var claim = new Claim(model.Type, model.Value);
            var result = await _userManager.AddClaimAsync(user, claim);

            return Ok(Result<object>.Success(model, 0, "Thêm Claim thành công."));

        }

        [HttpPost("EditUserRoleClaim")]
        public async Task<IActionResult> EditUserRoleClaim(string userId, [FromBody] UserClaimDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Ok(Result<object>.Success(null, 0, "User không tồn tại."));
             
            var existingClaims = await _userManager.GetClaimsAsync(user);

            var claimToUpdate = await _Db.UserClaims.FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);
            if (claimToUpdate == null)
                return BadRequest("Claim cần cập nhật không tồn tại trong UserClaim.");

            var isDuplicateClaim = existingClaims.Any(c => c.Type == model.Type && c.Value == model.Value && (c.Type != claimToUpdate.ClaimType || c.Value != claimToUpdate.ClaimValue));
            if (isDuplicateClaim)
                return BadRequest("Claim mới đã tồn tại trong User.");

            claimToUpdate.ClaimType = model.Type;
            claimToUpdate.ClaimValue = model.Value;

            await _Db.SaveChangesAsync();
            return Ok(Result<object>.Success(model, 0, "Cập nhật Claim thành công."));
        }

        [HttpDelete("RemoveUserRoleClaim")]
        public async Task<IActionResult> RemoveUserRoleClaim(string userId, int idUserClaim)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Ok(Result<object>.Success(null, 0, "User không tồn tại."));

            var claimToRemove = await _Db.UserClaims.FindAsync(idUserClaim);
            if (claimToRemove == null)
                return NotFound("Claim không tồn tại.");

            var removeResult = await _userManager.RemoveClaimAsync(user, new Claim(claimToRemove.ClaimType, claimToRemove.ClaimValue));
            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            return Ok(Result<object>.Success("", 0, "Xoá thành công"));
        }
    }
}
