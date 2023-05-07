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

        public static string FindTrailerKey(string json)
        {
            var jObject = JObject.Parse(json);
            var jArray = (JArray)jObject["results"];

            foreach (var item in jArray)
            {
                var type = (string)item["type"];
                if (type == "Trailer")
                {
                    return (string)item["key"];
                }
            }

            return null;
        }

        public async Task<IActionResult> Index()
        {
            string apiEndpoint = "http://127.0.0.1:8080/wordpress/wp-json/wp/v2/posts";

            Random random = new Random();
            int movieId;
            string apiEndpoint2;
            string apiEndpoint3;
            string apiEndpoint4;
            HttpResponseMessage response2;
            
            var httpClient = new HttpClient();

            do
            {
                movieId = random.Next(10000, 100001);
                Console.WriteLine(movieId);
                apiEndpoint2 = $"https://api.themoviedb.org/3/movie/{movieId}?api_key=4446cb535a867cc6db4c689c8ebc7d97";
                apiEndpoint3 = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key=4446cb535a867cc6db4c689c8ebc7d97";
                apiEndpoint4 = $"https://api.themoviedb.org/3/genre/movie/list?api_key=4446cb535a867cc6db4c689c8ebc7d97";


                response2 = await httpClient.GetAsync(apiEndpoint2);

            } while (!response2.IsSuccessStatusCode);

            var posts = await GetPostsAsync(apiEndpoint);

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new MovieConverter());

            var response = await httpClient.GetFromJsonAsync<Movie>(apiEndpoint2, jsonOptions);

            var json = await httpClient.GetStringAsync(apiEndpoint3);

            var trailerLink = FindTrailerKey(json);

            response.Trailer = "https://www.youtube.com/watch?v=" + trailerLink;
            response.PosterPath = "https://image.tmdb.org/t/p/w200" + response.PosterPath;

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //_dbContext.Database.ExecuteSqlRaw("DELETE FROM movies WHERE id < 1000");

                _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Genres ON");
                 //Create transfer object and add to transfer table
                foreach (Genre genre in response.Genres)
                {
                    //var movieGenre = new MovieGenre { Movie = response, Genre = genre };
                    //_dbContext.MovieGenre.Add(movieGenre);
                }
                
                _dbContext.Movies.Add(response);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            var movies = ((OkObjectResult)GetAllMovies()).Value as List<Movie>;

            var viewModel = new PostMovieViewModel
            {
                Posts = posts,
                Movies = movies
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