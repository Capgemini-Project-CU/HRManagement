using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class DepartmentValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentValidator()
        {
            // DepartmentId Validation
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("DepartmentId must be greater than 0.");

            // DepartmentName Validation
            RuleFor(x => x.DepartmentName)
                .NotEmpty()
                .MaximumLength(30);

            // ManagerId Validation
            RuleFor(x => x.ManagerId)
                .GreaterThan(0)
                .When(x => x.ManagerId.HasValue);

            // LocationId Validation
            RuleFor(x => x.LocationId)
                .GreaterThan(0)
                .When(x => x.LocationId.HasValue);
        }
    }
}