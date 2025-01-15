using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrainingService.Models;

namespace TrainingService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Training> Trainings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trainee>()
                .HasMany(t => t.Trainings)
                .WithMany(t => t.Trainees);
        }
    }
}
