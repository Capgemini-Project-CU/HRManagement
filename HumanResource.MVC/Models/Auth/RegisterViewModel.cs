using System.ComponentModel.DataAnnotations;

namespace HumanResource.MVC.Models.Auth;

public class RegisterViewModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string JobId { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    [Range(1, double.MaxValue)]
    public decimal DepartmentId { get; set; }

    public decimal? ManagerId { get; set; }

    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }
}
