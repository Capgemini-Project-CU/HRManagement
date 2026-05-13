using HumanResource.API.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Helpers;
using Test.TestData;
using FluentAssertions;

namespace Test.Repositories
{
    public class CountryRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddCountry()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedRegion(context);

            var repository =
                new CountryRepository(context);

            var result =
                await repository.AddAsync(
                    CountryTestData.GetCountryEntity());

            result.CountryName.Should()
                .Be("India");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCountry_WhenExists()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedCountry(context);

            var repository =
                new CountryRepository(context);

            var result =
                await repository.GetByIdAsync("IN");

            result.Should().NotBeNull();

            result!.CountryId.Should()
                .Be("IN");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCountry()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedCountry(context);

            var repository =
                new CountryRepository(context);

            var country =
                CountryTestData.GetCountryEntity();

            country.CountryName = "Updated India";

            var result =
                await repository.UpdateAsync(country);

            result!.CountryName.Should()
                .Be("Updated India");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            TestUtilities.SeedCountry(context);

            var repository =
                new CountryRepository(context);

            var result =
                await repository.DeleteAsync("IN");

            result.Should().BeTrue();
        }
    }
}
