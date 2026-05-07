using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Models;

[Table("regions")]
public partial class Region
{
    [Key]
    [Column("region_id", TypeName = "decimal(18, 0)")]
    public decimal RegionId { get; set; }

    [Column("region_name")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RegionName { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();
}
