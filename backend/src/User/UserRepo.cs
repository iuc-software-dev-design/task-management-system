using System.Collections.Generic;
using System.Linq;

namespace api.src.User
{
    public class UserRepo
    {
        private static List<UserEntity> _users = new List<UserEntity>();
        private static int _nextId = 1;

        public UserEntity Add(UserEntity user)
        {
            user.Id = _nextId++;
            _users.Add(user);
            return user;
        }

        public UserEntity GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public UserEntity GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
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