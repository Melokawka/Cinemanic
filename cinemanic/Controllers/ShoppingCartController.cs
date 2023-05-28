using cinemanic.Data;
using cinemanic.Models;
using cinemanic.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace cinemanic.Controllers
{
    /// <summary>
    /// Controller for handling shopping cart-related actions.
    /// </summary>
    [Route("koszyk")]
    public class ShoppingCartController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CinemanicDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="dbContext">The database context.</param>
        public ShoppingCartController(UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Displays the shopping cart page.
        /// </summary>
        /// <returns>The view displaying the shopping cart contents.</returns>
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

        /// <summary>
        /// Processes the payment for an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>The result of the payment process.</returns>
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

            if (!ShoppingCartFunctions.CanPurchaseAdultContent(userAge, order)) return Content("Osoba niepelnoletnia nie moze kupic biletu na seans dla doroslych", "text/html");

            return await ProcessOrder(order);
        }

        /// <summary>
        /// Buys the items in the shopping cart.
        /// </summary>
        /// <returns>The result of the purchase.</returns>
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

            if (!ShoppingCartFunctions.CanPurchaseAdultContent(userAge, order)) return Content("Osoba niepelnoletnia nie moze kupic biletu na seans dla doroslych", "text/html");

            order.OrderStatus = OrderStatus.SUBMITTED;
            _dbContext.Update(order);
            await _dbContext.SaveChangesAsync();

            return await ProcessOrder(order);
        }

        /// <summary>
        /// Retrieves the count of products in the shopping cart.
        /// </summary>
        /// <returns>The number of products in the shopping cart.</returns>
        [HttpGet("liczba-produktow")]
        [Authorize]
        public async Task<int> CountProductsInCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var account = (await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserEmail == user.Email));

            if (account == null) return 0;

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.AccountId == account.Id && o.OrderStatus == OrderStatus.PENDING);

            if (order == null) return 0;

            return await _dbContext.Tickets.CountAsync(t => t.OrderId == order.Id);
        }

        /// <summary>
        /// Processes the order by preparing line items and creating a payment session.
        /// </summary>
        /// <param name="order">The order to process.</param>
        /// <returns>The result of the order processing.</returns>
        private async Task<IActionResult> ProcessOrder(Order order)
        {
            var lineItems = await ShoppingCartFunctions.PrepareLineItems(order.Tickets);

            var session = await CreatePaymentSession(lineItems, order.Id);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        /// <summary>
        /// Creates a payment session for the given line items and order ID.
        /// </summary>
        /// <param name="lineItems">The line items for the payment session.</param>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>The created payment session.</returns>
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
