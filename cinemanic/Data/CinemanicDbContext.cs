using cinemanic.Models;
using cinemanic.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

namespace cinemanic.Data
{
    public class CinemanicDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<NewsletterClient> NewsletterClients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Like> Likes { get; set; }
        //public DbSet<MovieGenre> MovieGenre { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configure the database connection
                optionsBuilder.UseSqlServer("Server=localhost\\CINEMANIC;Database=CINEMANIC2;Trusted_Connection=True;TrustServerCertificate=true;");
                //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            /*modelBuilder.Entity<Movie>()
            .HasMany(m => m.Genres)
            .WithMany(g => g.Movies)
            .UsingEntity<MovieGenre>(
                mg => mg.HasOne<Genre>().WithMany(),
                mg => mg.HasOne<Movie>().WithMany())
            .ToTable("movie_genre")
            .HasKey(mg => new { mg.MoviesId, mg.GenresId });*/

            /*modelBuilder.Entity<Movie>()
            .HasMany(m => m.Genres)
            .WithOne(g => g.Movie)
            .UsingEntity<MovieGenre>(
                gm => gm.HasOne<Genre>().WithMany(),
                gm => gm.HasOne<Movie>().WithMany())
            .ToTable("genre_list")
            .HasKey(gm => new { gm.MovieId, gm.Genre });

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Likes)
                .WithOne(l => l.Movie)
                .HasForeignKey(l => l.MovieId);*/
        }

     }
}
