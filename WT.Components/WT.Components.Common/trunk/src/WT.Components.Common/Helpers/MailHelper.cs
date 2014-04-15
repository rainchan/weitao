using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using WT.Components.Common.Utility;

namespace WT.Components.Common.Helpers
{
    public class MailHelper
    {
        #region 检测附件大小
        private static bool Attachment_MaiInit(string path)
        {

            try
            {
                FileStream fileStream_my = new FileStream(path, FileMode.Open);
                string name = fileStream_my.Name;
                int size = (int)(fileStream_my.Length / 1024 / 1024);
                fileStream_my.Close();
                //控制文件大小不大于10M
                if (size > 10)
                {
                    return false;
                }

                return true;
            }
            catch (IOException e)
            {
                return false;
            }

        }
        #endregion
        /// <summary>
        /// 给开发人员发邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static bool SendMailToDevUser(string title, string body)
        {
            string devUesr_email = "";
            try
            {
                devUesr_email = ConfigUtil.GetAppSetting("devUesr_email");                
            }
            catch (Exception e)
            {
                LogUtil.Info(string.Format("配置文件appSettings节未配置smtpserver：{0}", e.ToString()));
                return false;
            }
            if (!string.IsNullOrWhiteSpace(devUesr_email))
                return SendMail(title, body, devUesr_email);
            else
                return false;
            
        }
        /// <summary>
        /// 发送邮件，没有抄送、密送、附件
        /// Smtp服务器地址、端口号在配置文件中配置
        /// 配置节名称为：smtpserver，smtpport
        /// 收件人地址，发件人地址、发件人地址邮箱密码在配置文件中配置
        /// 配置节名称为：toadd,fromadd,fromaddpwd
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="title">邮件的主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="mailPwd">发件人地址邮箱密码</param>
        /// <returns></returns>
        public static bool SendMail(string title, string body, string mailto="")
        {
            //string mailto = string.Empty;
            string mailfrom = string.Empty;
            string mailfrompwd = string.Empty;

            if (string.IsNullOrWhiteSpace(mailto))
            {
                try
                {
                    mailto = ConfigUtil.GetAppSetting("mailto");
                }
                catch (Exception e)
                {
                    LogUtil.Info(string.Format("配置文件appSettings节未配置mailto：{0}", e.ToString()));
                }
            }
            try
            {
                mailfrom = ConfigUtil.GetAppSetting("mailfrom");
            }
            catch (Exception e)
            {
                LogUtil.Info(string.Format("配置文件appSettings节未配置mailfrom：{0}", e.ToString()));
            }

            try
            {
                mailfrompwd = ConfigUtil.GetAppSetting("mailfrompwd");
            }
            catch (Exception e)
            {
                LogUtil.Info(string.Format("配置文件appSettings节未配置mailfrompwd：{0}", e.ToString()));
            }

            return SendMail(mailto, mailfrom, title, body, mailfrompwd);
        }

       
        /// <summary>
        /// 发送邮件，没有抄送、密送、附件
        /// Smtp服务器地址、端口号从配置文件中取
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="title">邮件的主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="mailPwd">发件人地址邮箱密码</param>
        /// <returns></returns>
        public static bool SendMail(string to, string from, string title, string body, string mailPwd)
        {
            return SendMail(to, from, from, title, body, mailPwd);
        }

        /// <summary>
        /// 发送邮件，没有抄送、密送、附件
        /// Smtp服务器地址、端口号从配置文件中取
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="fromName">发件人名称</param>
        /// <param name="title">邮件的主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="mailPwd">发件人地址邮箱密码</param>
        /// <returns></returns>
        public static bool SendMail(string to, string from, string fromName, string title, string body, string mailPwd)
        {
            string smtpserver = string.Empty;
            int smtpport = 0;

            try
            {
                smtpserver = ConfigUtil.GetAppSetting("smtpserver");
            }
            catch (Exception e)
            {
                LogUtil.Info(string.Format("配置文件appSettings节未配置smtpserver：{0}", e.ToString()));
            }

            try
            {
                smtpport = Int32.Parse(ConfigUtil.GetAppSetting("smtpport"));
            }
            catch (Exception e)
            {
                LogUtil.Info(string.Format("配置文件appSettings节未配置smtpport：{0}", e.ToString()));
            }

            return SendMail(to, null, null, from, fromName, null, null, title, body, null, from, mailPwd, smtpserver, smtpport);
        }

        /// <summary>
        /// 发送邮件，没有抄送、密送、附件
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="title">邮件的主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="path">附件地址</param>
        /// <param name="mailPwd">发件人地址邮箱密码</param>
        /// <param name="smtpserver">Smtp服务器地址</param>
        /// <param name="smtpport">Smtp服务器端口</param>
        /// <returns></returns>
        public static bool SendMail(string to, string from, string title, string body, string mailPwd, string smtpserver, int smtpport)
        {
            return SendMail(to, null, null, from, from, null, null, title, body, null, from, mailPwd, smtpserver, smtpport);
        }

        /// <summary>
        /// 发送邮件，没有抄送、密送、附件
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="fromName">发件人名称</param>
        /// <param name="title">邮件的主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="path">附件地址</param>
        /// <param name="mailPwd">发件人地址邮箱密码</param>
        /// <param name="smtpserver">Smtp服务器地址</param>
        /// <param name="smtpport">Smtp服务器端口</param>
        /// <returns></returns>
        public static bool SendMail(string to, string from, string fromName, string title, string body, string mailPwd, string smtpserver, int smtpport)
        {
            return SendMail(to, null, null, from, fromName, null, null, title, body, null, from, mailPwd, smtpserver, smtpport);
        }

        /// <summary>
        /// 发送邮件，没有抄送、密送
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="fromName">发件人名称</param>
        /// <param name="title">邮件的主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="path">附件地址</param>
        /// <param name="mailPwd">发件人地址邮箱密码</param>
        /// <param name="smtpserver">Smtp服务器地址</param>
        /// <param name="smtpport">Smtp服务器端口</param>
        /// <returns></returns>
        public static bool SendMail(string to, string from, string fromName, string title, string body, string path, string mailPwd, string smtpserver, int smtpport)
        {
            return SendMail(to, null, null, from, fromName, null, null, title, body, path, from, mailPwd, smtpserver, smtpport);
        }

        /// <summary>  
        /// 发送邮件
        /// </summary>  
        /// <param name="To">收件人地址</param>
        /// <param name="cc">抄送人地址</param>
        /// <param name="bcc">密送人地址</param>
        /// <param name="from">发件人地址</param>
        /// <param name="fromName">发件人名称</param>
        /// <param name="Title">邮件的主题</param>
        /// <param name="Body">邮件正文</param> 
        /// <param name="path">附件地址</param>
        /// <param name="mailAddress">发件邮箱地址</param> 
        /// <param name="mailPwd">发件邮箱密码</param>  
        /// <param name="smtpserver">Smtp服务器地址</param> 
        /// <param name="smtpport">Smtp服务器端口</param>  
        public static bool SendMail(string to, string cc, string bcc, string from, string fromName, string replyTo, string replyToName, string title, string body, string path, string mailAddress, string mailPwd, string smtpserver, int smtpport)
        {
            //检测附件大小 发件必需小于10M 否则返回  不会执行以下代码
            if (!string.IsNullOrEmpty(path))
            {
                if (!Attachment_MaiInit(path))
                {
                    LogUtil.Info(string.Format("附件于10M， 收件人：{0}，发件人：{1}，邮件的主题：{2}，邮件正文：{3}", to, from, title, body));
                    return false;
                }
            }

            if (string.IsNullOrEmpty(smtpserver))
            {
                LogUtil.Info(string.Format("SMTP服务器名为空， 收件人：{0}，发件人：{1}，邮件的主题：{2}，邮件正文：{3}", to, from, title, body));
                return false;
            }

            if (string.IsNullOrEmpty(from))
            {
                LogUtil.Info(string.Format("发件人邮箱地址为空， 收件人：{0}，发件人：{1}，邮件的主题：{2}，邮件正文：{3}", to, from, title, body));
                return false;
            }

            if (string.IsNullOrEmpty(to))
            {
                LogUtil.Info(string.Format("收件人邮箱地址为空， 收件人：{0}，发件人：{1}，邮件的主题：{2}，邮件正文：{3}", to, from, title, body));
                return false;
            }

            if (string.IsNullOrEmpty(title))
            {
                LogUtil.Info(string.Format("邮件的主题为空， 收件人：{0}，发件人：{1}，邮件的主题：{2}，邮件正文：{3}", to, from, title, body));
                return false;
            }

            if (string.IsNullOrEmpty(body))
            {
                LogUtil.Info(string.Format("邮件的正文为空， 收件人：{0}，发件人：{1}，邮件的主题：{2}，邮件正文：{3}", to, from, title, body));
                return false;
            }

            try
            {
                SmtpClient smtp = new SmtpClient();

                //将smtp的出站方式设为 Network
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                //smtp服务器是否启用SSL加密
                smtp.EnableSsl = false;

                //指定 smtp 服务器地址
                smtp.Host = smtpserver;

                //指定 smtp 服务器的端口，默认是25，如果采用默认端口，可省去
                smtp.Port = smtpport <= 0 ? 25 : smtpport;

                //如果需要认证，则用下面的方式
                smtp.Credentials = new NetworkCredential(mailAddress, mailPwd);

                //实例化一个邮件类
                MailMessage mm = new MailMessage();

                //邮件的优先级，分为 Low, Normal, High，通常用 Normal即可
                mm.Priority = MailPriority.High;

                //收件方看到的邮件来源；
                //第一个参数是发信人邮件地址
                //第二参数是发信人显示的名称
                //第三个参数是 第二个参数所使用的编码，如果指定不正确，则对方收到后显示乱码
                //936是简体中文的codepage值
                //注：邮件来源，一定要和你登录邮箱的帐号一致，否则会认证失败
                mm.From = new MailAddress(from, fromName, Encoding.GetEncoding(936));

                if (!string.IsNullOrEmpty(replyTo))
                {
                    //ReplyTo 表示对方回复邮件时默认的接收地址，即：你用一个邮箱发信，但却用另一个来收信
                    //上面后两个参数的意义， 同 From 的意义
                    mm.ReplyTo = new MailAddress(replyTo, replyToName, Encoding.GetEncoding(936));
                }

                if (!string.IsNullOrEmpty(cc))
                {
                    //邮件的抄送者，支持群发，多个邮件地址之间用 半角逗号 分开
                    mm.CC.Add(cc);
                }

                if (!string.IsNullOrEmpty(bcc))
                {
                    //邮件的密送者，支持群发，多个邮件地址之间用 半角逗号 分开
                    mm.Bcc.Add(bcc);
                }

                //邮件的接收者，支持群发，多个地址之间用 半角逗号 分开
                mm.To.Add(to);

                //邮件标题
                mm.Subject = title;

                //这里非常重要，如果你的邮件标题包含中文，这里一定要指定，否则对方收到的极有可能是乱码。
                //936是简体中文的pagecode，如果是英文标题，这句可以忽略不用
                mm.SubjectEncoding = Encoding.GetEncoding(936);

                //邮件正文是否是HTML格式
                mm.IsBodyHtml = true;

                //邮件正文的编码， 设置不正确， 接收者会收到乱码
                mm.BodyEncoding = Encoding.GetEncoding(936);

                //邮件正文
                mm.Body = body;

                if (!string.IsNullOrEmpty(path))
                {
                    //添加附件
                    Attachments(path, mm);
                }

                try
                {
                    //发送邮件，如果不返回异常
                    smtp.Send(mm);
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    LogUtil.Info(string.Format("邮件发送失败！{0}" + ex.ToString()));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("邮件发送失败！{0}" + ex.ToString()));
                return false;
            }
        }

        /// <summary>  
        /// 添加附件  
        /// </summary>  
        private static void Attachments(string path, MailMessage mailMessage)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(','))
                {
                    string[] paths = path.Split(',');
                    Attachment data;
                    ContentDisposition disposition;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        data = new Attachment(paths[i], MediaTypeNames.Application.Octet);//实例化附件  
                        disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(paths[i]);//获取附件的创建日期  
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(paths[i]);//获取附件的修改日期  
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(paths[i]);//获取附件的读取日期  
                        mailMessage.Attachments.Add(data);//添加到附件中  
                    }
                }
                else
                {
                    Attachment data;
                    ContentDisposition disposition;
                    data = new Attachment(path, MediaTypeNames.Application.Octet);//实例化附件  
                    disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(path);//获取附件的创建日期  
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(path);//获取附件的修改日期  
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(path);//获取附件的读取日期  
                    mailMessage.Attachments.Add(data);//添加到附件中
                }
            }
        }

        /// <summary>
        /// 返回标题头DIV
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetTitleDiv(string t)
        {
            return string.Format("<div><br/><b>{0}</b><br/></div>", t);
        }

        /// <summary>
        /// 获取表格开头
        /// </summary>
        /// <param name="sb"></param>
        public static string GetHead()
        {
            return "<table border=\"1\" style=\"border-collapse:collapse\">";
        }

        /// <summary>
        /// 获取行
        /// </summary>
        /// <param name="str"></param>
        public static string GetHtmlRow(string str)
        {
            return string.Format("<tr>{0}</tr>", str);
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="str"></param>
        public static string GetHtmlCol(string str)
        {
            return string.Format("<td align=\"center\" width=\"130\">{0}</td>", str);
        }

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="str"></param>
        public static string GetHtmlColTh(string str)
        {
            return string.Format("<th align=\"center\" width=\"130\">{0}</th>", str);
        }

        /// <summary>
        /// 获取表格结尾
        /// </summary>
        /// <param name="sb"></param>
        public static string GetEnd()
        {
            return "</table>";
        }
    }
}
