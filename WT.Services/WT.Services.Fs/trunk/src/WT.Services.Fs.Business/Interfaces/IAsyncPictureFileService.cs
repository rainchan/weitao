using System.Collections.Generic;
using WT.Services.Fs.Models;

namespace WT.Services.Fs.Business.Interfaces
{
    /// <summary>
    /// 异步图片处理服务，For MQ
    /// </summary>
    public interface IAsyncPictureFileService
    {
        /// <summary>
        /// 异步图片缩放指定比例列表
        /// </summary>
        /// <param name="fileGuid">原始图片guid</param>
        /// <param name="sizes">尺寸列表</param>
        /// <returns>为空异常，不为空返回字典表，key：尺寸， value：guid</returns>
        AsyncScalePicFileListModel AsyncScalePictureFile(string fileGuid, List<string> sizes);
    }
}
