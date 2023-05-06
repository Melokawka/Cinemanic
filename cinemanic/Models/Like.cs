namespace cinemanic.Models
{
    public class Like
    {
        public int MovieId { get; set; }
        public string UserEmail { get; set; }
        public Movie Movie { get; set; }
        public Account Account { get; set; }
    }
}
