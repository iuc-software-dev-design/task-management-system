using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.src.Notification
{
    public class NotificationService
    {
        private readonly NotificationRepo _repo;

        public NotificationService(NotificationRepo repo)
        {
            _repo = repo;
        }

        public async Task<NotificationEntity> SendEmailNotification(string message, string recipientId, int? relatedTaskId = null)
        {
            var notification = new NotificationEntity
            {
                Message = message,
                RecipientId = recipientId,
                RelatedTaskId = relatedTaskId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            return await _repo.Add(notification);
        }

        public async Task<List<NotificationEntity>> GetNotificationsForUser(string userId)
        {
            return await _repo.GetByRecipientId(userId);
        }

        public async Task<bool> MarkAsRead(int notificationId)
        {
            return await _repo.MarkAsRead(notificationId);
        }
    }
}