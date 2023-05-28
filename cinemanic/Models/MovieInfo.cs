namespace cinemanic.Models
{
    /// <summary>
    /// Represents movie information.
    /// </summary>
    public class MovieInfo
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
        /// Gets or sets a value indicating whether the movie is Polish-made.
        /// </summary>
        public bool PolishMade { get; set; }

        /// <summary>
        /// Gets or sets the description of the movie.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the movie trailer.
        /// </summary>
        public string Trailer { get; set; }

        /// <summary>
        /// Gets or sets the file path of the movie poster.
        /// </summary>
        public string PosterPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the movie is for adults only.
        /// </summary>
        public bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the list of genres associated with the movie.
        /// </summary>
        public List<string> Genres { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of screenings for the movie.
        /// </summary>
        public List<Screening> Screenings { get; set; } = new();
    }
}
