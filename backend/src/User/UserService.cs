using System.Collections.Generic;
using System.Linq;
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
        public List<UserDto> GetAllUsers()
        {
            return _repo.GetAll().Select(u => ToDto(u)).ToList();
        }

        public UserDto GetUserById(int id)
        {
            var user = _repo.GetById(id);
            return user == null ? null : ToDto(user);
        }

        public UserDto UpdateProfile(int id, UpdateUserDto updateDto)
        {
            var user = _repo.GetById(id);
            if (user == null) return null;

            user.Name = updateDto.Name ?? user.Name;
            user.Email = updateDto.Email ?? user.Email;

            return ToDto(user);
        }

        public bool DeleteUser(int id)
        {
            return _repo.Delete(id);
        }

        public bool ChangeUserRole(int id, AppUser.Role newRole)
        {
            var user = _repo.GetById(id);
            if (user == null) return false;

            user.UserRole = newRole;
            return true;
        }

        public List<UserDto> GetUsersByRole(AppUser.Role role)
        {
            return _repo.GetByRole(role).Select(u => ToDto(u)).ToList();
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