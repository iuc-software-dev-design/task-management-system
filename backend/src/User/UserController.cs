using Microsoft.AspNetCore.Mvc;

namespace api.src.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service = new UserService();

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest user)
        {
            var entity = new UserEntity
            {
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.Password,
                UserRole = UserEntity.Role.USER
            };
            var created = _service.Register(entity);
            return Ok(created);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _service.Login(req.Email, req.Password);
            if (user == null) return Unauthorized();
            return Ok(user);
        }

        [HttpPost("{id}/verify")]
        public IActionResult VerifyEmail(int id)
        {
            var result = _service.VerifyEmail(id);
            if (!result) return NotFound();
            return Ok();
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}