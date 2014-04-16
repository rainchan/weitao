namespace WT.Services.Fs.Repository.Redis
{
    public class CommonFileRedisFactory
    {
        private const string redis_basiccomments_write_host_key = "redis_write_host";
        private const string redis_basiccomments_write_port_key = "redis_write_port";
        private const string redis_basiccomments_write_pwd_key = "redis_write_password";

        private const string redis_basiccomments_read_host_key = "redis_read_host";
        private const string redis_basiccomments_read_port_key = "redis_read_port";
        private const string redis_basiccomments_read_pwd_key = "redis_read_password";

        private static CommonFileRA commonFileRedisRead;
        private static CommonFileRA commonFileRedisWrite;

        private static object obj = new object();


        /// <summary>
        /// commonfile redis读对象
        /// </summary>
        /// <returns></returns>
        public static ICommonFileRedis CreateReadFilesRedis()
        {
            if (commonFileRedisRead == null)
            {
                lock (obj)
                {
                    if (commonFileRedisRead == null)
                    {
                        commonFileRedisRead = new CommonFileRA(redis_basiccomments_read_host_key, redis_basiccomments_read_port_key, redis_basiccomments_read_pwd_key);
                    }
                }
            }
            return commonFileRedisRead;
        }

        /// <summary>
        /// commonfile redis写对象
        /// </summary>
        /// <returns></returns>
        public static ICommonFileRedis CreateWriteFilesRedis()
        {
            if (commonFileRedisWrite == null)
            {
                lock (obj)
                {
                    if (commonFileRedisWrite == null)
                        commonFileRedisWrite = new CommonFileRA(redis_basiccomments_write_host_key, redis_basiccomments_write_port_key, redis_basiccomments_write_pwd_key);
                }
            }
            return commonFileRedisWrite;
        }
    }
}
