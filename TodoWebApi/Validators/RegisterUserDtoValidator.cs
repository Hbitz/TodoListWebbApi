using FluentValidation;
using TodoWebApi.DTOs;

namespace TodoWebApi.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(5).WithMessage("Username must be minimum 5 characters")
                .MaximumLength(50).WithMessage("Username length can't exceed 50 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(150).WithMessage("Password length can't exceed 150 characters");
            // Todo - Add more rules, uppercare and lowercase requirements, numbers, other signs like !$_ etc.
        }
    }
}
