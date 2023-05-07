using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinemanic.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime Birthdate { get; set; }
        public string Password { get; set; }
        public NewsletterClient NewsletterClient { get; set; } = null!;
        public List<Like> Likes { get; } = new();
        public List<Ticket> Tickets { get; } = new();
        public List<Order> Orders { get; } = new();
    }
}
