using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using WT.Components.Http.Interface;

namespace WT.Components.Http.Models
{
    public class HttpResponse : IHttpResponse
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpResponse()
        {
            Headers = new List<Parameter>();
            Cookies = new List<HttpCookie>();
            ErrorException = new Exception("请求失败！");
        }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 返回内容的长度
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// 返回内容的编码
        /// </summary>
        public string ContentEncoding { get; set; }

        private string _content;
        /// <summary>
        /// 返回的内容
        /// </summary>
        public string Content
        {
            get
            {
                if (_content == null)
                {
                    Encoding encoding = Encoding.UTF8;
                    _content = encoding.GetString(RawBytes);
                }
                return _content;
            }
        }

        /// <summary>
        /// 内容的字节流
        /// </summary>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// 返回响应的实际地址
        /// 如果请求过程中出现重定向，此地址
        /// </summary>
        public Uri ResponseUri { get; set; }

        /// <summary>
        /// 字符串，包含发送响应的服务器的名称
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 返回的Cookie列表
        /// </summary>
        public IList<HttpCookie> Cookies { get; private set; }

        private ResponseStatus _responseStatus = ResponseStatus.None;

        /// <summary>
        /// 返回的状态(请求传输等出现错误等状态)   默认是 None 成功为  Completed
        /// </summary>
        public ResponseStatus ResponseStatus
        {
            get
            {
                return _responseStatus;
            }
            set
            {
                _responseStatus = value;
            }
        }


        /// <summary>
        /// 返回的 Http状态码
        /// 如：  200， 301等
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 返回的状态描述  http状态码的枚举字符串 
        /// 如： OK，Moved  等
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 错误实体
        /// </summary>
        public Exception ErrorException { get; set; }
        /// <summary>
        /// 头部列表
        /// </summary>
        public IList<Parameter> Headers { get; set; }
    }
}
