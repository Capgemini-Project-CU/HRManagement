using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Models;

[Table("countries")]
public partial class Country
{
    [Key]
    [Column("country_id")]
    [StringLength(4)]
    [Unicode(false)]
    public string CountryId { get; set; } = null!;

    [Column("country_name")]
    [StringLength(60)]
    [Unicode(false)]
    public string? CountryName { get; set; }

    [Column("region_id", TypeName = "decimal(18, 0)")]
    public decimal? RegionId { get; set; }

    [InverseProperty("Country")]
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    [ForeignKey("RegionId")]
    [InverseProperty("Countries")]
    public virtual Region? Region { get; set; }
}
