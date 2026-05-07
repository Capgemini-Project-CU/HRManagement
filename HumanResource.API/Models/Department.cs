using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Models;

[Table("departments")]
public partial class Department
{
    [Key]
    [Column("department_id", TypeName = "decimal(4, 0)")]
    public decimal DepartmentId { get; set; }

    [Column("department_name")]
    [StringLength(30)]
    [Unicode(false)]
    public string DepartmentName { get; set; } = null!;

    [Column("manager_id", TypeName = "decimal(6, 0)")]
    public decimal? ManagerId { get; set; }

    [Column("location_id", TypeName = "decimal(4, 0)")]
    public decimal? LocationId { get; set; }

    [InverseProperty("Department")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("Department")]
    public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

    [ForeignKey("LocationId")]
    [InverseProperty("Departments")]
    public virtual Location? Location { get; set; }

    [ForeignKey("ManagerId")]
    [InverseProperty("Departments")]
    public virtual Employee? Manager { get; set; }
}
