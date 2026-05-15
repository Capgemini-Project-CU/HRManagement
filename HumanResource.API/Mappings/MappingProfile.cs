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
            CreateMap<Employee, MyTeamEmployeeDto>()
                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department != null
                    ? src.Department.DepartmentName
                    : null));
            CreateMap<JobHistory, JobHistoryDto>().ReverseMap();

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
                    opt => opt.MapFrom(src => src.Region != null ? src.Region.RegionName : string.Empty));

            CreateMap<CountryDto, Country>()
                .ForMember(dest => dest.Region, opt => opt.Ignore());

            CreateMap<Location, LocationResponseDto>()
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src =>
                        src.Country != null
                            ? src.Country.CountryName
                            : null));

            CreateMap<LocationRequestDto, Location>();

            CreateMap<UpdateLocationDto, Location>();
        }
    }
}