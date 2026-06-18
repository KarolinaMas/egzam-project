using System.Security.Claims;
using ExamProject.API.Models;
using ExamProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(TokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var userId = await _userService.AddAsync(
                request.UserName,
                request.Email,
                request.Password,
                "user"
            );
            return Created("/", new { userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.Email, request.Password);
            if (user == null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Role", user.Role ?? "user"),
            };

            var token = _tokenService.GenerateAccessToken(claims);
            return Ok(new { accessToken = token });
        }
    }
}
