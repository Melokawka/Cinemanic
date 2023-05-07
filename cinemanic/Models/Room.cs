namespace cinemanic.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Seats { get; set; }

        public List<Screening> Screenings { get; } = new();
    }
}
