namespace cinemanic.Models
{
    /// <summary>
    /// Represents the view model for posts and movie information.
    /// </summary>
    public class PostMovieViewModel
    {
        /// <summary>
        /// Gets or sets the list of posts.
        /// </summary>
        public List<Post> Posts { get; set; }

        /// <summary>
        /// Gets or sets the list of movie information.
        /// </summary>
        public List<MovieInfo> MoviesInfo { get; set; }
    }
}
