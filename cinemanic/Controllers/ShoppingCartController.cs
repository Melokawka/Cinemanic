using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace cinemanic.Controllers
{
    [Route("koszyk")]
    public class ShoppingCartController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CinemanicDbContext _dbContext;

        public ShoppingCartController(UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var tickets = _dbContext.Tickets
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Order)
                .Where(t => t.Order.Account.UserEmail == user.Email && t.Order.OrderStatus == OrderStatus.PENDING)
                .OrderBy(t => t.Screening.ScreeningDate)
                .ToList();

            return View(tickets);
        }

        [Authorize]
        [HttpPost("oplacenie")]
        public async Task<IActionResult> Pay(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _dbContext.Accounts.SingleAsync(a => a.UserEmail == user.Email);

            int userAge = ShoppingCartFunctions.GetUserAge(user.BirthDate);

            Order order = _dbContext.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefault(o => o.AccountId == account.Id && o.OrderStatus == OrderStatus.SUBMITTED && o.Id == orderId);

            if (!ShoppingCartFunctions.CanPurchaseAdultContent(userAge, order)) return Content("Osoba niepełnoletnia nie może kupić biletu na seans dla dorosłych", "text/html");

            return await ProcessOrder(order);
        }

        [Authorize]
        [HttpPost("zakup")]
        public async Task<IActionResult> Buy()
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _dbContext.Accounts.SingleAsync(a => a.UserEmail == user.Email);
            int userAge = ShoppingCartFunctions.GetUserAge(user.BirthDate);

            Order order = _dbContext.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefault(o => o.AccountId == account.Id && o.OrderStatus == OrderStatus.PENDING);

            if (!ShoppingCartFunctions.CanPurchaseAdultContent(userAge, order)) return Content("Osoba niepełnoletnia nie może kupić biletu na seans dla dorosłych", "text/html");

            order.OrderStatus = OrderStatus.SUBMITTED;
            _dbContext.Update(order);
            await _dbContext.SaveChangesAsync();

            return await ProcessOrder(order);
        }

        [HttpGet("liczba-produktow")]
        [Authorize]
        public async Task<int> CountProductsInCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var accountId = (await _dbContext.Accounts.SingleAsync(a => a.UserEmail == user.Email)).Id;

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.AccountId == accountId && o.OrderStatus == OrderStatus.PENDING);

            if (order == null) return 0;

            return await _dbContext.Tickets.CountAsync(t => t.OrderId == order.Id);
        }

        private async Task<IActionResult> ProcessOrder(Order order)
        {
            var lineItems = await ShoppingCartFunctions.PrepareLineItems(order.Tickets);

            var session = await CreatePaymentSession(lineItems, order.Id);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        private async Task<Session> CreatePaymentSession(List<SessionLineItemOptions> lineItems, int orderId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Request.Scheme + "://" + Request.Host.Value + "/platnosc/sukces?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Request.Scheme + "://" + Request.Host.Value + "/koszyk",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", orderId.ToString() },
                },
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }
    }
}
