using System.Collections.Generic;
using System.Linq;

namespace backend.src.Notification
{
    public class NotificationRepo
    {
        private static List<NotificationEntity> _notifications = new List<NotificationEntity>();
        private static int _nextId = 1;

        public NotificationEntity Add(NotificationEntity notification)
        {
            notification.Id = _nextId++;
            _notifications.Add(notification);
            return notification;
        }

        // RecipientId string olarak değiştir
        public List<NotificationEntity> GetByRecipientId(string recipientId)
        {
            return _notifications.Where(n => n.RecipientId == recipientId).ToList();
        }

        public NotificationEntity GetById(int id)
        {
            return _notifications.FirstOrDefault(n => n.Id == id);
        }

        public bool MarkAsRead(int id)
        {
            var notification = GetById(id);
            if (notification == null) return false;
            notification.IsRead = true;
            return true;
        }
    }
}