namespace HumanResource.API.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly HireDate { get; set; }

        public decimal Salary { get; set; }

        public int? ManagerId { get; set; }

        public int DepartmentId { get; set; }

        public string JobId { get; set; } = string.Empty;

        public int RoleId { get; set; }
    }
}
