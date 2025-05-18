using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;

namespace api.src.Task
{
    public class TaskEntity : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<User> Assignees { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public UserEntity CreatedBy { get; set; }
        public enum Status
        {
            NotStarted,
            InProgress,
            Completed,
        }
    }
}