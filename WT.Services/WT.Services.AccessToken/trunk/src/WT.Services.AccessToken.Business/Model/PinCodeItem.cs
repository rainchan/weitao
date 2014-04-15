using System;
using WT.Services.AccessToken.Business.Common;

namespace WT.Services.AccessToken.Business.Model
{
    public class PinCodeItem
    {
        private long version = 0;

        /// <summary>
        /// 
        /// </summary>
        public PinCodeItem()
        {
            Version = CommonHelper.GetVersion;
            CreateDateTime = DateTime.Now;
        }

        /// <summary>
        /// 微信Token信息
        /// </summary>
        public string pinCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string timesTamp
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string pid
        {
            get;
            set;
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public Int64 Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
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
