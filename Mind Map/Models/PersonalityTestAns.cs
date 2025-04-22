namespace Mind_Map.Models
{
    public class PersonalityTestAns
    {
        public int Id { get; set; }
        public int UserId { get; set; } 

        public int TraitId { get; set; } 
        public PersonalityTrait Trait { get; set; } = null!;

        public int Score { get; set; } 
    }
}
