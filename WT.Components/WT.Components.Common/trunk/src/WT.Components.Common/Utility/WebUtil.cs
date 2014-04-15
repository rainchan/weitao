using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace WT.Components.Common.Utility
{
    public static  class WebUtil
    {
        public static string GetPageHTML(string url)
        {
            return GetPageHTML(url, Encoding.Default);
        }

        public static string GetPageHTML(string url, Encoding encoding)
        {
            Stream stream = null;
            StreamReader sr = null;

            try
            {
                stream = WebRequest.Create(url).GetResponse().GetResponseStream();
                sr = new StreamReader(stream, encoding);
                return sr.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }

                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取真实IP
        /// </summary>
        /// <returns></returns>
        public static string GetRealIP()
        {
            string ip;
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return ip;
        }
    }
}
