using FluentValidation;

namespace Fixopolis.Application.Identity.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Debe ingresar un correo electrónico válido.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
