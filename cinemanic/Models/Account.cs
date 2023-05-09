namespace cinemanic.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime Birthdate { get; set; }
        public NewsletterClient NewsletterClient { get; set; } = null!;
        public List<Like> Likes { get; } = new();
        public List<Order> Orders { get; } = new();
    }
}
