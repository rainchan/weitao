using WT.Components.Common.Utility;

namespace WT.Components.Common.Helpers
{
    public class EncryptionHelper
    {
        public static string EcryptMd5String(string stringToEncrypt, string salt)
        {
            if (!string.IsNullOrEmpty(stringToEncrypt) && !string.IsNullOrEmpty(salt))
            {
                string result = MD5Encrypt.GetMD5String(stringToEncrypt).ToLower();
                return MD5Encrypt.GetMD5String(string.Format("{0}{1}", result, salt)).ToLower();
            }
            return "";
        }
    }
}
