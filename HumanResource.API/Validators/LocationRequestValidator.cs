using FluentValidation;
using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Models;
using Microsoft.EntityFrameworkCore;
namespace HumanResource.API.Validators
{
    public class LocationRequestValidator : AbstractValidator<LocationRequestDto>
    {
        public LocationRequestValidator()
        {
            
            RuleFor(x => x.StreetAddress)
                .NotEmpty()
                .MaximumLength(40);

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .MaximumLength(12);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.StateProvince)
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(x => x.CountryId)
                .NotEmpty()
                .Length(2, 4);
        }
    }
}
