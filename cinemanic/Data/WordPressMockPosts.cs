using cinemanic.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace cinemanic.Data
{
    public class WordPressMockPosts
    {
        private readonly string _baseUrl = "http://localhost:8080";
        //private readonly string _username;
        //private readonly string _password;

        public async Task UploadImageFromUrl()
        {
            string[] urls = { "https://images.unsplash.com/photo-1478720568477-152d9b164e26?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80",
                              "https://images.unsplash.com/photo-1598899134739-24c46f58b8c0?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1156&q=80",
                              "https://images.unsplash.com/photo-1536440136628-849c177e76a1?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1025&q=80"};

            int imageNr = 1;
            List<int> imageIDs = new();

            foreach (string imageUrl in urls)
            {
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/wp-json/wp/v2/media");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjEsIm5hbWUiOiJtZWxva2F3a2EiLCJpYXQiOjE2ODM3NjA5NjAsImV4cCI6MTg0MTQ0MDk2MH0.oTb_MWYE1VgPuC3-zcg2eDXedj8Xqel2hmiexvj0_Wg");

                var imageStream = await client.GetStreamAsync(imageUrl);
                var imageContent = new StreamContent(imageStream);
                //imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(imageContent, "file", "image" + imageNr + ".jpg");
                request.Content = multipartContent;

                Console.WriteLine(imageUrl);

                try
                {
                    var response = await client.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the response to get the image ID
                    JObject responseData = JObject.Parse(responseContent);
                    int imageId = (int)responseData["id"];

                    Console.WriteLine("Image uploaded with ID: " + imageId);
                    imageIDs.Add(imageId);
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }

                imageNr++;
            }
            CreateMockPosts(imageIDs);
        }

        public async void CreateMockPosts(List<int> imageIDs)
        {
            // Check if there are any existing posts in the site
            bool hasExistingPosts = await CheckForExistingPosts();

            // If there is only one post in the site, create some mock posts
            if (!hasExistingPosts)
            {
                int iterator = 1;
                foreach (int image in imageIDs) CreateMockPost("Mock Post " + iterator, "This is a mock post.", image);
            }
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
                    return null;
                }
            }
        }

        private async Task<bool> CheckForExistingPosts()
        {
            // Set up the request to get the posts from WordPress
            string url = _baseUrl + "/wp-json/wp/v2/posts";

            var posts = await GetPostsAsync(url);

            // Return true if there is more than one post in the site
            return (posts != null && posts.Count > 1);
        }

        private async void CreateMockPost(string title, string content, int imageID)
        {
            // Set up the request to create the post in WordPress
            string url = _baseUrl + "/wp-json/wp/v2/posts";

            HttpClient client = new HttpClient();

            // Set up the data for the new post
            string postData = "{ \"title\": \"" + title + "\", \"content\": \"" + content + "\", \"featured_media\": " + imageID + ", \"status\": \"publish\" }";


            byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData);

            // Create a new HttpRequestMessage object and set its content to the post data
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjEsIm5hbWUiOiJtZWxva2F3a2EiLCJpYXQiOjE2ODM3NjA5NjAsImV4cCI6MTg0MTQ0MDk2MH0.oTb_MWYE1VgPuC3-zcg2eDXedj8Xqel2hmiexvj0_Wg");
            request.Content = new ByteArrayContent(postDataBytes);

            // Set the content type of the request to application/json
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Send the request and get the response
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Console.WriteLine(responseContent);
        }

    }
}
