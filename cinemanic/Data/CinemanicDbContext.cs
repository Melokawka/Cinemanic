using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    public class CinemanicDbContext : DbContext
    {
        public CinemanicDbContext(DbContextOptions<CinemanicDbContext> options) : base(options)
        {
            // Additional configuration or initialization logic can be placed here
        }

        // Define DbSet properties for your entities (tables) here
        // For example:
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Movie> Movies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configure the database connection
                optionsBuilder.UseSqlServer("Server=localhost\\CINEMANIC;Database=CINEMANIC;Trusted_Connection=True;TrustServerCertificate=true;");
            }
        }

    }
}
