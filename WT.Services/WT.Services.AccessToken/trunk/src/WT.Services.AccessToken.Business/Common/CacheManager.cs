using System;
using System.Web;
using System.Web.Caching;

namespace WT.Services.AccessToken.Business.Common
{
    public class CacheManager
    {
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration">过期时间</param>
        public static void CacheInsert(string key, object value, DateTime absoluteExpiration)
        {
            if (value != null)
            {
                HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        public static T GetCache<T>(string key)
        {
            if (HasCache(key))
            {
                return (T)HttpRuntime.Cache[key];
            }

            return default(T);
        }

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasCache(string key)
        {
            return HttpRuntime.Cache[key] != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void CacheRemove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
