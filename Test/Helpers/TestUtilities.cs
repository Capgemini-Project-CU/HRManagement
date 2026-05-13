using AutoMapper;
using HumanResource.API.Data;
using HumanResource.API.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.TestData;

namespace Test.Helpers
{
    public static class TestUtilities
    {
        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            return config.CreateMapper();
        }
        public static void SeedRegion(HRDbContext context)
        {
            context.Regions.Add(
                RegionTestData.GetRegionEntity());

            context.SaveChanges();
        }

        public static void SeedCountry(HRDbContext context)
        {
            SeedRegion(context);

            context.Countries.Add(
                CountryTestData.GetCountryEntity());

            context.SaveChanges();
        }

        public static void SeedLocation(HRDbContext context)
        {
            SeedCountry(context);

            context.Locations.Add(
                LocationTestData.GetLocationEntity());

            context.SaveChanges();
        }
    }
}
