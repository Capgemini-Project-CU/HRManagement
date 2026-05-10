using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.FirstName)
                .NotEmpty()
                .WithMessage("First Name is required");

            RuleFor(e => e.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required");

            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Valid Email is required");

            RuleFor(e => e.Salary)
                .GreaterThan(0)
                .WithMessage("Salary must be greater than 0");

            RuleFor(e => e.DepartmentId)
                .GreaterThan(0)
                .WithMessage("Department is required");

            RuleFor(e => e.JobId)
                .NotEmpty()
                .WithMessage("Job Id is required");
        }
    }
}