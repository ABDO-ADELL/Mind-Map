namespace Mind_Map.Models
{
    public class TestResult
    {
    
         public int Id { get; set; }
         public int UserId { get; set; } // Link to user (if needed)

         public string PersonalityType { get; set; } = string.Empty; // e.g., "INTJ"
        

    }

}
