using FluentValidation;

namespace AISA.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Numele complet este obligatoriu.")
            .MaximumLength(100).WithMessage("Numele nu poate depăși 100 caractere.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email-ul este obligatoriu.")
            .EmailAddress().WithMessage("Formatul email-ului este invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parola este obligatorie.")
            .MinimumLength(8).WithMessage("Parola trebuie să aibă minim 8 caractere.")
            .Matches("[A-Z]").WithMessage("Parola trebuie să conțină cel puțin o literă mare.")
            .Matches("[0-9]").WithMessage("Parola trebuie să conțină cel puțin o cifră.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Parolele nu coincid.");
    }
}
