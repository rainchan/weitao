namespace WT.Services.Fs.Repository.Redis
{
    public class ScalePicFileKeys
    {
        /// <summary>
        /// 指定比例图片缓存key,用于获取url
        /// key含义：源图片GUID + 宽 + 高
        /// </summary>
        public const string ScalePicFileKey = "FS_guid_{0}_w_{1}_h_{2}";

        /// <summary>
        /// 指定比例图片缓存key,用于获取guid
        /// key含义：源图片GUID + 宽 + 高
        /// </summary>
        public const string ScalePicFileGuidKey = "FS_PicsGuid_{0}_w_{1}_h_{2}";

        /// <summary>
        /// 指定比例图片缓存，用于获取文件对象
        /// </summary>
        public const string ScalePicFileEntityKey = "FS_PicEntity:{0}_w:{1}:h:{2}";
    }
}
