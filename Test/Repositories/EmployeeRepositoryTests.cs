using FluentAssertions;
using HumanResource.API.Repositories.Implementations;
using Test.Helpers;
using Test.TestData;

namespace Test.Repositories
{
    public class EmployeeRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddEmployee()
        {
            var context =
                TestDbContextFactory.CreateDbContext();

            var repository =
                new EmployeeRepository(context);

            var employee =
                EmployeeTestData.GetEmployeeEntity();


            var result =
                await repository.AddAsync(employee);

            result.Should().NotBeNull();

            result.FirstName.Should().Be("Steven");

            result.Email.Should().Be("SKING");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEmployee_WhenExists()
        {

            var context =
                TestDbContextFactory.CreateDbContext();

            var employee =
                EmployeeTestData.GetEmployeeEntity();

            context.Employees.Add(employee);

            await context.SaveChangesAsync();

            var repository =
                new EmployeeRepository(context);

            
            var result =
                await repository.GetByIdAsync(100);



            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);

            result.FirstName.Should().Be("Steven");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmployee()
        {
            // Arrange

            var context =
                TestDbContextFactory.CreateDbContext();

            var employee =
                EmployeeTestData.GetEmployeeEntity();

            context.Employees.Add(employee);

            await context.SaveChangesAsync();

            var repository =
                new EmployeeRepository(context);


            employee.FirstName = "Updated Steven";
        

            var result =
                await repository.UpdateAsync(employee);
           

            result.Should().NotBeNull();

            result.FirstName.Should().Be("Updated Steven");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {

            var context =
                TestDbContextFactory.CreateDbContext();

            var employee =
                EmployeeTestData.GetEmployeeEntity();

            context.Employees.Add(employee);

            await context.SaveChangesAsync();

            var repository =
                new EmployeeRepository(context);

            var result =
                await repository.DeleteAsync(100);

            result.Should().BeTrue();
        }
    }
}