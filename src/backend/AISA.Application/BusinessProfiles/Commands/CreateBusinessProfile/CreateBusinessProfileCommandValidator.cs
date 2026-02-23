using FluentValidation;

namespace AISA.Application.BusinessProfiles.Commands.CreateBusinessProfile;

public class CreateBusinessProfileCommandValidator : AbstractValidator<CreateBusinessProfileCommand>
{
    public CreateBusinessProfileCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Numele afacerii este obligatoriu.")
            .MaximumLength(200).WithMessage("Numele nu poate depăși 200 de caractere.");

        RuleFor(x => x.Category)
            .MaximumLength(100).When(x => x.Category != null);

        RuleFor(x => x.Address)
            .MaximumLength(500).When(x => x.Address != null);
    }
}
