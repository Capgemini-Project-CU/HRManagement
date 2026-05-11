using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class RegionValidator : AbstractValidator<RegionDto>
    {
        public RegionValidator()
        {
            RuleFor(x => x.RegionName)
                .NotEmpty()
                .WithMessage("Region name is required.")
                .MaximumLength(25)
                .WithMessage("Region name cannot exceed 25 characters.")
                .Matches("^[A-Za-z ]+$")
                .WithMessage(
                    "Region name must contain only alphabets and spaces.");
        }
    }
}
