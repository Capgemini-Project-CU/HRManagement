using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Models;

namespace Test.TestData
{
    public static class LocationTestData
    {
        public static Location GetLocationEntity()
        {
            return new Location
            {
                LocationId = 1000,
                StreetAddress = "Sector 17",
                PostalCode = "160017",
                City = "Chandigarh",
                StateProvince = "Punjab",
                CountryId = "IN"
            };
        }

        public static LocationRequestDto GetLocationRequestDto()
        {
            return new LocationRequestDto
            {
                LocationId = 1000,
                StreetAddress = "Sector 17",
                PostalCode = "160017",
                City = "Chandigarh",
                StateProvince = "Punjab",
                CountryId = "IN"
            };
        }

        public static UpdateLocationDto GetUpdateLocationDto()
        {
            return new UpdateLocationDto
            {
                StreetAddress = "Updated Address",
                PostalCode = "160018",
                City = "Mohali",
                StateProvince = "Punjab",
                CountryId = "IN"
            };
        }

        public static LocationResponseDto GetLocationResponseDto()
        {
            return new LocationResponseDto
            {
                LocationId = 1000,
                StreetAddress = "Sector 17",
                PostalCode = "160017",
                City = "Chandigarh",
                StateProvince = "Punjab",
                CountryName = "India"
            };
        }
    }
}