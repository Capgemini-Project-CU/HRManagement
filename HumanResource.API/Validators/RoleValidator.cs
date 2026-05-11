using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class RoleValidator : AbstractValidator<RoleDto>
    {
        public RoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}