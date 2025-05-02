using MidAssignment.Domain;
using MidAssignment.Ultility;

namespace MidAssignment.DTOs.BorrowingRequest
{
    public record UpdateRequestDto(
        Status Status,
        string? ApproverId,
        [FutureDate(ErrorMessage = "DueDate must be in the future.")]
        DateTime DueDate,
        DateTime? ReturnDate,
        DateTime? UpdateAt,
        List<Guid> BookIds
    );
}
