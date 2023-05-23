using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    [Route("seanse")]
    public class ScreeningsController : Controller
    {
        private readonly CinemanicDbContext _dbContext;

        public ScreeningsController(CinemanicDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            var movies = _dbContext.Movies
                .Include(m => m.Genres)
                .Include(m => m.Screenings)
                .ToList();

            var moviesInfo = ScreeningFunctions.MapMoviesToMovieInfo(movies);

            ScreeningFunctions.SortScreeningsByDate(moviesInfo);

            var sortedDates = ScreeningFunctions.GetSortedUniqueDates(moviesInfo);

            int pageSize = 3; // dates per page

            var paginatedDates = ScreeningFunctions.GetPaginatedDates(sortedDates, page ?? 1, pageSize).ToList();

            ViewBag.CurrentPage = page ?? 1;
            ViewBag.TotalPages = (int)Math.Ceiling((double)sortedDates.Count / pageSize);

            var filteredMovies = moviesInfo
                .Where(movie => movie.Screenings
                    .Any(screening => paginatedDates.Contains(screening.ScreeningDate.Date.ToString("dd-MM-yyyy"))))
                .ToList();

            var model = new ScreeningViewModel
            {
                MoviesInfo = filteredMovies,
                CurrentPaginatedDates = paginatedDates
            };

            return View(model);
        }

        [HttpGet("film/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            var movie = await _dbContext.Movies
                .Include(m => m.Genres)
                .Include(m => m.Screenings)
                .FirstOrDefaultAsync(m => m.Id == id);

            var movieInfo = ScreeningFunctions.MapMoviesToMovieInfo(new List<Movie> { movie });

            ScreeningFunctions.SortScreeningsByDate(movieInfo);

            return View(movieInfo[0]);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> Admin()
        {
            var cinemanicDbContext = _dbContext.Screenings.Include(s => s.Movie).Include(s => s.Room);
            return View(await cinemanicDbContext.ToListAsync());
        }



        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_dbContext.Movies, "Id", "Id");
            ViewData["RoomId"] = new SelectList(_dbContext.Rooms, "Id", "Id");
            return View();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            _dbContext.Add(screening);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _dbContext.Screenings == null)
            {
                return NotFound();
            }

            var screening = await _dbContext.Screenings.FindAsync(id);
            if (screening == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_dbContext.Movies, "Id", "Id", screening.MovieId);
            ViewData["RoomId"] = new SelectList(_dbContext.Rooms, "Id", "Id", screening.RoomId);
            return View(screening);
        }

        [HttpPost("edit/{id}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            if (id != screening.Id)
            {
                return NotFound();
            }

            _dbContext.Update(screening);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        [HttpGet("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _dbContext.Screenings == null)
            {
                return NotFound();
            }

            var screening = await _dbContext.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (screening == null)
            {
                return NotFound();
            }

            return View(screening);
        }

        [HttpPost("delete/{id}"), ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_dbContext.Screenings == null)
            {
                return Problem("Entity set 'CinemanicDbContext.Screenings'  is null.");
            }
            var screening = await _dbContext.Screenings.FindAsync(id);
            if (screening != null)
            {
                _dbContext.Screenings.Remove(screening);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        private bool ScreeningExists(int id)
        {
            return (_dbContext.Screenings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
