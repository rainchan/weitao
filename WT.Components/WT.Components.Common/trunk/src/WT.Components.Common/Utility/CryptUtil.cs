using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WT.Components.Common.Utility
{
    public static class CryptUtil
    {
        /// <summary>
        /// MD5 encodes the passed string
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string MD5String(string input)
        {
            // Create a new instance of the 
            // MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte 
            // array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(
               Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verified a string against the passed MD5 hash.
        /// </summary>
        /// <param name="input">The string to compare.</param>
        /// <param name="hash">The hash to compare against.</param>
        /// <returns>True if the input and the hash 
        /// are the same, false otherwise.</returns>
        public static bool MD5VerifyString(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = CryptUtil.MD5String(input);

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string SHA1String(string input)
        {
            // Create a new instance of the 
            // SHA1CryptoServiceProvider object.
            SHA1 sha1Hasher = SHA1.Create();

            // Convert the input string to a byte 
            // array and compute the hash.
            byte[] data = sha1Hasher.ComputeHash(
               Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static bool SHA1VerifyString(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = CryptUtil.SHA1String(input);

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private static byte[] makeMD5(byte[] key)
        {
            MD5CryptoServiceProvider MD5Provider = new MD5CryptoServiceProvider();
            byte[] keyhash = MD5Provider.ComputeHash(key);
            MD5Provider.Clear();
            return keyhash;
        }

        public static string MakeMD5(string key)
        {
            return Convert.ToBase64String(makeMD5(Encoding.UTF8.GetBytes(key)));
        }

        public static string MakeMD5(params string[] strArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in strArray) sb.Append(item);
            return MakeMD5(sb.ToString());
        }
        public static string Encrypt(string original, string key)
        {
            TripleDESCryptoServiceProvider DESProvider = new TripleDESCryptoServiceProvider();
            byte[] org_bytes = Encoding.UTF8.GetBytes(original);
            byte[] key_bytes = Encoding.UTF8.GetBytes(key);

            DESProvider.Key = ComputeMD5(key_bytes);
            DESProvider.Mode = CipherMode.ECB;
            byte[] target = DESProvider.CreateEncryptor().TransformFinalBlock(org_bytes, 0, org_bytes.Length);
            DESProvider.Clear();
            return Convert.ToBase64String(target);
        }

        public static string Decrypt(string encrypted, string key)
        {
            TripleDESCryptoServiceProvider DESProvider = new TripleDESCryptoServiceProvider();
            try
            {
                byte[] encry_bytes = Convert.FromBase64String(encrypted);
                byte[] key_bytes = Encoding.UTF8.GetBytes(key);
                DESProvider.Key = ComputeMD5(key_bytes);
                DESProvider.Mode = CipherMode.ECB;
                byte[] source = DESProvider.CreateDecryptor().TransformFinalBlock(encry_bytes, 0, encry_bytes.Length);
                DESProvider.Clear();
                return Encoding.UTF8.GetString(source);
            }
            catch
            {
                return null;
            }
            finally
            {
                DESProvider.Clear();
            }
        }

        public static bool isEncrypted(string strEnc, string key)
        {
            return (CryptUtil.Decrypt(strEnc, key) != null);
        }


        public static byte[] RSASignSHA1
                                (
                                    byte[] data
                                    , string privateKey
                                )
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return RSASignSHA1
                        (
                            provider
                            , data
                            , privateKey
                        );
        }
        public static byte[] RSASignSHA1
                (
                    RSACryptoServiceProvider provider
                    , byte[] data
                    , string privateKey
                )
        {
            provider.FromXmlString(privateKey);
            return provider.SignHash
                                (
                                    ComputeSHA1(data)
                                    , "SHA1"
                                );
        }
        public static bool RSAVerifySHA1
                                (
                                    byte[] data
                                    , string publicKey
                                    , byte[] signature
                                )
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return RSAVerifySHA1
                            (
                                provider
                                , data
                                , publicKey
                                , signature
                            );
        }
        public static bool RSAVerifySHA1
                                (
                                    RSACryptoServiceProvider provider
                                    , byte[] data
                                    , string publicKey
                                    , byte[] signature
                                )
        {
            provider.FromXmlString(publicKey);
            return provider.VerifyHash
                                (
                                    ComputeSHA1(data)
                                    , "SHA1"
                                    , signature
                                );
        }
        public static byte[] RSASignMD5
                                (
                                    byte[] data
                                    , string privateKey
                                )
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return RSASignMD5
                        (
                            provider
                            , data
                            , privateKey
                        );
        }
        public static byte[] RSASignMD5
                                (
                                    RSACryptoServiceProvider provider
                                    , byte[] data
                                    , string privateKey
                                )
        {
            provider.FromXmlString(privateKey);
            return provider.SignHash
                        (
                            ComputeMD5(data)
                            , "MD5"
                        );
        }
        public static bool RSAVerifyMD5
                                (
                                    byte[] data
                                    , string publicKey
                                    , byte[] signature
                                )
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return RSAVerifyMD5
                                (
                                    provider
                                    , data
                                    , publicKey
                                    , signature
                                );
        }
        public static bool RSAVerifyMD5
                                (
                                    RSACryptoServiceProvider provider
                                    , byte[] data
                                    , string publicKey
                                    , byte[] signature
                                )
        {
            provider.FromXmlString(publicKey);
            return provider.VerifyHash
                                (
                                    ComputeMD5(data)
                                    , "MD5"
                                    , signature
                                );
        }
        public static byte[] RSAEncrypt
                                (
                                    byte[] data
                                    , string publicKey
                                    , bool DoOAEPPadding
                                )
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return RSAEncrypt
                        (
                            provider,
                            data,
                            publicKey,
                            DoOAEPPadding
                        );
        }
        public static byte[] RSAEncrypt
                        (
                            RSACryptoServiceProvider provider
                            , byte[] data
                            , string publicKey
                            , bool DoOAEPPadding
                        )
        {
            provider.FromXmlString(publicKey);
            return provider.Encrypt(data, DoOAEPPadding);
        }
        public static byte[] RSADecrypt
                                (
                                    byte[] data
                                    , string privateKey
                                    , bool DoOAEPPadding
                                )
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return RSADecrypt
                        (
                            provider,
                            data,
                            privateKey,
                            DoOAEPPadding
                        );
        }
        public static byte[] RSADecrypt
                        (
                            RSACryptoServiceProvider provider
                            , byte[] data
                            , string privateKey
                            , bool DoOAEPPadding
                        )
        {
            provider.FromXmlString(privateKey);
            return provider.Decrypt(data, DoOAEPPadding);
        }
        public static byte[] TripleDESDecrypt
                                        (
                                            byte[] data
                                            , byte[] Key
                                            , byte[] IV
                                        )
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = Key;
            des.IV = IV;
            return des.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] TripleDESDecrypt
                                        (
                                            string text
                                            , string HexStringKey
                                            , string HexStringIV
                                        )
        {
            return TripleDESDecrypt
                            (
                                HexStringToBytesArray(text)
                                , HexStringToBytesArray(HexStringKey)
                                , HexStringToBytesArray(HexStringIV)
                            );
        }
        public static byte[] TripleDESDecrypt
                                    (
                                        string text
                                        , byte[] Key
                                        , byte[] IV
                                    )
        {
            return TripleDESDecrypt
                            (
                                HexStringToBytesArray(text)
                                , Key
                                , IV
                            );
        }
        public static string TripleDESDecrypt
                                    (
                                        string text
                                        , string HexStringKey
                                        , string HexStringIV
                                        , Encoding e //原文的encoding
                                    )
        {
            return e.GetString
                (
                    TripleDESDecrypt
                        (
                            text
                            , HexStringKey
                            , HexStringIV
                        )
                );
        }
        public static string TripleDESDecrypt
                                    (
                                        string text
                                        , byte[] Key
                                        , byte[] IV
                                        , Encoding e //原文的encoding
                                    )
        {
            return e.GetString
                        (
                            TripleDESDecrypt
                                (
                                    text
                                    , Key
                                    , IV
                                )
                        );
        }
        public static string GenerateTripleDESHexStringKey()
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.GenerateKey();
            return BytesArrayToHexString(des.Key);
        }
        public static string GenerateTripleDESHexStringIV()
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.GenerateIV();
            return BytesArrayToHexString(des.IV);
        }
        public static byte[] TripleDESEncrypt
                                        (
                                            byte[] data
                                            , byte[] Key
                                            , byte[] IV
                                        )
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = Key;
            des.IV = IV;
            return des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }
        public static byte[] TripleDESEncrypt
                                        (
                                            string text
                                            , Encoding e
                                            , byte[] Key
                                            , byte[] IV
                                        )
        {
            return TripleDESEncrypt
                            (
                                e.GetBytes(text)
                                , Key
                                , IV
                            );
        }
        public static byte[] TripleDESEncrypt
                                        (
                                            string text
                                            , Encoding e
                                            , string HexStringKey
                                            , string HexStringIV
                                        )
        {
            return TripleDESEncrypt
                            (
                                text
                                , e
                                , HexStringToBytesArray(HexStringKey)
                                , HexStringToBytesArray(HexStringIV)
                            );
        }
        public static byte[] ComputeSHA1(byte[] data)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(data);
        }
        public static byte[] ComputeSHA1(string text, Encoding e)
        {
            return ComputeSHA1(e.GetBytes(text));
        }
        public static byte[] ComputeSHA1(string text)
        {
            return ComputeSHA1(text, Encoding.UTF8);
        }
        public static byte[] ComputeSHA1(Stream stream)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(stream);
        }
        public static byte[] ComputeMD5(byte[] data)
        {
            return new MD5CryptoServiceProvider().ComputeHash(data, 0, data.Length);
        }
        public static byte[] ComputeMD5(string text, Encoding e)
        {
            return ComputeMD5(e.GetBytes(text));
        }
        public static byte[] ComputeMD5(string text)
        {
            return ComputeMD5(text, Encoding.UTF8);
        }
        public static byte[] ComputeMD5(Stream stream)
        {
            return new MD5CryptoServiceProvider().ComputeHash(stream);
        }
        public static string BytesArrayToHexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }
        public static byte[] HexStringToBytesArray(string text)
        {
            text = text.Replace(" ", "");
            int l = text.Length;
            byte[] buffer = new byte[l / 2];
            for (int i = 0; i < l; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(text.Substring(i, 2), 16);
            }
            return buffer;
        }
    }
}
