using System;
using System.Drawing;
using System.Web;

namespace WT.Components.Common.Utility
{
    public class CheckCodeUtil
    {

        /// <summary>
        /// 创建
        /// </summary>
        public static byte[] Creat()
        {
            HttpContext.Current.Session.Remove("CheckCode");
            return CreateCheckCodeImage(GenerateCheckCode());
        }
        /// <summary>
        /// 验证验证码是否匹配
        /// </summary>
        /// <param name="code">用户输入的验证码</param>
        /// <returns></returns>
        public static bool CodeIsRight(string code)
        {
            try
            {
                if (HttpContext.Current.Session["CheckCode"].ToString().ToLower() == code.ToLower())
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <returns></returns>
        private static string GenerateCheckCode()
        {
            int number;
            char code;
            string checkCode = String.Empty;

            System.Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                number = random.Next();

                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));

                checkCode += code.ToString();
            }
            HttpContext.Current.Session["CheckCode"] = checkCode;
            return checkCode;
        }

        /// <summary>
        /// 根据验证码生成图片
        /// </summary>
        /// <param name="checkCode">验证码</param>
        private static byte[] CreateCheckCodeImage(string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return null;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 16.0)), 30);
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.LightGray);

                //画图片的背景噪音线
                for (int i = 0; i < 30; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.IndianRed), x1, y1, x2, y2);
                }

                Font font = new System.Drawing.Font("Verdana", 14, (System.Drawing.FontStyle.Regular | System.Drawing.FontStyle.Strikeout));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Black, Color.Black, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的前景噪音点
                for (int i = 0; i < 50; i++)
                {
                    int x = 0; random.Next(image.Width);
                    int y = 0; random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.LightGray), 0, 0, image.Width - 1, image.Height - 1);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                //输出图片流
                return ms.ToArray();
                //HttpContext.Current.Response.ClearContent();
                //HttpContext.Current.Response.ContentType = "image/Gif";
                //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }


    }
}
