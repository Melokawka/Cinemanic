namespace cinemanic.Models
{
    public class Screening
    {
        public int ScreeningId { get; set; }
        public DateTime ScreeningTime { get; set; }
        public bool Subtitles { get; set; }
        public bool Lector { get; set; }
        public bool Dubbing { get; set; }
        public bool Is3D { get; set; }
        public int? Budget { get; set; }
        public int? SeatsLeft { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
