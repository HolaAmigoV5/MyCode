using System.Security.Cryptography;
using System.Text;

namespace EncryptAndDecrypt
{
    public class DesEncryptHelper
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="sKey">秘钥</param>
        /// <param name="text">待加密文本</param>
        /// <returns></returns>
        public static string Encrypt(string sKey, string text)
        {
            try
            {
                using var mStream = new MemoryStream();
                byte[] _rgbKey = Encoding.ASCII.GetBytes(sKey[..8]);
                using (var des = DES.Create())
                using (ICryptoTransform encryptor = des.CreateEncryptor(_rgbKey, _rgbKey))
                using (var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                {
                    byte[] toEncrypt = Encoding.UTF8.GetBytes(text);
                    cStream.Write(toEncrypt, 0, toEncrypt.Length);
                    return Convert.ToBase64String(mStream.ToArray(), 0, (int)mStream.Length);
                }
            }
            catch (CryptographicException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="sKey">秘钥</param>
        /// <param name="encryptText">密文</param>
        /// <returns></returns>
        public static string Decrypt(string sKey, string encryptText)
        {
            try
            {
                //byte[] encrypted = Convert.FromBase64String(encryptText);
                //byte[] decrypted = new byte[encrypted.Length];
                //int offset = 0;

                //using MemoryStream mStream = new MemoryStream(encrypted);
                //byte[] _rgbKey = Encoding.UTF8.GetBytes(sKey[..8]);
                //using DES des = DES.Create();
                //using ICryptoTransform decryptor = des.CreateDecryptor(_rgbKey, _rgbKey);
                //using (var cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                //{
                //    int read = 1;
                //    while (read > 0)
                //    {
                //        read = cStream.Read(decrypted, offset, decrypted.Length - offset);
                //        offset += read;
                //    }
                //}
                //return Encoding.UTF8.GetString(decrypted, 0, offset);

                byte[] buffer = Convert.FromBase64String(encryptText);
                byte[] _rgbKey = Encoding.UTF8.GetBytes(sKey[..8]);

                using var tripleDES = DES.Create();

                using var memStream = new MemoryStream();
                using var crypStream = new CryptoStream(memStream, tripleDES.CreateDecryptor(_rgbKey, _rgbKey), CryptoStreamMode.Write);
                crypStream.Write(buffer, 0, buffer.Length);
                crypStream.FlushFinalBlock();
                return Encoding.Default.GetString(memStream.ToArray());
            }
            catch (CryptographicException e)
            {
                throw e;
            }
        }
    }
}
