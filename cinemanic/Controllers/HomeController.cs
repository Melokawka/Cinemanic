using AutoMapper;
using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace cinemanic.Controllers
{

    //[EnableCors("AllowWordPressApi")]
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

        public async Task<List<Post>> GetPostsAsync(string apiEndpoint)
        {
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjEsIm5hbWUiOiJtZWxva2F3a2EiLCJpYXQiOjE2ODM3NjA5NjAsImV4cCI6MTg0MTQ0MDk2MH0.oTb_MWYE1VgPuC3-zcg2eDXedj8Xqel2hmiexvj0_Wg");
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var posts = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);

                foreach (var post in posts)
                {
                    int id = post.Id;
                    var apiPath = "http://localhost:8080/wp-json/wp/v2/posts/" + id + "?_embed";

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

                return posts;
            }
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string apiEndpoint = "http://127.0.0.1:8080/wp-json/wp/v2/posts?per_page=3";

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

            //foreach (MovieInfo info in MoviesInfo) foreach (var property in typeof(MovieInfo).GetProperties()) Console.WriteLine(property.Name + " = " + property.GetValue(info));

            //foreach (Post post in posts) foreach (var property in typeof(Post).GetProperties()) Console.WriteLine(property.Name + " = " + property.GetValue(post));

            var viewModel = new PostMovieViewModel
            {
                Posts = posts,
                MoviesInfo = MoviesInfo
            };

            return View(viewModel);
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