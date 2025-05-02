using MidAssignment.Ultility;

namespace MidAssignment.DTOs.BorrowingRequest
{
    public record CreateRequestDto(
        string RequestorId,
        [FutureDate(ErrorMessage = "DueDate must be in the future.")]
        DateTime DueDate,
        List<Guid> BookIds
        );
}
