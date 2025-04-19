namespace Mind_Map.Application.Quiz.Commands.SubmitPersonalityTest
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Mind_Map.Models;

    public class SubmitPersonalityTestHandler : IRequestHandler<SubmitPersonalityTestCommand, TestResultDto>
    {
        private readonly AppDbContext _context;

        public SubmitPersonalityTestHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestResultDto> Handle(SubmitPersonalityTestCommand request, CancellationToken cancellationToken)
        {
            // Store Answers
            foreach (var answer in request.Answers)
            {
                var response = new PersonalityTestAns
                {
                    UserId = request.UserId,
                    TraitId = answer.TraitId,
                    Score = answer.Score
                };

                _context.PersonalityTestAnswers.Add(response);
            }
            await _context.SaveChangesAsync(cancellationToken);

            // Calculate Personality Type
            var personalityType = await CalculatePersonalityType(request.UserId, cancellationToken);

            // Update User's Personality Type
            var user = await _context.Users.FindAsync(request.UserId);
            if (user != null)
            {
                user.PersonalityType = personalityType;
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Store Test Result
            var testResult = new TestResult
            {
                UserId = request.UserId,
                PersonalityType = personalityType
            };
            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync(cancellationToken);

            return new TestResultDto
            {
                UserId = request.UserId,
                PersonalityType = personalityType
            };
        }

        private async Task<string> CalculatePersonalityType(int userId, CancellationToken cancellationToken)
        {
            var responses = await _context.PersonalityTestAnswers
                .Where(r => r.UserId == userId)
                .GroupBy(r => r.TraitId)
                .Select(g => new { TraitId = g.Key, TotalScore = g.Sum(r => r.Score) })
                .ToListAsync(cancellationToken);

            var traitMapping = new Dictionary<int, (string Positive, string Negative)>
    {
        { 1, ("E", "I") }, // Extroversion vs. Introversion
        { 2, ("S", "N") }, // Sensing vs. Intuition
        { 3, ("T", "F") }, // Thinking vs. Feeling
        { 4, ("J", "P") }, // Judging vs. Perceiving
        { 5, ("A", "T") }  // Assertive vs. Turbulent
    };

            string personalityType = "";

            foreach (var response in responses)
            {
                if (traitMapping.ContainsKey(response.TraitId))
                {
                    var (positive, negative) = traitMapping[response.TraitId];
                    personalityType += response.TotalScore >= 0 ? positive : negative;
                }
            }

            return personalityType.Length == 5 ? personalityType : "Unknown";
        }

    }

}
