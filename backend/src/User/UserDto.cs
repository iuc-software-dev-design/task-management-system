using api.Interfaces;

namespace api.src.User
{
    public class UserDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string UserRole { get; set; }
    }
}