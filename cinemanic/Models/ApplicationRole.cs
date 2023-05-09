using Microsoft.AspNetCore.Identity;

namespace cinemanic.Models
{
    public class ApplicationRole : IdentityRole
    {
        public const string User = "User";
        public const string Admin = "Admin";

        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
