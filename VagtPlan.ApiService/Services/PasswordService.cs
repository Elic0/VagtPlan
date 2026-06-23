using BCrypt.Net;

namespace ApiService.Services;

public static class PasswordService
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string plainPassword, string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }

    public static bool LooksLikeBcryptHash(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.StartsWith("$2a$", StringComparison.Ordinal)
            || value.StartsWith("$2b$", StringComparison.Ordinal)
            || value.StartsWith("$2y$", StringComparison.Ordinal);
    }
}
