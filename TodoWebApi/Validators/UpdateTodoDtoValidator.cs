using FluentValidation;
using TodoWebApi.DTOs;

namespace TodoWebApi.Validators
{
    public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
    {
        public UpdateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Title is required")
                .NotEmpty().WithMessage("Title cannot be empty")
                .MaximumLength(100).WithMessage("Title can't exceed 100 characters");

            RuleFor(x => x.Description)
                .NotNull().WithMessage("Description is required")
                .NotEmpty().WithMessage("Description cannot be empty")
                .MaximumLength(500).WithMessage("Description can't exceed 500 characters");


            //When(x => x.CategoryId.HasValue, () => {
            //    RuleFor(x => x.CategoryId.Value)
            //        .GreaterThan(0).WithMessage("CategoryId must be positive if provided");
            //});

            //When(x => !string.IsNullOrWhiteSpace(x.CategoryName), () => {
            //    RuleFor(x => x.CategoryName)
            //        .MaximumLength(50).WithMessage("Category name can't exceed 50 characters");
            //});
        }
    }
}
