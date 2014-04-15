using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using WT.Components.Http.Interface;
using WT.Components.Http.Models;
using HttpResponse = WT.Components.Http.Models.HttpResponse;

namespace WT.Components.Http.Connect
{
    public abstract class HttpBase
    {
        private const string _lineBreak = "\r\n";
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;
        protected IDictionary<string, Action<HttpWebRequest, string>> _notCanAddHeaderDictionary = null;
        public HttpBase()
        {
            _notCanAddHeaderDictionary = new Dictionary<string, Action<HttpWebRequest, string>>(StringComparer.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// 配置请求
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected virtual HttpWebRequest ConfigureWebRequest(IHttpRequest request)
        {
            Uri uri = BuildUri(request);   //  创建url请求
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);            
            ServicePointManager.Expect100Continue = false;
            webRequest.UseDefaultCredentials = false;
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;
            if (request.Timeout > 0)
            {
                webRequest.Timeout = request.Timeout;
            }
            webRequest.Method = request.HttpMothed.ToString();
            if (!request.HasFile)
            {
                webRequest.ContentLength = 0;
            }
            webRequest.ContentType = "application/x-www-form-urlencoded";  //添加一个默认类型，具体自定义可以在后边重写
            PrepareHeaders(webRequest, request);  //  添加头部信息
            PrepareCookies(webRequest, request);  //  添加cookie信息
            return webRequest;
        }



        /// <summary>
        /// 创建请求的 Url 对象
        /// </summary>
        /// <param name="request">请求的对象实体</param>
        /// <returns>返回的 System.Uri 对象</returns>
        protected virtual Uri BuildUri(IHttpRequest request)
        {
            string urlAddress = request.AddressUrl;
            if (string.IsNullOrEmpty(urlAddress))
            {
                throw new ArgumentNullException("无效的请求，请求对象中的 UrlAddress 不能为空");
            }
            urlAddress = string.Join(urlAddress.IndexOf('?') > 0 ? "&" : "?", urlAddress, string.Join("&", request.QueryParameters));
            return new Uri(urlAddress);
        }


        #region  准备模块

        /// <summary>
        /// 准备请求的 头部 数据
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="request"></param>
        protected virtual void PrepareHeaders(HttpWebRequest webRequest, IHttpRequest request)
        {
            foreach (var header in request.HeaderParameters)
            {
                if (_notCanAddHeaderDictionary.ContainsKey(header.Name))
                {
                    _notCanAddHeaderDictionary[header.Name].Invoke(webRequest, header.Value.ToString());
                }
                else
                {
                    webRequest.Headers.Add(header.Name, header.Value.ToString());
                }

            }
        }

        /// <summary>
        /// 准备请求的 cookie 数据
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="request"></param>
        protected virtual void PrepareCookies(HttpWebRequest webRequest, IHttpRequest request)
        {
            webRequest.CookieContainer = new CookieContainer();

            foreach (var httpCookie in request.CookieParameters)
            {
                var cookie = new Cookie
                {
                    Name = httpCookie.Name,
                    Value = httpCookie.Value.ToString(),
                    Domain = webRequest.RequestUri.Host
                };
                webRequest.CookieContainer.Add(cookie);
            }
        }
        #endregion


        /// <summary>
        /// 准备请求的 内容 数据   ===  同步
        /// </summary>
        /// <param name="webRequest">HttpWebRequest请求</param>
        protected virtual void SendBodyDataSync(HttpWebRequest webRequest, IHttpRequest request)
        {
            string boundary = GetBoundary();
            if (request.HasFile)
            {
                webRequest.ContentType = GetMultipartFormContentType(boundary);
                using (var webRequestStream = webRequest.GetRequestStream())
                {
                    WriteMultipartFormData(webRequestStream, request, boundary);
                }
            }
            else
            {
                if (request.HttpMothed == HttpMothed.POST)
                {
                    var bytes = _defaultEncoding.GetBytes(EncodeFormParameters(request));
                    webRequest.ContentLength = bytes.Length;
                    using (var webRequestStream = webRequest.GetRequestStream())
                    {
                        webRequestStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 准备请求的 内容 数据   ===  异步
        /// </summary>
        /// <param name="webRequest">HttpWebRequest请求</param>
        protected virtual void SendBodyDataAsyncAndCallBack(HttpWebRequest webRequest, IHttpRequest request,
            Action<IHttpResponse> callback)
        {
            if (request.HttpMothed == HttpMothed.POST && request.FormParameters.Count > 0 || request.HasFile)
            {
                if (!request.HasFile)
                {
                    webRequest.ContentLength = _defaultEncoding.GetByteCount(EncodeFormParameters(request));
                }
                webRequest.BeginGetRequestStream(ar => RquestStreamCallBack(ar, request, callback), webRequest);
            }
            else
            {
                webRequest.BeginGetResponse(ar => ResponseCallBackAsync(ar, callback), webRequest);
            }
        }

        /// <summary>
        /// 异步请求处理
        /// </summary>
        /// <param name="result"></param>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        private void RquestStreamCallBack(IAsyncResult result, IHttpRequest request,
            Action<IHttpResponse> callback)
        {


            HttpWebRequest webRequest = result.AsyncState as HttpWebRequest;

            if (request.HasFile)
            {
                string boundary = GetBoundary();

                webRequest.ContentType = GetMultipartFormContentType(boundary);

                using (var webRequestStream = webRequest.EndGetRequestStream(result))
                {
                    WriteMultipartFormData(webRequestStream, request, boundary);
                }
            }
            else
            {
                var bytes = _defaultEncoding.GetBytes(EncodeFormParameters(request));

                using (var webRequestStream = webRequest.EndGetRequestStream(result))
                {
                    webRequestStream.Write(bytes, 0, bytes.Length);
                }
            }

            webRequest.BeginGetResponse(ar => ResponseCallBackAsync(ar, callback), webRequest);
        }

        /// <summary>
        /// 异步响应处理
        /// </summary>
        /// <param name="result"></param>
        /// <param name="callback"></param>
        private void ResponseCallBackAsync(IAsyncResult result, Action<IHttpResponse> callback)
        {
            var response = new HttpResponse();

            HttpWebResponse web_response = null;
            try
            {
                var webRequest = (HttpWebRequest)result.AsyncState;
                web_response = webRequest.EndGetResponse(result) as HttpWebResponse;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw ex;
                }
                if (ex.Response is HttpWebResponse)
                {
                    response.ResponseStatus = ResponseStatus.ErrorButResponse;
                    web_response = ex.Response as HttpWebResponse;
                }
                else
                {
                    throw ex;
                }
            }

            ExtractResponseData(response, web_response);

            if (callback != null)
            {
                callback(response);
            }

            if (web_response != null)
                web_response.Close();
        }

        #region   请求数据的 内容 处理

        #region 处理带文件上传的数据处理
        /// <summary>
        /// 创建 请求 分割界限
        /// </summary>
        /// <returns></returns>
        protected static string GetBoundary()
        {
            string pattern = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder boundaryBuilder = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                var index = rnd.Next(pattern.Length);
                boundaryBuilder.Append(pattern[index]);
            }
            return string.Format("-----------------------------{0}", boundaryBuilder.ToString());
        }

        /// <summary>
        /// 返回含文件请求的ContentType
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns>返回  WebRequest 的 ContenType 信息</returns>
        protected static string GetMultipartFormContentType(string boundary)
        {
            return string.Format("multipart/form-data; boundary={0}", boundary);
        }

        /// <summary>
        /// 写入 Form 的内容值 【 非文件参数 + 文件头 + 文件参数（内部完成） + 请求结束符 】
        /// </summary> 
        /// <param name="webRequestStream"></param>
        /// <param name="request"></param>
        protected void WriteMultipartFormData(Stream webRequestStream, IHttpRequest request, string boundary)
        {
            var formparas = request.FormParameters;
            foreach (var param in formparas)
            {
                //写入form表单中的非文件数据
                WriteStringTo(webRequestStream, GetMultipartFormData(param, boundary));
            }
            foreach (var file in request.FileParameterList)
            {
                //文件头
                WriteStringTo(webRequestStream, GetMultipartFileHeader(file, boundary));
                //文件内容
                file.Writer(webRequestStream);
                //文件结尾
                WriteStringTo(webRequestStream, _lineBreak);
            }
            //写入整个请求的底部信息
            WriteStringTo(webRequestStream, GetMultipartFooter(boundary));
        }

        /// <summary>
        /// 写入 Form 的内容值（非文件参数）
        /// </summary>
        /// <param name="param"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        protected static string GetMultipartFormData(Parameter param, string boundary)
        {
            return string.Format("--{0}{3}Content-Disposition: form-data; name=\"{1}\"{3}{3}{2}{3}",
                boundary, param.Name, param.Value, _lineBreak);
        }

        /// <summary>
        /// 写入 Form 的内容值（文件头）
        /// </summary>
        /// <param name="file"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        protected static string GetMultipartFileHeader(FileParameter file, string boundary)
        {
            return string.Format("--{0}{4}Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"{4}Content-Type: {3}{4}{4}",
                boundary, file.Name, file.FileName, file.ContentType ?? "application/octet-stream", _lineBreak);
        }

        /// <summary>
        /// 写入 Form 的内容值  （请求结束符）
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        protected static string GetMultipartFooter(string boundary)
        {
            return string.Format("--{0}--{1}", boundary, _lineBreak);
        }
        #endregion

        #region 不包含文件的数据处理（正常 get/post 请求）
        /// <summary>
        /// 写入请求的内容信息 （非文件上传请求）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static string EncodeFormParameters(IHttpRequest request)
        {
            var formstring = new StringBuilder();
            foreach (var p in request.FormParameters)
            {
                if (formstring.Length > 1)
                    formstring.Append("&");
                formstring.AppendFormat("{0}={1}", p.Name, p.Value);
            }
            if (!string.IsNullOrEmpty(request.CustomBody))
            {
                if (formstring.Length > 1)
                    formstring.Append("&");
                formstring.Append(request.CustomBody);
            }
            return formstring.ToString();
        }
        #endregion

        #endregion

        #region 请求返回 -- 返回数据的处理
        /// <summary>
        /// 返回自定义
        /// </summary>
        /// <param name="request">传入 webrequest 对象</param>
        /// <param name="response">需要返回的自定义 response 对象</param>
        /// <returns></returns>
        protected virtual void GetResponse(HttpWebRequest request, ref IHttpResponse response)
        {
            response.ResponseStatus = ResponseStatus.None;
            try
            {
                bool isErrorButResponse;
                var webResponse = GetHttpResponse(request, out isErrorButResponse);
                ExtractResponseData(response, webResponse);
                if (isErrorButResponse)
                    response.ResponseStatus = ResponseStatus.ErrorButResponse;
            }
            catch (Exception ex)
            {
                response.ErrorException = ex;
                response.ResponseStatus = ResponseStatus.Error;
            }
        }
        /// <summary>
        /// 获取远程请求响应
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="isErrorButResponse">请求返回是否正常</param>
        /// <returns></returns>
        private static HttpWebResponse GetHttpResponse(HttpWebRequest request, out bool isErrorButResponse)
        {
            HttpWebResponse webResponse = null;
            isErrorButResponse = false;
            try
            {
                webResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    isErrorButResponse = true;
                    webResponse = ex.Response as HttpWebResponse;
                }
                else
                    throw;
            }
            return webResponse;
        }


        /// <summary>
        /// 填充自定义  response 值
        /// </summary>
        /// <param name="response"></param>
        /// <param name="webResponse"></param>
        private static void ExtractResponseData(IHttpResponse response, HttpWebResponse webResponse)
        {
            using (webResponse)
            {
                response.ContentEncoding = webResponse.ContentEncoding;
                response.Server = webResponse.Server;
                response.ContentType = webResponse.ContentType;
                response.ContentLength = webResponse.ContentLength;

                response.StatusCode = webResponse.StatusCode;
                response.StatusDescription = webResponse.StatusDescription;
                response.ResponseUri = webResponse.ResponseUri;
                response.ResponseStatus = ResponseStatus.Completed;

                response.RawBytes = ReadAsBytes(webResponse.GetResponseStream());

                if (webResponse.Cookies != null)
                {
                    foreach (Cookie cookie in webResponse.Cookies)
                    {
                        HttpCookie recookie = new HttpCookie(cookie.Name);
                        recookie.Domain = cookie.Domain;
                        recookie.Expires = cookie.Expires;
                        recookie.HttpOnly = cookie.HttpOnly;
                        recookie.Path = cookie.Path;
                        recookie.Secure = cookie.Secure;
                        recookie.Value = cookie.Value;
                        response.Cookies.Add(recookie);
                    }
                }

                foreach (var headerName in webResponse.Headers.AllKeys)
                {
                    var headerValue = webResponse.Headers[headerName];
                    response.Headers.Add(new Parameter { Name = headerName, Value = headerValue });
                }
                webResponse.Close();
            }
        }

        /// <summary>
        /// 流转化成字节
        /// </summary>
        /// <param name="input">要转化的流</param>
        /// <returns>byte[]</returns>
        public static byte[] ReadAsBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        #endregion

        #region 请求辅助方法
        /// <summary>
        /// 写入数据方法（将数据写入  webrequest）
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="toWrite"></param>
        protected static void WriteStringTo(Stream stream, string toWrite)
        {
            var bytes = _defaultEncoding.GetBytes(toWrite);
            stream.Write(bytes, 0, bytes.Length);
        }

        protected virtual void AddStaticHeaderDictionary()
        {
        }
        #endregion
    }
}
