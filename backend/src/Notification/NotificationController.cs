using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace backend.src.Notification
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _service = new NotificationService();

        [HttpPost("send")]
        public IActionResult SendEmailNotification([FromBody] SendNotificationRequest req)
        {
            var notification = _service.SendEmailNotification(req.Message, req.RecipientId, req.TaskId);
            return Ok(notification);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<List<NotificationEntity>> GetNotificationsForUser(string userId)
        {
            var notifications = _service.GetNotificationsForUser(userId);
            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            var result = _service.MarkAsRead(id);
            if (!result) return NotFound();
            return Ok();
        }
    }

    public class SendNotificationRequest
    {
        public string Message { get; set; }
        public string RecipientId { get; set; } // string olarak değiştir
        public int? TaskId { get; set; } // nullable yap
    }
}