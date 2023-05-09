using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    [Route("bilety")]
    public class TicketsController : Controller
    {
        private readonly CinemanicDbContext _context;

        public TicketsController(CinemanicDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> Index()
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("create")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Seat,PricingType,TicketPrice,ScreeningId,OrderId")] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", ticket.OrderId);
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id", ticket.ScreeningId);
            return View(ticket);
        }

        [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin")]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("edit/{id}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Seat,PricingType,TicketPrice,ScreeningId,OrderId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", ticket.OrderId);
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id", ticket.ScreeningId);
            return View(ticket);
        }

        [HttpGet("delete/{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
