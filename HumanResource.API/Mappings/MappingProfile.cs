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
            CreateMap<EmployeeDto, Employee>()
            .ForMember(
            dest => dest.PasswordHash,
            opt => opt.MapFrom(src => src.Password))
            .ReverseMap()
            .ForMember(
            dest => dest.Password,
            opt => opt.MapFrom(src => src.PasswordHash));
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
                .ForMember(dest => dest.CountryId,
                    opt => opt.MapFrom(src => src.CountryId.Trim()))
                .ForMember(dest => dest.RegionName,
                    opt => opt.MapFrom(src => src.Region.RegionName));
            CreateMap<CountryDto, Country>()
                .ForMember(dest => dest.Region, opt => opt.Ignore());
            CreateMap<Location, LocationResponseDto>()
                .ForMember(
                    dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country!.CountryName)
                );
            CreateMap<LocationRequestDto, Location>();
            CreateMap<UpdateLocationDto, Location>();
        }
    }
}