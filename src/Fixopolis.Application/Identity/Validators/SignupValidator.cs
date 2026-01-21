using FluentValidation;

namespace Fixopolis.Application.Identity.Commands.Validators;

public class SignupValidator : AbstractValidator<SignupCommand>
{
    public SignupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("Debe ingresar un correo electrónico válido.")
            .MaximumLength(200).WithMessage("El correo electrónico no puede tener más de 200 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol del usuario es obligatorio.");
    }
}
