using WT.Components.Common.Utility;
using ServiceStack.Redis;
using System;

namespace WT.Services.Fs.Repository.Redis
{
    public class ScalePicFileRA:ScalePicFileRedisInstance, IScalePicFileRedis
    {
        public ScalePicFileRA() : base() { }

        public ScalePicFileRA(string hostKey, string portKey, string pswdKey)
            : base(hostKey, portKey, pswdKey)
        {

        }

        public void SetScalePicFile(string guid, int width, int height, string url)
        {
            string key = string.Format(ScalePicFileKeys.ScalePicFileKey, guid, width, height);
            try
            {
                using (RedisClient m_redisClient = pooled.GetClient() as RedisClient)
                {
                    m_redisClient.SetEntry(key, url, TimeSpan.FromSeconds(expireSecends));
                }
                LogUtil.Info(string.Format("ScalePicFileRA.SetScalePicFile set key:{0} success", key));

            }
            catch (Exception e)
            {
                LogUtil.Error(string.Format("ScalePicFileRA.SetScalePicFile set key:{0} error msg:{1}", key, e.ToString()));
            }
        }

        public string GetScalePicURL(string guid, int width, int height)
        {
            string key = string.Format(ScalePicFileKeys.ScalePicFileKey, guid, width, height);
            using (RedisClient m_redisClient = pooled.GetClient() as RedisClient)
            {
                if ( m_redisClient.Ttl(key)>0 && m_redisClient.Ttl(key) <= 10)
                {
                    m_redisClient.Expire(key, incExpireSecends);
                }

                return m_redisClient.Get<string>(key);
            }
        }

        public void SetScalePicGuid(string guid, int width, int height, string ScaleGuid)
        {
            string key = string.Format(ScalePicFileKeys.ScalePicFileGuidKey, guid, width, height);

            try
            {
                using (RedisClient m_redisClient = pooled.GetClient() as RedisClient)
                {
                    m_redisClient.SetEntry(key, ScaleGuid, TimeSpan.FromSeconds(expireSecends));
                }
                LogUtil.Info(string.Format("ScalePicFileRA.SetScalePicGuid set key:{0} success", key));

            }
            catch (Exception e)
            {
                LogUtil.Error(string.Format("ScalePicFileRA.SetScalePicGuid set key:{0} error msg:{1}", key, e.ToString()));
            }
        }

        public string GetScalePicGuid(string file_guid, int width, int height)
        {
            string key = string.Format(ScalePicFileKeys.ScalePicFileGuidKey, file_guid, width, height);

            using (RedisClient m_redisClient = pooled.GetClient() as RedisClient)
            {
                if (m_redisClient.Ttl(key) > 0 && m_redisClient.Ttl(key) <= 10)
                {
                    m_redisClient.Expire(key, incExpireSecends);
                }

                return m_redisClient.Get<string>(key);
            }
        }

        public void SetScalePicEntity<T>(string guid, int width, int height, T entity)
        {
            string key = string.Format(ScalePicFileKeys.ScalePicFileEntityKey, guid, width, height);

            try
            {
                using (RedisClient m_redisClient = pooled.GetClient() as RedisClient)
                {
                    m_redisClient.Set<T>(key,entity,TimeSpan.FromSeconds(expireSecends));
                }
                LogUtil.Info(string.Format("ScalePicFileRA.SetScalePicEntity set key:{0} success", key));

            }
            catch (Exception e)
            {
                LogUtil.Error(string.Format("ScalePicFileRA.SetScalePicEntity set key:{0} error msg:{1}", key, e.ToString()));
            }
        }

        public T GetScalePicEntity<T>(string guid, int width, int height)
        {
            string key = string.Format(ScalePicFileKeys.ScalePicFileEntityKey, guid, width, height);

            using (RedisClient m_redisClient = pooled.GetClient() as RedisClient)
            {
                if (m_redisClient.Ttl(key) > 0 && m_redisClient.Ttl(key) <= 10)
                {
                    m_redisClient.Expire(key, incExpireSecends);
                }

                return m_redisClient.Get<T>(key);
            }
        }
    }
}
