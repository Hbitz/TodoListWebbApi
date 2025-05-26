using FluentValidation;
using TodoWebApi.DTOs;

namespace TodoWebApi.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is requiremd");
        }
    }
}
