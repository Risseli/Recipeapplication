using Microsoft.IdentityModel.Tokens;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace RecipeAppBackend.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _encryptionKey = "ThisIsAnEncryptionKeyThatShouldProbablyNotBeStoredLikeThis";

        public AuthServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private byte[] GetKey()
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(_encryptionKey));
            }
        }

        public string DecryptString(string text)
        {
            byte[] iv = new byte[16]; // IV size is always 16 bytes for AES

            // Extract the IV from the beginning of the ciphertext
            Array.Copy(Convert.FromBase64String(text), 0, iv, 0, 16);

            // Remove the IV from the ciphertext
            byte[] cipherText = Convert.FromBase64String(text);
            byte[] encryptedBytes = new byte[cipherText.Length - 16];
            Array.Copy(cipherText, 16, encryptedBytes, 0, encryptedBytes.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetKey(); // Use the same key used for encryption
                aesAlg.IV = iv; // Use the IV extracted from the ciphertext

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string EncryptString(string text)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetKey(); // Use the generated key
                aesAlg.GenerateIV(); // Generate a random IV for each encryption operation

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Write the IV to the beginning of the ciphertext
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("userId", user.Id.ToString()),
                    new Claim("isAdmin", user.Admin.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                                        , SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetUserId(string jwtToken)
        {
            if (jwtToken == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            //Extract userId claim
            var userIdClaim = token.Claims.FirstOrDefault(claim => claim.Type == "userId");

            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }
            else
            {
                return null;
            }
        }

        public bool IsAdmin(string jwtToken)
        {
            if (jwtToken == null) return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            //Extract admin claim
            var isAdminClaim = token.Claims.FirstOrDefault(claims => claims.Type == "isAdmin");

            return isAdminClaim.Value == "True" ? true : false;
        }

        public void RestorePassword(string recipientEmail, string encryptedPassword)
        {
            // Set up SMTP client and credentials
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("recipeappgl@gmail.com", "ktgg uftf gknq wjqr"),
                EnableSsl = true,
            };

            // Create and configure the email message
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("recipeappgl@gmail.com"),
                Subject = "Restoring a forgotten password",
                Body = "The password for your RecipeApp account is " + DecryptString(encryptedPassword),
                IsBodyHtml = true, // Set to true if your email body is in HTML format
            };
            mailMessage.To.Add(recipientEmail);

            // Send the email
            smtpClient.Send(mailMessage);
        }

        public bool VerifyPassword(string password, string encryptedPassword)
        {
            if (password.Equals(DecryptString(encryptedPassword)))
                return true;

            return false;
        }
    }
}
