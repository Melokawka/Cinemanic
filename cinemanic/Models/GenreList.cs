namespace cinemanic.Models
{
    public class GenreList
    {
        public List<string> Genres { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
