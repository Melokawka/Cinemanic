using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Stripe;
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
        [HttpPost("zakup")]
        public async Task<IActionResult> Buy()
        {
            var user = await _userManager.GetUserAsync(User);

            var account = await _dbContext.Accounts.SingleAsync(a => a.UserEmail == user.Email);

            if (user == null)
            {
                return NotFound();
            }

            Order order = _dbContext.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefault(o => o.AccountId == account.Id && o.OrderStatus == OrderStatus.PENDING);


            order.OrderStatus = OrderStatus.SUBMITTED;
            _dbContext.Update(order);
            await _dbContext.SaveChangesAsync();

            // Prepare the payment data
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var ticket in order.Tickets)
            {
                var price = ticket.TicketPrice;

                var productService = new ProductService();
                var products = await productService.ListAsync();

                // Find the corresponding Stripe product
                var product = products.FirstOrDefault(p => p.Name == ticket.Screening.Movie.Title);

                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "PLN",
                        UnitAmount = (long)(price * 100), // Price in grosze
                        Product = product?.Id,
                    },
                    Quantity = 1,
                };
                lineItems.Add(lineItem);
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Request.Scheme + "://" + Request.Host.Value + "/platnosc/sukces?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://example.com/cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", order.Id.ToString() },
                },
            };

            var service = new SessionService();
            var session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
