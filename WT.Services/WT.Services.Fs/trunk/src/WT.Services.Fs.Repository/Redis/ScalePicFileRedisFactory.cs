namespace WT.Services.Fs.Repository.Redis
{
    public class ScalePicFileRedisFactory
    {
        private const string redis_basiccomments_write_host_key = "redis_write_host";
        private const string redis_basiccomments_write_port_key = "redis_write_port";
        private const string redis_basiccomments_write_pwd_key = "redis_write_password";

        private const string redis_basiccomments_read_host_key = "redis_read_host";
        private const string redis_basiccomments_read_port_key = "redis_read_port";
        private const string redis_basiccomments_read_pwd_key = "redis_read_password";


        private static ScalePicFileRA scalePicFileRedisRead;
        private static ScalePicFileRA scalePicFileRedisWrite;

        private static object obj = new object();

        /// <summary>
        /// userdb.users表的redis读对象
        /// </summary>
        /// <returns></returns>
        public static IScalePicFileRedis CreateReadFileRedis()
        {
            if (scalePicFileRedisRead == null)
            {
                lock (obj)
                {
                    if (scalePicFileRedisRead == null)
                        scalePicFileRedisRead = new ScalePicFileRA(redis_basiccomments_read_host_key, redis_basiccomments_read_port_key, redis_basiccomments_read_pwd_key);
                }
            }
            return scalePicFileRedisRead;
        }

        /// <summary>
        /// userdb.user表的redis写对象
        /// </summary>
        /// <returns></returns>
        public static IScalePicFileRedis CreateWriteFileRedis()
        {
            if (scalePicFileRedisWrite == null)
            {
                lock (obj)
                {
                    if (scalePicFileRedisWrite == null)
                        scalePicFileRedisWrite = new ScalePicFileRA(redis_basiccomments_write_host_key, redis_basiccomments_write_port_key, redis_basiccomments_write_pwd_key);
                }
            }
            return scalePicFileRedisWrite;
        }
    }
}
