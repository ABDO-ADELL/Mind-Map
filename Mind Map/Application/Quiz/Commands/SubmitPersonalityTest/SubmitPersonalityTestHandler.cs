
namespace Mind_Map.Application.Quiz.Commands.SubmitPersonalityTest
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Mind_Map.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SubmitPersonalityTestHandler : IRequestHandler<SubmitPersonalityTestCommand, TestResultDto>
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<int, int> _questionToTraitMap;

        public SubmitPersonalityTestHandler(AppDbContext context)
        {
            _context = context;
            // Map questions to traits (1-5)
            // Questions 0-11 → Trait 1 (E/I)
            // Questions 12-23 → Trait 2 (S/N)
            // Questions 24-35 → Trait 3 (T/F)
            // Questions 36-47 → Trait 4 (J/P)
            // Questions 48-59 → Trait 5 (A/T)
            _questionToTraitMap = new Dictionary<int, int>();

            for (int i = 0; i < 60; i++)
            {
                _questionToTraitMap[i] = (i / 12) + 1;
            }
        }

        public async Task<TestResultDto> Handle(SubmitPersonalityTestCommand request, CancellationToken cancellationToken)
        {
            // Validate request
            if (request.Answers == null || request.Answers.Count != 60)
                throw new InvalidOperationException("Exactly 60 answers are required for the personality test");

            // Organize answers by trait
            var answersByTrait = new Dictionary<int, List<int>>();
            for (int i = 1; i <= 5; i++)
                answersByTrait[i] = new List<int>();

            for (int i = 0; i < request.Answers.Count; i++)
            {
                int traitId = _questionToTraitMap[i];
                answersByTrait[traitId].Add(request.Answers[i]);
            }

            // Process each trait and save answers
            foreach (var traitEntry in answersByTrait)
            {
                int traitId = traitEntry.Key;
                var scores = traitEntry.Value;

                // Calculate average score for the trait
                double avgScore = scores.Average();

                // Save to database
                var response = new PersonalityTestAns
                {
                    UserId = request.UserId,
                    TraitId = traitId,
                    Score = (int)Math.Round(avgScore)
                };

                _context.PersonalityTestAnswers.Add(response);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Calculate personality type and percentages
            var (personalityType, traitPercentages) = CalculatePersonalityType(answersByTrait);

            // Update user's personality type
            var user = await _context.Users.FindAsync(request.UserId);
            if (user != null)
            {
                user.PersonalityType = personalityType;
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Save test result
            var testResult = new TestResult
            {
                UserId = request.UserId,
                PersonalityType = personalityType,
                DateTaken = DateTime.UtcNow
            };

            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync(cancellationToken);

            // Create named trait percentages for response
            var traitNames = new Dictionary<int, string>
            {
                { 1, "Extraversion/Introversion" },
                { 2, "Sensing/Intuition" },
                { 3, "Thinking/Feeling" },
                { 4, "Judging/Perceiving" },
                { 5, "Assertive/Turbulent" }
            };

            var namedTraitPercentages = new Dictionary<string, int>();
            foreach (var entry in traitPercentages)
            {
                namedTraitPercentages[traitNames[entry.Key]] = entry.Value;
            }

            // Return result
            return new TestResultDto
            {
                UserId = request.UserId,
                PersonalityType = personalityType,
                TraitPercentages = namedTraitPercentages
            };
        }

        private (string PersonalityType, Dictionary<int, int> TraitPercentages) CalculatePersonalityType(
            Dictionary<int, List<int>> answersByTrait)
        {
            var traitMapping = new Dictionary<int, (string Positive, string Negative)>
            {
                { 1, ("E", "I") }, // Extraversion vs Introversion
                { 2, ("S", "N") }, // Sensing vs Intuition
                { 3, ("T", "F") }, // Thinking vs Feeling
                { 4, ("J", "P") }, // Judging vs Perceiving
                { 5, ("A", "T") }  // Assertive vs Turbulent
            };

            string personalityType = "";
            var traitPercentages = new Dictionary<int, int>();

            foreach (var traitId in traitMapping.Keys)
            {
                var scores = answersByTrait[traitId];

                // Analyze trait tendency (1-3 favors negative, 5-7 favors positive, 4 is neutral)
                double totalScore = 0;

                foreach (var score in scores)
                {
                    if (score == 4) continue; // Skip neutral responses

                    // Map 1-7 scale to preference strength
                    double strength = score < 4 ? 4 - score : score - 4;
                    totalScore += score < 4 ? -strength : strength;
                }

                // Maximum possible score = 12 questions * 3 points max deviation from neutral
                double maxScore = scores.Count * 3.0;

                // Calculate percentage of trait preference (0-100%)
                double percentage = Math.Min(100, Math.Round(Math.Abs(totalScore) * 100 / maxScore));

                // Determine trait letter based on score direction
                var (positive, negative) = traitMapping[traitId];
                personalityType += totalScore >= 0 ? positive : negative;

                // Store percentage for this trait
                traitPercentages[traitId] = (int)percentage;
            }

            return (personalityType, traitPercentages);
        }
    }
}