using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Dtos.ModelOveride;
using API.Helper.Result;
using API.Dtos;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DPContext _context;
        public UserManagersController(DPContext context, UserManager<AppUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> Gets(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = _context.AppUsers.Select(
                s => new Users
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    DiaChi = s.DiaChi,
                    SDT = s.SDT,
                    Avatar = s.ImagePath,
                    Email = s.Email,
                    Quyen = s.Quyen,
                }
                ).Where(s => string.IsNullOrEmpty(key) || s.FirstName.Contains(key) || s.LastName.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "ten":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.FirstName) : query.OrderByDescending(s => s.FirstName);
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

            var totalCount = await _context.AppUsers
             .Where(s => string.IsNullOrEmpty(key) || s.FirstName.Contains(key))
             .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<Users>>.Success(results, totalCount, ""));
            //return await _userManager.Users.ToListAsync();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
                return Ok(Result<object>.Success(null, 0, "password ko hop le"));

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return Ok(Result<object>.Success(null, 0, "khong ton tai user!!!"));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            return Ok(Result<object>.Success(null, 0, "cap nhat mat khau thanh cong!!!"));
        }
    }
}
