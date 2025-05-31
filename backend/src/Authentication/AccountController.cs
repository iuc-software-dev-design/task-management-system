using backend.src.ApplicationUser;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Login([FromBody] AuthLoginDto authLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _accountService.Login(authLoginDto.Email, authLoginDto.Password);
            if (user == null)
                return Unauthorized("Invalid email or password");

            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthRegisterDto authRegisterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                Name = authRegisterDto.Name,
                Email = authRegisterDto.Email,
                PasswordHash = authRegisterDto.Password,
                UserRole = AppUser.Role.USER
            };

            var created = _accountService.Register(user);
            if (created == null)
                return BadRequest("Email already exists");

            return Ok(created);
        }

        [HttpPost("{id}/verify")]
        public IActionResult VerifyEmail(int id)
        {
            var result = _accountService.VerifyEmail(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}