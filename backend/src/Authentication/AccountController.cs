using backend.src.ApplicationUser;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.src.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto authLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _accountService.Login(authLoginDto.Email, authLoginDto.Password);
            if (user == null)
                return Unauthorized("Invalid email or password");

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterDto authRegisterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                UserName = authRegisterDto.Email,
                Email = authRegisterDto.Email,
                Name = authRegisterDto.Name,
                UserRole = AppUser.Role.USER,
                EmailVerified = false
            };

            var created = await _accountService.Register(user, authRegisterDto.Password);
            if (created == null)
                return BadRequest("Email already exists or registration failed");

            return Ok(created);
        }

        [HttpPost("verify-email/{userId}")]
        public async Task<IActionResult> VerifyEmail(string userId)
        {
            var result = await _accountService.VerifyEmail(userId);
            if (!result)
                return NotFound("User not found");

            return Ok("Email verified successfully");
        }
    }




}