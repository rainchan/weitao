using System;
using WT.Services.AccessToken.Models;

namespace WT.Services.AccessToken.Repository
{
    class WeiXinAccessTokenRepository : CacheRepository<WeiXinAccessTokenItem>
    {
        private const string CacheKey = "LocalWeiXinAccessTokenItem";
        
        /// <summary>
        /// 缓存时间（分钟）
        /// </summary>
        private const int CacheExpire = 90;

        /// <summary>
        /// 获取实例
        /// </summary>
        public static WeiXinAccessTokenRepository Instance = null;

        static WeiXinAccessTokenRepository()
        {
            Instance = new WeiXinAccessTokenRepository();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override WeiXinAccessTokenItem Get()
        {
            var token = base.Get();
            if (token == null || String.IsNullOrEmpty(token.access_token))
            {
                return null;
            }
            return token;
        }

        protected override string GetCacheKey()
        {
            return CacheKey;
        }

        protected override DateTime GetCacheExpire()
        {
            return DateTime.Now.AddMinutes(CacheExpire);
        }
    }
}
