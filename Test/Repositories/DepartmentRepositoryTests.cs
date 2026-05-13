using FluentAssertions;
using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Test.Repositories
{
    public class DepartmentRepositoryTests
    {
        private readonly HRDbContext _context;

        private readonly DepartmentRepository _repository;

        public DepartmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HRDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HRDbContext(options);

            _repository = new DepartmentRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var location = new Location
            {
                LocationId = 1700,
                City = "Seattle"
            };

            var employee = new Employee
            {
                EmployeeId = 101,
                FirstName = "Steven",
                LastName = "King",
                Email = "steven@gmail.com",
                HireDate = new DateOnly(2020, 1, 1),
                JobId = "IT_PROG"
            };

            var department = new Department
            {
                DepartmentId = 10,
                DepartmentName = "IT",
                ManagerId = 101,
                LocationId = 1700,
                Manager = employee,
                Location = location
            };

            _context.Locations.Add(location);

            _context.Employees.Add(employee);

            _context.Departments.Add(department);

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnDepartments()
        {
            var result = await _repository.GetAllAsync();

            result.Should().NotBeNull();

            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDepartment_WhenDepartmentExists()
        {
            var result = await _repository.GetByIdAsync(10);

            result.Should().NotBeNull();

            result!.DepartmentName.Should().Be("IT");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenDepartmentDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldReturnDepartments()
        {
            var result = await _repository.GetByLocationAsync(1700);

            result.Should().NotBeNull();

            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDepartment()
        {
            var department = new Department
            {
                DepartmentId = 20,
                DepartmentName = "HR",
                ManagerId = 101,
                LocationId = 1700
            };

            var result = await _repository.AddAsync(department);

            result.Should().NotBeNull();

            result.DepartmentName.Should().Be("HR");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDepartment()
        {
            var department = await _repository.GetByIdAsync(10);

            department!.DepartmentName = "Updated IT";

            var result = await _repository.UpdateAsync(department);

            result.DepartmentName.Should().Be("Updated IT");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDepartmentDeleted()
        {
            var result = await _repository.DeleteAsync(10);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenDepartmentNotFound()
        {
            var result = await _repository.DeleteAsync(999);

            result.Should().BeFalse();
        }
    }
}