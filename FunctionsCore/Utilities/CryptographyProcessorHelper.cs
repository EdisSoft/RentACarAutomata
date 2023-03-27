using System;
using System.Security.Cryptography;

namespace FunctionsCore.Utilities
{
    public class CryptographyProcessorHelper
    {
        private static CryptographyProcessorHelper instance;
        public static CryptographyProcessorHelper Instance => instance ??= new CryptographyProcessorHelper();

        private static readonly HashAlgorithmName SHA512 = HashAlgorithmName.SHA512;

        private static readonly int Iterations = 100000;

        static CryptographyProcessorHelper() { }
        private CryptographyProcessorHelper() { }


        public string CreateSalt()
        {
            byte[] salt = new byte[24];
            new RNGCryptoServiceProvider().GetBytes(salt);
            return Convert.ToBase64String(salt);
        }


        public string GenerateHash(string plainText, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(plainText, saltBytes, Iterations, SHA512);
            byte[] hash = pbkdf2.GetBytes(24);            
            return Convert.ToBase64String(hash);
        }

        public bool AreEqual(string plainTextInput, string origDelimHash, string salt)
        {            
            var origSalt = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(plainTextInput, origSalt, Iterations, SHA512);
            byte[] testHash = pbkdf2.GetBytes(24);

            if (Convert.ToBase64String(testHash) == origDelimHash)
                return true;

            return false;
        }
    }
}
