using System;
using System.Web;
using System.Web.Caching;

namespace WT.Services.AccessToken.Repository
{
    /// <summary>
    /// 
    /// </summary>
    class CacheManager
    {
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration">过期时间</param>
        public static void Insert(string key, object value, DateTime absoluteExpiration)
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
        public static T Get<T>(string key)
        {
            if (Has(key))
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
        public static bool Has(string key)
        {
            return HttpRuntime.Cache[key] != null;
        }

        /// <summary>
        /// 从缓存中移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
