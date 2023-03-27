using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FunctionsCore.Utilities
{
    public enum RSAKeySize
    {
        Key512 = 512,
        Key1024 = 1024,
        Key2048 = 2048,
        Key4096 = 4096
    }
    public class RSAKeysTypes
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    public class RSACryptographyKeyGenerator
    {
        public RSAKeysTypes GenerateKeys(RSAKeySize rsaKeySize)
        {
            int keySize = (int)rsaKeySize;
            if (keySize % 2 != 0 || keySize < 512)
                throw new Exception("Key should be multiple of two and greater than 512.");
            var rsaKeysTypes = new RSAKeysTypes();
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                var publicKey = provider.ToXmlString(false);
                var privateKey = provider.ToXmlString(true);
                var publicKeyWithSize = IncludeKeyInEncryptionString(publicKey, keySize);
                var privateKeyWithSize = IncludeKeyInEncryptionString(privateKey, keySize);
                rsaKeysTypes.PublicKey = publicKeyWithSize;
                rsaKeysTypes.PrivateKey = privateKeyWithSize;
            }
            return rsaKeysTypes;
        }
        private string IncludeKeyInEncryptionString(string publicKey, int keySize)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(keySize.ToString() + "!" + publicKey));
        }
    }

    public static class RsaHelper
    {

        private static bool _optimalAsymmetricEncryptionPadding = false;

        private static string PublicKey = "MjA0OCE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+ci9XTFcwR0IxdWw3SXFLWFhXTnkvVjlEamRiaCt4MVMzNk1VUFRQUFNmQ1d3VUgvZko3bVVKbXZJNUtaTjcyaWVMeTNZM2FCZ1UrSHpXbnFxcWttMDFPL2ZmQ0lnZi81MU1DekRrTWsvU3RCb3pGbWpLTE5TYUJsNW5nMDBpT3VQZVdDbDFvTHpIeWFIaDE4bmtjcTZBcmxCQTQ5aTc1KzZiWHZvdVpiVkg1OERxZ1k1VDM4QXRUZFJ4MGZQdzBYQThCdGY2NCsrSUJkKy9TbXMrZUV2YnpjdVRPUlRWcThRNTBjK01qWGdubnJDK05hcWZMOGtrRTM2NEJDUmNmMUFIR2xHM1VnN05HeDFwbmN4ZXAreVZxL05MQUduNnE0WXoxcFNoQ1JHRFN6cCtqd0d2ekdiUURmMkJqL2dtaHFIc3ZoV0tLaEJVeUdsUEg4QmdWREhRPT08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjwvUlNBS2V5VmFsdWU+";
        private static string PrivateKey = "MjA0OCE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+ci9XTFcwR0IxdWw3SXFLWFhXTnkvVjlEamRiaCt4MVMzNk1VUFRQUFNmQ1d3VUgvZko3bVVKbXZJNUtaTjcyaWVMeTNZM2FCZ1UrSHpXbnFxcWttMDFPL2ZmQ0lnZi81MU1DekRrTWsvU3RCb3pGbWpLTE5TYUJsNW5nMDBpT3VQZVdDbDFvTHpIeWFIaDE4bmtjcTZBcmxCQTQ5aTc1KzZiWHZvdVpiVkg1OERxZ1k1VDM4QXRUZFJ4MGZQdzBYQThCdGY2NCsrSUJkKy9TbXMrZUV2YnpjdVRPUlRWcThRNTBjK01qWGdubnJDK05hcWZMOGtrRTM2NEJDUmNmMUFIR2xHM1VnN05HeDFwbmN4ZXAreVZxL05MQUduNnE0WXoxcFNoQ1JHRFN6cCtqd0d2ekdiUURmMkJqL2dtaHFIc3ZoV0tLaEJVeUdsUEg4QmdWREhRPT08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjxQPnl3SkpUa05mYnNML1BpSEdkVkpyNVJvK094ZjIrNU1IcGlSeDE4djRoUFhTZldxSFN1dlVCMXBxaUlsYVNTa0ZDVndlb3VFbjdzYjNMSnY3TjhxQ3BsWWNaNHhQcUVpbGRBYitvdmg1WTBlbTFMNlVveXhNbTJCemF0dmdoUjI1UjI4R1piZ1k0VVpSOTdOcUcwZDEwTGFZWGZzdjN0Q0VIaG54OFJKSUl4Yz08L1A+PFE+M2VPMFluYzBhR200eVVYY1ZVQnBONE04eGZHMSsxVHJWMEhJRHFWdTNNUGNIcUJTWFBUM3hFemxLdGRtc2NVdDNBZ2pxZnFuUGt6QklFMllveENxWVcwOG5JTHJqTTVLNGh1czR6M1ZHT2txb2NiU1dibzNmMzNjbnB4cEs2NWFVMTNBdzRWZXQrSmZlQ3U1S3dhb3luQlhjcWFnVXgzWlhBZUJ5eTAzZStzPTwvUT48RFA+WGF6YmFLcThBWjhuS21OcGNUK3NHOWFGT3Ixdk1WMENIWThablFzNHpSbWxuRXVvekZDUDBTak9tZHhQZGd6Z2p4WWI2T3JlZzFiNlBYSm9kTEVVb201L3d1UkM2Wk5FRFlzZ3V0RFBLcG1vaU9pOEs2TklZallhYkFlenpaa09vc0MvZ2ZaMlBKVTNRNFpkZ3VMeG9YSndVb1dTN0V6cGVmNWMzdkdocWRjPTwvRFA+PERRPlV4UWd4QVZSOEE0MWk5YkdhbjBWYUIyUk1hUVF1U3ZRZWZrOVNJNlVkY0EzdUpmYWZzZVJ4VVB0UWg4b2FrTWpxcEM1bWJrcHlWKy9wN0ttQmRnRmt6anRTc1NTSVVSVU1WaWZZTzRUemFIdnZtYk05NStMakFhNkJnRVdONjZ1VHl4NU1qdkxmUm9iR01sMktTL3pIdWoxTlE1Q1ZSeUY5N2t2MWJ6MlcwVT08L0RRPjxJbnZlcnNlUT5DRlZlZjZpRDRYMGtnL2Z4SDdzTU1zWXpTWjlpTnNxSWtUYjhHR3Mra3dpMzA0bEFZOFVZMW1UOElWUzA0ODA4dVgwT1F1YkZoY3dGbTZtc0EvVklVZU5kTks1ZE5sYkM3UXJ2NjNXNlhRU0JINVNzSzdyTE02RkQ3S0VkZ0VjUGlGNmZjWlBCQ29WRkR1VDhJbzcxdzMyNU5USCtjdEdtQkg4ZVZqRUxNMDA9PC9JbnZlcnNlUT48RD5Pb0w3YVVnYVJmM3ovV3QxekE5QzhMQmFCWE9jUitYVFpoeVRvVnAySnM5RFFsYVEvNzdUTmFBMVZpM3NsMVdQRG5rcXZuOXN4elJNSTJtQ0tHZ0JTaXJDTmIzamx0OHBFNjlVbTN5R2MwMk1FUmxmVVlyZG13c2tkNFFuaEcxckJwZ29XZTRPbUZ3aTM3ck1jdEdWbWYzdGMvRE1WSWp2S1FaZi9wcDlhbFhyM2N6MHdLTTlocG1tSVI5THdzMEdqald0d0h2aW0rWjNaTExxTmZMb01mUVBYQ3IzN1NxSEptS3pwMk9OM1J6VTY0NHB5ZytzY3haU2w1d21NNVA3S2VMand1NEwxMTZPSVFJemRLUGJWUi9wdEtnai9KYXpzTDFDckw1bVhjcHZ1b2E3MjNQcXZwbHYwYWpjZ2wrODNEMG9yYkxDVUMwRHhmQUlneHB6blE9PTwvRD48L1JTQUtleVZhbHVlPg==";

        public static void SetKeys(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public static string Encrypt(string plainText)
        {
            int keySize = 0;
            string publicKeyXml = "";
            GetKeyFromEncryptionString(PublicKey, out keySize, out publicKeyXml);
            var encrypted = Encrypt(Encoding.UTF8.GetBytes(plainText), keySize, publicKeyXml);

            return Convert.ToBase64String(encrypted);
        }

        private static byte[] Encrypt(byte[] data, int keySize, string publicKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            int maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength) throw new ArgumentException(String.Format("Maximum data length is { 0 }", maxLength), "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public static string Decrypt(string encryptedText)
        {
            int keySize = 0;
            string publicAndPrivateKeyXml = "";
            GetKeyFromEncryptionString(PrivateKey, out keySize, out publicAndPrivateKeyXml);
            var decrypted = Decrypt(Convert.FromBase64String(encryptedText.Replace("{x}", "+")), keySize, publicAndPrivateKeyXml);

            return Encoding.UTF8.GetString(decrypted);
        }

        private static byte[] Decrypt(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml)) throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        private static int GetMaxDataLength(int keySize)
        {
            if (_optimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }
            return ((keySize - 384) / 8) + 37;
        }

        private static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }

        private static void GetKeyFromEncryptionString(string rawkey, out int keySize, out string xmlKey)
        {
            keySize = 0;
            xmlKey = "";
            if (rawkey != null && rawkey.Length > 0)
            {
                byte[] keyBytes = Convert.FromBase64String(rawkey);
                var stringKey = Encoding.UTF8.GetString(keyBytes);
                if (stringKey.Contains("!"))
                {
                    var splittedValues = stringKey.Split(new char[] { '!' }, 2);
                    try
                    {
                        keySize = int.Parse(splittedValues[0]);
                        xmlKey = splittedValues[1];
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}
