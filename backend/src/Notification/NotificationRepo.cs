using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Notification
{
    public class NotificationRepo
    {
        private readonly AppDbContext _context;

        public NotificationRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationEntity> Add(NotificationEntity notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<List<NotificationEntity>> GetByRecipientId(string recipientId)
        {
            return await _context.Notifications
                .Include(n => n.Recipient)
                .Include(n => n.RelatedTask)
                .Where(n => n.RecipientId == recipientId)
                .ToListAsync();
        }

        public async Task<NotificationEntity> GetById(int id)
        {
            return await _context.Notifications
                .Include(n => n.Recipient)
                .Include(n => n.RelatedTask)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<bool> MarkAsRead(int id)
        {
            var notification = await GetById(id);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}