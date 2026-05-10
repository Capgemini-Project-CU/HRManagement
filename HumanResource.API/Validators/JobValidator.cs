using FluentValidation;
using HumanResource.API.DTOs;

namespace HumanResource.API.Validators
{
    public class JobValidator : AbstractValidator<JobDto>
    {
        public JobValidator()
        {
            RuleFor(x => x.JobId)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(x => x.JobTitle)
                .NotEmpty()
                .MaximumLength(35);

            RuleFor(x => x.MinSalary)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinSalary.HasValue);

            RuleFor(x => x.MaxSalary)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxSalary.HasValue);

            RuleFor(x => x.MaxSalary)
                .GreaterThanOrEqualTo(x => x.MinSalary)
                .When(x => x.MinSalary.HasValue && x.MaxSalary.HasValue);
        }
    }
}