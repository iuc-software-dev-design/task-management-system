using System;
using api.Interfaces;

namespace api.src.User
{
    public class UserEntity : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailVerified { get; set; } = false;
        public enum Role
        {
            USER,
            MANAGER,
            TEAM_LEAD,
        }
        public Role UserRole { get; set; } = Role.USER;
    }
}