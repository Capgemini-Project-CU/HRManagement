using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Helpers;
using Test.TestData;
using FluentAssertions;
using HumanResource.API.Repositories.Implementations;

namespace Test.Repositories
{
    public class RegionRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddRegion()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            var repository =
                new RegionRepository(context);

            var result =
                await repository.AddAsync(
                    RegionTestData.GetRegionEntity());

            result.RegionName.Should()
                .Be("Europe");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRegion_WhenExists()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedRegion(context);

            var repository =
                new RegionRepository(context);

            var result =
                await repository.GetByIdAsync(10);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateRegion()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedRegion(context);

            var repository =
                new RegionRepository(context);

            var region =
                RegionTestData.GetRegionEntity();

            region.RegionName = "Updated Europe";

            var result =
                await repository.UpdateAsync(region);

            result!.RegionName.Should()
                .Be("Updated Europe");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedRegion(context);

            var repository =
                new RegionRepository(context);

            var result =
                await repository.DeleteAsync(10);

            result.Should().BeTrue();
        }
    }
}
