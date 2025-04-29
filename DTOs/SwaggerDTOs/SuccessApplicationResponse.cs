using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public record SuccessApplicationResponse<T>
    {
        public bool Success { get; init; }
        [property: DefaultValue("200")]

        public int StatusCode { get; init; }

        [property: DefaultValue("null")]
        public List<string>? Errors { get; init; }
        public T? Content { get; init; }

        public SuccessApplicationResponse(int statusCode, T content)
        {
            Success = true;
            StatusCode = statusCode;
            Errors = null;
            Content = content;
        }
    }
}
