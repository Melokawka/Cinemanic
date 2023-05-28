namespace cinemanic.Models
{
    /// <summary>
    /// Represents a screening of a movie.
    /// </summary>
    public class Screening
    {
        /// <summary>
        /// Gets or sets the screening ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the screening.
        /// </summary>
        public DateTime ScreeningDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screening has subtitles.
        /// </summary>
        public bool Subtitles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screening has a lector.
        /// </summary>
        public bool Lector { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screening has dubbing.
        /// </summary>
        public bool Dubbing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screening is in 3D.
        /// </summary>
        public bool Is3D { get; set; }

        /// <summary>
        /// Gets or sets the number of seats left for the screening.
        /// </summary>
        public int SeatsLeft { get; set; }

        /// <summary>
        /// Gets or sets the associated room.
        /// </summary>
        public Room Room { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the associated room.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Gets or sets the associated movie.
        /// </summary>
        public Movie Movie { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the associated movie.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Gets or sets the list of tickets associated with the screening.
        /// </summary>
        public List<Ticket> Tickets { get; set; } = new();
    }
}
