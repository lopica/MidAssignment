namespace MidAssignment.Domain
{
    public class BookBorrowingRequest
    {
        public Guid Id { get; set; }
        public required string RequestorId { get; set; }
        public required ApplicationUser Requestor { get; set; }
        public required DateTime RequestDate { get; set; }

        public Status Status { get; set; }

        public required string ApproverId { get; set; }
        public required ApplicationUser Approver { get; set; }

        public required BookBorrowingRequestDetail BookBorrowingRequestDetail { get; set; }
    }
}
