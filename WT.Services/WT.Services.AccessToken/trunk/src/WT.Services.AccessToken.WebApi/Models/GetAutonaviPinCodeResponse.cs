using WT.Components.Common.Models;

namespace WT.Services.AccessToken.WebApi.Models
{
    public class GetAutonaviPinCodeResponse : ResultModel
    {
        /// <summary>
        /// 微信Token信息
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