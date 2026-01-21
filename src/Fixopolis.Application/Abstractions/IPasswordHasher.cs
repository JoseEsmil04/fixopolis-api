// src/Fixopolis.Application/Abstractions/IPasswordHasher.cs
namespace Fixopolis.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
