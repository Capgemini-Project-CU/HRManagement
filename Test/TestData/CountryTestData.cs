using HumanResource.API.DTOs;
using HumanResource.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TestData
{
    public static class CountryTestData
    {
        public static Country GetCountryEntity()
        {
            return new Country
            {
                CountryId = "IN",
                CountryName = "India",
                RegionId = 30
            };
        }
        public static CountryDto GetCountryDto()
        {
            return new CountryDto
            {
                CountryId = "IN",
                CountryName = "India",
                RegionId = 30
            };
        }
    }
}
