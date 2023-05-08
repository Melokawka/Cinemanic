using cinemanic.Data;
using cinemanic.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Net;
using System.Reflection.Metadata;

namespace cinemanic
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWordPressApi",
                    policyBuilder =>
                    {
                        policyBuilder.WithOrigins("http://127.0.0.1:8080") 
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<CinemanicDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowWordPressApi");            
            
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
            name: "login",
            pattern: "login",
            defaults: new { controller = "Accounts", action = "Login" });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext = services.GetRequiredService<CinemanicDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                await GenresService.GetGenres(dbContext);

                await MoviesService.GetMovies(dbContext);

                AccountSeeder.SeedAccounts(dbContext);
                RoomSeeder.SeedRooms(dbContext);
                await LikeSeeder.SeedLikes(dbContext);
                await NewsletterClientSeeder.SeedNewsletterClients(dbContext);
                await ScreeningSeeder.SeedScreenings(dbContext);
                await OrderSeeder.SeedOrders(dbContext);
                await TicketSeeder.SeedTickets(dbContext);
            }

            app.Run();
        }
    }
}