namespace Mind_Map.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Mind_Map.Application.Quiz.Commands.SubmitPersonalityTest;

    [Route("api/quiz")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitPersonalityTestCommand command)
        {
            if (command.Answers == null || command.Answers.Count != 60)
            {
                return BadRequest("Exactly 60 answers are required");
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}