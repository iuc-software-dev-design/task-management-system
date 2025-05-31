using Microsoft.AspNetCore.Identity;
using backend.src.Task;
using System.Collections.Generic;

namespace backend.src.ApplicationUser
{
    public class AppUser : IdentityUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool EmailVerified { get; set; } = false;
        public Role UserRole { get; set; } = Role.USER;
        public List<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

        public enum Role
        {
            USER,
            MANAGER,
            TEAM_LEAD,
        }
    }
}