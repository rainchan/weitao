using WT.Components.Common.Models;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// 异步文件下载响应结果
    /// </summary>
    public class AsyncFileDownloadResponse:ResultModel
    {
        public string File_url { get; set; } 
    }
}
