using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public class InternalErrorApplicationResponse
    {
        [property: DefaultValue("false")]

        public bool Success { get; init; }
        [property: DefaultValue("500")]
        public int StatusCode { get; init; }
        public List<string>? Errors { get; init; }
        [property: DefaultValue("null")]

        public object? Content { get; init; }

        public InternalErrorApplicationResponse(int statusCode, List<string> errors)
        {
            Success = false;
            StatusCode = statusCode;
            Errors = errors;
            Content = null;
        }
    }
}
