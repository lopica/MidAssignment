using System.ComponentModel.DataAnnotations;

namespace MidAssignment.DTOs.Category
{
    public record CreateCategoryDto([Required(ErrorMessage = "Name is required")] string Name);
}
