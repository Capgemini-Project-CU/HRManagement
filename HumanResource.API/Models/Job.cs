using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Models;

[Table("jobs")]
public partial class Job
{
    [Key]
    [Column("job_id")]
    [StringLength(10)]
    [Unicode(false)]
    public string JobId { get; set; } = null!;

    [Column("job_title")]
    [StringLength(35)]
    [Unicode(false)]
    public string JobTitle { get; set; } = null!;

    [Column("min_salary", TypeName = "decimal(6, 0)")]
    public decimal? MinSalary { get; set; }

    [Column("max_salary", TypeName = "decimal(6, 0)")]
    public decimal? MaxSalary { get; set; }

    [InverseProperty("Job")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("Job")]
    public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();
}
