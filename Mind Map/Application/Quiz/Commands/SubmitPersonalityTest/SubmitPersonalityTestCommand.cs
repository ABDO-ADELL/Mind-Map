namespace Mind_Map.Application.Quiz.Commands.SubmitPersonalityTest
{
    using MediatR;
    using System.Collections.Generic;

    public class SubmitPersonalityTestCommand : IRequest<TestResultDto>
    {
        public int UserId { get; set; }
        public List<int> Answers { get; set; } = new List<int>();
    }

    public class TestResultDto
    {
        public int UserId { get; set; }
        public string PersonalityType { get; set; }
        public Dictionary<string, int> TraitPercentages { get; set; } = new Dictionary<string, int>();
    }
}
