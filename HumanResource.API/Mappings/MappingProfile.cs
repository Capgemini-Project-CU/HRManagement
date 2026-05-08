using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace HumanResource.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();

            CreateMap<JobHistory, JobHistoryDto>().ReverseMap();
        }
    }
}