using Microsoft.AspNetCore.Identity;

namespace cinemanic.Models
{
    /// <summary>
    /// Represents an application role derived from IdentityRole.
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// The constant for the user role.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// The constant for the admin role.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
        /// </summary>
        public ApplicationRole()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class with the specified role name.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
