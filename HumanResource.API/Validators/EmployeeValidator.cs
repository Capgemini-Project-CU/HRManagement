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
                .WithMessage("First Name is required.")
                .MaximumLength(20)
                .WithMessage("First Name cannot exceed 20 characters.")
                .Matches("^[A-Za-z ]+$")
                .WithMessage(
                    "First Name must contain only alphabets.");

            RuleFor(e => e.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required.")
                .MaximumLength(25)
                .WithMessage("Last Name cannot exceed 25 characters.")
                .Matches("^[A-Za-z ]+$")
                .WithMessage(
                    "Last Name must contain only alphabets.");

            RuleFor(e => e.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Valid Email is required.")
                .MaximumLength(25)
                .WithMessage("Email cannot exceed 25 characters.");

            RuleFor(e => e.Salary)
                .GreaterThan(0)
                .WithMessage("Salary must be greater than 0.");

            RuleFor(e => e.DepartmentId)
                .GreaterThan(0)
                .WithMessage("Valid Department ID is required.");

            RuleFor(e => e.JobId)
                .NotEmpty()
                .WithMessage("Job ID is required.")
                .MaximumLength(10)
                .WithMessage("Job ID cannot exceed 10 characters.")
                .Matches("^[A-Z_]+$")
                .WithMessage(
                    "Job ID must contain only uppercase letters and underscore.");
        }
    }
}