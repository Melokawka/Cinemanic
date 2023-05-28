using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace cinemanic.Controllers
{

    /// <summary>
    /// Represents the controller for handling actions related to the home page and related functionality.
    /// </summary>
    [EnableCors("AllowWordPressApi")]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly PostService _postService;
        private readonly MovieService _movieService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CinemanicDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="postService">The post service.</param>
        /// <param name="movieService">The movie service.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="dbContext">The database context.</param>
        public HomeController(PostService postService, MovieService movieService, UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext)
        {
            _postService = postService;
            _movieService = movieService;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the maximum page number for post pagination.
        /// </summary>
        /// <returns>The maximum page number.</returns>
        [HttpGet]
        [Route("maksimum-paginacji-postow")]
        public async Task<int> GetMaxPage()
        {
            return await _postService.GetMaxPageFromResponse(3);
        }

        /// <summary>
        /// Handles the newsletter subscription.
        /// </summary>
        /// <returns>The result of the subscription attempt.</returns>
        [HttpGet("newsletter-zapis")]
        [Authorize]
        public async Task<IActionResult> Newsletter()
        {
            var user = await _userManager.GetUserAsync(User);
            var accountId = await _dbContext.Accounts
                .Where(a => a.UserEmail == user.Email)
                .Select(a => a.Id)
                .SingleAsync();

            var newsletterClient = new NewsletterClient { AccountId = accountId };
            bool isNewsletterClient = await _dbContext.NewsletterClients.AnyAsync(n => n.AccountId == accountId);

            if (!isNewsletterClient)
            {
                _dbContext.NewsletterClients.Add(newsletterClient);
                await _dbContext.SaveChangesAsync();
            }

            return Content("Dziekujemy, mozesz wypisac sie w kazdej chwili.", "text/html");
        }

        /// <summary>
        /// Retrieves posts in JSON format from the specified API endpoint.
        /// </summary>
        /// <param name="apiEndpoint">The API endpoint for retrieving posts.</param>
        /// <returns>The posts in JSON format.</returns>
        [HttpGet]
        [Route("posty-json")]
        public async Task<string> GetPostsJsonAsync(string apiEndpoint)
        {
            return await _postService.GetPostsJsonAsync(apiEndpoint);
        }

        /// <summary>
        /// Retrieves a list of posts based on the specified page number.
        /// </summary>
        /// <param name="page">The page number for retrieving posts.</param>
        /// <returns>A list of posts.</returns>
        [HttpGet("pobierz-posty")]
        public async Task<List<Post>> GetPosts(int page)
        {
            return await _postService.GetPosts(page, 3);
        }

        /// <summary>
        /// Handles the rendering of the home page.
        /// </summary>
        /// <returns>The view for the home page.</returns>
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

        /// <summary>
        /// Handles errors and displays the error view.
        /// </summary>
        /// <returns>The error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}