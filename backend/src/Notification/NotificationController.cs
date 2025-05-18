using Microsoft.AspNetCore.Mvc;
using api.src.User;
using api.src.Task;
using System.Collections.Generic;

namespace api.src.Notification
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _service = new NotificationService();

        [HttpPost("send")]
        public IActionResult SendEmailNotification([FromBody] SendNotificationRequest req)
        {
            // Normalde burada User ve Task servislerinden ilgili entity'ler çekilmeli
            var recipient = new UserEntity { Id = req.RecipientId, Name = req.RecipientName, Email = req.RecipientEmail };
            var relatedTask = new TaskEntity { Id = req.TaskId, Title = req.TaskTitle };
            var notification = _service.SendEmailNotification(req.Message, recipient, relatedTask);
            return Ok(notification);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<List<NotificationEntity>> GetNotificationsForUser(int userId)
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
        public int RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
    }
}