namespace MidAssignment.DTOs
{
    public record class ApplicationResponse<T>
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public List<string>? Errors { get; init; } = null;
        public T? Content { get; init; } = default;

        public ApplicationResponse(bool success, int statusCode, List<string>? errors = null, T? content = default)
        {
            Success = success;
            StatusCode = statusCode;
            Errors = errors;
            Content = content;
        }
    }
}
