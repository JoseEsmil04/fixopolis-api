using FluentValidation;
using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Products.Commands;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IAppDbContext _db;

    public CreateProductValidator(IAppDbContext db)
    {
        _db = db;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Must(BeUniqueCode)
            .WithMessage("El código de producto ya existe.");

        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryName).NotEmpty();
    }

    private bool BeUniqueCode(string code)
        => !_db.Products.Any(p => p.Code == code); // <-- Sincrono


}
