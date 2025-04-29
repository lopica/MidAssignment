using System.ComponentModel;

namespace MidAssignment.DTOs.SwaggerDTOs
{
    public class ViolateBusinessApplicationResponse
    {
        [property: DefaultValue("false")]

        public bool Success { get; init; }
        [property: DefaultValue("422")]
        public int StatusCode { get; init; }
        public List<string>? Errors { get; init; }
        [property: DefaultValue("null")]

        public object? Content { get; init; }

        public ViolateBusinessApplicationResponse(int statusCode, List<string> errors)
        {
            Success = false;
            StatusCode = statusCode;
            Errors = errors;
            Content = null;
        }
    }
}
