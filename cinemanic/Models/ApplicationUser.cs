using Microsoft.AspNetCore.Identity;

namespace cinemanic.Models
{
    /// <summary>
    /// Represents an application user derived from IdentityUser.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateTime BirthDate { get; set; }
    }
}
