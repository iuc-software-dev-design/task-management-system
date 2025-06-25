using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.src.ApplicationUser;
using backend.src.User;
using backend.src.Services;

namespace backend.src.Task
{
    public class TaskService
    {
        private readonly TaskRepo _repo;
        private readonly UserRepo _userRepo;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public TaskService(TaskRepo repo, UserRepo userRepo, AppDbContext context, EmailService emailService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<TaskDto>> ListTasks()
        {
            var tasks = await _repo.GetAll();
            return tasks.Select(ToDto).ToList();
        }

        // Kullanıcının sadece kendi tasklarını getir
        public async Task<List<TaskDto>> GetMyTasks(string userId)
        {
            var tasks = await _repo.GetAll();
            var myTasks = tasks.Where(t => t.Assignees.Any(a => a.Id == userId)).ToList();
            return myTasks.Select(ToDto).ToList();
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

            // ✅ Assignee'lere email notification gönder
            await SendTaskAssignmentEmails(created);

            return ToDto(created);
        }

        public async Task<bool> UpdateTask(int id, UpdateTaskDto updateDto)
        {
            var task = await _repo.GetById(id);
            if (task == null) return false;

            var oldStatus = task.TaskStatus;

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

            // ✅ Status değişikliği varsa email gönder
            if (updateDto.TaskStatus.HasValue && oldStatus != updateDto.TaskStatus.Value)
            {
                await SendTaskStatusUpdateEmails(task, oldStatus, updateDto.TaskStatus.Value);
            }

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

        // ✅ Task Assignment Email Gönderimi
        private async System.Threading.Tasks.Task SendTaskAssignmentEmails(TaskEntity task)
        {
            try
            {
                foreach (var assignee in task.Assignees)
                {
                    var subject = "🎯 New Task Assigned to You";
                    var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #007bff; text-align: center;'>New Task Assigned</h2>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                                <h3 style='color: #007bff;'>Hello {assignee.Name}! 👋</h3>
                                <p>You have been assigned a new task in the Task Management System.</p>
                                
                                <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #007bff;'>
                                    <h4 style='margin: 0 0 10px 0; color: #007bff;'>📋 Task Details:</h4>
                                    <p><strong>Title:</strong> {task.Title}</p>
                                    <p><strong>Description:</strong> {task.Description}</p>
                                    <p><strong>Start Date:</strong> {task.StartDate:dd/MM/yyyy}</p>
                                    <p><strong>End Date:</strong> {task.EndDate:dd/MM/yyyy}</p>
                                    <p><strong>Status:</strong> <span style='background-color: #6c757d; color: white; padding: 2px 6px; border-radius: 3px;'>{task.TaskStatus}</span></p>
                                    <p><strong>Created By:</strong> {task.CreatedBy?.Name}</p>
                                </div>
                                
                                <div style='background-color: #e3f2fd; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                                    <h4>📝 Next Steps:</h4>
                                    <ul>
                                        <li>Login to your account to view full task details</li>
                                        <li>Update task status as you progress</li>
                                        <li>Communicate with team members about the task</li>
                                        <li>Mark as completed when finished</li>
                                    </ul>
                                </div>
                                
                                <div style='text-align: center; margin: 20px 0;'>
                                    <a href='http://localhost:3000/home' 
                                       style='background-color: #007bff; color: white; padding: 12px 25px; 
                                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        🔗 View Task Details
                                    </a>
                                </div>
                                
                                <hr style='margin: 20px 0; border: 1px solid #dee2e6;'>
                                
                                <p style='font-size: 14px; color: #6c757d; text-align: center;'>
                                    This email was sent to {assignee.Email}<br>
                                    Task Management System - Automated Notification
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                    await _emailService.SendEmailAsync(assignee.Email, subject, body);
                    Console.WriteLine($"Task assignment email sent to {assignee.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send task assignment emails: {ex.Message}");
            }
        }

        // ✅ Task Status Update Email Gönderimi
        private async System.Threading.Tasks.Task SendTaskStatusUpdateEmails(TaskEntity task, TaskEntity.Status oldStatus, TaskEntity.Status newStatus)
        {
            try
            {
                // Creator'a bildirim gönder
                if (task.CreatedBy != null)
                {
                    var creatorSubject = "📊 Task Status Updated";
                    var creatorBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #28a745; text-align: center;'>Task Status Updated</h2>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                                <h3 style='color: #28a745;'>Hello {task.CreatedBy.Name}! 👋</h3>
                                <p>A task you created has been updated by one of the assignees.</p>
                                
                                <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #28a745;'>
                                    <h4 style='margin: 0 0 10px 0; color: #28a745;'>📋 Task Details:</h4>
                                    <p><strong>Title:</strong> {task.Title}</p>
                                    <p><strong>Description:</strong> {task.Description}</p>
                                    <p><strong>Status Changed:</strong> 
                                        <span style='background-color: #dc3545; color: white; padding: 2px 6px; border-radius: 3px;'>{oldStatus}</span>
                                        ➜
                                        <span style='background-color: #28a745; color: white; padding: 2px 6px; border-radius: 3px;'>{newStatus}</span>
                                    </p>
                                </div>
                                
                                <div style='text-align: center; margin: 20px 0;'>
                                    <a href='http://localhost:3000/home' 
                                       style='background-color: #28a745; color: white; padding: 12px 25px; 
                                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        📊 View All Tasks
                                    </a>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";

                    await _emailService.SendEmailAsync(task.CreatedBy.Email, creatorSubject, creatorBody);
                }

                // Diğer assignee'lere de bildirim gönder
                foreach (var assignee in task.Assignees)
                {
                    var assigneeSubject = "🔄 Task Status Updated";
                    var assigneeBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #17a2b8; text-align: center;'>Task Status Updated</h2>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                                <h3 style='color: #17a2b8;'>Hello {assignee.Name}! 👋</h3>
                                <p>The status of one of your assigned tasks has been updated.</p>
                                
                                <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #17a2b8;'>
                                    <h4 style='margin: 0 0 10px 0; color: #17a2b8;'>📋 Task Details:</h4>
                                    <p><strong>Title:</strong> {task.Title}</p>
                                    <p><strong>New Status:</strong> 
                                        <span style='background-color: #17a2b8; color: white; padding: 2px 6px; border-radius: 3px;'>{newStatus}</span>
                                    </p>
                                </div>
                                
                                <div style='text-align: center; margin: 20px 0;'>
                                    <a href='http://localhost:3000/home' 
                                       style='background-color: #17a2b8; color: white; padding: 12px 25px; 
                                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        🔗 View Task Details
                                    </a>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";

                    await _emailService.SendEmailAsync(assignee.Email, assigneeSubject, assigneeBody);
                }

                Console.WriteLine($"Task status update emails sent for task: {task.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send task status update emails: {ex.Message}");
            }
        }
    }
}