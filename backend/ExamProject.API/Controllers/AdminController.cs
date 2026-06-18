using ExamProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamProject.API.Controllers
{
    [ApiController]
    [Route("api/admin/tasks")]
    [Authorize(Policy = "adminOnly")]
    public class AdminTaskController : ControllerBase
    {
        private readonly ITaskItemService _service;

        public AdminTaskController(ITaskItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAdminAsync();
            return Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(result);
        }

    }
}