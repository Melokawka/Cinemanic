using cinemanic.Data;
using cinemanic.Seeders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace cinemanic
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWordPressApi",
                    policyBuilder =>
                    {
                        policyBuilder.WithOrigins("http://127.0.0.1:8080") // Replace PORT with your WordPress server port number
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            // Add services to the container.
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

            // Enable CORS for the specified policy
            app.UseCors("AllowWordPressApi");            
            
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
            name: "login",
            pattern: "login",
            defaults: new { controller = "Accounts", action = "Login" }
            );


            using (var scope = app.Services.CreateScope())
            {
               var services = scope.ServiceProvider;

             //get an instance of the CinemanicDbContext service
                var dbContext = services.GetRequiredService<CinemanicDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                //its useless because when adding movies the entity framework adds genres to both the transfer table and the genres table
                //await GenresService.GetGenres(dbContext);

             //perform a database operation
                AccountSeeder.SeedAccount(dbContext);
            }

            app.Run();
        }
    }
}