using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    [Route("bilety")]
    public class TicketsController : Controller
    {
        private readonly CinemanicDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(CinemanicDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> Admin()
        {
            var cinemanicDbContext = _context.Tickets.Include(t => t.Order).Include(t => t.Screening);
            return View(await cinemanicDbContext.ToListAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Order)
                .Include(t => t.Screening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpGet("kup")]
        [Authorize]
        public IActionResult Buy(int screeningId, int movieId)
        {
            var pricingTypes = Enum.GetValues(typeof(PricingType)).Cast<PricingType>();


            var maxOrderId = _context.Orders.Max(o => o.Id);
            var nextOrderId = maxOrderId + 1;

            var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
            var screening = _context.Screenings.FirstOrDefault(s => s.Id == screeningId);

            ViewBag.Price = 16.00;
            ViewBag.PricingTypes = new SelectList(pricingTypes);
            ViewBag.OrderId = nextOrderId;
            ViewBag.MoviePoster = movie.PosterPath;
            ViewBag.MovieTitle = movie.Title;
            ViewBag.ScreeningId = screening.Id;
            ViewBag.ScreeningDate = screening.ScreeningDate.ToString("dd-MM-yyyy HH:mm");
            ViewBag.ScreeningSubtitles = screening.Subtitles ? "tak" : "nie";
            ViewBag.ScreeningLector = screening.Lector ? "tak" : "nie";
            ViewBag.ScreeningDubbing = screening.Dubbing ? "tak" : "nie";
            ViewBag.Screening3d = screening.Is3D ? "tak" : "nie";
            ViewBag.ScreeningRoom = screening.RoomId;

            return View();
        }

        [HttpPost("kup")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([FromForm] Ticket ticket)
        {
            var hasDiscount = false;
            if (ticket.PricingType == PricingType.ULGOWY) hasDiscount = true;

            var user = await _userManager.GetUserAsync(User);

            var price = hasDiscount ? (ticket.TicketPrice / 2) : ticket.TicketPrice;
            var account = await _context.Accounts.Select(a => a).FirstOrDefaultAsync(a => a.UserEmail == user.Email);

            Order order = new Order
            {
                TotalPrice = price,
                AccountId = account.Id,
            };

            Ticket _ticket = new Ticket
            {
                Seat = ticket.Seat,
                PricingType = ticket.PricingType,
                TicketPrice = price,
                ScreeningId = ticket.ScreeningId,
                OrderId = ticket.OrderId,
                Order = order
            };

            _ticket.Order.TotalPrice = price;

            _context.Add(_ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = _ticket.Id });
        }

        [HttpGet("create")]
        [Authorize]
        public IActionResult Create()
        {
            var pricingTypes = Enum.GetValues(typeof(PricingType)).Cast<PricingType>();
            ViewBag.PricingTypes = new SelectList(pricingTypes);

            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id");
            return View();
        }

        [HttpPost("create")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Seat,PricingType,TicketPrice,ScreeningId,OrderId")] Ticket ticket)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        [HttpGet("edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var pricingTypes = Enum.GetValues(typeof(PricingType)).Cast<PricingType>();
            ViewBag.PricingTypes = new SelectList(pricingTypes);

            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", ticket.OrderId);
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id", ticket.ScreeningId);
            return View(ticket);
        }

        [HttpPost("edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Seat,PricingType,TicketPrice,ScreeningId,OrderId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            _context.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpGet("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Order)
                .Include(t => t.Screening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost("delete/{id}"), ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'CinemanicDbContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
