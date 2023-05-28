using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    /// <summary>
    /// Controller for managing tickets.
    /// </summary>
    [Route("bilety")]
    public class TicketsController : Controller
    {
        private readonly CinemanicDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketsController"/> class.
        /// </summary>
        /// <param name="context">The CinemanicDbContext.</param>
        /// <param name="userManager">The UserManager.</param>
        public TicketsController(CinemanicDbContext context, UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets the admin view for managing tickets.
        /// </summary>
        /// <returns>The admin view.</returns>
        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var cinemanicDbContext = _dbContext.Tickets.Include(t => t.Order).Include(t => t.Screening);
            return View(await cinemanicDbContext.ToListAsync());
        }

        /// <summary>
        /// Gets the details view for a ticket with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the ticket.</param>
        /// <returns>The details view.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _dbContext.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _dbContext.Tickets
                .Include(t => t.Order)
                .Include(t => t.Screening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        /// <summary>
        /// Gets the buy view for purchasing a ticket.
        /// </summary>
        /// <param name="screeningId">The ID of the screening.</param>
        /// <param name="movieId">The ID of the movie.</param>
        /// <returns>The buy view.</returns>
        [HttpGet("kup")]
        [Authorize]
        public async Task<IActionResult> Buy(int screeningId, int movieId)
        {
            var pricingTypes = Enum.GetValues(typeof(PricingType)).Cast<PricingType>();

            var maxOrderId = _dbContext.Orders.Max(o => o.Id);
            var nextOrderId = maxOrderId + 1;

            var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
            var screening = await _dbContext.Screenings.FirstOrDefaultAsync(s => s.Id == screeningId);

            var freeSeats = await GetFreeSeatsList(screeningId);

            ViewBag.PricingTypes = new SelectList(pricingTypes);
            ViewBag.Seats = new SelectList(freeSeats);
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

        /// <summary>
        /// Retrieves the list of free seats for the specified screening.
        /// </summary>
        /// <param name="screeningId">The ID of the screening.</param>
        /// <returns>The list of free seats.</returns>
        public async Task<List<int>> GetFreeSeatsList(int screeningId)
        {
            var seatsTaken = await _dbContext.Tickets.Where(t => t.ScreeningId == screeningId && t.IsActive).Select(t => t.Seat).ToListAsync();
            var seatsCount = await _dbContext.Screenings.Where(s => s.Id == screeningId).Select(s => s.Room.Seats).FirstOrDefaultAsync();

            List<int> freeSeats = Enumerable.Range(1, seatsCount).ToList()
                .Except(seatsTaken).ToList();

            return freeSeats;
        }


        /// <summary>
        /// Handles the purchase of a ticket.
        /// </summary>
        /// <param name="ticket">The ticket to be purchased.</param>
        /// <returns>The action result after purchasing the ticket.</returns>
        [HttpPost("kup")]
        [Authorize]
        [ValidateAntiForgeryToken]
        // doesnt factor in the situation where the customer buys the same seat for the same screening twice
        public async Task<IActionResult> Buy([FromForm] Ticket ticket)
        {
            bool hasDiscount = (ticket.PricingType == PricingType.ULGOWY);

            var user = await _userManager.GetUserAsync(User);

            ticket.TicketPrice = (decimal)await CalculatePrice(ticket.ScreeningId, ticket.Seat);

            var ticketPrice = hasDiscount ? (ticket.TicketPrice / 2) : ticket.TicketPrice;
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserEmail == user.Email);

            Ticket newTicket = new Ticket
            {
                Seat = ticket.Seat,
                PricingType = ticket.PricingType,
                IsActive = false,
                TicketPrice = ticketPrice,
                ScreeningId = ticket.ScreeningId
            };

            Order activeOrder = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.AccountId == account.Id && o.OrderStatus == OrderStatus.PENDING);

            if (activeOrder == null)
            {
                Order order = new Order
                {
                    TotalPrice = 0,
                    AccountId = account.Id,
                };

                AssignTicketToOrder(newTicket, order);
            }

            else AssignTicketToOrder(newTicket, activeOrder);

            _dbContext.Add(newTicket);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Screenings");
        }


        private void AssignTicketToOrder(Ticket ticket, Order order)
        {
            ticket.OrderId = order.Id;
            ticket.Order = order;
        }

        /// <summary>
        /// Calculates the price for a ticket for a given screening and seat.
        /// </summary>
        /// <param name="screeningId">The ID of the screening.</param>
        /// <param name="seat">The seat number.</param>
        /// <returns>The calculated ticket price.</returns>
        [HttpGet("oblicz-cene")]
        public async Task<double> CalculatePrice(int screeningId, int seat)
        {
            var ticketPrice = 12.0 + (seat * 0.1);

            var screening = await _dbContext.Screenings.Where(s => s.Id == screeningId)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync();

            if (screening.Movie.Adult) ticketPrice += 2.0;
            if (screening.Is3D) ticketPrice += 2.0;

            var seatsLeft = (await GetFreeSeatsList(screeningId)).Count;

            if (seatsLeft < 10) ticketPrice += 3.0;

            DateTime oneYearAgo = DateTime.Now.AddYears(-1);
            bool isNewerThan1Year = (screening.Movie.ReleaseDate > oneYearAgo);

            if (isNewerThan1Year) ticketPrice += 4.0;

            return ticketPrice;
        }

        /// <summary>
        /// Removes a ticket from the user's order.
        /// </summary>
        /// <param name="id">The ID of the ticket to be removed.</param>
        /// <returns>The action result after removing the ticket.</returns>
        [HttpPost("usun/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveTicket(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserEmail == user.Email);

            var ticket = await _dbContext.Tickets
                .Include(t => t.Order)
                .FirstOrDefaultAsync(t => t.Id == id);

            var isUsersTicket = await _dbContext.Orders.AnyAsync(o => o.AccountId == account.Id && o.Id == ticket.OrderId && !ticket.IsActive);

            if (ticket != null && isUsersTicket)
            {
                _dbContext.Tickets.Remove(ticket);

                var orderWithoutTicket = ticket.Order;
                orderWithoutTicket.TotalPrice -= ticket.TicketPrice;
                _dbContext.Orders.Update(orderWithoutTicket);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "ShoppingCart");
        }

        /// <summary>
        /// Retrieves the view for creating a new ticket.
        /// </summary>
        /// <returns>The view for creating a new ticket.</returns>
        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var pricingTypes = Enum.GetValues(typeof(PricingType)).Cast<PricingType>();
            ViewBag.PricingTypes = new SelectList(pricingTypes);

            ViewData["OrderId"] = new SelectList(_dbContext.Orders, "Id", "Id");
            ViewData["ScreeningId"] = new SelectList(_dbContext.Screenings, "Id", "Id");
            return View();
        }

        /// <summary>
        /// Creates a new ticket.
        /// </summary>
        /// <param name="ticket">The ticket to be created.</param>
        /// <returns>The action result after creating the ticket.</returns>
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Seat,PricingType,TicketPrice,ScreeningId,OrderId,IsActive")] Ticket ticket)
        {
            _dbContext.Add(ticket);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Retrieves the view for editing an existing ticket.
        /// </summary>
        /// <param name="id">The ID of the ticket to be edited.</param>
        /// <returns>The view for editing an existing ticket.</returns>
        [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _dbContext.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _dbContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var pricingTypes = Enum.GetValues(typeof(PricingType)).Cast<PricingType>();
            ViewBag.PricingTypes = new SelectList(pricingTypes);

            ViewData["OrderId"] = new SelectList(_dbContext.Orders, "Id", "Id", ticket.OrderId);
            ViewData["ScreeningId"] = new SelectList(_dbContext.Screenings, "Id", "Id", ticket.ScreeningId);
            return View(ticket);
        }

        /// <summary>
        /// Updates an existing ticket.
        /// </summary>
        /// <param name="id">The ID of the ticket to be updated.</param>
        /// <param name="ticket">The updated ticket information.</param>
        /// <returns>The action result after updating the ticket.</returns>
        [HttpPost("edit/{id}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Seat,PricingType,TicketPrice,ScreeningId,OrderId,IsActive")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            _dbContext.Update(ticket);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = id });
        }

        /// <summary>
        /// Retrieves the view for deleting a ticket.
        /// </summary>
        /// <param name="id">The ID of the ticket to be deleted.</param>
        /// <returns>The view for deleting a ticket.</returns>
        [HttpGet("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _dbContext.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _dbContext.Tickets
                .Include(t => t.Order)
                .Include(t => t.Screening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        /// <summary>
        /// Deletes a ticket.
        /// </summary>
        /// <param name="id">The ID of the ticket to be deleted.</param>
        /// <returns>The action result after deleting the ticket.</returns>
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_dbContext.Tickets == null)
            {
                return Problem("Entity set 'CinemanicDbContext.Tickets'  is null.");
            }
            var ticket = await _dbContext.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _dbContext.Tickets.Remove(ticket);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a ticket with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the ticket.</param>
        /// <returns>True if the ticket exists, otherwise false.</returns>
        private bool TicketExists(int id)
        {
            return (_dbContext.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
