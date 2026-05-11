namespace HumanResource.API.DTOs.LocationDto
{
    public class LocationResponseDto
    {
        public decimal LocationId { get; set; }

        public string? StreetAddress { get; set; }

        public string? PostalCode { get; set; }

        public string City { get; set; } = null!;

        public string? StateProvince { get; set; }

        public string? CountryName { get; set; }
    }
}
