namespace MidAssignment.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required int EditionNumber { get; set; }
        public int Quantity { get; set; } = 0;
        public bool IsAvailable { get; set; } = false;

        public ICollection<Category> Categories { get; set; } = [];

        public ICollection<BookBorrowingRequestDetail>? BookBorrowingRequestDetails { get; set; }
    }
}
