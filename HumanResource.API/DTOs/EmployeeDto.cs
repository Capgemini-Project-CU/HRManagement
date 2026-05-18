namespace HumanResource.API.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string? Password { get; set; }

        public string PhoneNumber { get; set; }

        public DateOnly HireDate { get; set; }

        public decimal Salary { get; set; }

        public int? ManagerId { get; set; }

        public int DepartmentId { get; set; }

        public string JobId { get; set; }

        public int RoleId { get; set; }

    }
}
