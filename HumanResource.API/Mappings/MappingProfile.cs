using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Models;

namespace HumanResource.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<JobHistory, JobHistoryDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Job, JobDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();

            CreateMap<Region, RegionDto>()
                .ForMember(dest => dest.CountryNames,
                    opt => opt.MapFrom(src =>
                        src.Countries.Select(c => c.CountryName).ToList()))
                .ReverseMap();
            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.RegionName,
                    opt => opt.MapFrom(src => src.Region.RegionName));
            CreateMap<CountryDto, Country>()
                .ForMember(dest => dest.Region, opt => opt.Ignore());
            CreateMap<Location, LocationResponseDto>()
                .ForMember(dest => dest.CountryName,
                   opt => opt.MapFrom(src => src.Country != null? src.Country.CountryName: null));
            CreateMap<LocationRequestDto, Location>();
            CreateMap<UpdateLocationDto, Location>();
        }
    }
}