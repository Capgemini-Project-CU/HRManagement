using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace HumanResource.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Job, JobDto>().ReverseMap();

            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}