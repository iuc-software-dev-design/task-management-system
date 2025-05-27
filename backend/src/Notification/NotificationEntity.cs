using System;
using api.Interfaces;
using api.src.User;
using api.src.Task;

namespace api.src.Notification
{
    public class NotificationEntity : IEntity
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public UserEntity Recipient { get; set; }
        public TaskEntity RelatedTask { get; set; }
        public bool IsRead { get; set; } = false;
    }
}