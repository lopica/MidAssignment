using System.ComponentModel;

namespace MidAssignment.DTOs
{
    public record BorrowingRequestDto(
        string RequestorId,
        DateTime RequestDate,
        [property: DefaultValue("Approved/Rejected/Waiting")]
        string Status,
        string ApproverId,
        DateTime DueDate,
        DateTime ReturnDate,
        DateTime UpdateAt
        );
}
