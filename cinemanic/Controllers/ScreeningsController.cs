using AutoMapper;
using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            var movies = _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Screenings)
                .ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movie, MovieInfo>()
                    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.GenreName).ToList()))
                    .ForMember(dest => dest.Screenings, opt => opt.MapFrom(src => src.Screenings.Select(s => s).ToList()));

                cfg.CreateMap<Genre, string>().ConvertUsing(g => g.GenreName);
            });

            IMapper mapper = config.CreateMapper();
            var moviesInfo = mapper.Map<List<Movie>, List<MovieInfo>>(movies);

            foreach (var movie in moviesInfo)
            {
                movie.Screenings = movie.Screenings.OrderBy(s => s.ScreeningDate).ToList();
            }

            var uniqueDates = new List<string>();

            foreach (var movie in moviesInfo)
            {
                foreach (var screening in movie.Screenings)
                {
                    if (!uniqueDates.Contains(screening.ScreeningDate.Date.ToString("dd-MM-yyyy")))
                    {
                        uniqueDates.Add(screening.ScreeningDate.Date.ToString("dd-MM-yyyy"));
                    }
                }
            }

            var sortedDates = uniqueDates.OrderBy(date => DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList();

            int pageSize = 1; // Number of unique dates per page
            int pageNumber = page ?? 1; // Current page number

            var paginatedDates = sortedDates.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)sortedDates.Count / pageSize);

            var filteredMovies = moviesInfo.Where(movie => movie.Screenings.Any(screening => screening.ScreeningDate.Date.ToString("dd-MM-yyyy") == paginatedDates.FirstOrDefault())).ToList();

            var currentPaginatedDate = paginatedDates.FirstOrDefault();

            var model = new ScreeningViewModel
            {
                MoviesInfo = filteredMovies,
                CurrentPaginatedDate = currentPaginatedDate
            };

            return View(model);
        }


        [Authorize]
        [HttpGet("admin")]
        public async Task<IActionResult> Admin()
        {
            var cinemanicDbContext = _context.Screenings.Include(s => s.Movie).Include(s => s.Room);
            return View(await cinemanicDbContext.ToListAsync());
        }

        [HttpGet("film/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Screenings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movie, MovieInfo>()
                    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.GenreName).ToList()))
                    .ForMember(dest => dest.Screenings, opt => opt.MapFrom(src => src.Screenings.Select(s => s).ToList()));

                cfg.CreateMap<Genre, string>().ConvertUsing(g => g.GenreName);
            });

            IMapper mapper = config.CreateMapper();
            var movieInfo = mapper.Map<Movie, MovieInfo>(movie);

            movieInfo.Screenings = movieInfo.Screenings.OrderBy(s => s.ScreeningDate).ToList();

            return View(movieInfo);
        }

        [HttpGet("create")]
        [Authorize]
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id");
            return View();
        }

        [HttpPost("create")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            _context.Add(screening);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        [HttpGet("edit/{id}")]
        [Authorize]
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

        [HttpPost("edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScreeningDate,Subtitles,Lector,Dubbing,Is3D,SeatsLeft,RoomId,MovieId")] Screening screening)
        {
            if (id != screening.Id)
            {
                return NotFound();
            }

            _context.Update(screening);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }

        [HttpGet("delete/{id}")]
        [Authorize]
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

        [HttpPost("delete/{id}"), ActionName("Delete")]
        [Authorize]
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
            return RedirectToAction(nameof(Admin));
        }

        private bool ScreeningExists(int id)
        {
            return (_context.Screenings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
