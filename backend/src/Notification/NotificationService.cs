using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Asenkron işlemler için Task sınıfı
using backend.src.Services;
using backend.src.Task; // Task namespace'i için

namespace backend.src.Notification
{
    public class NotificationService
    {
        private readonly NotificationRepo _repo;
        private readonly EmailService _emailService;

        public NotificationService(NotificationRepo repo, EmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        public async System.Threading.Tasks.Task<NotificationEntity> SendEmailNotification(string message, string recipientId, int? relatedTaskId = null)
        {
            var notification = new NotificationEntity
            {
                Message = message,
                RecipientId = recipientId,
                RelatedTaskId = relatedTaskId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var savedNotification = await _repo.Add(notification);

            // Email gönderme işlemi eklendi
            await _emailService.SendEmailAsync(
                notification.Recipient?.Email, // Alıcının email adresi
                "Task Management Notification",
                $"<h3>New Notification</h3><p>{message}</p>"
            );

            return savedNotification;
        }

        public async System.Threading.Tasks.Task<List<NotificationEntity>> GetNotificationsForUser(string userId)
        {
            return await _repo.GetByRecipientId(userId);
        }

        public async System.Threading.Tasks.Task<bool> MarkAsRead(int notificationId)
        {
            return await _repo.MarkAsRead(notificationId);
        }

        public async System.Threading.Tasks.Task SendCustomEmail(string userEmail, string subject, string message)
        {
            await _emailService.SendEmailAsync(userEmail, subject, message);
        }
    }
}