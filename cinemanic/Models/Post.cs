using Newtonsoft.Json;

namespace cinemanic.Models
{
    public class Post
    {
        public RenderedProperty Title { get; set; }
        public RenderedProperty Content { get; set; }
    }

    public class RenderedProperty
    {
        [JsonProperty("rendered")]
        public string Rendered { get; set; }
    }
}