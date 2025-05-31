using System.Collections.Generic;

namespace backend.src.Notification
{
    public class NotificationService
    {
        private readonly NotificationRepo _repo = new NotificationRepo();

        // Method signature'ı basit parametrelerle değiştirelim
        public NotificationEntity SendEmailNotification(string message, string recipientId, int? relatedTaskId = null)
        {
            var notification = new NotificationEntity
            {
                Message = message,
                RecipientId = recipientId,
                RelatedTaskId = relatedTaskId,
                IsRead = false
            };
            return _repo.Add(notification);
        }

        public List<NotificationEntity> GetNotificationsForUser(string userId)
        {
            return _repo.GetByRecipientId(userId);
        }

        public bool MarkAsRead(int notificationId)
        {
            return _repo.MarkAsRead(notificationId);
        }
    }
}