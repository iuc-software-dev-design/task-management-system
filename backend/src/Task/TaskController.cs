using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using backend.src.ApplicationUser;

namespace backend.src.Task
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _service;

        public TaskController(TaskService service) // Constructor injection
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskEntity>>> ListTasks()
        {
            var tasks = await _service.ListTasks();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskEntity>> GetTask(int id)
        {
            var task = await _service.GetTask(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskEntity>> CreateTask([FromBody] TaskEntity task)
        {
            var created = await _service.CreateTask(task);
            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskEntity task)
        {
            var result = await _service.UpdateTask(id, task);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _service.DeleteTask(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignTask(int id, [FromBody] List<AppUser> assignees)
        {
            var result = await _service.AssignTask(id, assignees);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}