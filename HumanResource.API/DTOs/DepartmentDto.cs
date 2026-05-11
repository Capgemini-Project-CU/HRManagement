namespace HumanResource.API.DTOs
{
    public class DepartmentDto
    {
        public decimal DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public decimal? ManagerId { get; set; }

        public decimal? LocationId { get; set; }

        public string? ManagerName { get; set; }

        public string? City { get; set; }
    }
}