using FluentValidation;

namespace Fixopolis.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Body.UserId).NotEmpty();
        RuleFor(x => x.Body.Items)
            .NotNull()
            .WithMessage("Items cannot be null.")
            .NotEmpty()
            .WithMessage("Items list cannot be empty.");

        RuleForEach(x => x.Body.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}