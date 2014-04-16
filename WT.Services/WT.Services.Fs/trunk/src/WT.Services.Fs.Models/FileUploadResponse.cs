using WT.Components.Common.Models;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// 文件同步上传响应结果
    /// </summary>
    public class FileUploadResponse:ResultModel
    {
        public string File_guid { get; set; } 
    }
}
