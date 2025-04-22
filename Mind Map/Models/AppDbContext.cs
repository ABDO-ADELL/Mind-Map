namespace Mind_Map.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PersonalityTrait> PersonalityTraits { get; set; }
        public DbSet<PersonalityTestAns> PersonalityTestAnswers { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<PersonalityTrait>().HasData(
    new PersonalityTrait { Id = 1, Name = "Extraversion", Description = "Describes sociability vs. reservedness" },
    new PersonalityTrait { Id = 2, Name = "Intuition", Description = "Focus on patterns vs. facts" },
    new PersonalityTrait { Id = 3, Name = "Thinking", Description = "Logic vs. empathy" },
    new PersonalityTrait { Id = 4, Name = "Judging", Description = "Preference for order vs. spontaneity" },
    new PersonalityTrait { Id = 5, Name = "Assertiveness", Description = "Self-assurance vs. sensitivity" }
);

            //modelBuilder.Entity<PersonalityTrait>().HasData(
            //    new PersonalityTrait { Id = 1, Name = "Introversion" },
            //    new PersonalityTrait { Id = 2, Name = "Extraversion" },
            //    new PersonalityTrait { Id = 3, Name = "Thinking" },
            //    new PersonalityTrait { Id = 4, Name = "Feeling" }
            //);
        }
    }
}
