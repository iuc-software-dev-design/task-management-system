using backend.src.ApplicationUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend.src.User
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authentication gerekli
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<UserDto>> GetAllUsers()
        {
            var users = _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDto> GetUser(int id)
        {
            var user = _service.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id, [FromBody] UpdateUserDto updateDto)
        {
            var user = _service.UpdateProfile(id, updateDto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler silebilir
        public IActionResult DeleteUser(int id)
        {
            var result = _service.DeleteUser(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "MANAGER")] // Sadece manager role değiştirebilir
        public IActionResult ChangeUserRole(int id, [FromBody] ChangeRoleDto roleDto)
        {
            var result = _service.ChangeUserRole(id, roleDto.Role);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("role/{role}")]
        public ActionResult<List<UserDto>> GetUsersByRole(AppUser.Role role)
        {
            var users = _service.GetUsersByRole(role);
            return Ok(users);
        }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ChangeRoleDto
    {
        public AppUser.Role Role { get; set; }
    }
}