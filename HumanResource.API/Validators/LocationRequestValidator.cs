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
            RuleFor(x => x.LocationId)
                .GreaterThan(1000)
                .WithMessage("LocationId must be greater than 1000");

            RuleFor(x => x.StreetAddress)
                .NotEmpty()
                .MaximumLength(40);

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .Matches("^[0-9A-Za-z\\s-]{3,10}$")
                .WithMessage("Invalid Postal Code");

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(30)
                .Matches("^[a-zA-Z ]+$")
                .WithMessage("City must contain only letters");

            RuleFor(x => x.StateProvince)
                .NotEmpty()
                .MaximumLength(25)
                .Matches("^[a-zA-Z ]+$")
                .WithMessage("StateProvince must contain only letters");

            RuleFor(x => x.CountryId)
                .NotEmpty()
                .Length(2, 4)
                .Matches("^[A-Z]+$")
                .WithMessage("CountryId must contain uppercase letters only");
        }
    }
}
