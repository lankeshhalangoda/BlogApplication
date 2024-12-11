using Microsoft.AspNetCore.Identity;

namespace BlogApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
