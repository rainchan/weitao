using System;
using System.Text;

namespace WT.Components.Common.Utility
{
    public static class RandomUtil
    {
        private static object lockObejct = new object();
        private static Random random = new Random();

        /// <summary>   
        /// 获得一个16位时间随机数   
        /// </summary>   
        /// <returns>返回随机数</returns>   
        public static string GetDateRandom()
        {
            string strData = DateTime.Now.ToString("yyyyMMddHHmmss");
            Random r = new Random();
            strData = strData + r.Next(100000);
            return strData;
        }

        /// <summary>   
        /// 获得随机数字   
        /// </summary>   
        /// <param name="Length">随机数字的长度</param>   
        /// <returns>返回长度为 Length 的　<see cref="System.Int32"/> 类型的随机数</returns>   
        /// <example>   
        /// Length 不能大于9,以下为示例演示了如何调用 GetRandomNext：<br />   
        /// <code>   
        ///  int le = GetRandomNext(8);   
        /// </code>   
        /// </example>   
        public static int GetRandomNext(int Length)
        {
            if (Length > 9)
                throw new System.IndexOutOfRangeException("Length的长度不能大于10");
            Guid gu = Guid.NewGuid();
            string str = "";
            for (int i = 0; i < gu.ToString().Length; i++)
            {
                if (StringUtil.isNumber(gu.ToString()[i]))
                {
                    str += ((gu.ToString()[i]));
                }
            }
            int guid = int.Parse(str.Replace("-", "").Substring(0, Length));
            if (!guid.ToString().Length.Equals(Length))
                guid = GetRandomNext(Length);
            return guid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            string token = "0123456789abcdefghijklmnopqrstuvwxyx";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(token[random.Next(token.Length)]);
            }
            return builder.ToString();
        }
    }
}
