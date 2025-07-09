using Microsoft.AspNetCore.Identity;

namespace RoleBasedAuthorization.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}