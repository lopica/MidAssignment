using Microsoft.AspNetCore.Identity;

namespace MidAssignment.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatarUrl { get; set; }

        public ICollection<BookBorrowingRequest>? MadeRequests { get; set; }
        public ICollection<BookBorrowingRequest>? ApprovedRequests { get; set; }
    }
}
