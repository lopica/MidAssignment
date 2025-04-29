
namespace MidAssignment.DTOs
{
    public record CreateBorrowingRequest
    (
        string RequestorId,
        DateTime DueDate,
        List<Guid> BookIds
        );
}
