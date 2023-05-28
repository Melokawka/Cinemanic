using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    /// <summary>
    /// Controller for managing orders.
    /// </summary>
    [Route("zamowienia")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly CinemanicDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="context">The CinemanicDbContext instance.</param>
        public OrdersController(CinemanicDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the admin page with a list of orders.
        /// </summary>
        /// <returns>The view for the admin page.</returns>
        public async Task<IActionResult> Admin()
        {
            var cinemanicDbContext = _context.Orders.Include(o => o.Account);
            return View(await cinemanicDbContext.ToListAsync());
        }

        /// <summary>
        /// Displays details for a specific order.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>The view for the order details page.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        /// <summary>
        /// Displays the page for creating a new order.
        /// </summary>
        /// <returns>The view for the order creation page.</returns>
        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            return View();
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="order">The order object to be created.</param>
        /// <returns>The result of the order creation process.</returns>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TotalPrice,AccountId")] Order order)
        {
            order.OrderStatus = OrderStatus.COMPLETED;

            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Displays the page for editing an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to be edited.</param>
        /// <returns>The view for the order editing page.</returns>
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", order.AccountId);
            return View(order);
        }

        /// <summary>
        /// Edits an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to be edited.</param>
        /// <param name="order">The updated order object.</param>
        /// <returns>The result of the order editing process.</returns>
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TotalPrice,AccountId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.COMPLETED;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Displays the page for deleting an order.
        /// </summary>
        /// <param name="id">The ID of the order to be deleted.</param>
        /// <returns>The view for the order deletion page.</returns>
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        /// <summary>
        /// Deletes a specific order.
        /// </summary>
        /// <param name="id">The ID of the order to be deleted.</param>
        /// <returns>The result of the order deletion process.</returns>
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'CinemanicDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
