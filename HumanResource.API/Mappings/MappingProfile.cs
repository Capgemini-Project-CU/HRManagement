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
            CreateMap<Location, LocationResponseDto>()
                .ForMember(
                    dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country!.CountryName)
                );

            CreateMap<LocationRequestDto, Location>();
        }
    }
}