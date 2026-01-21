using FluentValidation;
using Fixopolis.Application.Categories.Commands;
using Fixopolis.Application.Abstractions;

namespace Fixopolis.Application.Categories.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator(ICategoryValidatorService categoryValidatorService)
    {
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(25).WithMessage("El nombre no puede tener más de 25 caracteres.")
                .Must(categoryValidatorService.BeUniqueName).WithMessage("El nombre de categoría ya existe.");
        }
    }
}