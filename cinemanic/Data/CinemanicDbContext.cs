using cinemanic.Models;
using cinemanic.Seeders;
using Microsoft.EntityFrameworkCore;
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
        //public DbSet<MovieGenre> MovieGenre { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configure the database connection
                optionsBuilder.UseSqlServer("Server=localhost\\CINEMANIC;Database=CINEMANIC2;Trusted_Connection=True;TrustServerCertificate=true;");
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
                .HasForeignKey(l => l.MovieId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Likes)
                .WithOne(l => l.Account)
                .HasForeignKey(l => l.UserEmail);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Screening)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.ScreeningId);

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.OrderTickets)
                .WithOne(ot => ot.Ticket)
                .HasForeignKey(ot => ot.TicketId);

            modelBuilder.Entity<OrderTicket>()
                .HasKey(ot => new { ot.OrderId, ot.TicketId });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderTickets)
                .WithOne(ot => ot.Order)
                .HasForeignKey(ot => ot.OrderId);

            modelBuilder.Entity<NewsletterClient>()
                .HasKey(nc => nc.UserEmail);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Orders)
                .WithOne(o => o.Account)
                .HasForeignKey(o => o.UserEmail);

            modelBuilder.Entity<Screening>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Screenings)
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Screening>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Screenings)
                .HasForeignKey(s => s.RoomId);

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.MovieId, l.UserEmail });*/


        }

     }
}
