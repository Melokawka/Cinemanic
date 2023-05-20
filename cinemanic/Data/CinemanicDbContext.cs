using cinemanic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<ArchivedTicket> ArchivedTickets { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //public DbSet<MovieGenre> MovieGenre { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                // Configure the database connection
                optionsBuilder.UseSqlServer("server=localhost;user id=sa;database=cinemanic;password=A&VeryComplex123Password;multipleactiveresultsets=true;trustservercertificate=true");
                //optionsBuilder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(5), new List<int> { 18456 });
                //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .ToTable(t => t.HasTrigger("AddTicketTrigger"));

            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");

            modelBuilder.Entity<ApplicationUser>(entity => entity.ToTable(name: "AspNetUsers"));

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
                entity.ToTable("AspNetUserTokens");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
            });
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
