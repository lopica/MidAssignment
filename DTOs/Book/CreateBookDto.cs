using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MidAssignment.DTOs.Book
{
    public record CreateBookDto(
        [Required(ErrorMessage = "Title is required")] 
        string Title, 
        [Required(ErrorMessage = "Author is required")] 
        string Author, List<Guid> CategoryIds,
        [property: DefaultValue(1)]
        [Required(ErrorMessage = "Edition is required")] 
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")] 
        int EditionNumber
        );
}
