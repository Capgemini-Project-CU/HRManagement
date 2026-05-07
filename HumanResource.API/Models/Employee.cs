using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Models;

[Table("employees")]
[Index("Email", Name = "UQ_employees_email", IsUnique = true)]
public partial class Employee
{
    [Key]
    [Column("employee_id", TypeName = "decimal(6, 0)")]
    public decimal EmployeeId { get; set; }

    [Column("first_name")]
    [StringLength(20)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [StringLength(25)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [Column("email")]
    [StringLength(25)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("phone_number")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [Column("hire_date")]
    public DateOnly HireDate { get; set; }

    [Column("job_id")]
    [StringLength(10)]
    [Unicode(false)]
    public string JobId { get; set; } = null!;

    [Column("salary", TypeName = "decimal(8, 2)")]
    public decimal? Salary { get; set; }

    [Column("commission_pct", TypeName = "decimal(2, 2)")]
    public decimal? CommissionPct { get; set; }

    [Column("manager_id", TypeName = "decimal(6, 0)")]
    public decimal? ManagerId { get; set; }

    [Column("department_id", TypeName = "decimal(4, 0)")]
    public decimal? DepartmentId { get; set; }

    public string? PasswordHash { get; set; }

    public int? RoleId { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("DepartmentId")]
    [InverseProperty("Employees")]
    public virtual Department? Department { get; set; }

    [InverseProperty("Manager")]
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    [InverseProperty("Manager")]
    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    [ForeignKey("JobId")]
    [InverseProperty("Employees")]
    public virtual Job Job { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

    [ForeignKey("ManagerId")]
    [InverseProperty("InverseManager")]
    public virtual Employee? Manager { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Employees")]
    public virtual Role? Role { get; set; }
}
