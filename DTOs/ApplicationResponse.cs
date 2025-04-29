using Microsoft.OpenApi;

namespace MidAssignment.DTOs
{
    public record class ApplicationResponse
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public List<string>? Errors { get; init; }
        public object? Content { get; init; }

        public ApplicationResponse(bool success, int statusCode, List<string>? errors = null, object? content = null)
        {
            Success = success;
            StatusCode = statusCode;
            Errors = errors;
            Content = content;
        }
    }

    public record SuccessApplicationResponse<T> : ApplicationResponse
    {
        public SuccessApplicationResponse(int statusCode, T content) : base(true, statusCode, content: content)
        {
            Success = true;
            StatusCode = statusCode;
            Errors = null;
            Content = content;
        }
    }

    public record ErrorApplicationResponse : ApplicationResponse
    {
        public ErrorApplicationResponse(int statusCode, List<string> errors) : base(false, statusCode, errors: errors)
        {
            Success = false;
            StatusCode = statusCode;
            Errors = errors;
            Content = default;
        }
    }
}
