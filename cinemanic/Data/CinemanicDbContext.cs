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
        }

        public async Task TruncateScreeningsTableAsync()
        {
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE Screenings;");
        }

        public async Task TruncateTicketsTableAsync()
        {
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE Tickets;");
        }

        public async Task TruncateOrdersTableAsync()
        {
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE Orders;");
        }

        public async Task TruncateLikesTableAsync()
        {
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE Likes;");
        }

        public async Task TruncateRoomsTableAsync()
        {
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE Rooms;");
        }

        public async Task TruncateAccountsTableAsync()
        {
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE Accounts;");
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE AspNetUserRoles;");
            await Database.ExecuteSqlRawAsync("TRUNCATE TABLE AspNetUsers;");
        }
    }
}
