using System.Collections.Generic;
using System.Linq;

namespace api.src.Notification
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

        public List<NotificationEntity> GetByRecipientId(int recipientId)
        {
            return _notifications.Where(n => n.Recipient.Id == recipientId).ToList();
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