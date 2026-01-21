namespace Fixopolis.Persistence.Identity;

public class TokenHash
{
    public static string Execute(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }

}