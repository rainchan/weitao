namespace WT.Services.AccessToken.Models
{
    public class WeiXinAccessTokenItem : ItemBase
    {
        /// <summary>
        /// 
        /// </summary>
        public WeiXinAccessTokenItem()
        {
            
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
    }
}
