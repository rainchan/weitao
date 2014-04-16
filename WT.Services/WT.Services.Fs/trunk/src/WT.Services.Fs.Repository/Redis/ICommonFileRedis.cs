namespace WT.Services.Fs.Repository.Redis
{
    public interface ICommonFileRedis
    {
        /// <summary>
        /// 设置fsfs中文件地址在redis缓存中, key格式："fs:guid"
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="url"></param>
        void SetCommonFileGuidUrl(string guid, string url);

        /// <summary>
        /// 返回guid对应的文件url地址
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        string GetURLByGuid(string guid);


    }
}
