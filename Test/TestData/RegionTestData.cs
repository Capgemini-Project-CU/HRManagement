using HumanResource.API.DTOs;
using HumanResource.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TestData
{
    public static class RegionTestData
    {
        public static Region GetRegionEntity()
        {
            return new Region
            {
                RegionId = 10,
                RegionName = "Europe"
            };
        }

        public static RegionDto GetRegionDto()
        {
            return new RegionDto
            {
                RegionId = 10,
                RegionName = "Europe"
            };
        }
    }
}
