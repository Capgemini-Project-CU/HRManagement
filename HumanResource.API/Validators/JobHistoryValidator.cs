using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class JobHistoryValidator : AbstractValidator<JobHistoryDto>
    {
        public JobHistoryValidator()
        {
            RuleFor(j => j.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Employee Id is required");

            RuleFor(j => j.JobId)
                .NotEmpty()
                .WithMessage("Job Id is required");

            RuleFor(j => j.DepartmentId)
                .GreaterThan(0)
                .WithMessage("Department Id is required");

            RuleFor(j => j.StartDate)
                .NotEmpty()
                .WithMessage("Start Date is required");

            RuleFor(j => j.EndDate)
                .GreaterThan(j => j.StartDate)
                .WithMessage("End Date must be greater than Start Date");
        }
    }
}