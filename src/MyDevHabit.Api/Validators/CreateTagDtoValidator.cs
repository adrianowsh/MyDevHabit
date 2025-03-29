using FluentValidation;
using MyDevHabit.Api.DTOs.Tags;

namespace MyDevHabit.Api.Validators;

internal sealed class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(3);

        RuleFor(p => p.Description).MaximumLength(50);
    }
}
