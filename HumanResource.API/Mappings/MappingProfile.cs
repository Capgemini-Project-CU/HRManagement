using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace HumanResource.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.ManagerName,
                    opt => opt.MapFrom(src =>
                        src.Manager != null
                            ? src.Manager.FirstName + " " + src.Manager.LastName
                            : null))

                .ForMember(dest => dest.City,
                    opt => opt.MapFrom(src =>
                        src.Location != null
                            ? src.Location.City
                            : null))

                .ReverseMap();
        }
    }
}