namespace HumanResource.API.DTOs
{
    public class RegionDto
    {
        public decimal RegionId { get; set; }

        public string RegionName { get; set; } = null!;

        public List<string>? CountryNames { get; set; }
    }
}