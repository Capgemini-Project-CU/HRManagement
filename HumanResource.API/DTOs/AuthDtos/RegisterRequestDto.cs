namespace HumanResource.API.DTOs.AuthDtos
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string JobId { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public decimal DepartmentId { get; set; }

        public decimal? ManagerId { get; set; }

        public int RoleId { get; set; }
    }
}