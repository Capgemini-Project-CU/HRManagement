namespace HumanResource.API.DTOs
{
    public class CountryDto
    {
        public string? CountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public decimal RegionId { get; set; }
        public string? RegionName { get; set; }
    }
}
