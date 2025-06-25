using backend.src.ApplicationUser;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Login([FromBody] AuthLoginDto authLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _accountService.Login(authLoginDto.Email, authLoginDto.Password);
            if (user == null)
                return Unauthorized("Invalid email or password");

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterDto authRegisterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                UserName = authRegisterDto.Email,
                Email = authRegisterDto.Email,
                Name = authRegisterDto.Name,
                UserRole = AppUser.Role.USER,
                EmailVerified = false
            };

            var created = await _accountService.Register(user, authRegisterDto.Password);
            if (created == null)
                return BadRequest("Email already exists or registration failed");

            return Ok(created);
        }

        [HttpPost("send-verification/{userId}")]
        public async Task<IActionResult> SendVerificationEmail(string userId)
        {
            try
            {
                var result = await _accountService.SendVerificationEmail(userId);
                if (!result)
                    return NotFound("User not found or email send failed");

                return Ok("Verification email sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send verification email: {ex.Message}");
            }
        }

        [HttpGet("verify-email/{userId}")]
        public async Task<IActionResult> VerifyEmail(string userId)
        {
            try
            {
                var result = await _accountService.VerifyEmail(userId);
                if (!result)
                {
                    var errorHtml = @"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Verification Failed</title>
                        <style>
                            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); margin: 0; padding: 0; min-height: 100vh; display: flex; align-items: center; justify-content: center; }
                            .container { background: white; border-radius: 15px; padding: 40px; box-shadow: 0 10px 30px rgba(0,0,0,0.2); text-align: center; max-width: 500px; }
                            .icon { font-size: 80px; margin-bottom: 20px; }
                            .error { color: #dc3545; }
                            h1 { color: #333; margin-bottom: 20px; }
                            p { color: #666; font-size: 16px; line-height: 1.6; margin-bottom: 30px; }
                            .btn { background: #007bff; color: white; padding: 12px 30px; border: none; border-radius: 25px; font-size: 16px; cursor: pointer; text-decoration: none; display: inline-block; transition: all 0.3s; }
                            .btn:hover { background: #0056b3; transform: translateY(-2px); }
                            .footer { margin-top: 30px; font-size: 14px; color: #999; }
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='icon error'>❌</div>
                            <h1>Verification Failed</h1>
                            <p>Sorry, we couldn't verify your email address. The verification link may have expired or the user account was not found.</p>
                            <a href='http://localhost:3000/login' class='btn'>← Back to Login</a>
                            <div class='footer'>
                                <p>Need help? Contact our support team.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
                    return Content(errorHtml, "text/html");
                }

                var successHtml = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Email Verified Successfully</title>
                    <style>
                        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); margin: 0; padding: 0; min-height: 100vh; display: flex; align-items: center; justify-content: center; }
                        .container { background: white; border-radius: 15px; padding: 40px; box-shadow: 0 10px 30px rgba(0,0,0,0.2); text-align: center; max-width: 500px; animation: slideIn 0.5s ease-out; }
                        @keyframes slideIn { from { opacity: 0; transform: translateY(30px); } to { opacity: 1; transform: translateY(0); } }
                        .icon { font-size: 80px; margin-bottom: 20px; animation: bounce 2s infinite; }
                        @keyframes bounce { 0%, 20%, 50%, 80%, 100% { transform: translateY(0); } 40% { transform: translateY(-10px); } 60% { transform: translateY(-5px); } }
                        .success { color: #28a745; }
                        h1 { color: #333; margin-bottom: 20px; }
                        p { color: #666; font-size: 16px; line-height: 1.6; margin-bottom: 30px; }
                        .btn-group { display: flex; gap: 15px; justify-content: center; flex-wrap: wrap; }
                        .btn { background: #007bff; color: white; padding: 12px 30px; border: none; border-radius: 25px; font-size: 16px; cursor: pointer; text-decoration: none; display: inline-block; transition: all 0.3s; }
                        .btn:hover { background: #0056b3; transform: translateY(-2px); }
                        .btn-success { background: #28a745; }
                        .btn-success:hover { background: #218838; }
                        .footer { margin-top: 30px; font-size: 14px; color: #999; }
                        .checkmark { display: inline-block; width: 22px; height: 22px; border-radius: 50%; background: #28a745; color: white; line-height: 22px; margin-right: 8px; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='icon success'>🎉</div>
                        <h1><span class='checkmark'>✓</span>Email Verified Successfully!</h1>
                        <p>Congratulations! Your email address has been verified successfully. You can now access all features of the Task Management System.</p>
                        
                        <div class='btn-group'>
                            <a href='http://localhost:3000/login' class='btn'>🔐 Login Now</a>
                            <a href='http://localhost:3000/home' class='btn btn-success'>📊 Go to Dashboard</a>
                        </div>
                        
                        <div class='footer'>
                            <p><strong>What's next?</strong></p>
                            <p>✅ Login to your account<br>
                               ✅ Complete your profile<br>
                               ✅ Start managing tasks<br>
                               ✅ Collaborate with your team</p>
                            <br>
                            <p>Thank you for using Task Management System!</p>
                        </div>
                    </div>
                    
                    <script>
                        // Auto close after 10 seconds (optional)
                        setTimeout(function() {
                            if (confirm('Email verified successfully! Would you like to be redirected to the login page?')) {
                                window.location.href = 'http://localhost:3000/login';
                            }
                        }, 5000);
                    </script>
                </body>
                </html>";

                return Content(successHtml, "text/html");
            }
            catch (Exception ex)
            {
                var errorHtml = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Email Verification Error</title>
                    <style>
                        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); margin: 0; padding: 0; min-height: 100vh; display: flex; align-items: center; justify-content: center; }
                        .container { background: white; border-radius: 15px; padding: 40px; box-shadow: 0 10px 30px rgba(0,0,0,0.2); text-align: center; max-width: 500px; }
                        .icon { font-size: 80px; margin-bottom: 20px; }
                        .error { color: #dc3545; }
                        h1 { color: #333; margin-bottom: 20px; }
                        p { color: #666; font-size: 16px; line-height: 1.6; margin-bottom: 30px; }
                        .btn { background: #007bff; color: white; padding: 12px 30px; border: none; border-radius: 25px; font-size: 16px; cursor: pointer; text-decoration: none; display: inline-block; transition: all 0.3s; }
                        .btn:hover { background: #0056b3; transform: translateY(-2px); }
                        .footer { margin-top: 30px; font-size: 14px; color: #999; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='icon error'>⚠️</div>
                        <h1>Something Went Wrong</h1>
                        <p>We encountered an error while verifying your email address. Please try again or contact our support team.</p>
                        <p><strong>Error:</strong> " + ex.Message + @"</p>
                        <a href='http://localhost:3000/login' class='btn'>← Back to Login</a>
                        <div class='footer'>
                            <p>Need assistance? Contact our support team.</p>
                        </div>
                    </div>
                </body>
                </html>";

                return Content(errorHtml, "text/html");
            }
        }
    }




}