using CTB.Services.AccessToken.WinService;
using WT.Components.Http;
using WT.Components.Http.Interface;
using WT.Components.Http.Models;
using WT.Components.Common.Utility;
using System;
using System.Threading;

namespace WT.Services.AccessToken.WinService
{
    public class ServiceProcess
    {
        private static bool isRun = true;
        private static object lockObject = new object();
        private static string host = "112.124.17.248";
        private static TokenItem tokenItem = null;

        /// <summary>
        /// 当前处理服务信息
        /// </summary>
        private static ServiceProcess serviceProcess;

        /// <summary>
        /// 当前服务信息
        /// </summary>
        public static ServiceProcess Current
        {
            get
            {
                return serviceProcess;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static ServiceProcess()
        {
            serviceProcess = new ServiceProcess();
        }

        /// <summary>
        /// 
        /// </summary>
        private ServiceProcess()
        {
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            try
            {
                host = ConfigUtil.GetAppSetting("accessTokenUrl");
                ThreadStart start = new ThreadStart(Do);
                Thread thread = new Thread(start);
                thread.Start();
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("服务启动异常:ex={0}", ex.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            isRun = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Do()
        {
            try
            {
                LogUtil.Info("正在启动");
                while (isRun)
                {
                    PostData();
                    // 等待
                    Thread.Sleep(60000);
                }
                LogUtil.Info(string.Format("服务已停止"));
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("服务启动异常:ex={0}", ex.ToString()));
            }
        }

        /// <summary>
        /// 检测微信AccessToken是否有效
        /// </summary>
        /// <returns></returns>
        private static void PostData()
        {
            try
            {
                if (tokenItem == null)
                {
                    GetNewToken();
                }

                if (tokenItem == null)
                {
                    LogUtil.Error("无法从服务器获取Token信息");
                    return;
                }

                string userId = ConfigUtil.GetAppSetting("userId");
                string msg = "{\"touser\":\"" + userId + "\",\"msgtype\":\"text\",\"text\":{\"content\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "数据正常\"}}";

                string result = DoPost(string.Format("https://api.weixin.qq.com/cgi-bin/message/send?access_token={0}", tokenItem.access_token), msg);
                if (!string.IsNullOrEmpty(result))
                {
                    ResultItem resultItem = JsonUtil<ResultItem>.FromJosn(result);
                    if (resultItem.errcode == "0")
                    {
                        LogUtil.Info(string.Format("{0}数据正常", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    else
                    {
                        LogUtil.Info(string.Format("{0}数据失败,{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), result));
                        RemoveKey();
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static void GetNewToken()
        {
            try
            {
                string url = string.Format("http://{0}/accesstoken/getweixinaccesstoken?format=json", host);
                string result = DoPost(url, "app_id=1&app_version=1.0&os_type=1&device_id=");
                if (!string.IsNullOrEmpty(result))
                {
                    TokenItem item = JsonUtil<TokenItem>.FromJosn(result);
                    if (item != null && item.ret == 200)
                    {
                        LogUtil.Info(string.Format("正常获得Token{0}", result));
                        tokenItem = item;
                    }
                    else
                    {
                        LogUtil.Error(string.Format("获取access_token出错{0}", result));
                        tokenItem = null;
                    }
                }
                else
                {
                    LogUtil.Error(string.Format("获取access_token出错{0}", result));
                    tokenItem = null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("获取access_token出错:ex={0}", ex.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static void RemoveKey()
        {
            if (tokenItem != null)
            {
                try
                {
                    string url = string.Format("http://{0}/accesstoken/removeweixinaccesstoken?format=json", host);
                    string result = DoPost(url, string.Format("app_id=1&app_version=1.0&os_type=1&device_id=&version={0}", tokenItem.version));
                    tokenItem = null;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(string.Format("清除Token信息出错:ex={0}", ex.ToString()));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static string DoPost(string url, string postData)
        {
            try
            {
                IHttpRequest request = new HttpRequest();
                request.HttpMothed = HttpMothed.POST;
                request.AddressUrl = string.Format(url);
                request.CustomBody = postData;
                IHttpResponse response = HttpUtil.ExecuteSync(request);
                return response.Content.ToString();
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("访问{0}异常:ex={0}", url, ex.ToString()));
            }
            return "";
        }
    }
}
