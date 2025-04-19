namespace Mind_Map.Application.Quiz.Commands.SubmitPersonalityTest
{
    using Mind_Map.Models;
    using MediatR;

    public class SubmitPersonalityTestCommand : IRequest<TestResultDto>
    {
        public int UserId { get; set; }
        public List<AnswerDto> Answers { get; set; } = new();
    }

    public class AnswerDto
    {
        public int TraitId { get; set; } // Which trait this answer affects
        public int Score { get; set; }   // Score for this trait (-3 to +3)
    }
}
