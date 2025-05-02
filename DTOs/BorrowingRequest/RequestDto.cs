using MidAssignment.Domain;
using MidAssignment.Ultility;

namespace MidAssignment.DTOs.BorrowingRequest
{
    public record RequestDto(
        Guid Id,
        string RequestorEmail,
        List<Guid> BookIds,
        Status Status,
        DateTime DueDate,
        DateTime UpdateAt,
        DateTime? ReturnDate = null,
        string? ApproverEmail = null
        );
}
