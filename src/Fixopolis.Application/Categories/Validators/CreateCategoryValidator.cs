using FluentValidation;
using Fixopolis.Application.Categories.Commands;
using Fixopolis.Application.Abstractions;

namespace Fixopolis.Application.Categories.Validators;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator(ICategoryValidatorService categoryValidator)
    {

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(25).WithMessage("El nombre no puede tener más de 25 caracteres.")
            .Must(categoryValidator.BeUniqueName).WithMessage("El nombre de categoría ya existe.");

    }

}
