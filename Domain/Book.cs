namespace MidAssignment.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }

        public ICollection<Category>? Categories { get; set; }

        public ICollection<BookBorrowingRequestDetail>? BookBorrowingRequestDetails { get; set; }
    }
}
