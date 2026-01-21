namespace Fixopolis.Application.Abstractions;

public interface ICategoryValidatorService
{
    bool BeUniqueName(string name);
}