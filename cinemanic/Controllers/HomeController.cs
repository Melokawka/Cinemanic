using AutoMapper;
using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace cinemanic.Controllers
{

    [EnableCors("AllowWordPressApi")]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly CinemanicDbContext _dbContext;
        public HomeController(CinemanicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult GetAllMovies()
        {
            var movies = _dbContext.Movies.ToList();

            return Ok(movies);
        }

        [HttpGet]
        [Route("getpostsjson")]
        public async Task<string> GetPostsJsonAsync(string apiEndpoint)
        {
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjEsIm5hbWUiOiJtZWxva2F3a2EiLCJpYXQiOjE2ODM3NjA5NjAsImV4cCI6MTg0MTQ0MDk2MH0.oTb_MWYE1VgPuC3-zcg2eDXedj8Xqel2hmiexvj0_Wg");
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                string jsonResponse = await response.Content.ReadAsStringAsync();

                return jsonResponse;
            }
        }

        [HttpGet("pobierz-posty")]
        public async Task<List<Post>> GetPosts(int page)
        {
            string jsonResponse = await GetPostsJsonAsync("http://127.0.0.1:8080/wp-json/wp/v2/posts?per_page=3&page=" + page);

            var posts = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);

            foreach (var post in posts)
            {
                int id = post.Id;
                var apiPath = "http://localhost:8080/wp-json/wp/v2/posts/" + id + "?_embed";

                using (HttpClient client = new())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjEsIm5hbWUiOiJtZWxva2F3a2EiLCJpYXQiOjE2ODM3NjA5NjAsImV4cCI6MTg0MTQ0MDk2MH0.oTb_MWYE1VgPuC3-zcg2eDXedj8Xqel2hmiexvj0_Wg");

                    HttpResponseMessage mediaResponse = await client.GetAsync(apiPath);

                    string mediaJsonResponse = await mediaResponse.Content.ReadAsStringAsync();

                    JObject mediaJsonObject = JObject.Parse(mediaJsonResponse);

                    JToken featuredMedia = mediaJsonObject["_embedded"]["wp:featuredmedia"].FirstOrDefault();
                    if (featuredMedia != null)
                    {
                        string mediaUrl = featuredMedia["source_url"].ToString();
                        post.FeaturedMediaUrl = mediaUrl;
                    }
                }
            }
            return posts;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await GetPosts(1);

            var movies = _dbContext.Movies
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
            var MoviesInfo = mapper.Map<List<Movie>, List<MovieInfo>>(movies);

            foreach (var movie in MoviesInfo)
            {
                movie.Screenings = movie.Screenings.OrderBy(s => s.ScreeningDate).ToList();
            }

            foreach (Post post in posts) foreach (var property in typeof(Post).GetProperties()) Console.WriteLine(property.Name + " = " + property.GetValue(post));

            var viewModel = new PostMovieViewModel
            {
                Posts = posts,
                MoviesInfo = MoviesInfo
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}