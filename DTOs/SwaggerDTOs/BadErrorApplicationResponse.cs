using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public class BadErrorApplicationResponse
    {
        [property: DefaultValue("false")]

        public bool Success { get; init; }
        [property: DefaultValue("400")]
        public int StatusCode { get; init; }
        public List<string>? Errors { get; init; }
        [property: DefaultValue("null")]

        public object? Content { get; init; }

        public BadErrorApplicationResponse(int statusCode, List<string> errors)
        {
            Success = false;
            StatusCode = statusCode;
            Errors = errors;
            Content = null;
        }
    }
}
