namespace Cosmetics_Shopping_Website.GenericPattern.Security;

public static class EncryptPassword
{
    public static string TextToEncrypt(string Password)
    {
        return BCrypt.Net.BCrypt.HashPassword(Password);
    }
    public static bool VerifyPassword(string Password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(Password, hashedPassword);
    }
}