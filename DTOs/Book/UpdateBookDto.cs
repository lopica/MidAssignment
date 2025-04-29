using System.ComponentModel.DataAnnotations;

namespace MidAssignment.DTOs.Book
{
    public record UpdateBookDto(
        [Required(ErrorMessage = "Title is required")] 
        string Title, 
        [Required(ErrorMessage = "Author is required")] 
        string Author, 
        [Required(ErrorMessage = "Edition is required")] 
        [Range(1, int.MaxValue, ErrorMessage = "Edition must be at least 1.")] 
        int EditionNumber, 
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0")]  
        int Quantity, 
        bool IsAvailable, 
        List<Guid> CategoryIds
        );
}
