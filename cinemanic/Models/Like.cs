namespace cinemanic.Models
{
    /// <summary>
    /// Represents a user's like for a movie.
    /// </summary>
    public class Like
    {
        /// <summary>
        /// Gets or sets the ID of the like.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the movie the like belongs to.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the account that made the like.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the movie the like belongs to.
        /// </summary>
        public Movie Movie { get; set; } = null!;

        /// <summary>
        /// Gets or sets the account that made the like.
        /// </summary>
        public Account Account { get; set; } = null!;
    }
}
