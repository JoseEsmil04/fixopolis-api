using FluentValidation;
using Fixopolis.Application.Categories.Commands;
using Fixopolis.Application.Abstractions;

namespace Fixopolis.Application.Categories.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IAppDbContext _db;
    public UpdateCategoryValidator(IAppDbContext db)
    {
        _db = db;
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(150)
                .Must(BeUniqueName).WithMessage("El nombre de categoría ya existe.");
        }
    }

    private bool BeUniqueName(string name) => !_db.Categories.Any(p => p.Name == name);
}
