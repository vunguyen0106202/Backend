using API.Data;
using API.Dtos;
using API.Helper.Result;
using API.Helper.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    public class RoleController : Controller
    {
        private readonly DPContext _Db;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext, RoleManager<IdentityRole> roleManager)
        {
            _Db = context;
            _hubContext = hubContext;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetsRole(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = _Db.Roles.Select(s => new
            {
                s.Id,
                s.Name,
                s.NormalizedName,
                s.ConcurrencyStamp
            }).Where(s => string.IsNullOrEmpty(key) || s.Name.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "name":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name);
                        break;
                    default:
                        query = query.OrderBy(s => s.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

            query = query.Skip(offset).Take(limit);

            var results = await query.ToListAsync();

            var totalCount = await _Db.Roles
               .Where(s => string.IsNullOrEmpty(key) || s.Name.Contains(key))
               .CountAsync();

            return Ok(Result<IEnumerable<object>>.Success(results, totalCount, ""));
        }

        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] IdentityRole model)
        {
            
            IdentityRole role;
            if (!string.IsNullOrEmpty(model.Id))
            {
                role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    return NotFound("Role not found.");
                }

                role.Name = model.Name;
                role.NormalizedName = model.Name.ToUpper();
                var updateResult = await _roleManager.UpdateAsync(role);
                if (updateResult.Succeeded)
                {
                    return Ok(Result<object>.Success(role, 0, ""));
                }
                return BadRequest(updateResult.Errors);
            }
            else
            {
                role = new IdentityRole
                {
                    Name = model.Name,
                    NormalizedName = model.Name.ToUpper()
                };

                var createResult = await _roleManager.CreateAsync(role);
                if (createResult.Succeeded)
                {
                    return Ok(Result<object>.Success(role, 0, ""));
                }
                return BadRequest(createResult.Errors);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return Ok(Result<object>.Success(role,1,""));
        }
        [HttpDelete("{idRole}")]
        public async Task<IActionResult> DeleteRole(string idRole)
        {
            var role = await _roleManager.FindByIdAsync(idRole);
            if (role == null)
                return NotFound("Role không tồn tại!");

            var userRoles = _Db.UserRoles.Where(ur => ur.RoleId == idRole).ToList();
            var roleClaims = _Db.RoleClaims.Where(rc => rc.RoleId == idRole).ToList();
            if (userRoles.Any())
                _Db.UserRoles.RemoveRange(userRoles);

            if (roleClaims.Any())
                _Db.RoleClaims.RemoveRange(roleClaims);

            await _Db.SaveChangesAsync();
            var result = await _roleManager.DeleteAsync(role);

            return Ok(Result<object>.Success(result, 0, "Xóa Role thành công."));
        }

        [HttpGet("GetRoleClaims/{idRole}")]
        public async Task<IActionResult> GetRoleClaims(string idRole)
        {
            var role = await _roleManager.FindByIdAsync(idRole);
            if (role == null)
                return Ok(Result<object>.Success(null, 0, "Role không tồn tại."));

            //var claims = await _roleManager.GetClaimsAsync(role);

            var c = await _Db.RoleClaims.Where(c => c.RoleId == idRole).ToListAsync();

            var claimDtos = c.Select(c => new ClaimDto
            {
                Id = c.Id,
                Type = c.ClaimType,
                Value = c.ClaimValue
            });

            return Ok(Result<IEnumerable<ClaimDto>>.Success(claimDtos, 0, ""));
        }

        [HttpPost("AddRoleClaim")]
        public async Task<IActionResult> AddRoleClaim(string roleId, [FromBody] ClaimDto model)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return Ok(Result<object>.Success(null, 0, "Role không tồn tại."));

            var claim = new Claim(model.Type, model.Value);

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            var existingClaim = existingClaims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);

            if (existingClaim != null)
                return Ok(Result<object>.Success(model, 0, "Claim đã tồn tại trong Role."));

            var result = await _roleManager.AddClaimAsync(role, claim);

            return Ok(Result<object>.Success(model, 0, "Thêm Claim thành công."));

        }

        [HttpPost("UpdateRoleClaim")]
        public async Task<IActionResult> UpdateRoleClaim(string roleId, [FromBody] ClaimDto model)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return BadRequest("Role không tồn tại.");

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            var claimToUpdate = await _Db.RoleClaims.FirstOrDefaultAsync(c => c.Id == model.Id && c.RoleId == roleId);
            if (claimToUpdate == null)
                return BadRequest("Claim cần cập nhật không tồn tại trong Role.");

            var isDuplicateClaim = existingClaims.Any(c => c.Type == model.Type && c.Value == model.Value && (c.Type != claimToUpdate.ClaimType || c.Value != claimToUpdate.ClaimValue));
            if (isDuplicateClaim)
                return BadRequest("Claim mới đã tồn tại trong Role.");

            claimToUpdate.ClaimType = model.Type;
            claimToUpdate.ClaimValue = model.Value;
            await _Db.SaveChangesAsync();
            return Ok(Result<object>.Success(model, 0, "Cập nhật Claim thành công."));
        }

        [HttpDelete("RemoveRoleClaim/{roleId}")]
        public async Task<IActionResult> RemoveRoleClaim(string roleId, int claimId)

        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound("Role không tồn tại.");

            var claimToRemove = await _Db.RoleClaims.FindAsync(claimId);
            if (claimToRemove == null)
                return NotFound("Claim không tồn tại.");

            var removeResult = await _roleManager.RemoveClaimAsync(role, new Claim(claimToRemove.ClaimType, claimToRemove.ClaimValue));
            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            return Ok(Result<object>.Success("", 0, "Xoa thanh cong"));
        }


    }
}
