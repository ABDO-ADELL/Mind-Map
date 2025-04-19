using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mind_Map.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Mind_Map.Application.Users.Commands.LoginUser;

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

    }


}