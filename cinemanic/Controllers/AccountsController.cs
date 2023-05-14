using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cinemanic.Controllers
{

    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CinemanicDbContext _dbContext;

        public AccountsController(UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [Route("logowanie")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("konto")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Account()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var birthDate = new DateTimeOffset(user.BirthDate);
            var tickets = _dbContext.Tickets
                .Include(at => at.Screening)
                    .ThenInclude(s => s.Movie)
                        .ThenInclude(m => m.Genres)
                .Where(t => t.Order.Account.UserEmail == user.Email)
                .OrderBy(t => t.Screening.ScreeningDate)
                .ToList();

            var archivedTickets = _dbContext.ArchivedTickets
                .Include(at => at.Screening)
                    .ThenInclude(s => s.Movie)
                        .ThenInclude(m => m.Genres)
                .Join(_dbContext.Orders, at => at.OrderId, o => o.Id, (at, o) => new { ArchivedTicket = at, Order = o })
                .Where(j => j.Order.Account.UserEmail == user.Email)
                .OrderBy(j => j.ArchivedTicket.Screening.ScreeningDate)
                .Select(j => j.ArchivedTicket)
                .ToList();

            var likes = _dbContext.Likes
                .Include(l => l.Movie)
                    .ThenInclude(m => m.Genres)
                .Include(l => l.Account)
                .Where(l => l.Account.UserEmail == user.Email)
                .ToList();

            var orders = _dbContext.Orders
                .Include(o => o.Account)
                .Include(o => o.Tickets)
                .Where(o => o.Account.UserEmail == user.Email)
                .ToList();

            foreach (Order info in orders) foreach (var property in typeof(Order).GetProperties()) Console.WriteLine(property.Name + " = " + property.GetValue(info));

            var model = new AccountViewModel()
            {
                Email = user.Email,
                BirthDate = birthDate,
                Tickets = tickets,
                ArchivedTickets = archivedTickets,
                Likes = likes,
                Orders = orders
            };

            return View(model);
        }

        [AllowAnonymous]
        [Route("logowanie")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Authenticate user
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Create authentication cookie // below is default cause mine doesnt work CookieAuthenticationDefaults.AuthenticationScheme
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("o")),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    var principal = new ClaimsPrincipal(identity);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("wyloguj")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Route("rejestracja")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("rejestracja")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Create authentication cookie
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                    var principal = new ClaimsPrincipal(identity);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    // Create an account for the regular user
                    var userAccount = new Account
                    {
                        UserEmail = model.Email,
                        Birthdate = model.DateOfBirth,
                        // set any other properties you need here
                    };

                    _dbContext.Accounts.Add(userAccount);
                    await _dbContext.SaveChangesAsync();

                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
    }
}
