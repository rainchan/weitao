using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTB.Services.AccessToken.WinService
{
    public class TokenItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int ret
        {
            get;
            set;
        }

        /// <summary>
        ///  结果说明
        /// </summary>
        public string message
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
        /// 版本号
        /// </summary>
        public long version
        {
            get;
            set;
        }
    }
}
