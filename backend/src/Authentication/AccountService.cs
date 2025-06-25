using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using backend.src.ApplicationUser;
using backend.Interfaces;
using backend.src.User;
using backend.src.Services;

namespace backend.src.Authentication
{
    public class AccountService
    {
        private readonly UserRepo _userRepo;
        private readonly ITokenService _tokenService;
        private readonly EmailService _emailService;


        public AccountService(UserRepo userRepo, ITokenService tokenService, EmailService emailService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _emailService = emailService;
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

                // ✅ Welcome email gönder
                await SendWelcomeEmail(created);

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

        // ✅ Welcome email metodu
        private async System.Threading.Tasks.Task SendWelcomeEmail(AppUser user)
        {
            try
            {
                var subject = "Welcome to Task Management System! 🎉";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #007bff; text-align: center;'>Welcome to Task Management System!</h2>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <h3>Hello {user.Name}! 👋</h3>
                            <p>Thank you for joining our Task Management System. Your account has been successfully created!</p>
                            
                            <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                                <h4>Account Details:</h4>
                                <ul>
                                    <li><strong>Name:</strong> {user.Name}</li>
                                    <li><strong>Email:</strong> {user.Email}</li>
                                    <li><strong>Role:</strong> {user.UserRole}</li>
                                    <li><strong>Registration Date:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                                </ul>
                            </div>
                            
                            <div style='background-color: #e3f2fd; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                                <h4>🚀 What's Next?</h4>
                                <ul>
                                    <li>Login to your account with your credentials</li>
                                    <li>Complete your profile information</li>
                                    <li>Start managing your tasks efficiently</li>
                                    <li>Collaborate with your team members</li>
                                </ul>
                            </div>
                            
                            <div style='text-align: center; margin: 20px 0;'>
                                <a href='http://localhost:3000/login' 
                                   style='background-color: #007bff; color: white; padding: 12px 25px; 
                                          text-decoration: none; border-radius: 5px; display: inline-block;'>
                                    🔐 Login to Your Account
                                </a>
                            </div>
                            
                            <hr style='margin: 20px 0; border: 1px solid #dee2e6;'>
                            
                            <p style='font-size: 14px; color: #6c757d; text-align: center;'>
                                Need help? Contact our support team or visit our help center.<br>
                                This email was sent to {user.Email}
                            </p>
                        </div>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
                Console.WriteLine($"Welcome email sent to {user.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                // Email hatası register işlemini engellemez
            }
        }

        // ✅ Email verification metodu
        public async Task<bool> SendVerificationEmail(string userId)
        {
            try
            {
                var user = await _userRepo.GetById(userId);
                if (user == null) return false;

                var subject = "Verify Your Email Address 📧";
                var verificationLink = $"http://localhost:5132/api/Account/verify-email/{userId}";

                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745; text-align: center;'>Verify Your Email Address</h2>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px;'>
                            <p>Hello {user.Name},</p>
                            <p>Please click the button below to verify your email address:</p>
                            
                            <div style='text-align: center; margin: 25px 0;'>
                                <a href='{verificationLink}' 
                                   style='background-color: #28a745; color: white; padding: 12px 25px; 
                                          text-decoration: none; border-radius: 5px; display: inline-block;'>
                                    ✅ Verify Email Address
                                </a>
                            </div>
                            
                            <p style='font-size: 14px; color: #6c757d;'>
                                If the button doesn't work, copy and paste this link into your browser:<br>
                                <a href='{verificationLink}'>{verificationLink}</a>
                            </p>
                        </div>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send verification email: {ex.Message}");
                return false;
            }
        }

        // ✅ Email verification metodu
        public async Task<bool> VerifyEmail(string userId)
        {
            try
            {
                var user = await _userRepo.GetById(userId);
                if (user == null) return false;

                user.EmailVerified = true;
                await _userRepo.Update(user);

                Console.WriteLine($"Email verified for user: {user.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email verification error: {ex.Message}");
                return false;
            }
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
    }
}