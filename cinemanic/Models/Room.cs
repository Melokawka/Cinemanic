namespace cinemanic.Models
{
    /// <summary>
    /// Represents a room in a cinema.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets or sets the room ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the number of seats in the room.
        /// </summary>
        public int Seats { get; set; }

        /// <summary>
        /// Gets the list of screenings scheduled in the room.
        /// </summary>
        public List<Screening> Screenings { get; } = new();
    }
}
