using System.ComponentModel.DataAnnotations;

namespace Mind_Map.Models
{
    public class PersonalityTrait
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

    }
    }

