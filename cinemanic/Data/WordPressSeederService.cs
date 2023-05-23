using Bogus;
using cinemanic.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace cinemanic.Data
{
    public class WordPressSeederService
    {
        private readonly string _baseUrl;
        private readonly string _wordpressApiKey;
        private readonly HttpClient _httpClient;
        private readonly string[] imageUrls = { "https://images.unsplash.com/photo-1478720568477-152d9b164e26?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80",
                                                "https://images.unsplash.com/photo-1598899134739-24c46f58b8c0?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1156&q=80",
                                                "https://images.unsplash.com/photo-1536440136628-849c177e76a1?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1025&q=80"};

        public WordPressSeederService(string wordpressApiKey)
        {
            _wordpressApiKey = wordpressApiKey;
            _baseUrl = "http://localhost:8080";
            _httpClient = new HttpClient();
        }

        public async Task UploadImageFromUrl()
        {
            List<int> imageIDs = new();

            foreach (string imageUrl in imageUrls)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/wp-json/wp/v2/media");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _wordpressApiKey);

                var imageStream = await _httpClient.GetStreamAsync(imageUrl);
                var imageContent = new StreamContent(imageStream);

                var multipartContent = new MultipartFormDataContent
                {{ imageContent, "file", "image" + (imageIDs.Count+1) + ".jpg" }};
                request.Content = multipartContent;

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                JObject responseData = JObject.Parse(responseContent);
                int imageId = (int)responseData["id"];

                imageIDs.Add(imageId);
            }
            await CreateMockPosts(imageIDs);
        }

        public async Task CreateMockPosts(List<int> imageIDs)
        {
            bool hasExistingPosts = await CheckForExistingPosts();

            if (!hasExistingPosts)
            {
                var faker = new Faker();
                string loremText;

                int iterator = 3;
                foreach (int image in imageIDs)
                {
                    loremText = faker.Lorem.Paragraphs(3);
                    var encodedLoremText = HttpUtility.HtmlEncode(loremText);
                    await CreateMockPost("Post " + iterator, encodedLoremText, image);
                    iterator--;
                }
            }
        }

        private async Task CreateMockPost(string title, string content, int imageID)
        {
            string url = _baseUrl + "/wp-json/wp/v2/posts";

            HttpClient client = new HttpClient();

            var postData = new
            {
                title,
                content,
                featured_media = imageID,
                status = "publish"
            };

            var jsonBody = JsonSerializer.Serialize(postData);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _wordpressApiKey);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseContent);
        }

        public async Task<List<Post>> GetPostsAsync(string apiEndpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);

            string jsonResponse = await response.Content.ReadAsStringAsync();

            var posts = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);

            return posts;
        }

        private async Task<bool> CheckForExistingPosts()
        {
            string url = _baseUrl + "/wp-json/wp/v2/posts";

            var posts = await GetPostsAsync(url);

            return (posts != null && posts.Count > 1);
        }
    }
}
