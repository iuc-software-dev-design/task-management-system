using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.src.ApplicationUser;
using backend.src.User;

namespace backend.src.Task
{
    public class TaskService
    {
        private readonly TaskRepo _repo;
        private readonly UserRepo _userRepo;
        private readonly AppDbContext _context;

        public TaskService(TaskRepo repo, UserRepo userRepo, AppDbContext context)
        {
            _repo = repo;
            _userRepo = userRepo;
            _context = context;
        }

        public async Task<List<TaskDto>> ListTasks()
        {
            var tasks = await _repo.GetAll();
            return tasks.Select(ToDto).ToList();
        }

        public async Task<TaskDto> GetTask(int id)
        {
            var task = await _repo.GetById(id);
            return task == null ? null : ToDto(task);
        }

        public async Task<TaskDto> CreateTask(CreateTaskDto createDto, string createdById)
        {
            // Assignees'leri bul
            var assignees = new List<AppUser>();
            if (createDto.AssigneeIds?.Any() == true)
            {
                foreach (var assigneeId in createDto.AssigneeIds)
                {
                    var user = await _userRepo.GetById(assigneeId);
                    if (user != null) assignees.Add(user);
                }
            }

            var createdBy = await _userRepo.GetById(createdById);
            if (createdBy == null) return null;

            var task = new TaskEntity
            {
                Title = createDto.Title,
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                CreatedBy = createdBy,
                Assignees = assignees,
                TaskStatus = TaskEntity.Status.NotStarted
            };

            var created = await _repo.Add(task);
            return ToDto(created);
        }

        public async Task<bool> UpdateTask(int id, UpdateTaskDto updateDto)
        {
            var task = await _repo.GetById(id);
            if (task == null) return false;

            // Null olmayan alanları güncelle
            if (!string.IsNullOrWhiteSpace(updateDto.Title))
                task.Title = updateDto.Title;
                
            if (!string.IsNullOrWhiteSpace(updateDto.Description))
                task.Description = updateDto.Description;
                
            if (updateDto.StartDate.HasValue)
                task.StartDate = updateDto.StartDate.Value;
                
            if (updateDto.EndDate.HasValue)
                task.EndDate = updateDto.EndDate.Value;
                
            if (updateDto.TaskStatus.HasValue)
                task.TaskStatus = updateDto.TaskStatus.Value;

            // Direkt context üzerinden save et
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTask(int id)
        {
            return await _repo.Delete(id);
        }

        public async Task<bool> AssignTask(int taskId, AssignTaskDto assignDto)
        {
            var assignees = new List<AppUser>();
            foreach (var assigneeId in assignDto.AssigneeIds)
            {
                var user = await _userRepo.GetById(assigneeId);
                if (user != null) assignees.Add(user);
            }

            return await _repo.Assign(taskId, assignees);
        }

        private TaskDto ToDto(TaskEntity task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                CreatedBy = task.CreatedBy?.Name,
                CreatedById = task.CreatedBy?.Id,
                TaskStatus = task.TaskStatus,
                Assignees = task.Assignees?.Select(a => new TaskUserDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Email = a.Email
                }).ToList() ?? new List<TaskUserDto>()
            };
        }
    }
}