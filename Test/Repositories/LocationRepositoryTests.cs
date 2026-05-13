using FluentAssertions;
using HumanResource.API.Repositories.Implementations;
using Test.Helpers;
using Test.TestData;

namespace Test.Repositories
{
    public class LocationRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddLocation()
        {
            var context = TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedCountry(context);

            var repository = new LocationRepository(context);

            var result = await repository.AddAsync(
                LocationTestData.GetLocationEntity());

            result.City.Should().Be("Chandigarh");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLocation_WhenExists()
        {
            var context = TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedCountry(context);

            context.Locations.Add(
                LocationTestData.GetLocationEntity());

            await context.SaveChangesAsync();

            var repository = new LocationRepository(context);

            var result = await repository.GetByIdAsync(1000);

            result.Should().NotBeNull();

            result!.LocationId.Should().Be(1000);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenLocationNotFound()
        {
            var context = TestDbContextFactory.CreateDbContext();

            var repository = new LocationRepository(context);

            var result = await repository.UpdateAsync(
                LocationTestData.GetLocationEntity());

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenLocationNotFound()
        {
            var context = TestDbContextFactory.CreateDbContext();

            var repository = new LocationRepository(context);

            var result = await repository.DeleteAsync(9999);

            result.Should().BeFalse();
        }
    }
}