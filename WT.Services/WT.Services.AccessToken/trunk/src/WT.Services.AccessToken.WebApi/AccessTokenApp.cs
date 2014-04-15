using WT.Components.Common.Utility;
using CTB.Services.AccessToken.WebApi;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using System;
using System.Diagnostics;
using WT.Services.AccessToken.Business.Model;
using WT.Services.AccessToken.Business.Services;
using WT.Services.AccessToken.WebApi.Models;

namespace WT.Services.AccessToken.WebApi
{
    public class AccessTokenApp : Service
    {
        /// <summary>
        /// 获取微信的Token
        /// </summary>
        /// <param name="request">
        /// 
        /// </param>
        /// <returns></returns>
        public object Post(GetWeiXinAccessToken request)
        {
            GetWeiXinAccessTokenResponse response = new GetWeiXinAccessTokenResponse();
            try
            {
                string remoteIp = Request.RemoteIp;
                string realIp = Request.XRealIp;


                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                WeiXinAccessTokenItem tokenItem = AccessTokenService.GetWeiXinAccessToken(remoteIp, realIp);

                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 2000)
                {
                    LogUtil.Error(string.Format("GetWeiXinAccessToken用时:{0},请求内容:{1}", stopwatch.ElapsedMilliseconds, JsonSerializer.SerializeToString<GetWeiXinAccessToken>(request)));
                }

                if (tokenItem == null || string.IsNullOrEmpty(tokenItem.access_token))
                {
                    response.ret = 400;
                    response.message = "no Token";
                    return response;
                }

                response.ret = 200;
                response.version = tokenItem.Version;
                response.access_token = tokenItem.access_token;
            }
            catch (Exception ex)
            {
                response.ret = 500;
                response.message = "server error";
                string err = string.Format("Exception in GetWeiXinAccessToken(): ex={0}, request={1}", ex.ToString(), JsonSerializer.SerializeToString<GetWeiXinAccessToken>(request));
                LogUtil.Error(ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 清楚微信的Token在本地的缓存
        /// </summary>
        /// <param name="request">
        /// 
        /// </param>
        /// <returns></returns>
        public object Post(RemoveWeiXinAccessToken request)
        {
            RemoveWeiXinAccessTokenResponse response = new RemoveWeiXinAccessTokenResponse();
            try
            {
                string remoteIp = Request.RemoteIp;
                string realIp = Request.XRealIp;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                bool result = AccessTokenService.RemoveWeiXinAccessToken(request.version, remoteIp, realIp);

                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 2000)
                {
                    LogUtil.Error(string.Format("RemoveWeiXinAccessToken用时:{0},请求内容:{1}", stopwatch.ElapsedMilliseconds, JsonSerializer.SerializeToString<RemoveWeiXinAccessToken>(request)));
                }

                if (result)
                {
                    response.ret = 200;
                }
                else
                {
                    response.ret = 500;
                    response.message = "server error";
                }
            }
            catch (Exception ex)
            {
                response.ret = 500;
                response.message = "server error";
                string err = string.Format("Exception in RemoveWeiXinAccessToken(): ex={0}, request={1}", ex.ToString(), JsonSerializer.SerializeToString<RemoveWeiXinAccessToken>(request));
                LogUtil.Error(ex.ToString());
            }
            return response;
        }
    }
}