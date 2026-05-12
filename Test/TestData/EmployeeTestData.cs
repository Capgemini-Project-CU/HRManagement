using HumanResource.API.DTOs;
using HumanResource.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TestData
{
    public static class EmployeeTestData
    {
        public static EmployeeDto GetEmployeeDto()
        {
            return new EmployeeDto
            {
                EmployeeId = 100,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@hr.com",
                PhoneNumber = "9876543210",
                Salary = 50000,
                DepartmentId = 10,
                JobId = "IT_PROG",
                RoleId = 1
            };
        }

        public static Employee GetEmployeeEntity()
        {
            return new Employee
            {
                EmployeeId = 100,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@hr.com",
                PhoneNumber = "9876543210",
                Salary = 50000,
                DepartmentId = 10,
                JobId = "IT_PROG",
                RoleId = 1
            };
        }

        public static List<EmployeeDto> GetEmployees()
        {
            return new List<EmployeeDto>
            {
                GetEmployeeDto()
            };
        }
    }
}
