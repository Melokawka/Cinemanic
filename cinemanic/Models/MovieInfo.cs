namespace cinemanic.Models
{
    public class MovieInfo
    {
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? Duration { get; set; }
        public bool PolishMade { get; set; }
        public string Description { get; set; }
        public string Trailer { get; set; }
        public string PosterPath { get; set; }
        public bool Adult { get; set; }

        public List<string> Genres { get; set; } = new();
        public List<Screening> Screenings { get; set; } = new();
    }
}
