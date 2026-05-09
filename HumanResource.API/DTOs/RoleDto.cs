using System.ComponentModel.DataAnnotations;

namespace HumanResource.API.DTOs
{
    public class RoleDto
    {
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;
    }
}