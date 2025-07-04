using backend.Interfaces;

namespace backend.src.User
{
    public class UserDto : IDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string UserRole { get; set; }
    }
}