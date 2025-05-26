using FluentValidation;
using TodoWebApi.DTOs;

namespace TodoWebApi.Validators
{
    public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
    {
        public CreateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Title is required")
                .Must(title => !string.IsNullOrWhiteSpace(title)).WithMessage("Title cannot be empty or whitespace")
                .MaximumLength(100).WithMessage("Title can't exceed 100 characters");
            

            RuleFor(x => x.Description)
                .NotNull().WithMessage("Description is required")
                .Must(desc => !string.IsNullOrWhiteSpace(desc))
                .WithMessage("Description cannot be empty or whitespace")
                .MaximumLength(500).WithMessage("Description can't exceed 500 characters");

            // Todo - validation between existing data in db?
        }
    }
}
