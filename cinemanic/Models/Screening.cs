namespace cinemanic.Models
{
    public class Screening
    {
        public int Id { get; set; }
        public DateTime ScreeningTime { get; set; }
        public bool Subtitles { get; set; }
        public bool Lector { get; set; }
        public bool Dubbing { get; set; }
        public bool Is3D { get; set; }
        public int? Budget { get; set; }
        public int? SeatsLeft { get; set; }

        public Room Room { get; set; } = null!;
        public int RoomId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int MovieId { get; set; }
        public List<Ticket> Tickets { get; } = new();
    }
}
