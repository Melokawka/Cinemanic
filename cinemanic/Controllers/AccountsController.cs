using cinemanic.Data;
using cinemanic.Models;
using cinemanic.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace cinemanic.Controllers
{
    /// <summary>
    /// Represents a controller for managing user accounts and related functionality.
    /// </summary>
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CinemanicDbContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="signInManager">The sign-in manager.</param>
        public AccountsController(UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Gets the user account page.
        /// </summary>
        /// <returns>The view for the user account page.</returns>
        [Route("konto")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Account()
        {
            var user = await _userManager.GetUserAsync(User);

            Console.WriteLine(user.BirthDate);

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

        /// <summary>
        /// Exports the newsletter email addresses as a CSV file.
        /// </summary>
        /// <returns>The CSV file containing newsletter email addresses.</returns>
        [HttpGet("eksportuj-emaile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportNewsletterEmails()
        {
            var newsletterClientEmails = await _dbContext.NewsletterClients.Include(n => n.Account).Select(n => n.Account.UserEmail).ToListAsync();

            // Format the email addresses as a CSV string
            string csvContent = string.Join(Environment.NewLine, newsletterClientEmails);

            byte[] csvBytes = Encoding.UTF8.GetBytes(csvContent);
            var fileContentResult = File(csvBytes, "text/csv", "newsletter-emails.csv");

            return fileContentResult;
        }

        /// <summary>
        /// Exports the financial results as a PDF file.
        /// </summary>
        /// <returns>The PDF file containing the financial results.</returns>
        [HttpGet("wyniki-finansowe")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportFinancialResults()
        {
            DateTime oneYearAgo = DateTime.Now.AddYears(-1);

            var archivedScreeningsData = _dbContext.ArchivedScreenings
                .Include(s => s.Movie)
                .Where(a => a.ScreeningDate > oneYearAgo)
                .OrderBy(a => a.ScreeningDate)
                .ToList();

            Document document = new();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            string dateRange = $"{oneYearAgo.ToString("dd.MM.yyyy")} - {DateTime.Now.ToString("dd.MM.yyyy")}";
            document.Add(FinancialReportFunctions.CreateParagraph(dateRange + "\n", 17, -1));
            document.Add(FinancialReportFunctions.CreateParagraph("Financial Results\n\n", 16, -1));

            document.Add(FinancialReportFunctions.CreateReportTable(archivedScreeningsData));

            decimal overallIncome = archivedScreeningsData.Sum(a => a.GrossIncome);
            document.Add(FinancialReportFunctions.CreateParagraph("Income Overall: " + overallIncome + "zl", 16, 2));

            // Set the response headers for downloading the PDF file
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{DateTime.Now.ToString("yyyy")}-financial-results.pdf\"");
            Response.ContentType = "application/pdf";

            document.Close();

            // Write the PDF file to the response
            Response.Body.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);

            return new EmptyResult();
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <returns>The view for the login page.</returns>
        [Route("logowanie")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Authenticates a user based on the provided login credentials.
        /// </summary>
        /// <param name="model">The login view model containing the user's email and password.</param>
        /// <returns>The result of the login attempt.</returns>
        [AllowAnonymous]
        [Route("logowanie")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("o")),
                    };

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <returns>The result of the logout operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("wyloguj")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        /// <returns>The view for the registration page.</returns>
        [Route("rejestracja")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The registration view model containing the user's email, password, and date of birth.</param>
        /// <returns>The result of the registration process.</returns>
        [Route("rejestracja")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, BirthDate = model.DateOfBirth };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("o")),
                    };

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    var userAccount = new Account
                    {
                        UserEmail = model.Email,
                        Birthdate = model.DateOfBirth,
                    };

                    _dbContext.Accounts.Add(userAccount);
                    await _dbContext.SaveChangesAsync();

                    await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}
