using System;
using WT.Services.AccessToken.Business.Common;
using WT.Services.AccessToken.Business.Model;

namespace WT.Services.AccessToken.Business.Services
{
    public class WeiXinAccessTokenCache
    {
        private const string cacheItem = "LocalWeiXinAccessTokenItem";
        private const string cacheVersion = "LocalWeiXinAccessTokenItemVersion_{0}";

        /// <summary>
        /// 获取本地缓存
        /// </summary>
        /// <returns></returns>
        public static WeiXinAccessTokenItem GetWeiXinTokenItem()
        {
            WeiXinAccessTokenItem result = CacheManager.GetCache<WeiXinAccessTokenItem>(cacheItem);

            if (result == null || string.IsNullOrEmpty(result.access_token))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// 设置本地缓存
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        public static void SetWeiXinTokenItem(WeiXinAccessTokenItem item, int cacheTime)
        {
            CacheManager.CacheInsert(cacheItem, item, DateTime.Now.AddMinutes(cacheTime));
        }

        /// <summary>
        /// 删除本地缓存
        /// </summary>
        public static void RemoveWeiXinTokenItem()
        {
            CacheManager.CacheRemove(cacheItem);
        }
    }
}
