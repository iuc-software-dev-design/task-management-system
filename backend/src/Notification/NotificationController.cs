using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace backend.src.Notification
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _service;

        public NotificationController(NotificationService service)
        {
            _service = service;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmailNotification([FromBody] SendNotificationRequest req)
        {
            var notification = await _service.SendEmailNotification(req.Message, req.RecipientId, req.TaskId);
            return Ok(notification);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<NotificationEntity>>> GetNotificationsForUser(string userId)
        {
            var notifications = await _service.GetNotificationsForUser(userId);
            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _service.MarkAsRead(id);
            if (!result) return NotFound();
            return Ok();
        }
    }

    public class SendNotificationRequest
    {
        public string Message { get; set; }
        public string RecipientId { get; set; }
        public int? TaskId { get; set; }
    }
}