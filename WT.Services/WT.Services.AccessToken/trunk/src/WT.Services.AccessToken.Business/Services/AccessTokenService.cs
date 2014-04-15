using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using WT.Components.Common.Config;
using WT.Components.Common.Utility;
using WT.Components.Http;
using WT.Components.Http.Interface;
using WT.Components.Http.Models;
using WT.Services.AccessToken.Business.Model;

namespace WT.Services.AccessToken.Business.Services
{
    public class AccessTokenService
    {
        private const int cacheWeiXinTime = 90;
        private static object lockWeiXinObject = new object();
        private static SettingSection weiXinConfig = null;

        /// <summary>
        /// 
        /// </summary>
        private static SettingSection WeiXinConfig
        {
            get
            {
                if (weiXinConfig == null)
                {
                    weiXinConfig = (SettingSection)ConfigurationManager.GetSection("weiXinConfig");
                }

                return weiXinConfig;
            }
        }

        static AccessTokenService()
        {
        }

        /// <summary>
        /// 获取微信Token信息
        /// </summary>
        /// <returns></returns>
        public static WeiXinAccessTokenItem GetWeiXinAccessToken(string remoteIp, string realIp)
        {
            WeiXinAccessTokenItem result = WeiXinAccessTokenCache.GetWeiXinTokenItem();
            if (result == null)
            {
                lock (lockWeiXinObject)
                {
                    //异步时处理，避免重复读
                    result = WeiXinAccessTokenCache.GetWeiXinTokenItem();
                    if (result == null)
                    {
                        LogUtil.Info(string.Format("从缓存中获取空的WeiXinToken,重新生成Token信息，线程为{0}", Thread.CurrentThread.ManagedThreadId));
                        result = GetWeiXinAccessTokenFromWeiXin();
                        if (result != null)
                        {
                            WeiXinAccessTokenCache.SetWeiXinTokenItem(result, cacheWeiXinTime);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 从缓存中清楚Token信息
        /// </summary>
        /// <param name="versioin"></param>
        /// <param name="remoteIp"></param>
        /// <param name="realIp"></param>
        /// <returns></returns>
        public static bool RemoveWeiXinAccessToken(long version, string remoteIp, string realIp)
        {
            try
            {
                lock (lockWeiXinObject)
                {

                    WeiXinAccessTokenItem item = WeiXinAccessTokenCache.GetWeiXinTokenItem();
                    if (item == null)
                    {
                        return true;
                    }

                    //今日缓存的版本比要求清楚的版本要新不删除
                    if (item.Version > version)
                    {
                        return true;
                    }

                    WeiXinAccessTokenCache.RemoveWeiXinTokenItem();
                    LogUtil.Info(string.Format("({0})({1})({2})正式清除缓存信息！", remoteIp, realIp, version));
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取微信的Token信息
        /// </summary>
        /// <returns></returns>
        private static WeiXinAccessTokenItem GetWeiXinAccessTokenFromWeiXin()
        {
            try
            {
                string url = WeiXinConfig.GetSetting("token_url");
                IHttpRequest request = ConfigHttpRequest(url, null, null, HttpMothed.POST, ParameterType.Form);
                IHttpResponse response = HttpUtil.ExecuteSync(request);
                string result = Encoding.GetEncoding("gbk").GetString(response.RawBytes);

                //WeiXinAccessTokenItem t = new WeiXinAccessTokenItem();
                //t.access_token = "S1i0qKt3hsXbHg4AaBKaKoVD_QqL_b1GRxaurOaPzVxKBkuQSpOuBCYqtSL2-yhTknrl2qMh8HKLju5e2QDMipm4pe1kx6K5jistBMqyTbQDaDWDQ6KGlUoA42Rve-0YGiMGfEyXlQMau6mj_GHA";
                //t.expires_in = 7200;
                //t.errcode = 0;
                //string result = JsonUtil<WeiXinAccessTokenItem>.ToJson(t);

                if (!string.IsNullOrEmpty(result))
                {
                    WeiXinAccessTokenItem token = JsonUtil<WeiXinAccessTokenItem>.FromJosn(result);
                    if (token != null && token.errcode == 0 && !string.IsNullOrEmpty(token.access_token))
                    {
                        return token;
                    }
                    else
                    {
                        string err = string.Format("从微信获取Token时出错:返回值为:{0}", result);
                        LogUtil.Error(err.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("从微信获取Token时出错: ex={0}", ex.ToString());
                LogUtil.Error(err.ToString());
            }

            return null;
        }

        /// <summary>
        /// 获取组件需要的 Httprequest 信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <param name="headers"></param>
        /// <param name="method"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        protected static IHttpRequest ConfigHttpRequest(string url, Dictionary<string, object> fields, Dictionary<string, object> headers, HttpMothed method, ParameterType parameterType)
        {

            IHttpRequest request = new HttpRequest();

            request.HttpMothed = method;
            IList<Parameter> parameters = new List<Parameter>();
            if (fields != null && fields.Count > 0)
            {
                foreach (var item in fields)
                {
                    Parameter p = new Parameter();
                    p.Name = item.Key;
                    p.Value = item.Value;
                    p.Type = parameterType;
                    parameters.Add(p);
                }
            }
            request.Parameters = parameters;

            parameters = new List<Parameter>();
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers)
                {
                    Parameter p = new Parameter();
                    p.Name = item.Key;
                    p.Value = item.Value;
                    p.Type = ParameterType.Header;
                    request.HeaderParameters.Add(p);
                }
            }

            request.AddressUrl = url;
            return request;
        }
    }
}
