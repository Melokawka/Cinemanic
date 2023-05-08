using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Controllers
{
    [Route("seanse")]
    public class old : Controller
    {
        private readonly CinemanicDbContext _dbContext;
        public old(CinemanicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: HomeController1
        [HttpGet("")]
        public ActionResult Index()
        {
            var screenings = _dbContext.Screenings.Include(s => s.Movie).Include(s => s.Room).ToList();

            return View(screenings);
        }

        // GET: HomeController1/Details/5
        [HttpGet("{id}")]
        public ActionResult Details(int id)
        {
            var screenings = _dbContext.Screenings.Where(a => a.Id == id).Include(s => s.Movie).Include(s => s.Room).FirstOrDefault();

            ViewBag.RoomId = new SelectList(_dbContext.Rooms, "Id", "Id");
            ViewBag.MovieId = new SelectList(_dbContext.Movies, "Id", "Title");

            return View(screenings);
        }

        // GET: HomeController1/Create
        [HttpGet("create")]
        public ActionResult Create()
        {

            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        //[HttpGet("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Edit/5
        [HttpGet("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var screening = _dbContext.Screenings.Include(s => s.Movie).Include(s => s.Room).FirstOrDefault(s => s.Id == id);

            ViewBag.RoomId = new SelectList(_dbContext.Rooms, "Id", "Id");
            ViewBag.MovieId = new SelectList(_dbContext.Movies, "Id", "Title");

            return View(screening);
        }

        // POST: HomeController1/Edit/5
        [HttpPost]
        //[HttpGet("edit/{id}")]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Delete/5
        [HttpGet("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var screenings = _dbContext.Screenings.Where(a => a.Id == id).Include(s => s.Movie).Include(s => s.Room).FirstOrDefault();

            return View(screenings);
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        //[HttpGet("delete/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
