namespace cinemanic.Models
{
    /// <summary>
    /// Represents an archived screening.
    /// </summary>
    public class ArchivedScreening
    {
        /// <summary>
        /// Gets or sets the ID of the archived screening.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the screening date of the archived screening.
        /// </summary>
        public DateTime ScreeningDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the archived screening has subtitles.
        /// </summary>
        public bool Subtitles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the archived screening has a lector.
        /// </summary>
        public bool Lector { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the archived screening has dubbing.
        /// </summary>
        public bool Dubbing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the archived screening is in 3D.
        /// </summary>
        public bool Is3D { get; set; }

        /// <summary>
        /// Gets or sets the number of seats left for the archived screening.
        /// </summary>
        public int SeatsLeft { get; set; }

        /// <summary>
        /// Gets or sets the ID of the room associated with the archived screening.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Gets or sets the movie associated with the archived screening.
        /// </summary>
        public Movie Movie { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the movie associated with the archived screening.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Gets or sets the gross income of the archived screening.
        /// </summary>
        public decimal GrossIncome { get; set; }
    }
}
