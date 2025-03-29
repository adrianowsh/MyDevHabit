using FluentValidation;
using MyDevHabit.Api.DTOs.Habits;
using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.Validators;

internal sealed class CreateHabitValidator : AbstractValidator<CreateHabitDto>
{
    private static readonly string[] AllowedUnits =
    [
        "minutes", "hours", "steps", "km", "cal",
        "pages", "books", "taks", "sessions"
    ];

    private static readonly string[] AllowedUnitsForBinaryHabits = ["sessions", "tasks"];

    public CreateHabitValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("Habit name must be betweeen 3 Unit and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid habit type");

        //FrequencyDto validation
        RuleFor(x => x.Frequency.Type)
            .IsInEnum()
            .WithMessage("Invalid frequency period");

        RuleFor(x => x.Frequency.TimesPerPeriod)
            .GreaterThan(0)
            .WithMessage("Frequency mus be greather than 0");

        //Target validation
        RuleFor(x => x.Target.Value)
            .GreaterThan(0)
            .WithMessage("Target value mus be greather than 0");

        RuleFor(x => x.Target.Unit)
            .NotEmpty()
            .Must(unit => AllowedUnits.Contains(unit))
            .WithMessage($"Unit must be one of: {string.Join(", ", AllowedUnits)}");

        //EndDate validation
        RuleFor(x => x.EndDate)
            .Must(date => date is null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("End date must be in the future");

        //Milestone validation
        When(x => x.Milestone is not null, () =>
        {
            RuleFor(x => x.Milestone!.Target)
                .GreaterThan(0)
                .WithMessage("Milestone target mus be greather than 0");
        });

        //Complex rules
        RuleFor(x => x.Target.Unit)
           .Must((dto, unit) => IsTargetUnitCompatibleWithTypes(dto.Type, unit))
           .WithMessage("Target unit is not compatible");


    }

    private static bool IsTargetUnitCompatibleWithTypes(HabitType type, string unit)
    {
        string normalizedUnit = unit;

        return type switch
        {
            HabitType.Binary => AllowedUnitsForBinaryHabits.Contains(normalizedUnit),

            HabitType.MEasureble => AllowedUnits.Contains(normalizedUnit),

            _ => false
        };
    }

}
