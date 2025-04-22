using Mind_Map.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TestResult

    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [StringLength(5)]
        public string PersonalityType { get; set; }

        public DateTime DateTaken { get; set; } = DateTime.UtcNow;
    }
