using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace Test.TestData
{
    public static class JobHistoryTestData
    {
        public static JobHistory GetJobHistoryEntity()
        {
            return new JobHistory
            {
                EmployeeId = 100,
                StartDate = new DateOnly(2020, 1, 1),
                EndDate = new DateOnly(2022, 1, 1),
                JobId = "AD_PRES",
                DepartmentId = 90
            };
        }

        public static JobHistoryDto GetJobHistoryDto()
        {
            return new JobHistoryDto
            {
                EmployeeId = 100,
                StartDate = new DateOnly(2020, 1, 1),
                EndDate = new DateOnly(2022, 1, 1),
                JobId = "AD_PRES",
                DepartmentId = 90
            };
        }
    }
}