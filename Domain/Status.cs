namespace MidAssignment.Domain
{
    public enum Status
    {
        Approved,
        Rejected,
        Waiting
    }

    public static class StatusExtensions
    {
        public static string ToStringValue(this Status status)
        {
            return status switch
            {
                Status.Approved => "Approved",
                Status.Rejected => "Rejected",
                Status.Waiting => "Waiting",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
