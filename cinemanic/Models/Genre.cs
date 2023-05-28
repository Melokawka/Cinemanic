namespace cinemanic.Models
{
    /// <summary>
    /// Represents a genre of movies.
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Gets or sets the ID of the genre.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the genre.
        /// </summary>
        public string GenreName { get; set; }

        /// <summary>
        /// Gets or sets the list of movies associated with the genre.
        /// </summary>
        public List<Movie> Movies { get; set; } = new();
    }
}
