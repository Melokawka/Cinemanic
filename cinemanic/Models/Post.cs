using Newtonsoft.Json;
using System.ComponentModel;

namespace cinemanic.Models
{
    /// <summary>
    /// Represents a post.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Gets or sets the post ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the rendered title of the post.
        /// </summary>
        public RenderedProperty Title { get; set; }

        /// <summary>
        /// Gets or sets the rendered content of the post.
        /// </summary>
        public RenderedProperty Content { get; set; }

        /// <summary>
        /// Gets or sets the URL of the featured media for the post.
        /// </summary>
        [JsonProperty("featured_media")]
        [DefaultValue("/placeholders/placeholder.jpg")]
        public string? FeaturedMediaUrl { get; set; }

        /// <summary>
        /// Gets or sets the link to the post.
        /// </summary>
        public string Link { get; set; }
    }

    /// <summary>
    /// Represents a rendered property.
    /// </summary>
    public class RenderedProperty
    {
        /// <summary>
        /// Gets or sets the rendered value.
        /// </summary>
        [JsonProperty("rendered")]
        public string Rendered { get; set; }
    }
}
