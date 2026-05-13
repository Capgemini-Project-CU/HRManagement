using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace Test.TestData
{
    public static class JobTestData
    {
        public static List<Job> GetJobs()
        {
            return new List<Job>
            {
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
                }
            };
        }

        public static Job GetJob()
        {
            return new Job
            {
                JobId = "IT_PROG",
                JobTitle = "Programmer",
                MinSalary = 4000,
                MaxSalary = 10000
            };
        }

        public static JobDto GetJobDto()
        {
            return new JobDto
            {
                JobId = "DEV_JOB",
                JobTitle = "Developer",
                MinSalary = 5000,
                MaxSalary = 12000
            };
        }

        public static JobDto GetUpdatedJobDto()
        {
            return new JobDto
            {
                JobId = "DEV_JOB",
                JobTitle = "Senior Developer",
                MinSalary = 7000,
                MaxSalary = 15000
            };
        }
    }
}
