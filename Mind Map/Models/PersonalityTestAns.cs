namespace Mind_Map.Models
{
    public class PersonalityTestAns
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Link to user (if tracking)

        public int TraitId { get; set; } // The personality trait affected by this response
        public PersonalityTrait Trait { get; set; } = null!;

        public int Score { get; set; } // Score assigned to this trait (e.g., -3 to +3)
    }
}
