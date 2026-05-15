namespace HumanResource.API.DTOs.Common
{
    public class PaginatedResponseDto<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }

}