using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Test.Repositories
{
    public class JobRepositoryTests
    {
        private HRDbContext GetDbContext()
        {
            var options =
                new DbContextOptionsBuilder<HRDbContext>()
                .UseInMemoryDatabase(
                    databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new HRDbContext(options);

            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllJobs()
        {
            // Arrange

            var context = GetDbContext();

            context.Jobs.AddRange(
                new Job
                {
                    JobId = "IT_PROG",
                    JobTitle = "Programmer",
                    MinSalary = 4000,
                    MaxSalary = 10000
                },

                new Job
                {
                    JobId = "HR_REP",
                    JobTitle = "HR Representative",
                    MinSalary = 3000,
                    MaxSalary = 8000
                });

            await context.SaveChangesAsync();

            var repository =
                new JobRepository(context);

            // Act

            var result =
                await repository.GetAllAsync();

            // Assert

            Assert.NotNull(result);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsJob()
        {
            // Arrange

            var context = GetDbContext();

            context.Jobs.Add(
                new Job
                {
                    JobId = "IT_PROG",
                    JobTitle = "Programmer",
                    MinSalary = 4000,
                    MaxSalary = 10000
                });

            await context.SaveChangesAsync();

            var repository =
                new JobRepository(context);

            // Act

            var result =
                await repository.GetByIdAsync(
                    "IT_PROG");

            // Assert

            Assert.NotNull(result);

            Assert.Equal(
                "IT_PROG",
                result!.JobId);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange

            var context = GetDbContext();

            var repository =
                new JobRepository(context);

            // Act

            var result =
                await repository.GetByIdAsync(
                    "INVALID");

            // Assert

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsJobSuccessfully()
        {
            // Arrange

            var context = GetDbContext();

            var repository =
                new JobRepository(context);

            var job = new Job
            {
                JobId = "DEV_JOB",
                JobTitle = "Developer",
                MinSalary = 5000,
                MaxSalary = 12000
            };

            // Act

            await repository.AddAsync(job);

            // Assert

            var addedJob =
                await context.Jobs.FindAsync(
                    "DEV_JOB");

            Assert.NotNull(addedJob);

            Assert.Equal(
                "Developer",
                addedJob!.JobTitle);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesJobSuccessfully()
        {
            // Arrange

            var context = GetDbContext();

            var job = new Job
            {
                JobId = "IT_PROG",
                JobTitle = "Programmer",
                MinSalary = 4000,
                MaxSalary = 10000
            };

            context.Jobs.Add(job);

            await context.SaveChangesAsync();

            var repository =
                new JobRepository(context);

            // Update values

            job.JobTitle = "Senior Developer";

            // Act

            await repository.UpdateAsync(job);

            // Assert

            var updatedJob =
                await context.Jobs.FindAsync(
                    "IT_PROG");

            Assert.NotNull(updatedJob);

            Assert.Equal(
                "Senior Developer",
                updatedJob!.JobTitle);
        }

        [Fact]
        public async Task DeleteAsync_RemovesJobSuccessfully()
        {
            // Arrange

            var context = GetDbContext();

            var job = new Job
            {
                JobId = "IT_PROG",
                JobTitle = "Programmer",
                MinSalary = 4000,
                MaxSalary = 10000
            };

            context.Jobs.Add(job);

            await context.SaveChangesAsync();

            var repository =
                new JobRepository(context);

            // Act

            await repository.DeleteAsync(job);

            // Assert

            var deletedJob =
                await context.Jobs.FindAsync(
                    "IT_PROG");

            Assert.Null(deletedJob);
        }

        [Fact]
        public async Task GetBySalaryRangeAsync_ReturnsFilteredJobs()
        {
            // Arrange

            var context = GetDbContext();

            context.Jobs.AddRange(
                new Job
                {
                    JobId = "IT_PROG",
                    JobTitle = "Programmer",
                    MinSalary = 4000,
                    MaxSalary = 10000
                },

                new Job
                {
                    JobId = "HR_REP",
                    JobTitle = "HR Representative",
                    MinSalary = 2000,
                    MaxSalary = 5000
                });

            await context.SaveChangesAsync();

            var repository =
                new JobRepository(context);

            // Act

            var result =
                await repository
                    .GetBySalaryRangeAsync(
                        3000,
                        12000);

            // Assert

            Assert.Single(result);
        }
    }
}