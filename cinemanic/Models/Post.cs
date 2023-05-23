using Newtonsoft.Json;
using System.ComponentModel;

namespace cinemanic.Models
{
    public class Post
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public RenderedProperty Title { get; set; }
        public RenderedProperty Content { get; set; }

        [JsonProperty("featured_media")]
        [DefaultValue("/placeholders/placeholder.jpg")]
        public string? FeaturedMediaUrl { get; set; }
        public string Link { get; set; }
    }

    public class RenderedProperty
    {
        [JsonProperty("rendered")]
        public string Rendered { get; set; }
    }
}