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
                .MaximumLength(25);
        }
    }
}
