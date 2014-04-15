using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTB.Services.AccessToken.WebApi
{
    [Route("/accesstoken/getweixinaccesstoken", "POST")]
    public class GetWeiXinAccessToken
    {
        /// <summary>
        /// 应用id  1:车托帮APP ， 2:微信路况
        /// </summary>
        public int app_id
        {
            get;
            set;
        }

        /// <summary>
        /// 应用版本号
        /// </summary>
        public string app_version
        {
            get;
            set;
        }

        /// <summary>
        /// 手机类型 1:ios，2:android，3：windows
        /// </summary>
        public int os_type
        {
            get;
            set;
        }

        /// <summary>
        /// 设备号
        /// </summary>
        public string device_id
        {
            get;
            set;
        }

    }
}