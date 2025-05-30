using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using api.src.User;

namespace api.src.Task
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _service = new TaskService();

        [HttpGet]
        public ActionResult<List<TaskEntity>> ListTasks()
        {
            return Ok(_service.ListTasks());
        }

        [HttpGet("{id}")]
        public ActionResult<TaskEntity> GetTask(int id)
        {
            var task = _service.GetTask(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public ActionResult<TaskEntity> CreateTask([FromBody] TaskEntity task)
        {
            var created = _service.CreateTask(task);
            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskEntity task)
        {
            var result = _service.UpdateTask(id, task);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var result = _service.DeleteTask(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/assign")]
        public IActionResult AssignTask(int id, [FromBody] List<UserEntity> assignees) // DÜZELTİN
        {
            var result = _service.AssignTask(id, assignees);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}