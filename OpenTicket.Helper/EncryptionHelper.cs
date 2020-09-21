using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenTicket.Helper
{
    internal static class EncryptionHelper
    {
        public static string Sha256(string plain)
        {
            using (var hash = SHA256.Create())
            {
                return Convert.ToBase64String(hash.ComputeHash(Encoding.ASCII.GetBytes(plain)));
            }
        }

        public static string CreateChallengeCode(string codeVerifier) =>
            Sha256(codeVerifier).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static RSA GetDefaultRsaKey()
        {
            var rsa = RSACng.Create();
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
            return rsa;
        }

        public static string Encrypt(string text)
        {
            var input = Encoding.UTF8.GetBytes(text);
            // by default this will create a 128 bits AES (Rijndael) object
            SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
            ICryptoTransform ct = sa.CreateEncryptor();
            byte[] encrypt = ct.TransformFinalBlock(input, 0, input.Length);

            using (var rsa = GetDefaultRsaKey())
            {
                var fmt = new RSAPKCS1KeyExchangeFormatter(rsa);
                byte[] keyex = fmt.CreateKeyExchange(sa.Key);

                // return the key exchange, the IV (public) and encrypted data
                byte[] result = new byte [keyex.Length + sa.IV.Length + encrypt.Length];
                Buffer.BlockCopy(keyex, 0, result, 0, keyex.Length);
                Buffer.BlockCopy(sa.IV, 0, result, keyex.Length, sa.IV.Length);
                Buffer.BlockCopy(encrypt, 0, result, keyex.Length + sa.IV.Length, encrypt.Length);
                return Convert.ToBase64String(result);
            }
        }

        public static string Decrypt(string cipher)
        {
            using (var rsa = GetDefaultRsaKey())
            {
                var input = Convert.FromBase64String(cipher);
                // by default this will create a 128 bits AES (Rijndael) object
                SymmetricAlgorithm sa = SymmetricAlgorithm.Create();

                byte[] keyex = new byte [rsa.KeySize >> 3];
                Buffer.BlockCopy(input, 0, keyex, 0, keyex.Length);

                RSAPKCS1KeyExchangeDeformatter def = new RSAPKCS1KeyExchangeDeformatter(rsa);
                byte[] key = def.DecryptKeyExchange(keyex);

                byte[] iv = new byte [sa.IV.Length];
                Buffer.BlockCopy(input, keyex.Length, iv, 0, iv.Length);

                ICryptoTransform ct = sa.CreateDecryptor(key, iv);
                byte[] decrypt = ct.TransformFinalBlock(input, keyex.Length + iv.Length,
                    input.Length - (keyex.Length + iv.Length));
                return Encoding.UTF8.GetString(decrypt);
            }
        }
    }
}