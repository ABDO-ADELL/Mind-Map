using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mind_Map.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Mind_Map.Application.Users.Commands.LoginUser;
    using Mind_Map.Models;

    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(new { UserId = userId });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var token = await _mediator.Send(command);

            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            return Ok(new { Token = token });
        }
        [ApiController]
        [Route("api/auth")]
        public class AuthController : ControllerBase
        {
            private readonly AppDbContext _context;

            public AuthController(AppDbContext context)
            {
                _context = context;
            }

            [HttpGet("verify-email")]
            public async Task<IActionResult> VerifyEmail([FromQuery] string token)
            {
                var user =  _context.Users
                    .FirstOrDefault(u => u.EmailVerificationToken == token);

                if (user == null) return BadRequest("Invalid token");

                user.IsEmailVerified = true;
                user.EmailVerificationToken = null; // Clear token after verification
                await _context.SaveChangesAsync();

                return Ok("Email verified successfully");
            }
        }
    }


}