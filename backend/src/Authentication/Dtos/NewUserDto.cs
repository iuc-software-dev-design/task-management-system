using backend.Interfaces;

namespace backend.src.Authentication
{
    public class NewUserDto : IDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string UserRole { get; set; }
        public string Token { get; set; }
    }


}
