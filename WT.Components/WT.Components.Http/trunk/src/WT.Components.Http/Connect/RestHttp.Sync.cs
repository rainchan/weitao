using System;
using System.Net;
using WT.Components.Http.Interface;
using WT.Components.Http.Models;

namespace WT.Components.Http.Connect
{
    internal partial class RestHttp : HttpBase, IHttp
    {
        public RestHttp()
        {
            AddStaticHeaderDictionary();
        }

        /// <summary>
        /// 同步的请求方式
        /// </summary>
        /// <param name="request">请求的参数</param>
        /// <returns>自定义的Response结果</returns>
        public IHttpResponse ExecuteSync(IHttpRequest request)
        {
            IHttpResponse response = new HttpResponse();
            try
            {
                if (string.IsNullOrEmpty(request.AddressUrl)
                    || request.HttpMothed == HttpMothed.None)
                {
                    response.ResponseStatus = ResponseStatus.Error;
                    response.ErrorException = new Exception("对不起，请求出错，请检查参数等设置（地址，请求方式等）！");
                    return response;
                }
                HttpWebRequest webrequest = ConfigureWebRequest(request);

                SendBodyDataSync(webrequest, request);

                GetResponse(webrequest, ref response);
            }
            catch (Exception ex)
            {
                response.ErrorException = ex;
                response.ResponseStatus = ResponseStatus.Error;
            }
            return response;
        }

        protected override void AddStaticHeaderDictionary()
        {
            _notCanAddHeaderDictionary.Add("Accept", (r, v) => r.Accept = v);
            _notCanAddHeaderDictionary.Add("Content-Type", (r, v) => r.ContentType = v);

            _notCanAddHeaderDictionary.Add("Date", (r, v) =>
                {
                    DateTime parsed;
                    if (DateTime.TryParse(v, out parsed))
                    {
                        r.Date = parsed;
                    }
                });
            _notCanAddHeaderDictionary.Add("Host", (r, v) => r.Host = v);
            _notCanAddHeaderDictionary.Add("Connection", (r, v) => r.Connection = v);
            _notCanAddHeaderDictionary.Add("Content-Length", (r, v) => r.ContentLength = Convert.ToInt64(v));
            _notCanAddHeaderDictionary.Add("Expect", (r, v) => r.Expect = v);
            _notCanAddHeaderDictionary.Add("If-Modified-Since", (r, v) => r.IfModifiedSince = Convert.ToDateTime(v));
            _notCanAddHeaderDictionary.Add("Referer", (r, v) => r.Referer = v);
            _notCanAddHeaderDictionary.Add("Transfer-Encoding", (r, v) => { r.TransferEncoding = v; r.SendChunked = true; });
            _notCanAddHeaderDictionary.Add("User-Agent", (r, v) => r.UserAgent = v);
        }


 
    }
}
