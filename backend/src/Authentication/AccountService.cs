using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using backend.src.ApplicationUser;
using backend.Interfaces;
using backend.src.User;

namespace backend.src.Authentication
{
    public class AccountService
    {
        private readonly UserRepo _userRepo;
        private readonly ITokenService _tokenService;

        public AccountService(UserRepo userRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }

        public async Task<NewUserDto> Register(AppUser user, string password)
        {
            try
            {
                // Email zaten kayıtlı mı kontrol et
                var existingUser = await _userRepo.GetByEmail(user.Email);
                if (existingUser != null) return null;

                // Password hash
                user.PasswordHash = HashPassword(password);
                user.EmailVerified = false;

                // User'ı kaydet
                var created = await _userRepo.Add(user);

                // DTO'ya çevir
                var dto = ToDto(created);
                dto.Token = _tokenService.CreateToken(created);

                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
                return null;
            }
        }

        public async Task<NewUserDto> Login(string email, string password)
        {
            try
            {
                var user = await _userRepo.GetByEmail(email);
                if (user == null) return null;

                if (!VerifyPassword(password, user.PasswordHash)) return null;

                var dto = ToDto(user);
                dto.Token = _tokenService.CreateToken(user);
                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> VerifyEmail(string userId) // int'den string'e değişti
        {
            return await _userRepo.VerifyEmail(userId);
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

        private NewUserDto ToDto(AppUser user)
        {
            return new NewUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                UserRole = user.UserRole.ToString(),
                Token = "" // Controller'da set edilecek
            };
        }
    }
}