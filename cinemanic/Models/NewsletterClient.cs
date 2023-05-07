using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cinemanic.Models
{        
    public class NewsletterClient
    {
        [Key]
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;
    }
}
