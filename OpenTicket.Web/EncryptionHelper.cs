using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenTicket.Web
{
    public static class EncryptionHelper
    {
        private static RSAParameters? _publicKey;
        private static RSAParameters? _privateKey;

        public static string Sha256(string plain)
        {
            using (var hash = SHA256.Create())
            {
                return Convert.ToBase64String(hash.ComputeHash(Encoding.ASCII.GetBytes(plain)));
            }
        }

        public static string CreateChallengeCode(string codeVerifier) =>
            Sha256(codeVerifier).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static (RSAParameters Public, RSAParameters Private) GetDefaultRsaKey()
        {
            if (_publicKey.HasValue && _privateKey.HasValue) return (_publicKey.Value, _privateKey.Value);
            using (var rsa = RSACng.Create())
            {
                rsa.FromXmlString(@"<RSAKeyValue>
    <Modulus>w7ClTEFNtbi5pnTrF7sN5weE/0L/B5/A9yY7saDpbyRPLJQ5fFfW0scwL/904r/EQ+KFtVOPH7mZQEDR393gzjbN6mx8/LIIVtfxVPmUD9zmOy8MOiO5PZg5m2C2AT/LI09R7/VESaOOV5mh8/UoT4mfI3fsI1pjwDsOmHFvOaE=</Modulus>
    <Exponent>AQAB</Exponent>
    <P>9yckheLIFkn4HHxNw4+wb80CMGUI3Hy69Qaft6n7OfXsMTzVxnq+VOc8JWaW7Jlecv5ZFfd3iFFbRIRnqu0ddw==</P>
    <Q>yrHoctrSXraYhEIy2rykYacuLrF3GCnir8pQ6v8Do2TzXWqOFEF8OyN2FzjqPyBWpo17vCbWZAJfLDk1YFNHpw==</Q>
    <DP>eJdvuaf6qu1ykyuPofD1TMfB3q0dkr+FVVLjEXFt9Ezq7udZA1wWjES7UyBoWY9Hx2IVQ6OYfjn8B0V3c634Zw==</DP>
    <DQ>qoxNQK68CXNsGwS0U3Ycfgo0ApfR1GQR18XMlh4iio37c2Ofzo1XIU9yIpICD0F/hz5OmX64L4gLWmN8dOM9yw==</DQ>
    <InverseQ>sNtg7hiG+w3XyEOkmjeLzw1aQWR4dlmKFBKKZoXo40fE0U3k1eUjPZllBmkBw+1EfGH1mm/fpPJ+58KrnHpxXw==</InverseQ>
    <D>XAABRn9xJF0LUvmmyQpU9+euHEAIHDJ8CvW7nL/03x6n5mJtQEsfoQqqWfZ0omHbyLqHd/8ny5d7OB0BTolGtXjK4g4XzPy6WfL7f6DGcWZpIDIy6WxWCkLqU1N0VEmpVAERbTrP+p6HBshabVLMH+lO+muzAIN1HvPF7NtWau0=</D>
</RSAKeyValue>");
                _publicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);
                return (_publicKey.Value, _privateKey.Value);
            }
        }

        public static string Encrypt(string text, RSAParameters? publicKey = null)
        {
            using (var provider = RSACng.Create())
            {
                if (!publicKey.HasValue)
                    (publicKey, _) = GetDefaultRsaKey();
                provider.ImportParameters(publicKey.Value);
                var textBytes = Encoding.UTF8.GetBytes(text);
                var cipherBytes = provider.Encrypt(textBytes, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(cipherBytes);
            }
        }

        public static string Decrypt(string cipher, RSAParameters? privateKey = null)
        {
            using (var provider = RSACng.Create())
            {
                if (!privateKey.HasValue)
                    (_, privateKey) = GetDefaultRsaKey();
                provider.ImportParameters(privateKey.Value);
                var cipherBytes = Convert.FromBase64String(cipher);
                var plainBytes = provider.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
}