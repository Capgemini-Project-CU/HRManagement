using System.ComponentModel.DataAnnotations;

namespace HumanResource.API.DTOs
{
    public class JobDto
    {
        [Required]
        [StringLength(10)]
        public string JobId { get; set; } = null!;

        [Required]
        [StringLength(35)]
        public string JobTitle { get; set; } = null!;

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }
    }
}