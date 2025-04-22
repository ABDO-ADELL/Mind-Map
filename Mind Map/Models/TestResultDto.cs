namespace Mind_Map.Models
{
    public class TestResultDto
    {
        public int UserId { get; set; }
        public string PersonalityType { get; set; } = string.Empty;
        public Dictionary<int, int> TraitPercentages { get; set; }
    }

}
