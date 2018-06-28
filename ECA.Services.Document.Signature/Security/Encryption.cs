using System;
using System.Security.Cryptography;
using System.Text;

namespace ECA.Services.Document.Signature.Security
{
    public class Encryption
    {
        private static byte[] _aesIV = Encoding.ASCII.GetBytes("!QAZ2&SX#EDC4RFV");
        private static byte[] _aesKey = Encoding.ASCII.GetBytes("5TGB&YHN!UJM(IK<5TGB&YHN7UJM(IK<");
        
        public static string DecryptStringAes(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            // setup the AesCrypto provider
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = _aesIV;
            aes.Key = _aesKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // convert Base64 string to byte array
            byte[] source = Convert.FromBase64String(cipherText);

            // do decryption
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(source, 0, source.Length);
                return Encoding.Unicode.GetString(dest);
            }
        }
    }
}
