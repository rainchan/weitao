using WT.Components.Common.Utility;
using ServiceStack.Redis;
using System;

namespace WT.Services.Fs.Repository.Redis
{
    public class CommonFileRA:CommonFileRedisInstance, ICommonFileRedis
    {
        public CommonFileRA()
            : base()
        {
        }

        public CommonFileRA(string hostKey, string portKey, string pswdKey)
            : base(hostKey, portKey, pswdKey)
        {           

        }

        public void SetCommonFileGuidUrl(string guid, string url)
        {
            string key = string.Format(CommonFileKeys.CommonFileKey,guid);

            try
            {
                using (RedisClient m_redisClient = this.pooled.GetClient() as RedisClient)
                {
                    m_redisClient.SetEntry(key, url, TimeSpan.FromSeconds(expireSecends));
                    LogUtil.Info(string.Format("CommonFileRA.SetCommonFileGuidUrl set key:{0} success", key));
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(string.Format("CommonFileRA.SetCommonFileGuidUrl set key:{0} error msg:{1}", key, e.ToString()));
            }
        }

        public string GetURLByGuid(string guid)
        {
            string key = string.Format(CommonFileKeys.CommonFileKey, guid);

            using (RedisClient m_redisClient = this.pooled.GetClient() as RedisClient)
            {
                if(m_redisClient.Ttl(key)>0 && m_redisClient.Ttl(key) <= 10)
                {
                    m_redisClient.Expire(key, incExpireSecends);
                }

                return m_redisClient.Get<string>(key);
            }

        }
    }
}
