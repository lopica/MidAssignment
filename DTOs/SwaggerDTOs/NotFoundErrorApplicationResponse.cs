using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public class NotFoundApplicationResponse
    {
        [property: DefaultValue("false")]

        public bool Success { get; init; }
        [property: DefaultValue("404")]
        public int StatusCode { get; init; }
        public List<string>? Errors { get; init; }
        [property: DefaultValue("null")]

        public object? Content { get; init; }

        public NotFoundApplicationResponse(int statusCode, List<string> errors)
        {
            Success = false;
            StatusCode = statusCode;
            Errors = errors;
            Content = null;
        }
    }
}
