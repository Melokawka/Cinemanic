namespace cinemanic.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int AccountId { get; set; }
        public Movie Movie { get; set; } = null!;
        public Account Account { get; set; } = null!;
    }
}
