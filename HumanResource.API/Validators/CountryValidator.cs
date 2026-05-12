using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class CountryValidator : AbstractValidator<CountryDto>
    {
        public CountryValidator()
        {
            RuleFor(x => x.CountryId)
                .NotEmpty()
                .WithMessage("Country ID is required.")
                .Length(2, 4)
                .WithMessage("Country ID must be between 2 and 4 characters.")
                .Matches("^[A-Z]+$")
                .WithMessage("Country ID must contain only uppercase letters.");

            RuleFor(x => x.CountryName)
                .NotEmpty()
                .WithMessage("Country name is required.")
                .MaximumLength(60)
                .WithMessage("Country name cannot exceed 60 characters.");

            RuleFor(x => x.RegionId)
                .GreaterThan(0)
                .WithMessage("Valid Region ID is required.");
        }
    }
}
