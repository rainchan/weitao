namespace WT.Services.Fs.Repository.Redis
{
    public class CommonFileKeys
    {
        /// <summary>
        /// FS文件系统，文件在Redis中缓存的key值，前缀FS + ：+ GUID
        /// Vvalue 对应 文件URL地址
        /// </summary>
        public const string CommonFileKey = "FS_guid_{0}";
    }
}
