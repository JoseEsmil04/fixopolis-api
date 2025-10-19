using FluentValidation;
using Fixopolis.Application.Products.Commands;
using Fixopolis.Application.Abstractions;

namespace Fixopolis.Application.Products.Validators;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IAppDbContext _db;
    public UpdateProductValidator(IAppDbContext db)
    {
        _db = db;
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50)
        .Must(BeUniqueCode)
        .WithMessage("El código de producto ya existe.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryName).NotEmpty();
    }

    private bool BeUniqueCode(string code)
        => !_db.Products.Any(p => p.Code == code); // <-- Sincrono
}
