using System;

namespace WT.Services.AccessToken.Repository
{
    abstract class CacheRepository<T> : IRepository<T> 
    {
        public void Save(T t)
        {
            string cacheKey = GetCacheKey();
            CacheManager.Insert(cacheKey, t, GetCacheExpire());
        }

        public void Remove()
        {
            string cacheKey = GetCacheKey();
            CacheManager.Remove(cacheKey);
        }

        public virtual T Get()
        {
            var obj = CacheManager.Get<T>(GetCacheKey());
            return obj;
        }

        /// <summary>
        /// 缓存键值
        /// </summary>
        /// <returns></returns>
        protected abstract string GetCacheKey();

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        /// <returns></returns>
        protected abstract DateTime GetCacheExpire();
    }
}
