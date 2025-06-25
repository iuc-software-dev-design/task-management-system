using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Claims;

namespace backend.src.Task
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _service;

        public TaskController(TaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> ListTasks()
        {
            var tasks = await _service.ListTasks();
            return Ok(tasks);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<TaskDto>>> GetMyTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var tasks = await _service.GetMyTasks(userId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _service.GetTask(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JWT token'dan user ID'yi al
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _service.CreateTask(createDto, userId);
            if (created == null)
                return BadRequest("Task creation failed");

            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateTask(id, updateDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _service.DeleteTask(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/assign")]
        [Authorize(Roles = "MANAGER,TEAM_LEAD")]
        public async Task<IActionResult> AssignTask(int id, [FromBody] AssignTaskDto assignDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AssignTask(id, assignDto);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}