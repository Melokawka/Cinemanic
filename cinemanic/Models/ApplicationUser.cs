using Microsoft.AspNetCore.Identity;

namespace cinemanic.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime BirthDate { get; set; }
    }
}
