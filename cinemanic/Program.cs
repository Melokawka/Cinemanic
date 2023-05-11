using cinemanic.Data;
using cinemanic.Models;
using cinemanic.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

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
            //builder.Services.AddDbContext<CinemanicDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("CinemanicDb")));

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<CinemanicDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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

            await new WordPressMockPostCreator().UploadImageFromUrl();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext = services.GetRequiredService<CinemanicDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                await GenresService.GetGenres(dbContext);

                await MoviesService.GetMovies(dbContext);

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                var adminRole = new ApplicationRole(ApplicationRole.Admin);
                await roleManager.CreateAsync(adminRole);

                var userRole = new ApplicationRole(ApplicationRole.User);
                await roleManager.CreateAsync(userRole);

                await AccountSeeder.SeedAccounts(userManager, dbContext);
                RoomSeeder.SeedRooms(dbContext);
                await LikeSeeder.SeedLikes(dbContext);
                await NewsletterClientSeeder.SeedNewsletterClients(dbContext);
                await ScreeningSeeder.SeedScreenings(dbContext);
                await OrderSeeder.SeedOrders(dbContext);
                await TicketSeeder.SeedTickets(dbContext);
                await IdentityDataInitializer.SeedData(userManager, roleManager);
            }

            app.Run();
        }
    }
}