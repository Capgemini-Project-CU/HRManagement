using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace Test.TestData
{
    public static class DepartmentTestData
    {
        public static DepartmentDto GetDepartmentDto()
        {
            return new DepartmentDto
            {
                DepartmentId = 10,
                DepartmentName = "IT",
                ManagerId = 101,
                LocationId = 1700,
                ManagerName = "Steven King",
                City = "Seattle"
            };
        }

        public static Department GetDepartmentEntity()
        {
            return new Department
            {
                DepartmentId = 10,
                DepartmentName = "IT",
                ManagerId = 101,
                LocationId = 1700
            };
        }
    }
}