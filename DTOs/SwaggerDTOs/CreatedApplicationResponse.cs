using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public record CreatedApplicationResponse<T>
    {
        public bool Success { get; init; }
        [property: DefaultValue("201")]

        public int StatusCode { get; init; }

        [property: DefaultValue("null")]
        public List<string>? Errors { get; init; }
        public T? Content { get; init; }

        public CreatedApplicationResponse(int statusCode, T content)
        {
            Success = true;
            StatusCode = statusCode;
            Errors = null;
            Content = content;
        }
    }
}
