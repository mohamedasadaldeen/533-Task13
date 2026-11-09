using Microsoft.AspNetCore.Identity;

namespace Cairo_Cinema.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }
    }
}
