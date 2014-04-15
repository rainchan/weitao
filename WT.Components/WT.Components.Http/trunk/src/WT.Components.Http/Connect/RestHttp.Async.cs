using System;
using System.Net;
using WT.Components.Http.Interface;
using WT.Components.Http.Models;

namespace WT.Components.Http.Connect
{
    internal partial class RestHttp
    {
        public void ExecuteAsync(IHttpRequest request, Action<IHttpResponse> callback)
        {
            IHttpResponse response = new HttpResponse();
            try
            {
                if (string.IsNullOrEmpty(request.AddressUrl)
                    || request.HttpMothed == HttpMothed.None)
                {
                    response.ResponseStatus = ResponseStatus.Error;
                    response.ErrorException = new Exception("对不起，请求出错，请检查参数等设置（地址，请求方式等）！");
                }
                else
                {
                    HttpWebRequest webrequest = ConfigureWebRequest(request);
                    SendBodyDataAsyncAndCallBack(webrequest, request, callback);
                    return;
                }
            }
            catch (Exception ex)
            {
                response.ErrorException = ex;
                response.ResponseStatus = ResponseStatus.Error;
            }
            if (callback != null)
                callback(response);

        }
    }
}
