using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            Order order = _dbContext.Orders.FirstOrDefault(o => o.AccountId == account.Id && o.OrderStatus == OrderStatus.PENDING);

            order.OrderStatus = OrderStatus.SUBMITTED;

            _dbContext.Update(order);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
