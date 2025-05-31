using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Interfaces;
using backend.src.ApplicationUser;

namespace backend.src.Task
{
    public class TaskEntity : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<AppUser> Assignees { get; set; } = new List<AppUser>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AppUser CreatedBy { get; set; }
        public Status TaskStatus { get; set; } = Status.NotStarted;
        public enum Status
        {
            NotStarted,
            InProgress,
            Completed,
        }
    }
}