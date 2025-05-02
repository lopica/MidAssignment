namespace MidAssignment.Domain
{
    public class BookBorrowingRequestDetail
    {
        public Guid BookBorrowingRequestId { get; set; }

        private BookBorrowingRequest _bookBorrowingRequest = null!;
        public BookBorrowingRequest BookBorrowingRequest
        {
            get => _bookBorrowingRequest;
            set
            {
                _bookBorrowingRequest = value;
                BookBorrowingRequestId = value.Id;
            }
        }
        public required DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public required ICollection<Book> Books { get; set; }

    }
}
