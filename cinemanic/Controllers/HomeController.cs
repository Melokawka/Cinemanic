using cinemanic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Net;

namespace cinemanic.Controllers
{
    [EnableCors("AllowWordPressApi")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            string apiEndpoint = "http://127.0.0.1:8080/wordpress/wp-json/wp/v2/posts";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            request.Method = "GET";
            request.ContentType = "application/json";

            WebResponse response = await request.GetResponseAsync();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();

            List<Post> posts = new List<Post>();
            posts = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);

            if (posts.Count > 0) return View(posts);

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