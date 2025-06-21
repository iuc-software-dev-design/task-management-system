using System;
using System.Collections.Generic;

namespace backend.src.Task
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; } // User name
        public string CreatedById { get; set; } // User ID
        public TaskEntity.Status TaskStatus { get; set; }
        public List<TaskUserDto> Assignees { get; set; } = new List<TaskUserDto>();
    }

    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> AssigneeIds { get; set; } = new List<string>(); // Sadece user ID'leri
    }

    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TaskEntity.Status? TaskStatus { get; set; }
    }

    public class AssignTaskDto
    {
        public List<string> AssigneeIds { get; set; } = new List<string>(); // Sadece user ID'leri
    }

    public class TaskUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}