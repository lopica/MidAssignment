using System.ComponentModel;

namespace MidAssignment.DTOs
{
    public record PaginateDto<T>(
        List<T> Data,
        [property: DefaultValue("1")]
        int CurentPage = 1,
        [property: DefaultValue("1")]
        int TotalPage = 1,
        [property: DefaultValue("5")]
        int Limit = 5
        );
}
