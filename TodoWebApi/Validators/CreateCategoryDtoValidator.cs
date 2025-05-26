using FluentValidation;
using TodoWebApi.DTOs;

namespace TodoWebApi.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Category name is required")
                .Must(name => !string.IsNullOrWhiteSpace(name))
                    .WithMessage("Category name cannot be empty or whitespace")
                .Length(1, 50).WithMessage("Category name must be between 1 and 50 characters");
        }
    }
}
