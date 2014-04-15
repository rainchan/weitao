using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT.Components.Authorisation.Model
{
    public class AutonaviLogOnResult
    {
        /// <summary>
        /// 返回结果
        ///  2xx   成功相关状态（如： 200）
        ///  3xx   参数相关错误 
        ///  4xx   用户授权相关错误
        ///  5xx   服务器内部相关错误信息
        ///  6xx   系统级定制错误信息，如升级维护等
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
        /// 高德信息
        /// </summary>
        public string pinCode
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

        /// <summary>
        /// 有效时间
        /// </summary>
        public int expires_in
        {
            get;
            set;
        }
    }
}
