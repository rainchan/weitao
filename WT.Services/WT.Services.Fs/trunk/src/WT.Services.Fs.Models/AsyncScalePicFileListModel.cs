using WT.Components.Common.Models;
using System.Collections.Generic;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// 异步图片缩放列表
    /// </summary>
    public class AsyncScalePicFileListModel:ResultModel
    {
        /// <summary>
        /// 图片尺寸与GUID关联的字典
        /// </summary>
        public Dictionary<string, string> PicGuidDics { get; set; }
    }
}
