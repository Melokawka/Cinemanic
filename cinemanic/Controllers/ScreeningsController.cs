using cinemanic.Data;
using cinemanic.Models;
using cinemanic.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    /// <summary>
    /// Controller for handling screenings-related actions.
    /// </summary>
    [Route("seanse")]
    public class ScreeningsController : Controller
    {
        private readonly CinemanicDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreeningsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ScreeningsController(CinemanicDbContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Retrieves a list of screenings.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <returns>The view displaying the list of screenings.</returns>
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

        /// <summary>
        /// Retrieves details of a specific movie screening.
        /// </summary>
        /// <param name="id">The ID of the movie.</param>
        /// <returns>The view displaying the details of the movie screening.</returns>
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

        /// <summary>
        /// Retrieves a list of screenings for administrative purposes.
        /// </summary>
        /// <returns>The view displaying the list of screenings for admin.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> Admin()
        {
            var cinemanicDbContext = _dbContext.Screenings.Include(s => s.Movie).Include(s => s.Room);
            return View(await cinemanicDbContext.ToListAsync());
        }

        /// <summary>
        /// Retrieves a list of archived screenings for administrative purposes.
        /// </summary>
        /// <returns>The view displaying the list of archived screenings for admin.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/archiwum")]
        public async Task<IActionResult> AdminArchive()
        {
            var cinemanicDbContext = _dbContext.ArchivedScreenings.Include(s => s.Movie);
            return View(await cinemanicDbContext.ToListAsync());
        }

        /// <summary>
        /// Displays the create screening form for admin.
        /// </summary>
        /// <returns>The view displaying the create screening form.</returns>
        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<Movie> movies = _dbContext.Movies.ToList();

            ViewData["MovieId"] = new SelectList(movies, "Id", "Title");
            ViewData["RoomId"] = new SelectList(_dbContext.Rooms, "Id", "Id");
            return View();
        }

        /// <summary>
        /// Creates a new screening.
        /// </summary>
        /// <param name="screening">The screening object to create.</param>
        /// <returns>Redirects to the admin view after successful creation.</returns>
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == screening.RoomId);
            screening.SeatsLeft = room.Seats;

            _dbContext.Add(screening);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Displays the edit screening form for admin.
        /// </summary>
        /// <param name="id">The ID of the screening to edit.</param>
        /// <returns>The view displaying the edit screening form.</returns>
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

        /// <summary>
        /// Updates an existing screening.
        /// </summary>
        /// <param name="id">The ID of the screening to update.</param>
        /// <param name="screening">The updated screening object.</param>
        /// <returns>Redirects to the admin view after successful update.</returns>
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

        /// <summary>
        /// Displays the delete screening form for admin.
        /// </summary>
        /// <param name="id">The ID of the screening to delete.</param>
        /// <returns>The view displaying the delete screening form.</returns>
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

        /// <summary>
        /// Deletes a screening.
        /// </summary>
        /// <param name="id">The ID of the screening to delete.</param>
        /// <returns>Redirects to the admin view after successful deletion.</returns>
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

        /// <summary>
        /// Checks if a screening exists.
        /// </summary>
        /// <param name="id">The ID of the screening.</param>
        /// <returns><c>true</c> if the screening exists; otherwise, <c>false</c>.</returns>
        private bool ScreeningExists(int id)
        {
            return (_dbContext.Screenings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
