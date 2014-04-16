using WT.Components.Common.Models;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// 异步文件上传响应结果
    /// </summary>
    public class AsyncsFileUploadResponse : ResultModel
    {
        public string File_guid { get; set; }
    }
}
