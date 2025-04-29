using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public record CreateBorrowingRequestDto(
        string RequestorId,
        DateTime RequestDate,
        [property: DefaultValue("Waiting")]
        string Status,
        [property: DefaultValue("null")]
        string ApproverId,
        DateTime DueDate,
        [property: DefaultValue("null")]
        DateTime ReturnDate,
        DateTime UpdateAt
        );
}
