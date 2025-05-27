using api.src.User;
using api.src.Task;
using System.Collections.Generic;

namespace api.src.Notification
{
    public class NotificationService
    {
        private readonly NotificationRepo _repo = new NotificationRepo();

        public NotificationEntity SendEmailNotification(string message, UserEntity recipient, TaskEntity relatedTask)
        {
            var notification = new NotificationEntity
            {
                Message = message,
                Recipient = recipient,
                RelatedTask = relatedTask,
                IsRead = false
            };
            // Burada gerçek bir e-posta gönderme işlemi yapılabilir (şimdilik sadece kaydediyoruz)
            return _repo.Add(notification);
        }

        public List<NotificationEntity> GetNotificationsForUser(int userId)
        {
            return _repo.GetByRecipientId(userId);
        }

        public bool MarkAsRead(int notificationId)
        {
            return _repo.MarkAsRead(notificationId);
        }
    }
}