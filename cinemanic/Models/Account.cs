using System.ComponentModel.DataAnnotations;

namespace cinemanic.Models
{
    public class Account
    {
        [Key]
        public string UserEmail { get; set; }
        public DateTime Birthdate { get; set; }
        public string Password { get; set; }
    }
}
