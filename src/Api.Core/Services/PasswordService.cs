using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Api.Dto.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Api.Core.Services
{
    public class PasswordService : IPasswordService
    {
        public PasswordDto GenerateSaltAndPassword(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            var saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var salt = Convert.ToBase64String(saltBytes);

            return new PasswordDto
            {
                Hash = hash,
                Salt = salt
            };
        }

        public bool CheckAccessPassword([NotNull] PasswordDto hash, string password)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));

            var newHash = GenerateSaltAndPassword(password);

            return hash.Equals(newHash);
        }
    }
}