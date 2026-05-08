using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class DepartmentValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentValidator()
        {
            // Department Name Validation
            RuleFor(x => x.DepartmentName)

                .NotEmpty()
                .WithMessage("Department Name is required")

                .MaximumLength(30)
                .WithMessage("Department Name cannot exceed 30 characters");


            // Manager Id Validation
            RuleFor(x => x.ManagerId)

                .NotNull()
                .WithMessage("Manager Id is required")

                .GreaterThan(0)
                .WithMessage("Manager Id must be greater than 0");


            // Location Id Validation
            RuleFor(x => x.LocationId)

                .NotNull()
                .WithMessage("Location Id is required")

                .GreaterThan(0)
                .WithMessage("Location Id must be greater than 0");
        }
    }
}