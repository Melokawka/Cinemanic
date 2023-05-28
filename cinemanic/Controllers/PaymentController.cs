using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace cinemanic.Controllers
{
    /// <summary>
    /// Controller for handling payment-related actions.
    /// </summary>
    [Route("platnosc")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly CinemanicDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="userManager">The user manager.</param>
        public PaymentController(CinemanicDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Action method for handling successful payment.
        /// </summary>
        /// <param name="session_id">The session ID for the payment.</param>
        /// <returns>The result of the payment success action.</returns>
        [HttpGet("sukces")]
        public ActionResult PaymentSuccess([FromQuery] string session_id)
        {
            var service = new SessionService();
            Session session = service.Get(session_id);

            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == int.Parse(session.Metadata.GetValueOrDefault("OrderId")));

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.PAID;
            _dbContext.Update(order);
            _dbContext.SaveChanges();

            return RedirectToAction("PaymentConfirmation", new { orderId = order.Id, session_id = session_id });
        }

        /// <summary>
        /// Action method for displaying payment confirmation.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="session_id">The session ID for the payment.</param>
        /// <returns>The result of the payment confirmation action.</returns>
        [HttpGet("potwierdzenie")]
        public IActionResult PaymentConfirmation(int orderId, string session_id)
        {
            var service = new SessionService();
            Session session = service.Get(session_id);

            var order = _dbContext.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefault(o => o.Id == orderId);

            if (session == null || order == null)
            {
                return NotFound();
            }

            foreach (var ticket in order.Tickets)
            {
                ticket.IsActive = true;
            }

            List<Ticket> unpaidTicketsForDeletion = new();
            foreach (Ticket ticket in order.Tickets)
            {
                var unpaidTicketsForTheSameSeat = _dbContext.Tickets
                    .Where(t => t.ScreeningId == ticket.ScreeningId && t.Seat == ticket.Seat && t.IsActive == false)
                    .ToList();

                unpaidTicketsForDeletion.AddRange(unpaidTicketsForTheSameSeat);
            }
            _dbContext.RemoveRange(unpaidTicketsForDeletion);

            order.OrderStatus = OrderStatus.COMPLETED;
            _dbContext.Update(order);

            _dbContext.SaveChanges();

            return View(order);
        }
    }
}
