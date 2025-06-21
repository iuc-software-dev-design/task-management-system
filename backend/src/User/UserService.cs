using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.src.ApplicationUser;

namespace backend.src.User
{
    public class UserService
    {
        private readonly UserRepo _repo;

        public UserService(UserRepo repo)
        {
            _repo = repo;
        }

        // User Management Operations
        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _repo.GetAll();
            return users.Select(ToDto).ToList();
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var user = await _repo.GetById(id);
            return user == null ? null : ToDto(user);
        }

        public async Task<UserDto> UpdateProfile(string id, UpdateUserDto updateDto)
        {
            var user = await _repo.GetById(id);
            if (user == null) return null;

            // Sadece gönderilen (null olmayan) alanları güncelle
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                user.Name = updateDto.Name;
                
            if (!string.IsNullOrWhiteSpace(updateDto.Email))
                user.Email = updateDto.Email;

            var result = await _repo.UpdateAsync(user);
            if (!result) return null;
            
            return ToDto(user);
        }

        public async Task<bool> DeleteUser(string id)
        {
            return await _repo.Delete(id);
        }

        public async Task<bool> ChangeUserRole(string id, AppUser.Role newRole)
        {
            var user = await _repo.GetById(id);
            if (user == null) return false;

            user.UserRole = newRole;
            await _repo.UpdateAsync(user);
            return true;
        }

        public async Task<List<UserDto>> GetUsersByRole(AppUser.Role role)
        {
            var users = await _repo.GetByRole(role);
            return users.Select(ToDto).ToList();
        }

        private UserDto ToDto(AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                UserRole = user.UserRole.ToString()
            };
        }
    }
}