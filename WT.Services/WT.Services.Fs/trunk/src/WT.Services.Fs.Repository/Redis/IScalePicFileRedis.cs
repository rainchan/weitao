namespace WT.Services.Fs.Repository.Redis
{
    public interface IScalePicFileRedis
    {
        /// <summary>
        /// 设置请求参数为guid + width + height，值为url
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="url"></param>
        void SetScalePicFile(string guid, int width, int height, string url);

        /// <summary>
        /// 返回guid + width + height 的 url地址
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        string GetScalePicURL(string guid, int width, int height);

        /// <summary>
        /// 设置请求参数为guid + width + height，值为新图片guid
        /// </summary>
        /// <param name="guid">原始图片guid</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="ScaleGuid">新缩放图片guid</param>
        void SetScalePicGuid(string guid, int width, int height, string ScaleGuid);

        /// <summary>
        /// 返回guid + width + height 的 Guid
        /// </summary>
        /// <param name="file_guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        string GetScalePicGuid(string file_guid, int width, int height);

        /// <summary>
        /// 设置请求参数为原始guid + width + height，值为文件对象
        /// </summary>
        /// <param name="guid">原始图片guid </param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="entity"></param>
        void SetScalePicEntity<T>(string guid, int width, int height, T entity);

        /// <summary>
        /// 获取指定guid和尺寸的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        T GetScalePicEntity<T>(string guid, int width, int height);
    }
}
