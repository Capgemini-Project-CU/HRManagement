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
                .Length(4)
                .When(x => !string.IsNullOrEmpty(x.CountryId));

            RuleFor(x => x.CountryName)
                .NotEmpty()
                .MaximumLength(60);

            RuleFor(x => x.RegionId)
                .NotNull()
                .GreaterThan(0);
        }
    }
}
