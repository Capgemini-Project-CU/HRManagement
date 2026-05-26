using FluentValidation;
using HumanResource.API.DTOs.AuthDtos;

namespace HumanResource.API.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Valid Email is required.")
                .MaximumLength(25)
                .WithMessage("Email cannot exceed 25 characters.");

            RuleFor(e => e.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone Number is required.")
                .MaximumLength(20)
                .WithMessage("Phone Number cannot exceed 20 characters.")
                .Matches(@"^[0-9+\-() ]+$")
                .WithMessage("Invalid Phone Number format.");
        }
    }
}
