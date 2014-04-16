using System.Collections.Generic;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// MQ消息体定义
    /// </summary>
    public class ScalePicFileListMessageEntity
    {
        /// <summary>
        /// 原始图片GUID
        /// </summary>
        public string SourcePicGuid { get; set; }

        /// <summary>
        /// 缩放图片尺寸与guid字典
        /// </summary>
        public Dictionary<string, string> ScalePicGuidDic { get; set; }


    }
}
