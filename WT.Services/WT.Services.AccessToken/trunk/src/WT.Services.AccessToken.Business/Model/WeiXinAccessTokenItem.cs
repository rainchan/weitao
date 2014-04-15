using System;
using WT.Services.AccessToken.Business.Common;

namespace WT.Services.AccessToken.Business.Model
{
    public class WeiXinAccessTokenItem
    {
        /// <summary>
        /// 
        /// </summary>
        public WeiXinAccessTokenItem()
        {
            Version = CommonHelper.GetVersion;
            CreateDateTime = DateTime.Now;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public int errcode
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg
        {
            get;
            set;
        }

        /// <summary>
        /// 微信Token信息
        /// </summary>
        public string access_token
        {
            get;
            set;
        }

        /// <summary>
        /// 微信Token有效时间
        /// </summary>
        public int expires_in
        {
            get;
            set;
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public Int64 Version
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime
        {
            get;
            set;
        }
    }
}
