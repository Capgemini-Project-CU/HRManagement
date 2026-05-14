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

        public static DepartmentDto GetInvalidDepartmentDto()
        {
            return new DepartmentDto
            {
                DepartmentId = 100,
                DepartmentName = "Invalid",
                ManagerId = null,
                LocationId = null
            };
        }

        public static List<DepartmentDto> GetDepartmentDtoList()
        {
            return new List<DepartmentDto>
            {
                new DepartmentDto
                {
                    DepartmentId = 10,
                    DepartmentName = "IT",
                    ManagerId = 101,
                    LocationId = 1700,
                    ManagerName = "Steven King",
                    City = "Seattle"
                },

                new DepartmentDto
                {
                    DepartmentId = 20,
                    DepartmentName = "HR",
                    ManagerId = 102,
                    LocationId = 1800,
                    ManagerName = "Neena Kochhar",
                    City = "New York"
                }
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

        public static List<Department> GetDepartmentEntityList()
        {
            return new List<Department>
            {
                new Department
                {
                    DepartmentId = 10,
                    DepartmentName = "IT",
                    ManagerId = 101,
                    LocationId = 1700
                },

                new Department
                {
                    DepartmentId = 20,
                    DepartmentName = "HR",
                    ManagerId = 102,
                    LocationId = 1800
                }
            };
        }
    }
}