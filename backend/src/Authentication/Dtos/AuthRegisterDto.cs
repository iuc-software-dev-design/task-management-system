using backend.Interfaces;

namespace backend.src.Authentication
{
    public class AuthRegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
