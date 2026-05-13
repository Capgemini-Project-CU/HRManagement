namespace HumanResource.API.DTOs
{
    public class MyTeamEmployeeDto
    {
        public decimal EmployeeId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public DateOnly HireDate { get; set; }

        public string? DepartmentName { get; set; }
    }
}