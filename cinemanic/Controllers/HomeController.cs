using cinemanic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using cinemanic.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using static Bogus.DataSets.Name;
using System.Security.Cryptography.Xml;
using AutoMapper;

namespace cinemanic.Controllers
{
    [EnableCors("AllowWordPressApi")]
    public class HomeController : Controller
    {
        private readonly CinemanicDbContext _dbContext;
        public HomeController(CinemanicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movies = _dbContext.Movies.ToList();

            return Ok(movies);
        }

        public async Task<List<Post>> GetPostsAsync(string apiEndpoint)
        {
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var posts = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);
                    return posts;
                }
                else
                {
                    // handle error
                    return null;
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            string apiEndpoint = "http://127.0.0.1:8080/wordpress/wp-json/wp/v2/posts";

            var posts = await GetPostsAsync(apiEndpoint);

            var movies = ((OkObjectResult)GetAllMovies()).Value as List<Movie>;

            _dbContext.Movies.Include(m => m.Genres).ToList();

            List<MovieInfo> MoviesInfo = new();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movie, MovieInfo>()
                    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.GenreName).ToList()));

                cfg.CreateMap<Genre, string>().ConvertUsing(g => g.GenreName);
            });

            IMapper mapper = config.CreateMapper();

            foreach (Movie movie in movies) MoviesInfo.Add(mapper.Map<MovieInfo>(movie));

            var viewModel = new PostMovieViewModel
            {
                Posts = posts,
                MoviesInfo = MoviesInfo
            };

            if (posts != null && posts.Count > 0) return View(viewModel);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}