using System.Collections.Generic;
using System.Linq;
using backend.src.ApplicationUser;

namespace backend.src.User
{
    public class UserRepo
    {
        private static List<AppUser> _users = new List<AppUser>();
        private static int _nextId = 1;

        public AppUser Add(AppUser user)
        {
            user.Id = _nextId++;
            _users.Add(user);
            return user;
        }

        public AppUser GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public AppUser GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public List<AppUser> GetAll()
        {
            return _users.ToList();
        }

        public List<AppUser> GetByRole(AppUser.Role role)
        {
            return _users.Where(u => u.UserRole == role).ToList();
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null) return false;
            _users.Remove(user);
            return true;
        }

        public bool VerifyEmail(int id)
        {
            var user = GetById(id);
            if (user == null) return false;
            user.EmailVerified = true;
            return true;
        }
    }
}