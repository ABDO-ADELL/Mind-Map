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

            // Optional: Seed initial personality traits
            modelBuilder.Entity<PersonalityTrait>().HasData(
                new PersonalityTrait { Id = 1, Name = "Introversion" },
                new PersonalityTrait { Id = 2, Name = "Extraversion" },
                new PersonalityTrait { Id = 3, Name = "Thinking" },
                new PersonalityTrait { Id = 4, Name = "Feeling" }
            );
        }
    }
}
