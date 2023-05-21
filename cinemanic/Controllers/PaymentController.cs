using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace cinemanic.Controllers
{
    [Route("platnosc")]
    public class PaymentController : Controller
    {
        private readonly CinemanicDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(CinemanicDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            //StripeConfiguration.ApiKey = "sk_test_51N864dCp4aYDEjGbkQaMMmeZsMB2U6UOnIoOwFeeIr1fBlOET4xV7gr7ArhPPmkXY90215DjmBaHZeTDm0E3nxAQ00jcgZV0Vf";
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
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

            return RedirectToAction("PaymentConfirmation", new { orderId = order.Id });
        }

        [Authorize]
        [HttpGet("potwierdzenie")]
        public IActionResult PaymentConfirmation(int orderId)
        {
            var order = _dbContext.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var ticket in order.Tickets)
            {
                ticket.IsActive = true;
            }

            order.OrderStatus = OrderStatus.COMPLETED;
            _dbContext.Update(order);
            _dbContext.SaveChanges();

            return View(order);
        }

    }
}
