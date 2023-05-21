using cinemanic.Data;
using cinemanic.Models;
using cinemanic.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace cinemanic
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var stripeApiKey = "sk_test_51N864dCp4aYDEjGbkQaMMmeZsMB2U6UOnIoOwFeeIr1fBlOET4xV7gr7ArhPPmkXY90215DjmBaHZeTDm0E3nxAQ00jcgZV0Vf";

            var builder = WebApplication.CreateBuilder(args);

            StripeConfiguration.ApiKey = stripeApiKey;

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
            //builder.Services.AddDbContext<CinemanicDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("CinemanicDb")));

            builder.Services.AddHostedService<TicketArchiveHostedService>();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<CinemanicDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultScheme = IdentityConstants.ApplicationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "_auth";

                options.LoginPath = "/logowanie";
                options.LogoutPath = "/wyloguj";

                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;

                options.Cookie.HttpOnly = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/logowanie";
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowWordPressApi");

            // apparently the order is important...
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
            name: "login",
            pattern: "login",
            defaults: new { controller = "Accounts", action = "Login" });

            //await new WordPressMockPosts().UploadImageFromUrl();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext = services.GetRequiredService<CinemanicDbContext>();

                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();

                if (!dbContext.Genres.Any())
                {
                    await GenresService.GetGenres(dbContext);
                }

                if (!dbContext.Movies.Any())
                {
                    await MoviesService.GetMovies(dbContext);
                }

                StripeProductService stripe = new(dbContext);
                if (!await stripe.HasProducts())
                {
                    await stripe.AddStripeProducts();
                }

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                var adminRoleName = ApplicationRole.Admin;
                var userRoleName = ApplicationRole.User;

                if (!await roleManager.RoleExistsAsync(adminRoleName))
                {
                    var adminRole = new ApplicationRole(adminRoleName);
                    await roleManager.CreateAsync(adminRole);
                }

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

                await IdentityDataInitializer.SeedData(userManager, roleManager);
            }

            app.Run();
        }
    }
}