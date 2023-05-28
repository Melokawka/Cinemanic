using cinemanic.Data;
using cinemanic.Data.Seeders;
using cinemanic.Models;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace cinemanic
{
    /// <summary>
    /// The entry point class for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true);

            // Configure CORS policy
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

            // Add CinemanicDbContext
            builder.Services.AddDbContext<CinemanicDbContext>();

            // Add TicketArchiveHostedService
            builder.Services.AddHostedService<TicketArchiveHostedService>();

            // Add Identity services
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<CinemanicDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<ApplicationRole>();

            builder.Services.AddScoped<SignInManager<ApplicationUser>>();

            builder.Services.AddAuthentication();

            // Configure application cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "_auth";

                options.LoginPath = "/logowanie";
                options.LogoutPath = "/wyloguj";

                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;

                options.Cookie.HttpOnly = true;
            });

            // Configure Identity options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            // Configure logging
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            // Add services
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<MovieService>();

            var app = builder.Build();

            StripeConfiguration.ApiKey = app.Configuration["StripeApiSecretKey"];

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowWordPressApi");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            await new WordPressSeederService(app.Configuration["WordpressApiKey"]).UploadImageFromUrl();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<CinemanicDbContext>();

                await PrepareDatabase(dbContext, services, app.Configuration);
            }

            app.Run();
        }

        private static async Task PrepareDatabase(CinemanicDbContext dbContext, IServiceProvider services, IConfiguration configuration)
        {
            // Seed genres if they don't exist
            if (!dbContext.Genres.Any())
            {
                await GenresService.GetGenres(dbContext);
            }

            // Seed movies if they don't exist
            if (!dbContext.Movies.Any())
            {
                var movieService = new MovieService(dbContext, configuration);
                await movieService.GetMovies();

                // Remove existing orders
                var orders = dbContext.Orders.ToList();
                dbContext.Orders.RemoveRange(orders);
                await dbContext.SaveChangesAsync();

                // Delete Stripe products
                var stripeProductService = new ProductService();
                var products = stripeProductService.List(new ProductListOptions { Limit = 20 }); // Increase the limit if you have more movies

                foreach (var product in products)
                {
                    stripeProductService.Delete(product.Id);
                }
            }

            // Add Stripe products if they don't exist
            StripeProductService stripe = new(dbContext);
            if (!await stripe.HasProducts())
            {
                await stripe.AddStripeProducts();
            }

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            var adminRoleName = ApplicationRole.Admin;
            var userRoleName = ApplicationRole.User;

            // Create admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                var adminRole = new ApplicationRole(adminRoleName);
                await roleManager.CreateAsync(adminRole);
            }

            // Create user role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(userRoleName))
            {
                var userRole = new ApplicationRole(userRoleName);
                await roleManager.CreateAsync(userRole);
            }

            if (!dbContext.Accounts.Any())
            {
                await AccountSeeder.SeedAccounts(userManager, dbContext);
            }

            if (!dbContext.Rooms.Any())
            {
                RoomSeeder.SeedRooms(dbContext);
            }

            if (!dbContext.Likes.Any())
            {
                await LikeSeeder.SeedLikes(dbContext);
            }

            if (!dbContext.NewsletterClients.Any())
            {
                await NewsletterClientSeeder.SeedNewsletterClients(dbContext);
            }

            if (!dbContext.Screenings.Any())
            {
                await ScreeningSeeder.SeedScreenings(dbContext);
            }

            if (!dbContext.Orders.Any())
            {
                await OrderSeeder.SeedOrders(dbContext);
            }

            if (!dbContext.Tickets.Any())
            {
                await TicketSeeder.SeedTickets(dbContext);
            }

            // Seed identity data
            await IdentityDataInitializer.SeedData(userManager, roleManager);
        }
    }
}