using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Helpers;
using API.Models;
using API.Helper.Result;
using Google.Apis.Auth;
using API.Helper.Factory;
using Google.Apis.Auth.OAuth2.Responses;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly DPContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtFactory _jwtFactory;
        public AccountsController(UserManager<AppUser> userManager, IMapper mapper, DPContext context, IJwtFactory jwtFactory)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _jwtFactory = jwtFactory;

        }
        // POST api/accounts
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdentity = _mapper.Map<AppUser>(model);
            var result = await _userManager.CreateAsync(userIdentity, model.Password);
            AppUser user = new AppUser();
            user = await _context.AppUsers.FirstOrDefaultAsync(s => s.Id == userIdentity.Id);
            await _userManager.AddToRoleAsync(userIdentity, "Customer");
            _context.AppUsers.Update(user);
            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AdminCreateUser")]
        public async Task<IActionResult> AdminCreateUser([FromForm] CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var userIdentity = _mapper.Map<AppUser>(model);

            AppUser user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                SDT = model.Phone
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.Roles != null)
                {
                    foreach (var r in model.Roles)
                    {
                        await _userManager.AddToRoleAsync(user, r);
                    }
                }
            }
            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            return Ok(Result<object>.Success(user));
        }

        [HttpPost("AdminUpdateUser")]
        public async Task<IActionResult> AdminUpdateUser([FromForm] EditAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tìm người dùng dựa trên UserName hoặc Email
            var existingUser = await _userManager.FindByNameAsync(model.UserName);

            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            // Cập nhật thông tin người dùng
            existingUser.Email = model.Email ?? existingUser.Email;
            existingUser.UserName = model.UserName ?? existingUser.UserName;
            existingUser.FirstName = model.FirstName ?? existingUser.FirstName;
            existingUser.LastName = model.LastName ?? existingUser.LastName;
            existingUser.SDT = model.Phone ?? existingUser.SDT;

            var updateResult = await _userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(updateResult, ModelState));
            }

            // Cập nhật vai trò của người dùng
            var currentRoles = await _userManager.GetRolesAsync(existingUser);
            var rolesToAdd = model.Roles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(model.Roles).ToList();

            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(existingUser, rolesToAdd);
            }

            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(existingUser, rolesToRemove);
            }

            return Ok(Result<object>.Success(existingUser));
        }

        [HttpPut("updateprofile/{id}")]
        public async Task<IActionResult> Put([FromForm] UpdateUserProfile model, string id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            user.SDT = model.SDT;
            user.DiaChi = model.DiaChi;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PasswordHash = model.Password;
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string tokenId)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenId);
                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    var newUser = new AppUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        //ImagePath = payload.Picture,
                    };

                    var result = await _userManager.CreateAsync(newUser);
                    if (!result.Succeeded)
                    {
                        return BadRequest(new { message = "Could not create user." });
                    }

                    await _userManager.AddToRoleAsync(newUser, "Customer");

                    user = newUser;
                }
                var token = _jwtFactory.GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.Email,
                        user.FirstName,
                        user.LastName
                    }
                });
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { message = "Invalid Google token." });
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            var tokenResponse = await _jwtFactory.GetTokenFromGoogle(code);

            var tokenId = tokenResponse.IdToken;

            return await LoginWithGoogle(tokenId);
        }
    }
}