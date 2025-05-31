using System;
using backend.Interfaces;
using backend.src.ApplicationUser;
using backend.src.Task;

namespace backend.src.Notification
{
    public class NotificationEntity : IEntity
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string RecipientId { get; set; }
        public AppUser Recipient { get; set; }
        public TaskEntity? RelatedTask { get; set; }
        public int? RelatedTaskId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}