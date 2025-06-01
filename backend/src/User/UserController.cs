using backend.src.ApplicationUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler tüm kullanıcıları görebilir
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("me")] // Kendi profilini görme
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var user = await _service.GetUserById(currentUserId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler başka kullanıcıları görebilir
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _service.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("me")] // Kendi profilini güncelleme
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto updateDto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var user = await _service.UpdateProfile(currentUserId, updateDto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler başka kullanıcıları güncelleyebilir
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateUserDto updateDto)
        {
            // Güvenlik kontrolü: Normal user başkasının profilini güncelleyemez
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != id && !IsManagerOrTeamLead(userRole))
            {
                return Forbid("You can only update your own profile");
            }

            var user = await _service.UpdateProfile(id, updateDto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("me")] // Kendi hesabını silme
        public async Task<IActionResult> DeleteMyAccount()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _service.DeleteUser(currentUserId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler başka kullanıcıları silebilir
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Güvenlik kontrolü: Normal user başkasını silemez
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != id && !IsManagerOrTeamLead(userRole))
            {
                return Forbid("You can only delete your own account");
            }

            var result = await _service.DeleteUser(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "MANAGER")] // Sadece manager role değiştirebilir
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] ChangeRoleDto roleDto)
        {
            var result = await _service.ChangeUserRole(id, roleDto.Role);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("role/{role}")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")] // Sadece yöneticiler role'e göre listeleyebilir
        public async Task<ActionResult<List<UserDto>>> GetUsersByRole(AppUser.Role role)
        {
            var users = await _service.GetUsersByRole(role);
            return Ok(users);
        }

        // Helper method
        private bool IsManagerOrTeamLead(string role)
        {
            return role == "MANAGER" || role == "TEAM_LEAD";
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