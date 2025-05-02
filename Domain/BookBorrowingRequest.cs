namespace MidAssignment.Domain
{
    public class BookBorrowingRequest
    {
        public Guid Id { get; set; }
        public string? RequestorId { get; set; }
        private ApplicationUser _requestor = null!;
        public required ApplicationUser Requestor
        {
            get => _requestor;
            set
            {
                _requestor = value;
                RequestorId = value.Id;
            }
        }
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public Status Status { get; set; }

        public string? ApproverId { get; set; }
        private ApplicationUser? _approver;
        public ApplicationUser? Approver
        {
            get => _approver;
            set
            {
                _approver = value;
                ApproverId = value?.Id;
            }
        }

        public BookBorrowingRequestDetail? BookBorrowingRequestDetail { get; set; }
    }
}
