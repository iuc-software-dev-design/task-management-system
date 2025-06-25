using backend.Data;
using backend.src.ApplicationUser;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend.src.User
{
    public class UserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> Add(AppUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<AppUser> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser> GetById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<AppUser>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<AppUser>> GetByRole(AppUser.Role role)
        {
            return await _context.Users.Where(u => u.UserRole == role).ToListAsync();
        }
        public async Task<bool> UpdateAsync(AppUser user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> Update(AppUser user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update user error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> Delete(string id)
        {
            var user = await GetById(id);
            if (user == null) return false;

            // Önce bu user'ın oluşturduğu task'leri bul ve sil
            var userTasks = await _context.Tasks
                .Where(t => t.CreatedBy.Id == id)
                .ToListAsync();

            if (userTasks.Any())
            {
                foreach (var task in userTasks)
                {
                    task.CreatedBy = null; // Task'ı silmek yerine creator'ı null yap
                }
            }

            // User'ın atanmış olduğu task'lerden de kaldır
            var assignedTasks = await _context.Tasks
                .Where(t => t.Assignees.Any(a => a.Id == id))
                .Include(t => t.Assignees)
                .ToListAsync();

            foreach (var task in assignedTasks)
            {
                var assigneeToRemove = task.Assignees.FirstOrDefault(a => a.Id == id);
                if (assigneeToRemove != null)
                {
                    task.Assignees.Remove(assigneeToRemove);
                }
            }

            // Son olarak user'ı sil
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyEmail(string id)
        {
            var user = await GetById(id);
            if (user == null) return false;

            user.EmailVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}