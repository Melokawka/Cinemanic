using cinemanic.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace cinemanic.Data
{
    public class PostService
    {
        private readonly HttpClient _httpClient;

        public PostService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjEsIm5hbWUiOiJtZWxva2F3a2EiLCJpYXQiOjE2ODM3NjA5NjAsImV4cCI6MTg0MTQ0MDk2MH0.oTb_MWYE1VgPuC3-zcg2eDXedj8Xqel2hmiexvj0_Wg");
        }

        public async Task<List<Post>> GetPosts(int page, int postsPerPage)
        {
            string jsonResponse = await GetPostsJsonAsync($"http://127.0.0.1:8080/wp-json/wp/v2/posts?per_page={postsPerPage}&page={page}");

            var posts = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);

            foreach (var post in posts)
            {
                int id = post.Id;
                var apiPath = $"http://localhost:8080/wp-json/wp/v2/posts/{id}?_embed";

                string mediaJsonResponse = await GetPostMediaJsonAsync(apiPath);

                JObject mediaJsonObject = JObject.Parse(mediaJsonResponse);

                try
                {
                    JToken featuredMedia = mediaJsonObject["_embedded"]["wp:featuredmedia"].FirstOrDefault();
                    post.FeaturedMediaUrl = featuredMedia["source_url"].ToString();
                }
                catch (Exception ex)
                {
                    post.FeaturedMediaUrl = "";
                }
            }

            return posts;
        }

        public async Task<string> GetPostsJsonAsync(string apiEndpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }

        public async Task<string> GetPostMediaJsonAsync(string apiPath)
        {
            HttpResponseMessage mediaResponse = await _httpClient.GetAsync(apiPath);
            string mediaJsonResponse = await mediaResponse.Content.ReadAsStringAsync();
            return mediaJsonResponse;
        }

        public async Task<int> GetMaxPageFromResponse(int postsPerPage)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"http://127.0.0.1:8080/wp-json/wp/v2/posts?per_page={postsPerPage}");

            if (response.Headers.TryGetValues("X-WP-TotalPages", out var values))
            {
                string maxPage = values.FirstOrDefault();

                return int.Parse(maxPage);
            }

            // The response is an empty array or an unexpected format, default to 1 page
            return 1;
        }
    }
}
