using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace Test.TestData
{
    public static class EmployeeTestData
    {
        public static Employee GetEmployeeEntity()
        {
            return new Employee
            {
                EmployeeId = 100,
                FirstName = "Steven",
                LastName = "King",
                Email = "SKING",
                PhoneNumber = "1.515.555.0100",
                HireDate = new DateOnly(2013, 06, 17),
                JobId = "AD_PRES",
                Salary = 24000,
                DepartmentId = 90,
                ManagerId = null,
                RoleId = 1,
                PasswordHash = "admin@123",
                IsActive = true
            };
        }

        public static EmployeeDto GetEmployeeDto()
        {
            return new EmployeeDto
            {
                EmployeeId = 100,
                FirstName = "Steven",
                LastName = "King",
                Email = "SKING",
                PhoneNumber = "1.515.555.0100",
                HireDate = new DateOnly(2013, 06, 17),
                JobId = "AD_PRES",
                Salary = 24000,
                DepartmentId = 90,
                ManagerId = null,
                RoleId = 1,
                Password = "admin@123"
            };
        }
    }
}