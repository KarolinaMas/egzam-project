using System.Security.Claims;
using ExamProject.Entities;
using ExamProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "userOnly")]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemService _service;

        public TaskItemController(ITaskItemService service)
        {
            _service = service;
        }

        private int GetUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskItemDto dto)
        {
            var userId = GetUserId();

            var id = await _service.CreateAsync(userId, dto);

            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var userId = GetUserId();

            var taskItem = await _service.GetAsync(userId, id);

            if (taskItem == null)
                return NotFound();

            return Ok(taskItem);
        }

        [HttpGet("pages/{page:int}/{itemsPerPage:int}")]
        public async Task<IActionResult> GetAllAsync(int page, int itemsPerPage)
        {
            var userId = GetUserId();

            var result = await _service.GetAllAsync(userId, page, itemsPerPage);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTaskItemDto dto)
        {
            var userId = GetUserId();

            var updatedTask = await _service.UpdateAsync(userId, id, dto);

            if (updatedTask == null)
                return NotFound();

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = GetUserId();

            var deleted = await _service.DeleteAsync(userId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}