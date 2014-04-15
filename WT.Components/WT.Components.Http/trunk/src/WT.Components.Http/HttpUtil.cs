using System;
using WT.Components.Http.Connect;
using WT.Components.Http.Interface;

namespace WT.Components.Http
{
    public static class HttpUtil
    {
        private static IHttp httpclient = new RestHttp();

        /// <summary>
        /// 同步的请求方式
        /// </summary>
        /// <param name="request">请求的参数</param>
        /// <returns>自定义的Response结果</returns>
        public static IHttpResponse ExecuteSync(IHttpRequest request)
        {
            return httpclient.ExecuteSync(request);
        }


        /// <summary>
        /// 异步的请求方式
        /// </summary>
        /// <param name="request">请求的参数</param>
        /// <returns>自定义的Response结果</returns>
        public static void ExecuteAsync(IHttpRequest request,Action<IHttpResponse> callback )
        {
             httpclient.ExecuteAsync(request, callback);
        }
    }
}
