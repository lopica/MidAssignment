using Microsoft.AspNetCore.Identity;

namespace MidAssignment.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatarUrl { get; set; }
    }
}
