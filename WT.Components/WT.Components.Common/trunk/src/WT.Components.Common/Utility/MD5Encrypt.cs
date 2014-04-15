using System.Text;
using System.Web.Security;
using System.Security.Cryptography;

namespace WT.Components.Common.Utility
{
    public class MD5Encrypt
    {
        /// <summary>
        /// 进行MD5加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string GetMD5String(string encryptString)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(encryptString, "md5");
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string GetMd5Hash32(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

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
    }
}
