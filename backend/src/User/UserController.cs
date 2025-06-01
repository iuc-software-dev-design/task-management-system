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
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id) // int'den string'e değişti
        {
            var user = await _service.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateUserDto updateDto) // int'den string'e değişti
        {
            var user = await _service.UpdateProfile(id, updateDto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler silebilir
        public async Task<IActionResult> DeleteUser(string id) // int'den string'e değişti
        {
            var result = await _service.DeleteUser(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "MANAGER")] // Sadece manager role değiştirebilir
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] ChangeRoleDto roleDto) // int'den string'e değişti
        {
            var result = await _service.ChangeUserRole(id, roleDto.Role);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<List<UserDto>>> GetUsersByRole(AppUser.Role role)
        {
            var users = await _service.GetUsersByRole(role);
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