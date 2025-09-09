using System.Security.Cryptography;

namespace StoreService.Application.Security
{
    public interface IPasswordHasher
    {
        byte[] Hash(string password);
        bool Verify(byte[] stored, string password);
    }

    public class Pbkdf2PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public byte[] Hash(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            var result = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);
            return result;
        }

        public bool Verify(byte[] stored, string password)
        {
            if (stored == null || stored.Length != SaltSize + KeySize) return false;

            var salt = new byte[SaltSize];
            var key = new byte[KeySize];
            Buffer.BlockCopy(stored, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(stored, SaltSize, key, 0, KeySize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var keyToCheck = pbkdf2.GetBytes(KeySize);

            var diff = 0;
            for (int i = 0; i < KeySize; i++) diff |= key[i] ^ keyToCheck[i];
            return diff == 0;
        }
    }
}