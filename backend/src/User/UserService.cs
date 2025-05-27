using System;
using System.Security.Cryptography;
using System.Text;

namespace api.src.User
{
    public class UserService
    {
        private readonly UserRepo _repo = new UserRepo();

        public UserDto Register(UserEntity user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            user.EmailVerified = false;
            var created = _repo.Add(user);
            return ToDto(created);
        }

        public UserDto Login(string email, string password)
        {
            var user = _repo.GetByEmail(email);
            if (user == null) return null;
            if (!VerifyPassword(password, user.PasswordHash)) return null;
            return ToDto(user);
        }

        public bool VerifyEmail(int id)
        {
            return _repo.VerifyEmail(id);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private UserDto ToDto(UserEntity user)
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