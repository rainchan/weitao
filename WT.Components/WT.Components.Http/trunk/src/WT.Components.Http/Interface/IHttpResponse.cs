using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using WT.Components.Http.Models;

namespace WT.Components.Http.Interface
{
    public interface IHttpResponse
    {
        /// <summary>
        /// 返回类型
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// 返回内容的长度
        /// </summary>
        long ContentLength { get; set; }

        /// <summary>
        /// 返回内容的编码
        /// </summary>
        string ContentEncoding { get; set; }

        /// <summary>
        /// 返回的内容
        /// </summary>
        string Content { get; }

        /// <summary>
        /// 内容的字节流
        /// </summary>
        byte[] RawBytes { get; set; }

        /// <summary>
        /// 返回响应的实际地址
        /// 如果请求过程中出现重定向，此地址
        /// </summary>
        Uri ResponseUri { get; set; }

        /// <summary>
        /// 字符串，包含发送响应的服务器的名称
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// 返回的Cookie列表
        /// </summary>
        IList<HttpCookie> Cookies { get; }

        /// <summary>
        /// 返回的状态(请求传输等出现错误等状态)   默认是 None 成功为  Completed
        /// </summary>
        ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        /// 返回的 Http状态码
        /// 如：  200， 301等
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 返回的状态描述  http状态码的枚举字符串 
        /// 如： OK，Moved  等
        /// </summary>
        string StatusDescription { get; set; }

        /// <summary>
        /// 错误实体
        /// </summary>
        Exception ErrorException { get; set; }

        /// <summary>
        /// 头部列表
        /// </summary>
        IList<Parameter> Headers { get; set; }
    }
}
