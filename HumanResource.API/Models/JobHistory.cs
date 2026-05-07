using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Models;

[PrimaryKey("EmployeeId", "StartDate")]
[Table("job_history")]
public partial class JobHistory
{
    [Key]
    [Column("employee_id", TypeName = "decimal(6, 0)")]
    public decimal EmployeeId { get; set; }

    [Key]
    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("job_id")]
    [StringLength(10)]
    [Unicode(false)]
    public string JobId { get; set; } = null!;

    [Column("department_id", TypeName = "decimal(4, 0)")]
    public decimal? DepartmentId { get; set; }

    [ForeignKey("DepartmentId")]
    [InverseProperty("JobHistories")]
    public virtual Department? Department { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("JobHistories")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("JobId")]
    [InverseProperty("JobHistories")]
    public virtual Job Job { get; set; } = null!;
}
