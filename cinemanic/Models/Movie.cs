namespace cinemanic.Models
{
    /// <summary>
    /// Represents a movie.
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Gets or sets the ID of the movie.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the release date of the movie.
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the movie in minutes.
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the movie is made in Polish.
        /// </summary>
        public bool PolishMade { get; set; }

        /// <summary>
        /// Gets or sets the description of the movie.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the budget of the movie.
        /// </summary>
        public int? Budget { get; set; }

        /// <summary>
        /// Gets or sets the trailer of the movie.
        /// </summary>
        public string Trailer { get; set; }

        /// <summary>
        /// Gets or sets the poster path of the movie.
        /// </summary>
        public string PosterPath { get; set; }

        /// <summary>
        /// Gets or sets the TMDB popularity of the movie.
        /// </summary>
        public int? TmdbPopularity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the movie is for adults only.
        /// </summary>
        public bool Adult { get; set; }

        /// <summary>
        /// Gets the list of screenings associated with the movie.
        /// </summary>
        public List<Screening> Screenings { get; } = new();

        /// <summary>
        /// Gets the list of likes for the movie.
        /// </summary>
        public List<Like> Likes { get; } = new();

        /// <summary>
        /// Gets or sets the list of genres associated with the movie.
        /// </summary>
        public List<Genre> Genres { get; set; } = new();
    }
}
