using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cinemanic.Data;
using cinemanic.Models;

namespace cinemanic.Controllers
{
    [Route("seanse")]
    public class ScreeningsController : Controller
    {
        private readonly CinemanicDbContext _context;

        public ScreeningsController(CinemanicDbContext context)
        {
            _context = context;
        }

        // GET: Screenings1
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cinemanicDbContext = _context.Screenings.Include(s => s.Movie).Include(s => s.Room);
            return View(await cinemanicDbContext.ToListAsync());
        }

        // GET: Screenings1/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Screenings == null)
            {
                return NotFound();
            }

            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (screening == null)
            {
                return NotFound();
            }

            return View(screening);
        }

        // GET: Screenings1/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id");
            return View();
        }

        // POST: Screenings1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(screening);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", screening.MovieId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", screening.RoomId);
            return View(screening);
        }

        // GET: Screenings1/Edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Screenings == null)
            {
                return NotFound();
            }

            var screening = await _context.Screenings.FindAsync(id);
            if (screening == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", screening.MovieId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", screening.RoomId);
            return View(screening);
        }

        // POST: Screenings1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            if (id != screening.Id)
            {
                return NotFound();
            }

            //if (!ModelState.IsValid)
            //{
                try
                {
                    Console.WriteLine("troll");
                    _context.Update(screening);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScreeningExists(screening.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            //foreach (var error in errors)
            //{
            //    Console.WriteLine(error.ErrorMessage);
            //}

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", screening.MovieId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", screening.RoomId);
            return View(screening);
        }

        // GET: Screenings1/Delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Screenings == null)
            {
                return NotFound();
            }

            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (screening == null)
            {
                return NotFound();
            }

            return View(screening);
        }

        // POST: Screenings1/Delete/5
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Screenings == null)
            {
                return Problem("Entity set 'CinemanicDbContext.Screenings'  is null.");
            }
            var screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                _context.Screenings.Remove(screening);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScreeningExists(int id)
        {
          return (_context.Screenings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
