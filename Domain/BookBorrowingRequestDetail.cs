namespace MidAssignment.Domain
{
    public class BookBorrowingRequestDetail
    {
        public required Guid BookBorrowingRequestId { get; set; }
        public required BookBorrowingRequest BookBorrowingRequest { get; set; }
        public required DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime UpdateAt { get; set; }
        public required ICollection<Book> Books { get; set; }

    }
}
