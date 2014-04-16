using WT.Components.Common.Utility;
using WT.Components.Common.Extention;
using ServiceStack.Redis;
using WT.Components.Redis;

namespace WT.Services.Fs.Repository.Redis
{
    public class ScalePicFileRedisInstance:RedisInstance
    {
        //protected RedisClient m_redisClient { get; set; }
        protected int expireSecends { get; set; }
        protected int incExpireSecends { get; set; }
        protected PooledRedisClientManager pooled { get; set; } 

        public ScalePicFileRedisInstance()
        {
            RedisHost = ConfigUtil.GetAppSetting("redis_host");
            RedisPort = ConfigUtil.GetAppSetting("redis_port").ToInt32();
            RedisPassword = ConfigUtil.GetAppSetting("redis_password");
            //m_redisClient = new RedisClient(RedisHost, RedisPort, RedisPassword);
            pooled = new PooledRedisClientManager(string.Format("{0}@{1}:{2}", RedisPassword, RedisHost, RedisPort));
            pooled.ConnectTimeout = 5000;
            pooled.PoolTimeout = 5000;
            pooled.SocketSendTimeout = 5000;
            pooled.SocketReceiveTimeout = 5000;
            expireSecends = 60 * 60 * 12;
            incExpireSecends = 60 * 60 * 12;
            expireSecends = 60*5;
            incExpireSecends = 60*30;
        }

        public ScalePicFileRedisInstance(string hostKey, string portKey, string passwordKey)
        {
            RedisHost = ConfigUtil.GetAppSetting(hostKey);
            RedisPort = ConfigUtil.GetAppSetting(portKey).ToInt32();
            RedisPassword = ConfigUtil.GetAppSetting(passwordKey);
            //m_redisClient = new RedisClient(RedisHost, RedisPort, RedisPassword);
            pooled = new PooledRedisClientManager(string.Format("{0}@{1}:{2}", RedisPassword, RedisHost, RedisPort));
            pooled.ConnectTimeout = 5000;
            pooled.PoolTimeout = 5000;
            pooled.SocketSendTimeout = 5000;
            pooled.SocketReceiveTimeout = 5000;
            expireSecends = 60 * 60 * 12;
            incExpireSecends = 60 * 60 * 12;
            expireSecends = 60 * 5;
            incExpireSecends = 60 * 30;
            expireSecends = 60 * 5;
            incExpireSecends = 60 * 30;

        }
    }
}
