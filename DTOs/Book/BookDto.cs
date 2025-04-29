namespace MidAssignment.DTOs.Book
{
    public record BookDto(Guid Id, string Title, string Author, int EditionNumber, int Quantity, bool IsAvailable, List<Guid> CategoryIds);
}
