using FluentAssertions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Implementations;
using Test.Helpers;

namespace Test.Repositories
{
    public class JobHistoryRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddJobHistory()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            var repository =
                new JobHistoryRepository(context);

            var jobHistory =
                new JobHistory
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024, 1, 1),
                    EndDate = new DateOnly(2025, 1, 1)
                };

            var result =
                await repository.AddAsync(jobHistory);

            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnJobHistory_WhenExists()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            context.JobHistories.Add(
                new JobHistory
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024, 1, 1),
                    EndDate = new DateOnly(2025, 1, 1)
                });

            await context.SaveChangesAsync();

            var repository =
                new JobHistoryRepository(context);

            var result =
                await repository.GetByIdAsync(100);

            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            context.JobHistories.Add(
                new JobHistory
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024, 1, 1),
                    EndDate = new DateOnly(2025, 1, 1)
                });

            await context.SaveChangesAsync();

            var repository =
                new JobHistoryRepository(context);

            var result =
                await repository.DeleteAsync(100);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetByDepartmentAsync_ShouldReturnRecords()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            context.JobHistories.Add(
                new JobHistory
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024, 1, 1),
                    EndDate = new DateOnly(2025, 1, 1)
                });

            await context.SaveChangesAsync();

            var repository =
                new JobHistoryRepository(context);

            var result =
                await repository.GetByDepartmentAsync(60);

            result.Should().NotBeNull();

            result.Count().Should().Be(1);
        }
    }
}