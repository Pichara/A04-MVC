/*
* FILE         : PasswordHashService.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : OpenAI Codex
* FIRST VERSION: 2026-04-09
* DESCRIPTION  : Password hashing utilities for secure credential storage.
*                Uses PBKDF2 with SHA-256 and per-password salts.
*/

using System.Security.Cryptography;

namespace A04_MVC.Services
{
    /// <summary>
    /// Helper methods for hashing and verifying application passwords.
    /// </summary>
    public static class PasswordHashService
    {
        private const string HashPrefix = "PBKDF2";
        private const int DefaultIterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        /// <summary>
        /// Hashes a plain text password using PBKDF2 with a random salt.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        /// <returns>Encoded password hash.</returns>
        public static string HashPassword(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                DefaultIterations,
                HashAlgorithmName.SHA256,
                HashSize);

            string result = string.Join(
                '$',
                HashPrefix,
                DefaultIterations.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));

            return result;
        }

        /// <summary>
        /// Verifies a password against a stored value.
        /// </summary>
        /// <param name="password">Plain text password supplied by the user.</param>
        /// <param name="storedValue">Stored password hash or legacy plain text value.</param>
        /// <returns>True when the password matches; otherwise false.</returns>
        public static bool VerifyPassword(string password, string storedValue)
        {
            bool result = false;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedValue))
            {
                return result;
            }

            if (!IsPasswordHash(storedValue))
            {
                result = string.Equals(password, storedValue, StringComparison.Ordinal);

                return result;
            }

            string[] parts = storedValue.Split('$', StringSplitOptions.None);

            if (parts.Length != 4 || !int.TryParse(parts[1], out int iterations))
            {
                return result;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                result = CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Determines whether the stored value is an application password hash.
        /// </summary>
        /// <param name="storedValue">Stored password value.</param>
        /// <returns>True when the value is a password hash; otherwise false.</returns>
        public static bool IsPasswordHash(string storedValue)
        {
            bool result = !string.IsNullOrWhiteSpace(storedValue)
                && storedValue.StartsWith($"{HashPrefix}$", StringComparison.Ordinal);

            return result;
        }
    }
}
