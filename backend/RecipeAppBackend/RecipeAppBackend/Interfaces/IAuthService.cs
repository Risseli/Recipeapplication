using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IAuthService
    {
        bool VerifyPassword(string password, string passwordHash);
        string GenerateToken(User user);
        bool IsAdmin(string jwtToken);
        string GetUserId(string jwtToken);
        void RestorePassword(string recipientEmail, string encryptedPassword);
        string EncryptString(string text);
        string DecryptString(string text);
    }
}
