using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace cinemanic.Controllers
{

    [EnableCors("AllowWordPressApi")]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly PostService _postService;
        private readonly MovieService _movieService;
        public HomeController(PostService postService, MovieService movieService)
        {
            _postService = postService;
            _movieService = movieService;
        }

        [HttpGet]
        [Route("maksimum-paginacji-postow")]
        public async Task<int> GetMaxPage()
        {
            return await _postService.GetMaxPageFromResponse(3);
        }

        [HttpGet]
        [Route("posty-json")]
        public async Task<string> GetPostsJsonAsync(string apiEndpoint)
        {
            return await _postService.GetPostsJsonAsync(apiEndpoint);
        }

        [HttpGet("pobierz-posty")]
        public async Task<List<Post>> GetPosts(int page)
        {
            return await _postService.GetPosts(page, 3);
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetPosts(1, 3);

            var movies = _movieService.GetMoviesInfo();

            var viewModel = new PostMovieViewModel
            {
                Posts = posts,
                MoviesInfo = movies
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