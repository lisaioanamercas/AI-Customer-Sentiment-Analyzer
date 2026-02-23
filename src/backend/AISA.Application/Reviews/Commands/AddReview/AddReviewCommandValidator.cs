using FluentValidation;

namespace AISA.Application.Reviews.Commands.AddReview;

/// <summary>
/// Validare FluentValidation pentru AddReviewCommand.
/// </summary>
public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Conținutul recenziei este obligatoriu.")
            .MinimumLength(3).WithMessage("Recenzia trebuie să aibă cel puțin 3 caractere.")
            .MaximumLength(5000).WithMessage("Recenzia nu poate depăși 5000 de caractere.");

        RuleFor(x => x.BusinessProfileId)
            .NotEmpty().WithMessage("ID-ul profilului de business este obligatoriu.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Sursa recenziei este obligatorie.")
            .MaximumLength(50);

        RuleFor(x => x.AuthorName)
            .MaximumLength(200).When(x => x.AuthorName != null);
    }
}
