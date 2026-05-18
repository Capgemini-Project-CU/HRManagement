namespace HumanResource.API.DTOs.LocationDto
{
    public class LocationRequestDto
    {
        public decimal LocationId { get; set; }

        public string StreetAddress { get; set; } = null!;

        public string PostalCode { get; set; } = null!;

        public string City { get; set; } = null!;

        public string StateProvince { get; set; } = null!;

        public string CountryId { get; set; } = null!;
    }
}