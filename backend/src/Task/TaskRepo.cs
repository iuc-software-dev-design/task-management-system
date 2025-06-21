using backend.Data;
using backend.src.ApplicationUser;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Task
{
    public class TaskRepo
    {
        private readonly AppDbContext _context;

        public TaskRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskEntity>> GetAll()
        {
            return await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Assignees)
                .ToListAsync();
        }

        public async Task<TaskEntity> GetById(int id)
        {
            return await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Assignees)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskEntity> Add(TaskEntity task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> Update(int id, TaskEntity updatedTask)
        {
            var task = await GetById(id);
            if (task == null) return false;

            // Direkt entity'yi güncelle (Service layer'da zaten kontrol edildi)
            _context.Entry(task).CurrentValues.SetValues(updatedTask);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var task = await GetById(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Assign(int taskId, List<AppUser> assignees)
        {
            var task = await GetById(taskId);
            if (task == null) return false;

            task.Assignees = assignees;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}